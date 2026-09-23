using System;
using Jellyfin.Plugin.JVFI.Runtime;
using MediaBrowser.Model.Plugins;

namespace Jellyfin.Plugin.JVFI.Configuration;

/// <summary>
/// Plugin configuration persisted by Jellyfin.
/// </summary>
public sealed class PluginConfiguration : BasePluginConfiguration
{
	/// <summary>Gets or sets a value indicating whether frame interpolation is globally enabled.</summary>
	public bool Enabled { get; set; }

	/// <summary>Gets or sets a value indicating whether the concurrent JVFI user limit is enabled.</summary>
	public bool ConcurrentViewingLimitEnabled { get; set; }

	/// <summary>Gets or sets the maximum number of independent playback sessions that may hold JVFI slots at the same time.</summary>
	public int ConcurrentViewingUserLimit { get; set; }

	/// <summary>
	/// Gets or sets the Jellyfin virtual-folder ids selected for interpolation.
	/// Null preserves the pre-library-filter behavior (all libraries); an empty array selects none.
	/// </summary>
	public string[]? SelectedLibraryIds { get; set; }

	/// <summary>
	/// Gets or sets the Jellyfin library root paths selected for interpolation.
	/// The configuration page refreshes these from /Library/VirtualFolders when saved.
	/// </summary>
	public string[]? SelectedLibraryPaths { get; set; }

	/// <summary>
	/// Gets or sets the Jellyfin user ids selected for interpolation.
	/// Null preserves the pre-user-filter behavior (all users); an empty array selects none.
	/// </summary>
	public string[]? SelectedUserIds { get; set; }

	/// <summary>
	/// Gets or sets per-user media library selections.
	/// Null keeps compatibility with the legacy global SelectedUserIds/SelectedLibraryIds fields.
	/// An empty array disables interpolation for every user.
	/// </summary>
	public UserLibrarySelection[]? UserLibrarySelections { get; set; }

	/// <summary>Gets or sets the JVFI user-interface and playback HUD language.</summary>
	public string InterfaceLanguage { get; set; }

	/// <summary>Gets or sets the default profile id.</summary>
	public string DefaultProfileId { get; set; }

	/// <summary>Gets or sets the output video encoder selection policy.</summary>
	public OutputEncoderMode OutputEncoderMode { get; set; }

	/// <summary>Gets or sets the HUD mode.</summary>
	public HudMode HudMode { get; set; }

	/// <summary>Gets or sets the HUD position.</summary>
	public HudPosition HudPosition { get; set; }

	/// <summary>Gets or sets compact or diagnostic HUD content.</summary>
	public HudDetailMode HudDetailMode { get; set; }

	/// <summary>Gets or sets the number of seconds to show the startup HUD.</summary>
	public int HudSeconds { get; set; }

	/// <summary>Gets or sets the minimum H.264 bitrate for 480p60 output. Zero disables protection.</summary>
	public int Minimum480pBitrateMbps { get; set; }

	/// <summary>Gets or sets the minimum H.264 bitrate for 720p60 output. Zero disables protection.</summary>
	public int Minimum720pBitrateMbps { get; set; }

	/// <summary>Gets or sets the minimum H.264 bitrate for 1080p60 output. Zero disables protection.</summary>
	public int Minimum1080pBitrateMbps { get; set; }

	/// <summary>Gets or sets the minimum H.264 bitrate for original-resolution 4K60 output. Zero disables protection.</summary>
	public int Minimum4KBitrateMbps { get; set; }

	/// <summary>Gets or sets a value indicating whether the JVFI bitrate defaults migration has been applied.</summary>
	public bool BitrateDefaultsApplied { get; set; }

	/// <summary>Gets or sets the sustained FFmpeg speed threshold that triggers auto downgrade.</summary>
	public double DowngradeSpeedThreshold { get; set; }

	/// <summary>Gets or sets the sustained FFmpeg speed threshold that permits recovery.</summary>
	public double RecoverySpeedThreshold { get; set; }

	/// <summary>Gets or sets the ordered profile fallback list used by Auto mode.</summary>
	public string[] AutoProfiles { get; set; }

	/// <summary>Gets or sets the available frame interpolation profiles.</summary>
	public InterpolationProfile[] Profiles { get; set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Jellyfin.Plugin.JVFI.Configuration.PluginConfiguration" /> class.
	/// </summary>
	public PluginConfiguration()
	{
		Enabled = false;
		ConcurrentViewingLimitEnabled = false;
		ConcurrentViewingUserLimit = 2;
		InterfaceLanguage = "zh-TW";
		DefaultProfileId = "auto60";
		OutputEncoderMode = OutputEncoderMode.AutoHardware;
		HudMode = HudMode.FirstSeconds;
		HudPosition = HudPosition.TopLeft;
		HudDetailMode = HudDetailMode.Compact;
		HudSeconds = 10;
		Minimum480pBitrateMbps = 4;
		Minimum720pBitrateMbps = 8;
		Minimum1080pBitrateMbps = 16;
		Minimum4KBitrateMbps = 40;
		BitrateDefaultsApplied = false;
		DowngradeSpeedThreshold = 0.9;
		RecoverySpeedThreshold = 1.25;
		AutoProfiles = Array.Empty<string>();
		Profiles = new InterpolationProfile[2]
		{
			new InterpolationProfile
			{
				Id = "off",
				Name = "Off",
				Enabled = true,
				TargetFps = 0.0,
				MaxWidth = 0,
				Quality = JVFIQualityProfile.Duplicate
			},
			new InterpolationProfile
			{
				Id = "auto60",
				Name = "JVFI Custom FPS",
				Enabled = true,
				TargetFps = 60.0,
				MaxWidth = 0,
				Quality = JVFIQualityProfile.Balanced
			}
		};
	}
}
