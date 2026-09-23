using System;
using System.Collections.Concurrent;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Estimates FFmpeg speed from media-time and wall-time deltas.</summary>
public sealed class TranscodeSpeedTracker
{
	private sealed record Sample(long MediaPositionTicks, DateTimeOffset ObservedAt);

	private sealed record SessionSamples(Sample First, Sample Previous, int SampleCount = 0);

	private readonly ConcurrentDictionary<string, SessionSamples> _samples = new ConcurrentDictionary<string, SessionSamples>(StringComparer.Ordinal);

	public double? Observe(string sessionId, long mediaPositionTicks, DateTimeOffset observedAt)
	{
		return ObserveDetailed(sessionId, mediaPositionTicks, observedAt)?.Instantaneous;
	}

	public TranscodeSpeedObservation? ObserveDetailed(string sessionId, long mediaPositionTicks, DateTimeOffset observedAt)
	{
		Sample sample = new Sample(mediaPositionTicks, observedAt);
		if (!_samples.TryGetValue(sessionId, out SessionSamples value))
		{
			_samples[sessionId] = new SessionSamples(sample, sample);
			return null;
		}
		long ticks = (observedAt - value.Previous.ObservedAt).Ticks;
		long num = mediaPositionTicks - value.Previous.MediaPositionTicks;
		if (ticks <= 0 || num <= 0)
		{
			_samples[sessionId] = new SessionSamples(sample, sample);
			return null;
		}
		long num2 = Math.Max(TimeSpan.FromSeconds(60L).Ticks, ticks * 20);
		if (num > num2)
		{
			_samples[sessionId] = new SessionSamples(sample, sample);
			return null;
		}
		int sampleCount = value.SampleCount + 1;
		_samples[sessionId] = value with
		{
			Previous = sample,
			SampleCount = sampleCount
		};
		long ticks2 = (observedAt - value.First.ObservedAt).Ticks;
		long num3 = mediaPositionTicks - value.First.MediaPositionTicks;
		return new TranscodeSpeedObservation(Math.Round((double)num / (double)ticks, 3), Math.Round((double)num3 / (double)ticks2, 3), sampleCount);
	}

	public void Remove(string sessionId)
	{
		_samples.TryRemove(sessionId, out SessionSamples _);
	}
}
