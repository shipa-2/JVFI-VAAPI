using System;
using System.Collections.Concurrent;

namespace Jellyfin.Plugin.JVFI.Services;

/// <summary>Keeps per-play-session profile overrides across FFmpeg restarts.</summary>
public sealed class AdaptiveProfileStore
{
	private readonly ConcurrentDictionary<string, string> _profiles = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);

	public string? Get(string sessionId)
	{
		if (!_profiles.TryGetValue(sessionId, out string value))
		{
			return null;
		}
		return value;
	}

	public void Set(string sessionId, string profileId)
	{
		_profiles[sessionId] = profileId;
	}

	public void Clear(string sessionId)
	{
		_profiles.TryRemove(sessionId, out string _);
	}
}
