namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Interpolation engines usable without requiring a custom jellyfin-ffmpeg binary.</summary>
public enum InterpolationBackend
{
	None,
	OfficialFramerate,
	OfficialMinterpolate,
	IntelQsvFrc,
	NativeCpu,
	NativeExternal,
	Duplicate
}
