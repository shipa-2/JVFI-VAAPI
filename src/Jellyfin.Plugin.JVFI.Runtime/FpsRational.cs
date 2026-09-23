using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Represents an FFmpeg frame-rate expression without rounding NTSC-style rates.</summary>
public readonly record struct FpsRational(int Numerator, int Denominator)
{
	public static FpsRational FromDouble(double value)
	{
		if (!double.IsFinite(value) || value <= 0.0)
		{
			return new FpsRational(60, 1);
		}
		(double, int, int)[] array = new (double, int, int)[5]
		{
			(23.976, 24000, 1001),
			(29.97, 30000, 1001),
			(47.952, 48000, 1001),
			(59.94, 60000, 1001),
			(119.88, 120000, 1001)
		};
		for (int i = 0; i < array.Length; i++)
		{
			(double, int, int) tuple = array[i];
			if (Math.Abs(value - tuple.Item1) < 0.01)
			{
				return new FpsRational(tuple.Item2, tuple.Item3);
			}
		}
		double num = Math.Round(value);
		if (Math.Abs(value - num) < 0.0001)
		{
			return new FpsRational((int)num, 1);
		}
		int num2 = (int)Math.Round(value * 1000.0, MidpointRounding.AwayFromZero);
		int num3 = GreatestCommonDivisor(Math.Abs(num2), 1000);
		return new FpsRational(num2 / num3, 1000 / num3);
	}

	public string ToFfmpegExpression()
	{
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(1, 2, invariantCulture);
		handler.AppendFormatted(Numerator);
		handler.AppendLiteral("/");
		handler.AppendFormatted(Denominator);
		return string.Create(invariantCulture, ref handler);
	}

	public override string ToString()
	{
		return ToFfmpegExpression();
	}

	private static int GreatestCommonDivisor(int a, int b)
	{
		while (b != 0)
		{
			int num = b;
			b = a % b;
			a = num;
		}
		if (a != 0)
		{
			return a;
		}
		return 1;
	}
}
