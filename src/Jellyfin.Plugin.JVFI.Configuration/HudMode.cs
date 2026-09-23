namespace Jellyfin.Plugin.JVFI.Configuration;

/// <summary>
/// HUD display behavior.
/// </summary>
public enum HudMode
{
	/// <summary>No on-video HUD.</summary>
	Off,
	/// <summary>Show the HUD for the first few seconds.</summary>
	FirstSeconds,
	/// <summary>Always show the HUD while interpolation is active.</summary>
	Always,
	/// <summary>Only show downgrade or failure warnings.</summary>
	WarningsOnly
}
