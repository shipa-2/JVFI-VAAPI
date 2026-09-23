using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Parses the line-oriented metrics contract without inventing client values.</summary>
public static class JVFIMetricsParser
{
	public static JVFIMetrics? Parse(string? payload)
	{
		if (string.IsNullOrWhiteSpace(payload))
		{
			return null;
		}
		Dictionary<string, string> dictionary = (from line in payload.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
			select line.Split('=', 2) into parts
			where parts.Length == 2
			select parts).ToDictionary((string[] parts) => parts[0].Trim(), (string[] parts) => parts[1].Trim(), StringComparer.OrdinalIgnoreCase);
		if (!TryFiniteDouble(dictionary, "timeline", out var value) || !TryFiniteDouble(dictionary, "target", out var value2) || !TryFiniteDouble(dictionary, "compute", out var value3) || !TryFiniteDouble(dictionary, "speed", out var value4) || !TryLong(dictionary, "generated", out var value5) || !TryLong(dictionary, "fallback", out var value6) || value < 0.0 || value2 <= 0.0 || value3 < 0.0 || value4 < 0.0 || value5 < 0 || value6 < 0)
		{
			return null;
		}
		return new JVFIMetrics(value, value2, value3, value4, value5, value6, dictionary.GetValueOrDefault("backend", "unknown"), TryOptionalDouble(dictionary, "client_render"), TryOptionalLong(dictionary, "dropped"), TryOptionalLong(dictionary, "repeated"));
	}

	private static bool TryFiniteDouble(IReadOnlyDictionary<string, string> values, string key, out double value)
	{
		value = 0.0;
		if (values.TryGetValue(key, out string value2) && double.TryParse(value2, NumberStyles.Float, CultureInfo.InvariantCulture, out value))
		{
			return double.IsFinite(value);
		}
		return false;
	}

	private static bool TryLong(IReadOnlyDictionary<string, string> values, string key, out long value)
	{
		value = 0L;
		if (values.TryGetValue(key, out string value2))
		{
			return long.TryParse(value2, NumberStyles.Integer, CultureInfo.InvariantCulture, out value);
		}
		return false;
	}

	private static double? TryOptionalDouble(IReadOnlyDictionary<string, string> values, string key)
	{
		if (!TryFiniteDouble(values, key, out var value))
		{
			return null;
		}
		return value;
	}

	private static long? TryOptionalLong(IReadOnlyDictionary<string, string> values, string key)
	{
		if (!TryLong(values, key, out var value) || value < 0)
		{
			return null;
		}
		return value;
	}
}
