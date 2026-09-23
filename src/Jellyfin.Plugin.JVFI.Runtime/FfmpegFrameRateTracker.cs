using System;
using System.Collections.Concurrent;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Measures short-window FFmpeg output-frame completion rate from -progress frame counters.</summary>
public sealed class FfmpegFrameRateTracker
{
	private sealed class State(long frame, DateTimeOffset time)
	{
		public long Frame = frame;

		public DateTimeOffset Time = time;

		public double Last;

		public void Reset(long f, DateTimeOffset t)
		{
			Frame = f;
			Time = t;
			Last = 0.0;
		}
	}

	private readonly ConcurrentDictionary<string, State> _states = new ConcurrentDictionary<string, State>(StringComparer.Ordinal);

	public FfmpegFrameRateObservation? Observe(string sessionId, long frame, DateTimeOffset now, double targetFps)
	{
		State orAdd = _states.GetOrAdd(sessionId, (string _) => new State(frame, now));
		lock (orAdd)
		{
			if (frame < orAdd.Frame || now <= orAdd.Time)
			{
				orAdd.Reset(frame, now);
				return null;
			}
			if (frame == orAdd.Frame)
			{
				return null;
			}
			double totalSeconds = (now - orAdd.Time).TotalSeconds;
			long num = frame - orAdd.Frame;
			orAdd.Frame = frame;
			orAdd.Time = now;
			if (totalSeconds <= 0.0 || totalSeconds > 2.0 || num <= 0)
			{
				return null;
			}
			double num2 = (double)num / totalSeconds;
			return new FfmpegFrameRateObservation(orAdd.Last = Math.Clamp(num2, 0.0, targetFps), num2, 1000.0 / Math.Max(num2, 0.001), num, totalSeconds * 1000.0);
		}
	}

	public void Remove(string sessionId)
	{
		_states.TryRemove(sessionId, out State _);
	}
}
