namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Output encoder policy selected by the administrator.</summary>
public enum OutputEncoderMode
{
	AutoHardware,
	FollowJellyfin,
	Rkmpp,
	Vaapi,
	Qsv,
	Nvenc,
	Amf,
	VideoToolbox,
	Software
}
