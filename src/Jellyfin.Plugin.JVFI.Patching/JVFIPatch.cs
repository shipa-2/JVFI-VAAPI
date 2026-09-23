using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jellyfin.Database.Implementations.Entities;
using Jellyfin.Plugin.JVFI.Configuration;
using Jellyfin.Plugin.JVFI.Models;
using Jellyfin.Plugin.JVFI.Runtime;
using Jellyfin.Plugin.JVFI.Services;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Entities;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.MediaEncoding;
using MediaBrowser.Controller.Streaming;
using MediaBrowser.Model.Dto;
using MediaBrowser.Model.Entities;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JVFI.Patching;

/// <summary>Harmony entry points. All exceptions fail open to Jellyfin's original behavior.</summary>
public static class JVFIPatch
{
	private sealed record PlaybackUserContext(Guid UserId, DateTimeOffset LastSeen);

	private static FrameInterpolationStateStore? _stateStore;

	private static HudFileStore? _hudFiles;

	private static MetricsFileStore? _metricsFiles;

	private static AdaptiveProfileStore? _profileOverrides;

	private static IConfigurationManager? _configurationManager;

	private static ILibraryManager? _libraryManager;

	private static ConcurrentViewingLimiter? _concurrentViewingLimiter;

	private static HardwareCapabilityService? _capabilities;

	private static ILogger? _logger;

	private static readonly ConcurrentDictionary<string, PlaybackUserContext> PlaybackUsers = new ConcurrentDictionary<string, PlaybackUserContext>(StringComparer.Ordinal);

	private static readonly TimeSpan PlaybackUserRetention = TimeSpan.FromHours(12);

	public static void Configure(FrameInterpolationStateStore stateStore, HudFileStore hudFiles, MetricsFileStore metricsFiles, AdaptiveProfileStore profileOverrides, IConfigurationManager configurationManager, ILibraryManager libraryManager, ConcurrentViewingLimiter concurrentViewingLimiter, HardwareCapabilityService capabilities, ILogger logger)
	{
		_stateStore = stateStore;
		_hudFiles = hudFiles;
		_metricsFiles = metricsFiles;
		_profileOverrides = profileOverrides;
		_configurationManager = configurationManager;
		_libraryManager = libraryManager;
		_concurrentViewingLimiter = concurrentViewingLimiter;
		_capabilities = capabilities;
		_logger = logger;
	}

	public static void PlaybackPrefix(BaseItem item, string playSessionId, Guid userId, ref bool enableDirectPlay, ref bool enableDirectStream, ref bool enableTranscoding, ref bool allowVideoStreamCopy)
	{
		PluginConfiguration pluginConfiguration = ((BasePlugin<PluginConfiguration>)(object)Plugin.Instance)?.Configuration;
		if (pluginConfiguration != null && pluginConfiguration.Enabled)
		{
			TrackPlaybackUser(playSessionId, userId);
		}
		int num;
		if (pluginConfiguration != null && pluginConfiguration.Enabled)
		{
			HardwareCapabilityService? capabilities = _capabilities;
			if (capabilities != null && capabilities.Current.CanEnableFrameGeneration && !string.Equals(pluginConfiguration.DefaultProfileId, "off", StringComparison.OrdinalIgnoreCase))
			{
				num = (IsPlaybackScopeSelected(item, userId, pluginConfiguration) ? 1 : 0);
				goto IL_0065;
			}
		}
		num = 0;
		goto IL_0065;
		IL_0065:
		bool flag = (byte)num != 0;
		bool flag2 = flag && (!pluginConfiguration.ConcurrentViewingLimitEnabled || (_concurrentViewingLimiter?.TryAcquire(playSessionId, userId, Math.Clamp(pluginConfiguration.ConcurrentViewingUserLimit, 1, 100)) ?? false));
		PlaybackDecisionResult playbackDecisionResult = PlaybackDecision.Decide(flag & flag2, item is Video, enableDirectPlay, enableDirectStream, enableTranscoding, allowVideoStreamCopy);
		if (!playbackDecisionResult.Apply)
		{
			if ((flag && (pluginConfiguration?.ConcurrentViewingLimitEnabled ?? false)) & flag2)
			{
				_concurrentViewingLimiter?.Release(playSessionId);
			}
			if (flag && pluginConfiguration != null && pluginConfiguration.ConcurrentViewingLimitEnabled && !flag2)
			{
				ILogger? logger = _logger;
				if (logger != null)
				{
					LoggerExtensions.LogInformation(logger, "JVFI：同時觀看限制已達上限，本次播放保留 Jellyfin 原本方式，user={UserId} item={ItemId}", new object[2] { userId, item.Id });
				}
			}
		}
		else
		{
			enableDirectPlay = playbackDecisionResult.EnableDirectPlay;
			enableDirectStream = playbackDecisionResult.EnableDirectStream;
			enableTranscoding = playbackDecisionResult.EnableTranscoding;
			allowVideoStreamCopy = playbackDecisionResult.AllowVideoStreamCopy;
			ILogger? logger2 = _logger;
			if (logger2 != null)
			{
				LoggerExtensions.LogInformation(logger2, "JVFI：已要求影片進入視訊轉碼，item={ItemId} concurrentLimit={ConcurrentLimit} activeSessions={ActiveSessions}", new object[3]
				{
					item.Id,
					pluginConfiguration?.ConcurrentViewingLimitEnabled ?? false,
					_concurrentViewingLimiter?.ActiveSessionCount ?? 0
				});
			}
		}
	}

