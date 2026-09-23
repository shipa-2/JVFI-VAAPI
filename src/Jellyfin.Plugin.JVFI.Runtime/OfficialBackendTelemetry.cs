namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Estimated counters for an official FFmpeg interpolation backend.</summary>
public sealed record OfficialBackendTelemetry(long OutputFrames, long GeneratedFrames, double ProcessedMediaSeconds);
