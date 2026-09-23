namespace Jellyfin.Plugin.JVFI.Services;

public sealed record FfmpegProgressSnapshot(long Frame, double? Fps, string Progress);
