using System;
using System.Collections.Concurrent;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>
/// Estimates timeline/output/generated frame counters for the current official FFmpeg job.
/// Counters represent only the media processed by the current continuous transcode run.
/// A seek/rebuild resets the counters so pre-seek and post-seek work is never combined.
/// </summary>
public sealed class OfficialBackendTelemetryEstimator
{
	private sealed record Observation(long MediaPositionTicks, DateTimeOffset ObservedAt);

	private sealed record SampleState(Observation Previous, long AccumulatedMediaTicks);

	private readonly ConcurrentDictionary<string, SampleState> _states = new ConcurrentDictionary<string, SampleState>(StringComparer.Ordinal);

	public OfficialBackendTelemetry? Observe(string sessionId, long mediaPositionTicks, DateTimeOffset observedAt, double inputFps, double targetFps)
	{
		if (string.IsNullOrWhiteSpace(sessionId) || inputFps <= 0.0 || targetFps <= 0.0)
		{
			return null;
		}
		Observation previous = new Observation(mediaPositionTicks, observedAt);
		if (!_states.TryGetValue(sessionId, out SampleState value))
		{
			_states[sessionId] = new SampleState(previous, 0L);
			return null;
		}
		long ticks = (observedAt - value.Previous.ObservedAt).Ticks;
		long num = mediaPositionTicks - value.Previous.MediaPositionTicks;
		long num2 = ((ticks > 0) ? Math.Max(TimeSpan.FromSeconds(60L).Ticks, ticks * 20) : TimeSpan.FromSeconds(60L).Ticks);
		if (num < 0 || num > num2)
		{
			_states[sessionId] = new SampleState(previous, 0L);
			return Build(0L, inputFps, targetFps);
		}
		if (num == 0L)
		{
			_states[sessionId] = value with
			{
				Previous = previous
			};
			return Build(value.AccumulatedMediaTicks, inputFps, targetFps);
		}
		long num3 = value.AccumulatedMediaTicks + num;
		_states[sessionId] = new SampleState(previous, num3);
		return Build(num3, inputFps, targetFps);
	}

	public void Remove(string sessionId)
	{
		_states.TryRemove(sessionId, out SampleState _);
	}

	private static OfficialBackendTelemetry Build(long mediaTicks, double inputFps, double targetFps)
	{
		double num = Math.Max(0.0, (double)mediaTicks / 10000000.0);
		long num2 = (long)Math.Floor(num * targetFps);
		long num3 = (long)Math.Floor(num * inputFps);
		long generatedFrames = Math.Max(0L, num2 - num3);
		return new OfficialBackendTelemetry(num2, generatedFrames, num);
	}
}
