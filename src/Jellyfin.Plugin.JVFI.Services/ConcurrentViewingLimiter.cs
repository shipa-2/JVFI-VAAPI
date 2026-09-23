using System;
using System.Collections.Generic;
using System.Linq;

namespace Jellyfin.Plugin.JVFI.Services;

/// <summary>
/// Tracks playback sessions that currently hold a JVFI interpolation slot.
/// Each independent PlaySessionId consumes one slot, even for the same user or device.
/// </summary>
public sealed class ConcurrentViewingLimiter
{
	private readonly object _sync = new object();

	private readonly Dictionary<string, Guid> _sessionUsers = new Dictionary<string, Guid>(StringComparer.Ordinal);

	/// <summary>Gets the number of distinct users represented by active playback slots.</summary>
	public int ActiveUserCount
	{
		get
		{
			lock (_sync)
			{
				return _sessionUsers.Values.Distinct().Count();
			}
		}
	}

	/// <summary>Gets the number of playback sessions currently holding JVFI reservations.</summary>
	public int ActiveSessionCount
	{
		get
		{
			lock (_sync)
			{
				return _sessionUsers.Count;
			}
		}
	}

	/// <summary>
	/// Attempts to reserve a JVFI slot for a playback session.
	/// Re-entering with the same PlaySessionId keeps the existing reservation and consumes no extra slot.
	/// </summary>
	public bool TryAcquire(string? playSessionId, Guid userId, int maxSessions)
	{
		if (string.IsNullOrWhiteSpace(playSessionId) || userId == Guid.Empty || maxSessions <= 0)
		{
			return false;
		}
		lock (_sync)
		{
			if (_sessionUsers.TryGetValue(playSessionId, out var value))
			{
				return value == userId;
			}
			if (_sessionUsers.Count >= maxSessions)
			{
				return false;
			}
			_sessionUsers[playSessionId] = userId;
			return true;
		}
	}

	/// <summary>Returns true when a playback session currently owns a JVFI slot.</summary>
	public bool HasReservation(string? playSessionId)
	{
		if (string.IsNullOrWhiteSpace(playSessionId))
		{
			return false;
		}
		lock (_sync)
		{
			return _sessionUsers.ContainsKey(playSessionId);
		}
	}

	/// <summary>Releases one playback session slot.</summary>
	public bool Release(string? playSessionId)
	{
		if (string.IsNullOrWhiteSpace(playSessionId))
		{
			return false;
		}
		lock (_sync)
		{
			return _sessionUsers.Remove(playSessionId);
		}
	}
}
