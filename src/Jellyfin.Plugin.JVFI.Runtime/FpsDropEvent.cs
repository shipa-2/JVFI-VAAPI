namespace Jellyfin.Plugin.JVFI.Runtime;

public sealed record FpsDropEvent(FpsDropEventKind Kind, double CurrentFps, double MinimumFps, double TargetFps, double FrameIntervalMs, double DurationMs, string Severity);
