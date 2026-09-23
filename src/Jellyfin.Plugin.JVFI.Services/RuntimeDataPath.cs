using System;
using System.IO;

namespace Jellyfin.Plugin.JVFI.Services;

/// <summary>
/// Resolves a version-independent runtime data location for JVFI.
/// Jellyfin repository installs versioned plugin directories such as JVFI_0.7.3.0,
/// so runtime files must never be tied to the current package directory name.
/// </summary>
public static class RuntimeDataPath
{
	private const string RuntimeDirectoryName = "JVFI-runtime";

	/// <summary>Gets the stable JVFI runtime root.</summary>
	public static string GetRuntimeRoot()
	{
		string location = typeof(Plugin).Assembly.Location;
		string text = ((!string.IsNullOrWhiteSpace(location)) ? Path.GetDirectoryName(Path.GetFullPath(location)) : null);
		if (text == null)
		{
			text = AppContext.BaseDirectory;
		}
		for (DirectoryInfo directoryInfo = new DirectoryInfo(text); directoryInfo != null; directoryInfo = directoryInfo.Parent)
		{
			if (string.Equals(directoryInfo.Name, "plugins", StringComparison.OrdinalIgnoreCase))
			{
				return Path.Combine(directoryInfo.FullName, "JVFI-runtime");
			}
		}
		return Path.Combine(text, "JVFI-runtime");
	}

	/// <summary>Gets a stable subdirectory beneath the JVFI runtime root.</summary>
	public static string GetSubdirectory(string name)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(name, "name");
		return Path.Combine(GetRuntimeRoot(), name);
	}
}
