using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Plugin.JVFI.Configuration;
using Jellyfin.Plugin.JVFI.Models;
using Jellyfin.Plugin.JVFI.Runtime;
using MediaBrowser.Common.Plugins;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JVFI.Services;

/// <summary>
/// Stores live frame interpolation session status in memory.
/// </summary>
public sealed class FrameInterpolationStateStore
{
	private readonly ConcurrentDictionary<string, FrameInterpolationSession> _sessions = new ConcurrentDictionary<string, FrameInterpolationSession>(StringComparer.Ordinal);

	private readonly ILogger<FrameInterpolationStateStore> _logger;

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Jellyfin.Plugin.JVFI.Services.FrameInterpolationStateStore" /> class.
	/// </summary>
	/// <param name="logger">Logger.</param>
	public FrameInterpolationStateStore(ILogger<FrameInterpolationStateStore> logger)
	{
		_logger = logger;
	}

	/// <summary>
	/// Starts or replaces a session.
	/// </summary>
	/// <param name="request">Start request.</param>
	/// <returns>The stored session.</returns>
	public FrameInterpolationSession Start(SessionStartRequest request)
	{
		DateTimeOffset utcNow = DateTimeOffset.UtcNow;
		_sessions.TryGetValue(request.SessionId, out FrameInterpolationSession value);
		FrameInterpolationSession frameInterpolationSession = new FrameInterpolationSession
		{
			SessionId = request.SessionId,
			User = request.User,
			Device = request.Device,
			ItemId = request.ItemId,
			ItemName = request.ItemName,
			RequestedProfileId = request.ProfileId,
			ActiveProfileId = request.ProfileId,
			InputFps = request.InputFps,
			OutputEncoder = request.OutputEncoder,
			SourceWidth = request.SourceWidth,
			SourceHeight = request.SourceHeight,
			StartedAt = (value?.StartedAt ?? utcNow),
			UpdatedAt = utcNow,
			Status = "啟動中",
			HudText = (value?.HudText ?? BuildStartingHudText(((BasePlugin<PluginConfiguration>)(object)Plugin.Instance)?.Configuration.InterfaceLanguage))
		};
		_sessions[request.SessionId] = frameInterpolationSession;
		LoggerExtensions.LogInformation((ILogger)(object)_logger, "JVFI：補幀工作階段已建立，session={SessionId} user={User} device={Device} item={ItemName} profile={ProfileId} inputFps={InputFps}", new object[6] { request.SessionId, request.User, request.Device, request.ItemName, request.ProfileId, request.InputFps });
		return frameInterpolationSession;
	}

	/// <summary>Determines whether the startup HUD window is still open for a playback session.</summary>
	public bool ShouldShowStartupHud(string sessionId, HudMode mode, int seconds, DateTimeOffset now)
	{
		if ((mode == HudMode.Off || mode == HudMode.WarningsOnly) ? true : false)
		{
			return false;
		}
		if (mode == HudMode.Always || !_sessions.TryGetValue(sessionId, out FrameInterpolationSession value))
		{
			return true;
		}
		return HudDisplayPolicy.ShouldShow(mode, seconds, value.StartedAt, now, downgraded: false);
	}

	/// <summary>
	/// Updates a running session.
	/// </summary>
	/// <param name="request">Status update request.</param>
	/// <returns>The updated session, or null when the session is unknown.</returns>
	public FrameInterpolationSession? Update(StatusUpdateRequest request)
	{
		if (!_sessions.TryGetValue(request.SessionId, out FrameInterpolationSession value))
		{
			LoggerExtensions.LogWarning((ILogger)(object)_logger, "JVFI：收到未知工作階段的補幀狀態，已忽略，session={SessionId}", new object[1] { request.SessionId });
			return null;
		}
		value.ActiveProfileId = request.ProfileId;
		value.OutputFps = request.OutputFps;
		value.ProcessingFps = request.ProcessingFps;
		value.Speed = request.Speed;
		value.AverageSpeed = request.AverageSpeed;
		value.EstimatedOutputFps = request.EstimatedOutputFps;
		value.TimelineFps = request.TimelineFps;
		value.ComputeFps = request.ComputeFps;
		value.GeneratedFrames = request.GeneratedFrames;
		value.GeneratedFramesEstimated = request.GeneratedFramesEstimated;
		value.OutputFrames = request.OutputFrames;
		value.ProcessedMediaSeconds = request.ProcessedMediaSeconds;
		value.FallbackFrames = request.FallbackFrames;
		value.ClientRenderedFps = request.ClientRenderedFps;
		value.ClientDroppedFrames = request.ClientDroppedFrames;
		value.Width = request.Width;
		value.Height = request.Height;
		value.Downgraded = request.Downgraded;
		if (!string.IsNullOrWhiteSpace(request.OutputEncoder))
		{
			value.OutputEncoder = request.OutputEncoder;
		}
		value.Status = (request.Downgraded ? "已自動降級" : "補幀中");
		value.HudText = (string.IsNullOrWhiteSpace(request.HudText) ? HudTextFormatter.Format(request.ProfileId, request.OutputFps, request.ProcessingFps, request.AverageSpeed, request.Downgraded, value.OutputEncoder, ((BasePlugin<PluginConfiguration>)(object)Plugin.Instance)?.Configuration.InterfaceLanguage) : request.HudText);
		value.UpdatedAt = DateTimeOffset.UtcNow;
		LoggerExtensions.LogDebug((ILogger)(object)_logger, "JVFI：補幀狀態，session={SessionId} profile={ProfileId} outputFps={OutputFps} speed={Speed} downgraded={Downgraded}", new object[5] { request.SessionId, request.ProfileId, request.OutputFps, request.Speed, request.Downgraded });
		return value;
	}

