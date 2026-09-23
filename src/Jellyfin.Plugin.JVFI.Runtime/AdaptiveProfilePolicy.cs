using System;
using System.Collections.Generic;
using System.Linq;
using Jellyfin.Plugin.JVFI.Configuration;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Vendor-neutral profile selection for automatic interpolation mode.</summary>
public static class AdaptiveProfilePolicy
{
	public static int GetOutputWidth(int sourceWidth, int maxWidth)
	{
		if (maxWidth <= 0)
		{
			return sourceWidth;
		}
		return Math.Min(sourceWidth, maxWidth);
	}

	public static InterpolationProfile? FindNext(IReadOnlyList<InterpolationProfile> profiles, IReadOnlyList<string> order, string activeId, int sourceWidth, int direction)
	{
		InterpolationProfile result = profiles.FirstOrDefault((InterpolationProfile profile) => profile != null && profile.Enabled && profile.TargetFps > 0.0 && profile.Id == "auto60");
		InterpolationProfile[] orderedProfiles = GetOrderedProfiles(profiles, order);
		if (direction > 0)
		{
			if (activeId == "auto60")
			{
				return orderedProfiles.FirstOrDefault();
			}
			int num = Array.FindIndex(orderedProfiles, (InterpolationProfile profile) => profile.Id == activeId);
			if (num < 0 || num + 1 >= orderedProfiles.Length)
			{
				return null;
			}
			return orderedProfiles[num + 1];
		}
		if (direction < 0 && activeId != "auto60")
		{
			int num2 = Array.FindIndex(orderedProfiles, (InterpolationProfile profile) => profile.Id == activeId);
			if (num2 > 0)
			{
				return orderedProfiles[num2 - 1];
			}
			if (num2 != 0)
			{
				return null;
			}
			return result;
		}
		return null;
	}

	private static InterpolationProfile[] GetOrderedProfiles(IReadOnlyList<InterpolationProfile> profiles, IReadOnlyList<string> order)
	{
		return (from id in order
			select profiles.FirstOrDefault((InterpolationProfile profile) => profile.Id == id) into profile
			where profile != null && profile.Enabled && profile.TargetFps > 0.0
			select profile).Cast<InterpolationProfile>().ToArray();
	}
}
