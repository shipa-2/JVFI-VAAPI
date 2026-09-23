using System;
using System.Reflection;
using HarmonyLib;
using Jellyfin.Plugin.JVFI.Services;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Controller.Library;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JVFI.Patching;

/// <summary>Applies and removes JVFI's guarded Jellyfin runtime patches.</summary>
public sealed class JVFIPatcher
{
	private const string HarmonyId = "com.skillgodak.jellyfin.jvfi";

	private readonly FrameInterpolationStateStore _stateStore;

	private readonly HudFileStore _hudFiles;

	private readonly MetricsFileStore _metricsFiles;

	private readonly AdaptiveProfileStore _profileOverrides;

	private readonly IConfigurationManager _configurationManager;

	private readonly ILibraryManager _libraryManager;

	private readonly ConcurrentViewingLimiter _concurrentViewingLimiter;

	private readonly HardwareCapabilityService _capabilities;

	private readonly ILogger<JVFIPatcher> _logger;

	private Harmony? _harmony;

	public JVFIPatcher(FrameInterpolationStateStore stateStore, HudFileStore hudFiles, MetricsFileStore metricsFiles, AdaptiveProfileStore profileOverrides, IConfigurationManager configurationManager, ILibraryManager libraryManager, ConcurrentViewingLimiter concurrentViewingLimiter, HardwareCapabilityService capabilities, ILogger<JVFIPatcher> logger)
	{
		_stateStore = stateStore;
		_hudFiles = hudFiles;
		_metricsFiles = metricsFiles;
		_profileOverrides = profileOverrides;
		_configurationManager = configurationManager;
		_libraryManager = libraryManager;
		_concurrentViewingLimiter = concurrentViewingLimiter;
		_capabilities = capabilities;
		_logger = logger;
	}

	public PatchReport Apply()
	{
		//IL_00b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Expected Obj, but got Unknown
		//IL_00cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e3: Expected Obj, but got Unknown
		//IL_00f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Expected Obj, but got Unknown
		PatchReport patchReport = CompatibilityDetector.Detect();
		if (!patchReport.Compatible || (object)patchReport.PlaybackMethod == null || (object)patchReport.TranscodeMethod == null)
		{
			LoggerExtensions.LogError((ILogger)(object)_logger, "JVFI：{Message} Jellyfin={Version}", new object[2] { patchReport.Message, patchReport.JellyfinVersion });
			return patchReport;
		}
		try
		{
			JVFIPatch.Configure(_stateStore, _hudFiles, _metricsFiles, _profileOverrides, _configurationManager, _libraryManager, _concurrentViewingLimiter, _capabilities, (ILogger)(object)_logger);
			MethodInfo method = typeof(JVFIPatch).GetMethod("PlaybackPrefix", BindingFlags.Static | BindingFlags.Public);
			MethodInfo method2 = typeof(JVFIPatch).GetMethod("TranscodePrefix", BindingFlags.Static | BindingFlags.Public);
			_harmony = new Harmony("com.skillgodak.jellyfin.jvfi");
			_harmony.Patch((MethodBase)patchReport.PlaybackMethod, new HarmonyMethod(method)
			{
				priority = 0
			}, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			_harmony.Patch((MethodBase)patchReport.TranscodeMethod, new HarmonyMethod(method2)
			{
				priority = 0
			}, (HarmonyMethod)null, (HarmonyMethod)null, (HarmonyMethod)null);
			LoggerExtensions.LogInformation((ILogger)(object)_logger, "JVFI：播放與 FFmpeg 補幀掛接成功，Jellyfin={Version}", new object[1] { patchReport.JellyfinVersion });
			return patchReport;
		}
		catch (Exception ex)
		{
			Harmony? harmony = _harmony;
			if (harmony != null)
			{
				harmony.UnpatchAll("com.skillgodak.jellyfin.jvfi");
			}
			LoggerExtensions.LogError((ILogger)(object)_logger, ex, "JVFI：掛接失敗，已安全停用補幀，Jellyfin 正常播放不受影響。", Array.Empty<object>());
			return patchReport with
			{
				Compatible = false,
				Message = ex.Message
			};
		}
	}

	public void Remove()
	{
		Harmony? harmony = _harmony;
		if (harmony != null)
		{
			harmony.UnpatchAll("com.skillgodak.jellyfin.jvfi");
		}
		_harmony = null;
	}
}
