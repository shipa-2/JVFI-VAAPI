namespace Jellyfin.Plugin.JVFI.Runtime;

public sealed record TranscodeSpeedObservation(double Instantaneous, double Average, int SampleCount);
