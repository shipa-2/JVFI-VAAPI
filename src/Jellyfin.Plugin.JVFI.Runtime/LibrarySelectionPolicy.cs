using System;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>
/// Decides whether a media path belongs to one of the Jellyfin libraries selected for JVFI.
/// A null selection preserves legacy behavior (all libraries). An empty selection means none.
/// </summary>
public static class LibrarySelectionPolicy
{
	public static bool IsSelected(string? mediaPath, string[]? selectedLibraryPaths)
	{
		if (selectedLibraryPaths == null)
		{
			return true;
		}
		if (selectedLibraryPaths.Length == 0 || string.IsNullOrWhiteSpace(mediaPath))
		{
			return false;
		}
		string text = Normalize(mediaPath);
		StringComparison comparisonType = (OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
		foreach (string text2 in selectedLibraryPaths)
		{
			if (!string.IsNullOrWhiteSpace(text2))
			{
				string text3 = Normalize(text2);
				if (string.Equals(text, text3, comparisonType) || text.StartsWith(text3 + "/", comparisonType))
				{
					return true;
				}
			}
		}
		return false;
	}

	private static string Normalize(string path)
	{
		string text = path.Trim().Replace('\\', '/');
		if (text.Length <= 1)
		{
			return text;
		}
		return text.TrimEnd('/');
	}
}
