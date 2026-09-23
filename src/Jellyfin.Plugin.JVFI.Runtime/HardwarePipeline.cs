namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>FFmpeg hardware path detected from a Jellyfin command line.</summary>
public enum HardwarePipeline
{
	Software,
	Vaapi,
	Qsv,
	Cuda,
	Amf,
	VideoToolbox,
	Rkmpp,
	Unknown
}
