using System;
using System.Collections.Concurrent;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Debounces FPS-drop logging. HUD remains fast; logs only meaningful sustained drops.</summary>
public sealed class FpsDropEventTracker
{
	private sealed class State
	{
		public DateTimeOffset? LowSince;

		public bool Active;

		public double Minimum;

		public DateTimeOffset LastLog;

		public void Reset()
		{
			LowSince = null;
			Active = false;
			Minimum = 0.0;
			LastLog = default;
		}
	}

	private readonly ConcurrentDictionary<string, State> _states = new ConcurrentDictionary<string, State>(StringComparer.Ordinal);

	public FpsDropEvent Observe(string id, double fps, double target, double intervalMs, DateTimeOffset now)
	{
		State orAdd = _states.GetOrAdd(id, (string _) => new State());
		lock (orAdd)
		{
			if (fps < target - 1.0)
			{
				State state = orAdd;
				DateTimeOffset valueOrDefault = state.LowSince.GetValueOrDefault();
				if (!state.LowSince.HasValue)
				{
					valueOrDefault = now;
					state.LowSince = valueOrDefault;
				}
				orAdd.Minimum = ((orAdd.Minimum <= 0.0) ? fps : Math.Min(orAdd.Minimum, fps));
				double totalMilliseconds = (now - orAdd.LowSince.Value).TotalMilliseconds;
				if (!orAdd.Active && totalMilliseconds >= 100.0)
				{
					orAdd.Active = true;
					orAdd.LastLog = now;
					return Make(FpsDropEventKind.Started, fps, orAdd.Minimum, target, intervalMs, totalMilliseconds);
				}
				if (orAdd.Active && now - orAdd.LastLog >= TimeSpan.FromSeconds(1L))
				{
					orAdd.LastLog = now;
					return Make(FpsDropEventKind.Update, fps, orAdd.Minimum, target, intervalMs, totalMilliseconds);
				}
				return Make(FpsDropEventKind.None, fps, orAdd.Minimum, target, intervalMs, totalMilliseconds);
			}
			if (orAdd.Active && fps >= target - 0.5)
			{
				double duration = (orAdd.LowSince.HasValue ? (now - orAdd.LowSince.Value).TotalMilliseconds : 0.0);
				FpsDropEvent result = Make(FpsDropEventKind.Recovered, fps, orAdd.Minimum, target, intervalMs, duration);
				orAdd.Reset();
				return result;
			}
			if (!orAdd.Active)
			{
				orAdd.Reset();
			}
			return Make(FpsDropEventKind.None, fps, orAdd.Minimum, target, intervalMs, 0.0);
		}
	}

	public void Remove(string id)
	{
		_states.TryRemove(id, out State _);
	}

	private static FpsDropEvent Make(FpsDropEventKind kind, double fps, double min, double target, double ms, double duration)
	{
		FpsDropEventKind kind2 = kind;
		double currentFps = fps;
		double minimumFps = min;
		double targetFps = target;
		double frameIntervalMs = ms;
		double durationMs = duration;
		string severity;
		if (fps < target - 10.0)
		{
			severity = "Critical";
		}
		else
		{
			severity = ((fps < target - 5.0) ? "Severe" : "Warning");
		}
		return new FpsDropEvent(kind2, currentFps, minimumFps, targetFps, frameIntervalMs, durationMs, severity);
	}
}
