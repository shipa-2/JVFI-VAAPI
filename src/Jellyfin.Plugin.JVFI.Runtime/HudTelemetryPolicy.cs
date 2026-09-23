namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Prevents incomplete FFmpeg startup telemetry from appearing as a false low frame rate.</summary>
public static class HudTelemetryPolicy
{
	private const int MinimumSamples = 2;

	private const int ForcedDisplaySamples = 8;

	private const double PlausibleRatio = 0.75;

	public static bool IsReady(double targetFps, double processingFps, double averageSpeed, int sampleCount)
	{
		if (targetFps <= 0.0 || processingFps <= 0.0 || averageSpeed <= 0.0 || sampleCount < 2)
		{
			return false;
		}
		if (sampleCount >= 8)
		{
			return true;
		}
		return processingFps / averageSpeed >= targetFps * 0.75;
	}
}
