namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Settings required to add frame interpolation to one FFmpeg command.</summary>
public sealed record FfmpegTransformRequest(double TargetFps, int MaxWidth, string HudTextFile, bool ShowHud, string HudPosition, int HudSeconds = 0, OutputEncoderMode EncoderMode = OutputEncoderMode.AutoHardware, HardwarePipeline JellyfinHardwarePipeline = HardwarePipeline.Unknown, JVFIQualityProfile Quality = JVFIQualityProfile.Balanced, int SourceWidth = 0, int SourceHeight = 0, int Minimum480pBitrateMbps = 4, int Minimum720pBitrateMbps = 8, int Minimum1080pBitrateMbps = 16, int Minimum4KBitrateMbps = 40, string MetricsFile = "", InterpolationBackend Backend = InterpolationBackend.None, string SourcePixelFormat = "", bool IsHdr = false, string ProgressFile = "", string JellyfinQsvDevice = "", int OutputScaleHeight = 0)
{
	public FpsRational TargetRate => FpsRational.FromDouble(TargetFps);
}
