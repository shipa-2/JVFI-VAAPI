using System;
using System.Linq;

namespace Jellyfin.Plugin.JVFI.Configuration;

/// <summary>Migrates all legacy quality ladders to the single stable interpolation profile.</summary>
public static class PluginConfigurationNormalizer
{
	private static readonly string[] ProfileIds = new string[2] { "off", "auto60" };

	public static bool Normalize(PluginConfiguration config)
	{
		ArgumentNullException.ThrowIfNull(config, "config");
		bool result = false;
		if (string.IsNullOrWhiteSpace(config.InterfaceLanguage) || (config.InterfaceLanguage != "zh-TW" && config.InterfaceLanguage != "en" && config.InterfaceLanguage != "ja"))
		{
			config.InterfaceLanguage = "zh-TW";
			result = true;
		}
		if (!config.BitrateDefaultsApplied)
		{
			config.Minimum480pBitrateMbps = 4;
			config.Minimum720pBitrateMbps = 8;
			config.Minimum1080pBitrateMbps = 16;
			config.Minimum4KBitrateMbps = 40;
			config.BitrateDefaultsApplied = true;
			result = true;
		}
		if (config.Profiles == null || !config.Profiles.Select((InterpolationProfile profile) => profile.Id).SequenceEqual(ProfileIds, StringComparer.Ordinal) || config.AutoProfiles == null || config.AutoProfiles.Length != 0)
		{
			PluginConfiguration pluginConfiguration = new PluginConfiguration();
			config.Profiles = pluginConfiguration.Profiles;
			config.AutoProfiles = pluginConfiguration.AutoProfiles;
			result = true;
		}
		if (!string.Equals(config.DefaultProfileId, "off", StringComparison.Ordinal) && !string.Equals(config.DefaultProfileId, "auto60", StringComparison.Ordinal))
		{
			config.DefaultProfileId = "auto60";
			result = true;
		}
		int num = ((config.ConcurrentViewingUserLimit <= 0) ? 2 : Math.Clamp(config.ConcurrentViewingUserLimit, 1, 100));
		if (config.ConcurrentViewingUserLimit != num)
		{
			config.ConcurrentViewingUserLimit = num;
			result = true;
		}
		InterpolationProfile interpolationProfile = config.Profiles.First((InterpolationProfile profile) => profile.Id == "auto60");
		double num2 = (double.IsFinite(interpolationProfile.TargetFps) ? Math.Clamp(interpolationProfile.TargetFps, 23.976, 240.0) : 60.0);
		if (!double.IsFinite(interpolationProfile.TargetFps) || Math.Abs(interpolationProfile.TargetFps - num2) > 0.0001)
		{
			interpolationProfile.TargetFps = num2;
			result = true;
		}
		return result;
	}
}