	public static void TranscodePrefix(StreamState state, string outputPath, ref string commandLineArguments)
	{
		string playSessionId = state.Request.PlaySessionId;
		try
		{
			PluginConfiguration pluginConfiguration = ((BasePlugin<PluginConfiguration>)(object)Plugin.Instance)?.Configuration;
			if (pluginConfiguration == null || !pluginConfiguration.Enabled || state.VideoRequest == null || string.Equals(pluginConfiguration.DefaultProfileId, "off", StringComparison.OrdinalIgnoreCase))
			{
				_concurrentViewingLimiter?.Release(playSessionId);
				return;
			}
			if (!IsSelectedScopeForTranscode(playSessionId, ((EncodingJobInfo)state).MediaPath, pluginConfiguration))
			{
				_concurrentViewingLimiter?.Release(playSessionId);
				ILogger? logger = _logger;
				if (logger != null)
				{
					LoggerExtensions.LogDebug(logger, "JVFI：目前使用者或其媒體庫未啟用補幀，保留 Jellyfin 原始轉碼。playSessionId={PlaySessionId} path={MediaPath}", new object[2]
					{
						playSessionId,
						((EncodingJobInfo)state).MediaPath
					});
				}
				return;
			}
			if (pluginConfiguration.ConcurrentViewingLimitEnabled)
			{
				ConcurrentViewingLimiter? concurrentViewingLimiter = _concurrentViewingLimiter;
				if (concurrentViewingLimiter == null || !concurrentViewingLimiter.HasReservation(playSessionId))
				{
					ILogger? logger2 = _logger;
					if (logger2 != null)
					{
						LoggerExtensions.LogInformation(logger2, "JVFI：本次播放沒有同時觀看補幀名額，保留 Jellyfin 原始轉碼。playSessionId={PlaySessionId}", new object[1] { playSessionId });
					}
					return;
				}
			}
			HardwareCapabilityService? capabilities = _capabilities;
			if (capabilities == null || !capabilities.Current.CanEnableFrameGeneration)
			{
				_concurrentViewingLimiter?.Release(playSessionId);
				ILogger? logger3 = _logger;
				if (logger3 != null)
				{
					LoggerExtensions.LogWarning(logger3, "JVFI：官方 FFmpeg 沒有通過可用補幀 backend self-test，保留 Jellyfin 原始轉碼命令。", Array.Empty<object>());
				}
				return;
			}
			string text = (string.IsNullOrWhiteSpace(playSessionId) ? Path.GetFileNameWithoutExtension(outputPath) : playSessionId);
			MediaStream videoStream = ((EncodingJobInfo)state).VideoStream;
			InterpolationProfile interpolationProfile = SelectProfile(pluginConfiguration, (videoStream != null) ? videoStream.Width : ((int?)null), _profileOverrides?.Get(text));
			if (interpolationProfile == null || interpolationProfile.TargetFps <= 0.0)
			{
				_concurrentViewingLimiter?.Release(playSessionId);
				ILogger? logger4 = _logger;
				if (logger4 != null)
				{
					LoggerExtensions.LogWarning(logger4, "JVFI：找不到可用補幀模式，保留 Jellyfin 原始轉碼命令。", Array.Empty<object>());
				}
				return;
			}
			FrameInterpolationStateStore stateStore = _stateStore;
			bool flag;
			if (stateStore != null)
			{
				flag = stateStore.ShouldShowStartupHud(text, pluginConfiguration.HudMode, pluginConfiguration.HudSeconds, DateTimeOffset.UtcNow);
			}
			else
			{
				HudMode hudMode = pluginConfiguration.HudMode;
				bool flag2 = ((hudMode == HudMode.Off || hudMode == HudMode.WarningsOnly) ? true : false);
				flag = !flag2;
			}
			bool flag3 = flag;
			string text2 = (flag3 ? HudTextFormatter.FormatStarting(interpolationProfile.TargetFps, pluginConfiguration.InterfaceLanguage) : string.Empty);
			string hudTextFile = _hudFiles?.Initialize(text, text2) ?? string.Empty;
			string metricsFile = _metricsFiles?.Initialize(text) ?? string.Empty;
			string progressFile = _metricsFiles?.InitializeProgress(text) ?? string.Empty;
			int maxWidth = ((!string.Equals(interpolationProfile.Id, "auto60", StringComparison.OrdinalIgnoreCase)) ? interpolationProfile.MaxWidth : 0);
			double targetFps = interpolationProfile.TargetFps;
			bool showHud = pluginConfiguration.HudMode != HudMode.Off;
			string hudPosition = pluginConfiguration.HudPosition.ToString();
			int hudSeconds = ((pluginConfiguration.HudMode == HudMode.FirstSeconds) ? pluginConfiguration.HudSeconds : 0);
			OutputEncoderMode outputEncoderMode = pluginConfiguration.OutputEncoderMode;
			HardwarePipeline jellyfinHardwarePipeline = GetJellyfinHardwarePipeline();
			JVFIQualityProfile quality = interpolationProfile.Quality;
			MediaStream videoStream2 = ((EncodingJobInfo)state).VideoStream;
			int valueOrDefault = ((videoStream2 != null) ? videoStream2.Width : ((int?)null)).GetValueOrDefault();
			MediaStream videoStream3 = ((EncodingJobInfo)state).VideoStream;
			int valueOrDefault2 = ((videoStream3 != null) ? videoStream3.Height : ((int?)null)).GetValueOrDefault();
			int minimum480pBitrateMbps = pluginConfiguration.Minimum480pBitrateMbps;
			int minimum720pBitrateMbps = pluginConfiguration.Minimum720pBitrateMbps;
			int minimum1080pBitrateMbps = pluginConfiguration.Minimum1080pBitrateMbps;
			int minimum4KBitrateMbps = pluginConfiguration.Minimum4KBitrateMbps;
			InterpolationBackend recommendedBackend = _capabilities.Current.RecommendedBackend;
			MediaStream videoStream4 = ((EncodingJobInfo)state).VideoStream;
			FfmpegTransformRequest ffmpegTransformRequest = new FfmpegTransformRequest(targetFps, maxWidth, hudTextFile, showHud, hudPosition, hudSeconds, outputEncoderMode, jellyfinHardwarePipeline, quality, valueOrDefault, valueOrDefault2, minimum480pBitrateMbps, minimum720pBitrateMbps, minimum1080pBitrateMbps, minimum4KBitrateMbps, metricsFile, recommendedBackend, ((videoStream4 != null) ? videoStream4.PixelFormat : null) ?? string.Empty, IsProtectedHdrOrDolbyVision(((EncodingJobInfo)state).VideoStream), progressFile, GetJellyfinQsvDevice(), pluginConfiguration.OutputScaleHeight);
			FfmpegTransformResult ffmpegTransformResult = FfmpegCommandTransformer.Transform(commandLineArguments, ffmpegTransformRequest);
			if (!ffmpegTransformResult.Applied)
			{
				_concurrentViewingLimiter?.Release(playSessionId);
				_stateStore?.Fail(new ErrorReportRequest
				{
					SessionId = text,
					ProfileId = interpolationProfile.Id,
					Reason = ffmpegTransformResult.Reason,
					Detail = "FFmpeg 命令未修改。"
				});
				ILogger? logger5 = _logger;
				if (logger5 != null)
				{
					LoggerExtensions.LogWarning(logger5, "JVFI：{Reason}", new object[1] { ffmpegTransformResult.Reason });
				}
				return;
			}
			commandLineArguments = ffmpegTransformResult.CommandLine;
			if (ffmpegTransformResult.EncoderFallback)
			{
				ILogger? logger6 = _logger;
				if (logger6 != null)
				{
					LoggerExtensions.LogWarning(logger6, "JVFI：指定的輸出編碼器與 Jellyfin 硬體設定不相容，已保留原編碼器 {Encoder}。", new object[1] { ffmpegTransformResult.OutputEncoder });
				}
			}
			text2 = (flag3 ? HudTextFormatter.FormatStarting(interpolationProfile.TargetFps, pluginConfiguration.InterfaceLanguage) : string.Empty);
			_hudFiles?.Update(text, text2);
			FrameInterpolationStateStore? stateStore2 = _stateStore;
			if (stateStore2 != null)
			{
				SessionStartRequest sessionStartRequest = new SessionStartRequest
				{
					SessionId = text
				};
				User user = ((EncodingJobInfo)state).User;
				sessionStartRequest.User = ((user != null) ? user.Username : null) ?? string.Empty;
				sessionStartRequest.Device = ((BaseEncodingJobOptions)state.Request).DeviceId ?? string.Empty;
				MediaSourceInfo mediaSource = ((EncodingJobInfo)state).MediaSource;
				sessionStartRequest.ItemId = ((mediaSource != null) ? mediaSource.Id : null) ?? string.Empty;
				MediaSourceInfo mediaSource2 = ((EncodingJobInfo)state).MediaSource;
				sessionStartRequest.ItemName = ((mediaSource2 != null) ? mediaSource2.Name : null) ?? Path.GetFileName(((EncodingJobInfo)state).MediaPath);
				sessionStartRequest.ProfileId = pluginConfiguration.DefaultProfileId;
				MediaStream videoStream5 = ((EncodingJobInfo)state).VideoStream;
				sessionStartRequest.InputFps = ((videoStream5 != null) ? videoStream5.ReferenceFrameRate : ((float?)null)).GetValueOrDefault();
				sessionStartRequest.OutputEncoder = ffmpegTransformResult.OutputEncoder;
				MediaStream videoStream6 = ((EncodingJobInfo)state).VideoStream;
				sessionStartRequest.SourceWidth = ((videoStream6 != null) ? videoStream6.Width : ((int?)null)).GetValueOrDefault();
				MediaStream videoStream7 = ((EncodingJobInfo)state).VideoStream;
				sessionStartRequest.SourceHeight = ((videoStream7 != null) ? videoStream7.Height : ((int?)null)).GetValueOrDefault();
				stateStore2.Start(sessionStartRequest);
			}
			FrameInterpolationStateStore? stateStore3 = _stateStore;
			if (stateStore3 != null)
			{
				StatusUpdateRequest statusUpdateRequest = new StatusUpdateRequest
				{
					SessionId = text,
					ProfileId = interpolationProfile.Id,
					OutputFps = interpolationProfile.TargetFps
				};
				MediaStream videoStream8 = ((EncodingJobInfo)state).VideoStream;
				statusUpdateRequest.Width = AdaptiveProfilePolicy.GetOutputWidth(((videoStream8 != null) ? videoStream8.Width : ((int?)null)).GetValueOrDefault(), maxWidth);
				int? outputHeight = ((EncodingJobInfo)state).OutputHeight;
				int valueOrDefault3;
				if (!outputHeight.HasValue)
				{
					MediaStream videoStream9 = ((EncodingJobInfo)state).VideoStream;
					valueOrDefault3 = ((videoStream9 != null) ? videoStream9.Height : ((int?)null)).GetValueOrDefault();
				}
				else
				{
					valueOrDefault3 = outputHeight.GetValueOrDefault();
				}
				statusUpdateRequest.Height = valueOrDefault3;
				statusUpdateRequest.OutputEncoder = ffmpegTransformResult.OutputEncoder;
				statusUpdateRequest.HudText = text2;
				stateStore3.Update(statusUpdateRequest);
			}
			ILogger? logger7 = _logger;
			if (logger7 != null)
			{
				LoggerExtensions.LogInformation(logger7, "JVFI：已套用統一補幀，session={SessionId} profile={Profile} activeBackend={ActiveBackend} pipeline={Pipeline} encoder={Encoder} targetFps={TargetFps}", new object[6] { text, interpolationProfile.Id, ffmpegTransformRequest.Backend, ffmpegTransformResult.Pipeline, ffmpegTransformResult.OutputEncoder, interpolationProfile.TargetFps });
			}
		}
		catch (Exception ex)
		{
			_concurrentViewingLimiter?.Release(playSessionId);
			ILogger? logger8 = _logger;
			if (logger8 != null)
			{
				LoggerExtensions.LogError(logger8, ex, "JVFI：補幀命令建立失敗，已保留 Jellyfin 原始轉碼命令。", Array.Empty<object>());
			}
		}
	}

