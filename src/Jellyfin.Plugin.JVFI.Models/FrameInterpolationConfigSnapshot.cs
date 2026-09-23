using System;
using System.Collections.Generic;
using Jellyfin.Plugin.JVFI.Configuration;
using Jellyfin.Plugin.JVFI.Runtime;

namespace Jellyfin.Plugin.JVFI.Models;

/// <summary>
/// Configuration snapshot returned to an FFmpeg wrapper or client helper.
/// </summary>
public sealed class FrameInterpolationConfigSnapshot
{
	/// <summary>Gets or sets a value indicating whether frame interpolation is globally enabled.</summary>
	public bool Enabled { get; set; }

	/// <summary>Gets or sets the default profile id.</summary>
	public string DefaultProfileId { get; set; } = string.Empty;

	/// <summary>Gets or sets the output encoder selection policy.</summary>
	public OutputEncoderMode OutputEncoderMode { get; set; }

	/// <summary>Gets or sets the interpolation quality and performance mode.</summary>
	/// <summary>Gets or sets the HUD mode.</summary>
	public HudMode HudMode { get; set; }

	/// <summary>Gets or sets the HUD position.</summary>
	public HudPosition HudPosition { get; set; }

	/// <summary>Gets or sets the number of seconds to show a startup HUD.</summary>
	public int HudSeconds { get; set; }

	/// <summary>Gets or sets the speed threshold below which Auto should downgrade.</summary>
	public double DowngradeSpeedThreshold { get; set; }

	/// <summary>Gets or sets the speed threshold above which Auto may recover.</summary>
	public double RecoverySpeedThreshold { get; set; }

	/// <summary>Gets or sets the auto downgrade profile chain.</summary>
	public IReadOnlyList<string> AutoProfiles { get; set; } = Array.Empty<string>();

	/// <summary>Gets or sets the configured profiles.</summary>
	public IReadOnlyList<InterpolationProfile> Profiles { get; set; } = Array.Empty<InterpolationProfile>();
}
