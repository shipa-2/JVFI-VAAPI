using Jellyfin.Plugin.JVFI.Runtime;

namespace Jellyfin.Plugin.JVFI.Configuration;

/// <summary>
/// Frame interpolation profile.
/// </summary>
public sealed class InterpolationProfile
{
	/// <summary>Gets or sets the profile id.</summary>
	public string Id { get; set; } = string.Empty;

	/// <summary>Gets or sets the user-facing profile name.</summary>
	public string Name { get; set; } = string.Empty;

	/// <summary>Gets or sets a value indicating whether the profile can be selected.</summary>
	public bool Enabled { get; set; }

	/// <summary>Gets or sets the target output frame rate.</summary>
	public double TargetFps { get; set; }

	/// <summary>Gets or sets the maximum output width. A value of 0 keeps source width.</summary>
	public int MaxWidth { get; set; }

	/// <summary>Gets or sets the native Motion Core quality level.</summary>
	public JVFIQualityProfile Quality { get; set; } = JVFIQualityProfile.Balanced;
}
