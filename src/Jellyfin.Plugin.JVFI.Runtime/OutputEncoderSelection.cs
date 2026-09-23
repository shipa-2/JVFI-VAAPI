namespace Jellyfin.Plugin.JVFI.Runtime;

public sealed record OutputEncoderSelection(string Encoder, HardwarePipeline Pipeline, bool Fallback = false);