	private static bool IsProtectedHdrOrDolbyVision(MediaStream? stream)
	{
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		if (stream == null)
		{
			return false;
		}
		string text = ((object)stream.VideoRangeType/*cast due to constrained. prefix*/).ToString();
		if (!string.IsNullOrWhiteSpace(text) && !string.Equals(text, "Unknown", StringComparison.OrdinalIgnoreCase) && !string.Equals(text, "SDR", StringComparison.OrdinalIgnoreCase))
		{
			return true;
		}
		int? dvProfile = stream.DvProfile;
		if (!dvProfile.HasValue || dvProfile.GetValueOrDefault() <= 0)
		{
			dvProfile = stream.RpuPresentFlag;
			if (!dvProfile.HasValue || dvProfile.GetValueOrDefault() <= 0)
			{
				dvProfile = stream.ElPresentFlag;
				if (dvProfile.HasValue)
				{
					return dvProfile.GetValueOrDefault() > 0;
				}
				return false;
			}
		}
		return true;
	}

	private static InterpolationProfile? SelectProfile(PluginConfiguration config, int? sourceWidth, string? overrideProfileId)
	{
		if (!string.IsNullOrWhiteSpace(overrideProfileId))
		{
			return config.Profiles.FirstOrDefault((InterpolationProfile profile) => profile.Enabled && profile.Id == overrideProfileId);
		}
		if (!string.Equals(config.DefaultProfileId, "auto60", StringComparison.OrdinalIgnoreCase))
		{
			return config.Profiles.FirstOrDefault((InterpolationProfile profile) => profile.Enabled && profile.Id == config.DefaultProfileId);
		}
		return config.Profiles.FirstOrDefault((InterpolationProfile profile) => profile.Enabled && profile.Id == "auto60");
	}

