using System;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Determines whether a Jellyfin user is selected for frame interpolation.</summary>
public static class UserSelectionPolicy
{
	public static bool IsSelected(Guid userId, string[]? selectedUserIds)
	{
		if (selectedUserIds == null)
		{
			return true;
		}
		if (selectedUserIds.Length == 0 || userId == Guid.Empty)
		{
			return false;
		}
		for (int i = 0; i < selectedUserIds.Length; i++)
		{
			if (Guid.TryParse(selectedUserIds[i], out var result) && result == userId)
			{
				return true;
			}
		}
		return false;
	}
}
