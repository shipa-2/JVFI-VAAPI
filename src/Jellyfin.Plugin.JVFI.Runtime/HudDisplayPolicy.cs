using System;
using Jellyfin.Plugin.JVFI.Configuration;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Determines HUD visibility from wall-clock playback time.</summary>
public static class HudDisplayPolicy
{
	public static bool ShouldShow(HudMode mode, int firstSeconds, DateTimeOffset startedAt, DateTimeOffset now, bool downgraded)
	{
		return mode switch
		{
			HudMode.Always => true, 
			HudMode.WarningsOnly => downgraded, 
			HudMode.FirstSeconds => firstSeconds > 0 && now - startedAt < TimeSpan.FromSeconds(firstSeconds), 
			_ => false, 
		};
	}
}