	private static HardwarePipeline GetJellyfinHardwarePipeline()
	{
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		try
		{
			IConfigurationManager? configurationManager = _configurationManager;
			return OutputEncoderSelector.FromJellyfinHardware((configurationManager != null) ? ((object)EncodingConfigurationExtensions.GetEncodingOptions(configurationManager).HardwareAccelerationType/*cast due to constrained. prefix*/).ToString() : null);
		}
		catch (Exception ex)
		{
			ILogger? logger = _logger;
			if (logger != null)
			{
				LoggerExtensions.LogWarning(logger, ex, "JVFI：無法讀取 Jellyfin 硬體加速設定，將只依 FFmpeg 命令自動判斷。", Array.Empty<object>());
			}
			return HardwarePipeline.Unknown;
		}
	}

	private static string GetJellyfinQsvDevice()
	{
		try
		{
			IConfigurationManager? configurationManager = _configurationManager;
			return ((configurationManager != null) ? EncodingConfigurationExtensions.GetEncodingOptions(configurationManager).QsvDevice : null) ?? string.Empty;
		}
		catch (Exception ex)
		{
			ILogger? logger = _logger;
			if (logger != null)
			{
				LoggerExtensions.LogWarning(logger, ex, "JVFI：無法讀取 Jellyfin QSV 裝置設定，不自行猜測 GPU device。", Array.Empty<object>());
			}
			return string.Empty;
		}
	}

