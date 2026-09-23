namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Result of a safe FFmpeg command transformation.</summary>
public sealed record FfmpegTransformResult(bool Applied, string CommandLine, HardwarePipeline Pipeline, string Reason, string OutputEncoder = "", HardwarePipeline EncoderPipeline = HardwarePipeline.Unknown, bool EncoderFallback = false);
