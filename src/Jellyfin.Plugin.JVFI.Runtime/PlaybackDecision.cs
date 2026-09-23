namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Pure playback-decision logic used by the Jellyfin runtime patch.</summary>
public static class PlaybackDecision
{
	public static PlaybackDecisionResult Decide(bool enabled, bool isVideo, bool enableDirectPlay, bool enableDirectStream, bool enableTranscoding, bool allowVideoStreamCopy)
	{
		if (!enabled || !isVideo)
		{
			return new PlaybackDecisionResult(Apply: false, enableDirectPlay, enableDirectStream, enableTranscoding, allowVideoStreamCopy);
		}
		return new PlaybackDecisionResult(Apply: true, EnableDirectPlay: false, EnableDirectStream: false, EnableTranscoding: true, AllowVideoStreamCopy: false);
	}
}