	private static bool IsPlaybackScopeSelected(BaseItem item, Guid userId, PluginConfiguration config)
	{
		if (config.UserLibrarySelections == null)
		{
			if (UserSelectionPolicy.IsSelected(userId, config.SelectedUserIds))
			{
				return IsItemInSelectedLibrary(item, config.SelectedLibraryIds, config.SelectedLibraryPaths);
			}
			return false;
		}
		UserLibrarySelection userLibrarySelection = FindUserLibrarySelection(userId, config.UserLibrarySelections);
		if (userLibrarySelection != null)
		{
			return IsItemInSelectedLibrary(item, userLibrarySelection.LibraryIds, userLibrarySelection.LibraryPaths);
		}
		return false;
	}

	private static bool IsSelectedScopeForTranscode(string? playSessionId, string? mediaPath, PluginConfiguration config)
	{
		if (config.UserLibrarySelections == null)
		{
			if (IsSelectedUserForTranscode(playSessionId, config))
			{
				return IsMediaPathInSelectedLibrary(mediaPath, config.SelectedLibraryIds, config.SelectedLibraryPaths);
			}
			return false;
		}
		if (string.IsNullOrWhiteSpace(playSessionId) || !PlaybackUsers.TryGetValue(playSessionId, out PlaybackUserContext value))
		{
			return false;
		}
		PlaybackUsers[playSessionId] = value with
		{
			LastSeen = DateTimeOffset.UtcNow
		};
		UserLibrarySelection userLibrarySelection = FindUserLibrarySelection(value.UserId, config.UserLibrarySelections);
		if (userLibrarySelection != null)
		{
			return IsMediaPathInSelectedLibrary(mediaPath, userLibrarySelection.LibraryIds, userLibrarySelection.LibraryPaths);
		}
		return false;
	}