	/// <summary>Marks a session as completed after Jellyfin removes its transcode job.</summary>
	public FrameInterpolationSession? Complete(string sessionId)
	{
		if (!_sessions.TryGetValue(sessionId, out FrameInterpolationSession value))
		{
			return null;
		}
		value.Status = "已完成";
		value.HudText = string.Empty;
		value.UpdatedAt = DateTimeOffset.UtcNow;
		LoggerExtensions.LogInformation((ILogger)(object)_logger, "JVFI：補幀工作階段已完成，session={SessionId}", new object[1] { sessionId });
		return value;
	}

	/// <summary>Removes a session completely when its FFmpeg processing lifecycle has ended.</summary>
	public bool Remove(string sessionId)
	{
		if (!_sessions.TryRemove(sessionId, out FrameInterpolationSession _))
		{
			return false;
		}
		LoggerExtensions.LogInformation((ILogger)(object)_logger, "JVFI：補幀工作階段已移除，session={SessionId}", new object[1] { sessionId });
		return true;
	}

	/// <summary>
	/// Marks a session as failed.
	/// </summary>
	/// <param name="request">Error request.</param>
	/// <returns>The updated session, or null when the session is unknown.</returns>
	public FrameInterpolationSession? Fail(ErrorReportRequest request)
	{
		if (!_sessions.TryGetValue(request.SessionId, out FrameInterpolationSession value))
		{
			value = new FrameInterpolationSession
			{
				SessionId = request.SessionId,
				ActiveProfileId = request.ProfileId,
				RequestedProfileId = request.ProfileId,
				StartedAt = DateTimeOffset.UtcNow
			};
			_sessions[request.SessionId] = value;
		}
		value.Status = "補幀失敗";
		value.ErrorReason = request.Reason;
		string text = ((BasePlugin<PluginConfiguration>)(object)Plugin.Instance)?.Configuration.InterfaceLanguage;
		string text2;
		if (text != null && text.StartsWith("en", StringComparison.OrdinalIgnoreCase))
		{
			text2 = "Interpolation disabled | fallback";
		}
		else
		{
			text2 = ((text != null && text.StartsWith("ja", StringComparison.OrdinalIgnoreCase)) ? "補間停止 | フォールバック" : "補幀關閉 | 已回退");
		}
		value.HudText = (string.IsNullOrWhiteSpace(request.Reason) ? text2 : (text2 + " | " + request.Reason));
		value.UpdatedAt = DateTimeOffset.UtcNow;
		LoggerExtensions.LogError((ILogger)(object)_logger, "JVFI：補幀失敗，session={SessionId} profile={ProfileId} exitCode={ExitCode} reason={Reason} detail={Detail}", new object[5] { request.SessionId, request.ProfileId, request.ExitCode, request.Reason, request.Detail });
		return value;
	}

	/// <summary>
	/// Gets active and recent sessions.
	/// </summary>
	/// <returns>Sessions ordered by update time descending.</returns>
	public IReadOnlyList<FrameInterpolationSession> GetSessions()
	{
		DateTimeOffset cutoff = DateTimeOffset.UtcNow.AddHours(-6.0);
		FrameInterpolationSession[] array = _sessions.Values.Where((FrameInterpolationSession session) => session.UpdatedAt < cutoff).ToArray();
		foreach (FrameInterpolationSession frameInterpolationSession in array)
		{
			_sessions.TryRemove(frameInterpolationSession.SessionId, out FrameInterpolationSession _);
		}
		return (from session in _sessions.Values.Where((FrameInterpolationSession session) =>
			{
				switch (session.Status)
				{
				case "啟動中":
				case "補幀中":
				case "已自動降級":
					return true;
				default:
					return false;
				}
			})
			orderby session.UpdatedAt descending
			select session).Take(50).ToArray();
	}

	private static string BuildStartingHudText(string? language)
	{
		if (language == null || !language.StartsWith("en", StringComparison.OrdinalIgnoreCase))
		{
			if (language == null || !language.StartsWith("ja", StringComparison.OrdinalIgnoreCase))
			{
				return "補幀偵測中";
			}
			return "補間を検出中";
		}
		return "Detecting interpolation";
	}
}
