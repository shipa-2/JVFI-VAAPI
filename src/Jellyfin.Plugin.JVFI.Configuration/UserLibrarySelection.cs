using System;

namespace Jellyfin.Plugin.JVFI.Configuration;

/// <summary>
/// Per-user interpolation scope. Each Jellyfin user can have an independent library selection.
/// </summary>
public sealed class UserLibrarySelection
{
	/// <summary>Gets or sets the Jellyfin user id.</summary>
	public string UserId { get; set; } = string.Empty;

	/// <summary>Gets or sets the Jellyfin virtual-folder ids enabled for this user.</summary>
	public string[] LibraryIds { get; set; } = Array.Empty<string>();

	/// <summary>Gets or sets the Jellyfin library root paths enabled for this user.</summary>
	public string[] LibraryPaths { get; set; } = Array.Empty<string>();
}
