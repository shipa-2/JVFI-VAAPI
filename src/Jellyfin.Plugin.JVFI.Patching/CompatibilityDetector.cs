using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Jellyfin.Plugin.JVFI.Patching;

/// <summary>Finds Jellyfin runtime methods and refuses to patch changed signatures.</summary>
public static class CompatibilityDetector
{
	private const string MediaInfoHelperTypeName = "Jellyfin.Api.Helpers.MediaInfoHelper, Jellyfin.Api";

	private const string TranscodeManagerTypeName = "MediaBrowser.MediaEncoding.Transcoding.TranscodeManager, MediaBrowser.MediaEncoding";

	public static PatchReport Detect()
	{
		Type type = Type.GetType("Jellyfin.Api.Helpers.MediaInfoHelper, Jellyfin.Api", throwOnError: false);
		Type type2 = Type.GetType("MediaBrowser.MediaEncoding.Transcoding.TranscodeManager, MediaBrowser.MediaEncoding", throwOnError: false);
		string jellyfinVersion = type2?.Assembly.GetName().Version?.ToString() ?? type?.Assembly.GetName().Version?.ToString() ?? "未知";
		if ((object)type == null || (object)type2 == null)
		{
			return new PatchReport(Compatible: false, jellyfinVersion, null, null, "找不到 Jellyfin 播放或轉碼元件。");
		}
		MethodInfo methodInfo = type.GetMethods(BindingFlags.Instance | BindingFlags.Public).SingleOrDefault((MethodInfo method) => method.Name == "SetDeviceSpecificData" && HasPlaybackSignature(method));
		MethodInfo methodInfo2 = type2.GetMethods(BindingFlags.Instance | BindingFlags.Public).SingleOrDefault((MethodInfo method) => method.Name == "StartFfMpeg" && HasTranscodeSignature(method));
		if ((object)methodInfo == null || (object)methodInfo2 == null)
		{
			return new PatchReport(Compatible: false, jellyfinVersion, methodInfo, methodInfo2, "Jellyfin 方法簽章與 10.11.11 不相容，已安全停用補幀掛接。");
		}
		return new PatchReport(Compatible: true, jellyfinVersion, methodInfo, methodInfo2, "已找到相容的播放與 FFmpeg 掛點。");
	}

	private static bool HasPlaybackSignature(MethodInfo method)
	{
		HashSet<string> hashSet = (from parameter in method.GetParameters()
			select parameter.Name).ToHashSet(StringComparer.Ordinal);
		if (method.ReturnType == typeof(void) && hashSet.Contains("item") && hashSet.Contains("playSessionId") && hashSet.Contains("userId") && hashSet.Contains("enableDirectPlay") && hashSet.Contains("enableDirectStream") && hashSet.Contains("enableTranscoding"))
		{
			return hashSet.Contains("allowVideoStreamCopy");
		}
		return false;
	}

	private static bool HasTranscodeSignature(MethodInfo method)
	{
		ParameterInfo[] parameters = method.GetParameters();
		if (parameters.Length == 7 && parameters[0].Name == "state" && parameters[0].ParameterType.FullName == "MediaBrowser.Controller.Streaming.StreamState" && parameters[1].Name == "outputPath" && parameters[1].ParameterType == typeof(string) && parameters[2].Name == "commandLineArguments")
		{
			return parameters[2].ParameterType == typeof(string);
		}
		return false;
	}
}
