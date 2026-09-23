namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Builds short self-tests against capabilities available in official jellyfin-ffmpeg.</summary>
public static class HardwareSelfTestCommandBuilder
{
	public static string BuildFramerate()
	{
		return "-hide_banner -loglevel error -f lavfi -i testsrc2=size=64x64:rate=24:duration=0.25 -vf \"framerate=fps=60\" -f null -";
	}

	public static string BuildMinterpolate()
	{
		return "-hide_banner -loglevel error -f lavfi -i testsrc2=size=64x64:rate=24:duration=0.25 -vf \"minterpolate=fps=60:mi_mode=mci:mc_mode=aobmc:me_mode=bilat:me=epzs:mb_size=8:vsbmc=1\" -f null -";
	}

	/// <summary>
	/// Tests whether vpp_qsv can perform frame-rate conversion on the current host.
	/// A pass does NOT prove Intel advanced motion interpolation; that still requires a oneVPL capability query.
	/// </summary>
	public static string BuildQsvFrc()
	{
		return "-hide_banner -loglevel error -init_hw_device qsv=hw -filter_hw_device hw -f lavfi -i testsrc2=size=64x64:rate=24:duration=0.25 -vf \"format=nv12,hwupload=extra_hw_frames=16,vpp_qsv=framerate=60,hwdownload,format=nv12\" -f null -";
	}
}