	private static UserLibrarySelection? FindUserLibrarySelection(Guid userId, IEnumerable<UserLibrarySelection> selections)
	{
		if (userId == Guid.Empty)
		{
			return null;
		}
		foreach (UserLibrarySelection selection in selections)
		{
			if (Guid.TryParse(selection.UserId, out var result) && result == userId)
			{
				return selection;
			}
			if (string.Equals(selection.UserId, userId.ToString("N"), StringComparison.OrdinalIgnoreCase) || string.Equals(selection.UserId, userId.ToString("D"), StringComparison.OrdinalIgnoreCase))
			{
				return selection;
			}
		}
		return null;
	}

	private static bool IsMediaPathInSelectedLibrary(string? mediaPath, string[]? selectedLibraryIds, string[]? selectedLibraryPaths)
	{
		if (selectedLibraryIds == null)
		{
			return true;
		}
		if (selectedLibraryIds.Length == 0)
		{
			return false;
		}
		try
		{
			if (!string.IsNullOrWhiteSpace(mediaPath))
			{
				ILibraryManager? libraryManager = _libraryManager;
				BaseItem val = ((libraryManager != null) ? libraryManager.FindByPath(mediaPath, (bool?)null) : null);
				if (val != null)
				{
					return IsItemInSelectedLibrary(val, selectedLibraryIds, selectedLibraryPaths);
				}
			}
		}
		catch (Exception ex)
		{
			ILogger? logger = _logger;
			if (logger != null)
			{
				LoggerExtensions.LogDebug(logger, ex, "JVFI：無法由媒體路徑解析 Jellyfin 媒體庫，改用媒體庫根路徑比對。path={MediaPath}", new object[1] { mediaPath });
			}
		}
		return LibrarySelectionPolicy.IsSelected(mediaPath, selectedLibraryPaths);
	}

