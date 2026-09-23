using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.JVFI.Configuration;
using Jellyfin.Plugin.JVFI.Models;
using Jellyfin.Plugin.JVFI.Runtime;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.MediaEncoding;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JVFI.Services;

/// <summary>Synchronizes Jellyfin transcoding jobs with JVFI status and HUD files.</summary>
public sealed class TranscodingStatusMonitor : BackgroundService
{
	private sealed class LoadSamples
	{
		public int Low { get; set; }

		public int High { get; set; }
	}

	private const int DowngradeSamples = 8;

	private const int RecoverySamples = 30;

	private readonly ITranscodeManager _transcodeManager;

	private readonly FrameInterpolationStateStore _stateStore;

	private readonly HudFileStore _hudFiles;

	private readonly MetricsFileStore _metricsFiles;

	private readonly AdaptiveProfileStore _profileOverrides;

	private readonly ILogger<TranscodingStatusMonitor> _logger;

	private readonly TranscodeSpeedTracker _speedTracker = new TranscodeSpeedTracker();

	private readonly OfficialBackendTelemetryEstimator _officialTelemetry = new OfficialBackendTelemetryEstimator();

	private readonly FfmpegFrameRateTracker _frameRateTracker = new FfmpegFrameRateTracker();

	private readonly FpsDropEventTracker _dropTracker = new FpsDropEventTracker();

	private readonly ConcurrentDictionary<string, LoadSamples> _loadSamples = new ConcurrentDictionary<string, LoadSamples>(StringComparer.Ordinal);

