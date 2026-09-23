namespace Jellyfin.Plugin.JVFI.Runtime;

public sealed record FfmpegFrameRateObservation(double AchievedFps, double RawThroughputFps, double FrameIntervalMs, long DeltaFrames, double SampleWindowMs);