	private static bool IsItemInSelectedLibrary(BaseItem item, string[]? selectedLibraryIds, string[]? selectedLibraryPaths)
	{
		if (selectedLibraryIds == null)
		{
			return true;
		}
		if (selectedLibraryIds.Length == 0)
		{
			return false;
		}
		try
		{
			ILibraryManager? libraryManager = _libraryManager;
			List<Folder> list = ((libraryManager != null) ? libraryManager.GetCollectionFolders(item) : null);
			if (list != null)
			{
				bool flag = false;
				foreach (Folder item2 in list)
				{
					flag = true;
					if (IsSelectedLibraryId(((BaseItem)item2).Id, selectedLibraryIds))
					{
						return true;
					}
				}
				if (flag)
				{
					return false;
				}
			}
		}
		catch (Exception ex)
		{
			ILogger? logger = _logger;
			if (logger != null)
			{
				LoggerExtensions.LogDebug(logger, ex, "JVFI：無法解析 item 所屬 Jellyfin 媒體庫，改用路徑比對。item={ItemId}", new object[1] { item.Id });
			}
		}
		return LibrarySelectionPolicy.IsSelected(item.Path, selectedLibraryPaths);
	}

	private static bool IsSelectedLibraryId(Guid folderId, IEnumerable<string> selectedIds)
	{
		foreach (string selectedId in selectedIds)
		{
			if (Guid.TryParse(selectedId, out var result) && result == folderId)
			{
				return true;
			}
			if (string.Equals(selectedId, folderId.ToString("N"), StringComparison.OrdinalIgnoreCase) || string.Equals(selectedId, folderId.ToString("D"), StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static void ReleasePlaybackSession(string? playSessionId)
	{
		if (!string.IsNullOrWhiteSpace(playSessionId))
		{
			PlaybackUsers.TryRemove(playSessionId, out PlaybackUserContext _);
		}
	}

	private static void TrackPlaybackUser(string? playSessionId, Guid userId)
	{
		if (string.IsNullOrWhiteSpace(playSessionId) || userId == Guid.Empty)
		{
			return;
		}
		DateTimeOffset utcNow = DateTimeOffset.UtcNow;
		PlaybackUsers[playSessionId] = new PlaybackUserContext(userId, utcNow);
		foreach (KeyValuePair<string, PlaybackUserContext> playbackUser in PlaybackUsers)
		{
			if (utcNow - playbackUser.Value.LastSeen > PlaybackUserRetention)
			{
				PlaybackUsers.TryRemove(playbackUser.Key, out PlaybackUserContext _);
			}
		}
	}

	private static bool IsSelectedUserForTranscode(string? playSessionId, PluginConfiguration config)
	{
		if (config.SelectedUserIds == null)
		{
			return true;
		}
		if (config.SelectedUserIds.Length == 0 || string.IsNullOrWhiteSpace(playSessionId))
		{
			return false;
		}
		if (!PlaybackUsers.TryGetValue(playSessionId, out PlaybackUserContext value))
		{
			return false;
		}
		PlaybackUsers[playSessionId] = value with
		{
			LastSeen = DateTimeOffset.UtcNow
		};
		return UserSelectionPolicy.IsSelected(value.UserId, config.SelectedUserIds);
	}
}
