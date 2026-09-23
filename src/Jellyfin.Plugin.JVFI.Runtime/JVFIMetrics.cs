namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>One v1.6 native metrics snapshot produced by vf_jvfi.</summary>
public sealed record JVFIMetrics(double TimelineFps, double TargetFps, double ComputeFps, double PipelineSpeed, long GeneratedFrames, long FallbackFrames, string Backend, double? ClientRenderedFps, long? ClientDroppedFrames, long? ClientRepeatedFrames);
