using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.JVFI.Patching;
using MediaBrowser.Controller.Library;
using MediaBrowser.Controller.Session;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JVFI.Services;

/// <summary>
/// Releases concurrent-viewing reservations only when Jellyfin reports that the playback itself stopped.
/// FFmpeg child-process restarts for seek/HLS rebuild must not release the PlaySessionId slot.
/// </summary>
public sealed class PlaybackReservationMonitor : IHostedService
{
	private readonly ISessionManager _sessionManager;

	private readonly ConcurrentViewingLimiter _limiter;

	private readonly ILogger<PlaybackReservationMonitor> _logger;

	public PlaybackReservationMonitor(ISessionManager sessionManager, ConcurrentViewingLimiter limiter, ILogger<PlaybackReservationMonitor> logger)
	{
		_sessionManager = sessionManager;
		_limiter = limiter;
		_logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		_sessionManager.PlaybackStopped += OnPlaybackStopped;
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_sessionManager.PlaybackStopped -= OnPlaybackStopped;
		return Task.CompletedTask;
	}

	private void OnPlaybackStopped(object? sender, PlaybackStopEventArgs e)
	{
		string playSessionId = ((PlaybackProgressEventArgs)e).PlaySessionId;
		if (!string.IsNullOrWhiteSpace(playSessionId))
		{
			bool flag = _limiter.Release(playSessionId);
			JVFIPatch.ReleasePlaybackSession(playSessionId);
			if (flag)
			{
				LoggerExtensions.LogInformation((ILogger)(object)_logger, "JVFI：播放已結束，釋放同時觀看名額。playSessionId={PlaySessionId} activeSessions={ActiveSessions}", new object[2] { playSessionId, _limiter.ActiveSessionCount });
			}
		}
	}
}
