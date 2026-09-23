namespace Jellyfin.Plugin.JVFI.Runtime;

public sealed record PlaybackDecisionResult(bool Apply, bool EnableDirectPlay, bool EnableDirectStream, bool EnableTranscoding, bool AllowVideoStreamCopy);