	public TranscodingStatusMonitor(ITranscodeManager transcodeManager, FrameInterpolationStateStore stateStore, HudFileStore hudFiles, MetricsFileStore metricsFiles, AdaptiveProfileStore profileOverrides, ILogger<TranscodingStatusMonitor> logger)
	{
		_transcodeManager = transcodeManager;
		_stateStore = stateStore;
		_hudFiles = hudFiles;
		_metricsFiles = metricsFiles;
		_profileOverrides = profileOverrides;
		_logger = logger;
	}

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using PeriodicTimer timer = new PeriodicTimer(TimeSpan.FromMilliseconds(50L, 0L));
		while (await timer.WaitForNextTickAsync(stoppingToken).ConfigureAwait(continueOnCapturedContext: false))
		{
			await PollAsync(stoppingToken).ConfigureAwait(continueOnCapturedContext: false);
		}
	}

	private async Task PollAsync(CancellationToken cancellationToken)
	{
		PluginConfiguration pluginConfiguration = ((BasePlugin<PluginConfiguration>)(object)Plugin.Instance)?.Configuration;
		if (pluginConfiguration == null)
		{
			return;
		}
		foreach (FrameInterpolationSession item in _stateStore.GetSessions().Where(IsRunning))
		{
			FfmpegProgressSnapshot ffmpegProgressSnapshot = _metricsFiles.ReadProgress(item.SessionId);
			if (IsProgressEnded(ffmpegProgressSnapshot))
			{
				CleanupSession(item.SessionId, "FFmpeg progress=end");
				continue;
			}
			TranscodingJob transcodingJob = _transcodeManager.GetTranscodingJob(item.SessionId);
			if (transcodingJob == null)
			{
				if (DateTimeOffset.UtcNow - item.UpdatedAt > TimeSpan.FromSeconds(12L))
				{
					CleanupSession(item.SessionId, "Jellyfin transcode job missing");
				}
				continue;
			}
			TranscodeSpeedObservation transcodeSpeedObservation = (transcodingJob.TranscodingPositionTicks.HasValue ? _speedTracker.ObserveDetailed(item.SessionId, transcodingJob.TranscodingPositionTicks.Value, DateTimeOffset.UtcNow) : null);
			JVFIMetrics jVFIMetrics = _metricsFiles.Read(item.SessionId);
			double num = jVFIMetrics?.TargetFps ?? GetTargetFps(pluginConfiguration, item.ActiveProfileId);
			FfmpegFrameRateObservation ffmpegFrameRateObservation = (((object)jVFIMetrics == null && (object)ffmpegProgressSnapshot != null) ? _frameRateTracker.Observe(item.SessionId, ffmpegProgressSnapshot.Frame, DateTimeOffset.UtcNow, num) : null);
			double num2 = transcodeSpeedObservation?.Instantaneous ?? item.Speed;
			double averageSpeed = jVFIMetrics?.PipelineSpeed ?? transcodeSpeedObservation?.Average ?? item.AverageSpeed;
			double num3 = jVFIMetrics?.ComputeFps ?? ((num2 > 0.0) ? (num * num2) : 0.0);
			OfficialBackendTelemetry officialBackendTelemetry = null;
			if ((object)jVFIMetrics == null && transcodingJob.TranscodingPositionTicks.HasValue)
			{
				officialBackendTelemetry = _officialTelemetry.Observe(item.SessionId, transcodingJob.TranscodingPositionTicks.Value, DateTimeOffset.UtcNow, item.InputFps, num);
			}
			double timelineFps = jVFIMetrics?.TimelineFps ?? num;
			double num4 = ffmpegFrameRateObservation?.AchievedFps ?? ((item.EstimatedOutputFps > 0.0) ? item.EstimatedOutputFps : 0.0);
			double estimatedOutputFps = (((object)jVFIMetrics != null) ? Math.Clamp(jVFIMetrics.TimelineFps, 0.0, num) : num4);
			long generatedFrames = jVFIMetrics?.GeneratedFrames ?? officialBackendTelemetry?.GeneratedFrames ?? 0;
			long num5 = officialBackendTelemetry?.OutputFrames ?? 0;
			double processedMediaSeconds = officialBackendTelemetry?.ProcessedMediaSeconds ?? ((num5 > 0 && num > 0.0) ? ((double)num5 / num) : 0.0);
			bool generatedFramesEstimated = (object)jVFIMetrics == null;
			string text;
			if ((object)jVFIMetrics == null)
			{
				if (pluginConfiguration.HudDetailMode == HudDetailMode.Diagnostic)
				{
					text = HudTextFormatter.FormatOfficialDiagnostic(num, num4, num3, num2, item.Downgraded, pluginConfiguration.InterfaceLanguage);
				}
				else
				{
					text = (((object)ffmpegFrameRateObservation != null || item.EstimatedOutputFps > 0.0) ? HudTextFormatter.FormatMeasuredAchievement(num, num4, item.Downgraded, pluginConfiguration.InterfaceLanguage) : HudTextFormatter.FormatStarting(num, pluginConfiguration.InterfaceLanguage));
				}
			}
			else
			{
				text = ((pluginConfiguration.HudDetailMode == HudDetailMode.Diagnostic) ? HudTextFormatter.FormatDiagnostic(jVFIMetrics, item.Downgraded, pluginConfiguration.InterfaceLanguage) : HudTextFormatter.FormatCompact(jVFIMetrics, item.Downgraded, pluginConfiguration.InterfaceLanguage));
			}
			if (!HudDisplayPolicy.ShouldShow(pluginConfiguration.HudMode, pluginConfiguration.HudSeconds, item.StartedAt, DateTimeOffset.UtcNow, item.Downgraded))
			{
				text = string.Empty;
			}
			_stateStore.Update(new StatusUpdateRequest
			{
				SessionId = item.SessionId,
				ProfileId = item.ActiveProfileId,
				OutputFps = num,
				ProcessingFps = num3,
				Speed = num2,
				AverageSpeed = averageSpeed,
				EstimatedOutputFps = estimatedOutputFps,
				Width = item.Width,
				Height = item.Height,
				Downgraded = item.Downgraded,
				OutputEncoder = item.OutputEncoder,
				HudText = text,
				TimelineFps = timelineFps,
				ComputeFps = num3,
				GeneratedFrames = generatedFrames,
				GeneratedFramesEstimated = generatedFramesEstimated,
				OutputFrames = num5,
				ProcessedMediaSeconds = processedMediaSeconds,
				FallbackFrames = (jVFIMetrics?.FallbackFrames ?? 0),
				ClientRenderedFps = jVFIMetrics?.ClientRenderedFps,
				ClientDroppedFrames = jVFIMetrics?.ClientDroppedFrames
			});
			_hudFiles.Update(item.SessionId, text);
			if ((object)ffmpegFrameRateObservation != null)
			{
				FpsDropEvent fpsDropEvent = _dropTracker.Observe(item.SessionId, ffmpegFrameRateObservation.AchievedFps, num, ffmpegFrameRateObservation.FrameIntervalMs, DateTimeOffset.UtcNow);
				FpsDropEventKind kind = fpsDropEvent.Kind;
				if ((uint)(kind - 1) <= 1u)
				{
					LoggerExtensions.LogWarning((ILogger)(object)_logger, "JVFI FPS Drop: session={SessionId} currentFps={CurrentFps:F2} targetFps={TargetFps:F2} frameIntervalMs={FrameIntervalMs:F2} minimumFps={MinimumFps:F2} durationMs={DurationMs:F0} severity={Severity} profile={Profile} encoder={Encoder}", new object[9] { item.SessionId, fpsDropEvent.CurrentFps, fpsDropEvent.TargetFps, fpsDropEvent.FrameIntervalMs, fpsDropEvent.MinimumFps, fpsDropEvent.DurationMs, fpsDropEvent.Severity, item.ActiveProfileId, item.OutputEncoder });
				}
				else if (fpsDropEvent.Kind == FpsDropEventKind.Recovered)
				{
					LoggerExtensions.LogInformation((ILogger)(object)_logger, "JVFI FPS Recovered: session={SessionId} currentFps={CurrentFps:F2} targetFps={TargetFps:F2} minimumFps={MinimumFps:F2} dropDurationMs={DurationMs:F0} profile={Profile}", new object[6] { item.SessionId, fpsDropEvent.CurrentFps, fpsDropEvent.TargetFps, fpsDropEvent.MinimumFps, fpsDropEvent.DurationMs, item.ActiveProfileId });
				}
			}
		}
	}

	private static bool IsProgressEnded(FfmpegProgressSnapshot? progress)
	{
		return string.Equals(progress?.Progress, "end", StringComparison.OrdinalIgnoreCase);
	}

	private void CleanupSession(string sessionId, string reason)
	{
		_stateStore.Remove(sessionId);
		_speedTracker.Remove(sessionId);
		_officialTelemetry.Remove(sessionId);
		_frameRateTracker.Remove(sessionId);
		_dropTracker.Remove(sessionId);
		_metricsFiles.Remove(sessionId);
		_hudFiles.Remove(sessionId);
		_loadSamples.TryRemove(sessionId, out LoadSamples _);
		_profileOverrides.Clear(sessionId);
		LoggerExtensions.LogInformation((ILogger)(object)_logger, "JVFI：已清理結束的補幀工作階段，session={SessionId} reason={Reason}", new object[2] { sessionId, reason });
	}

	private async Task EvaluateAdaptiveProfileAsync(FrameInterpolationSession session, string? deviceId, double speed, PluginConfiguration config, CancellationToken cancellationToken)
	{
		LoadSamples orAdd = _loadSamples.GetOrAdd(session.SessionId, (string _) => new LoadSamples());
		orAdd.Low = ((speed < config.DowngradeSpeedThreshold) ? (orAdd.Low + 1) : 0);
		orAdd.High = ((speed > config.RecoverySpeedThreshold) ? (orAdd.High + 1) : 0);
		int num = ((orAdd.Low >= 8) ? 1 : ((orAdd.High >= 30) ? (-1) : 0));
		if (num == 0)
		{
			return;
		}
		orAdd.Low = 0;
		orAdd.High = 0;
		InterpolationProfile interpolationProfile = AdaptiveProfilePolicy.FindNext(config.Profiles, config.AutoProfiles, session.ActiveProfileId, session.SourceWidth, num);
		if (interpolationProfile != null && !string.IsNullOrWhiteSpace(deviceId))
		{
			_profileOverrides.Set(session.SessionId, interpolationProfile.Id);
			bool downgraded = !string.Equals(interpolationProfile.Id, "auto60", StringComparison.OrdinalIgnoreCase);
			string text = ((num > 0) ? ("補幀負擔過高，自動切換 " + interpolationProfile.Name) : ("補幀負擔恢復，自動切換 " + interpolationProfile.Name));
			_stateStore.Update(new StatusUpdateRequest
			{
				SessionId = session.SessionId,
				ProfileId = interpolationProfile.Id,
				OutputFps = interpolationProfile.TargetFps,
				ProcessingFps = session.ProcessingFps,
				Speed = speed,
				AverageSpeed = session.AverageSpeed,
				EstimatedOutputFps = session.EstimatedOutputFps,
				Width = ((interpolationProfile.MaxWidth > 0) ? Math.Min(session.SourceWidth, interpolationProfile.MaxWidth) : session.SourceWidth),
				Height = ((interpolationProfile.MaxWidth > 0 && session.SourceWidth > 0) ? ((int)Math.Round((double)session.SourceHeight * (double)Math.Min(session.SourceWidth, interpolationProfile.MaxWidth) / (double)session.SourceWidth)) : session.SourceHeight),
				Downgraded = downgraded,
				OutputEncoder = session.OutputEncoder,
				HudText = text,
				TimelineFps = session.TimelineFps,
				ComputeFps = session.ComputeFps,
				GeneratedFrames = session.GeneratedFrames,
				GeneratedFramesEstimated = session.GeneratedFramesEstimated,
				OutputFrames = session.OutputFrames,
				ProcessedMediaSeconds = session.ProcessedMediaSeconds,
				FallbackFrames = session.FallbackFrames,
				ClientRenderedFps = session.ClientRenderedFps,
				ClientDroppedFrames = session.ClientDroppedFrames
			});
			_hudFiles.Update(session.SessionId, text);
			LoggerExtensions.LogWarning((ILogger)(object)_logger, "JVFI：{Hud}，session={SessionId} speed={Speed}", new object[3] { text, session.SessionId, speed });
			await _transcodeManager.KillTranscodingJobs(deviceId, session.SessionId, (Func<string, bool>)((string _) => true)).ConfigureAwait(continueOnCapturedContext: false);
			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	private static InterpolationProfile? GetProfile(PluginConfiguration config, string id)
	{
		return config.Profiles.FirstOrDefault((InterpolationProfile profile) => profile.Id == id);
	}

	private static double GetTargetFps(PluginConfiguration config, string id)
	{
		return GetProfile(config, id)?.TargetFps ?? 60.0;
	}

	private static bool IsRunning(FrameInterpolationSession session)
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
	}
}
