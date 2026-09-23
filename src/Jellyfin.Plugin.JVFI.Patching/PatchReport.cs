using System.Reflection;

namespace Jellyfin.Plugin.JVFI.Patching;

public sealed record PatchReport(bool Compatible, string JellyfinVersion, MethodInfo? PlaybackMethod, MethodInfo? TranscodeMethod, string Message);
