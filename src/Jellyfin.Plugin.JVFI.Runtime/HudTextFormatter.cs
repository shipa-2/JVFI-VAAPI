using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Creates localized user-visible playback HUD text.</summary>
public static class HudTextFormatter
{
	private static string Lang(string? language)
	{
		if (language == null || !language.StartsWith("ja", StringComparison.OrdinalIgnoreCase))
		{
			if (language == null || !language.StartsWith("en", StringComparison.OrdinalIgnoreCase))
			{
				return "zh-TW";
			}
			return "en";
		}
		return "ja";
	}

	private static string Active(bool downgraded, string? language)
	{
		string text = Lang(language);
		if (!(text == "en"))
		{
			if (text == "ja")
			{
				if (downgraded)
				{
					return "縮退中";
				}
				return "補間中";
			}
			if (downgraded)
			{
				return "已降級";
			}
			return "補幀中";
		}
		if (downgraded)
		{
			return "Degraded";
		}
		return "Interpolating";
	}

	private static string Starting(string? language)
	{
		string text = Lang(language);
		if (!(text == "en"))
		{
			if (text == "ja")
			{
				return "補間を検出中";
			}
			return "補幀偵測中";
		}
		return "Detecting interpolation";
	}

	public static string Format(string profileId, double outputFps, double processingFps, double speed, bool downgraded, string outputEncoder, string? language = null)
	{
		string value = Active(downgraded, language);
		double value2 = ((speed > 0.0) ? Math.Clamp(processingFps / speed, 0.0, outputFps) : Math.Clamp(processingFps, 0.0, outputFps));
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(9, 3, invariantCulture);
		handler.AppendFormatted(value);
		handler.AppendLiteral("  ");
		handler.AppendFormatted(value2, "0.##");
		handler.AppendLiteral(" / ");
		handler.AppendFormatted(outputFps, "0.##");
		handler.AppendLiteral(" FPS");
		return string.Create(invariantCulture, ref handler);
	}

	public static string FormatMeasuredAchievement(double targetFps, double measuredFps, bool downgraded, string? language = null)
	{
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(9, 3, invariantCulture);
		handler.AppendFormatted(Active(downgraded, language));
		handler.AppendLiteral("  ");
		handler.AppendFormatted(Math.Clamp(measuredFps, 0.0, targetFps), "0.00");
		handler.AppendLiteral(" / ");
		handler.AppendFormatted(targetFps, "0.##");
		handler.AppendLiteral(" FPS");
		return string.Create(invariantCulture, ref handler);
	}

	public static string FormatStarting(double outputFps, string? language = null)
	{
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(11, 2, invariantCulture);
		handler.AppendFormatted(Starting(language));
		handler.AppendLiteral("  -- / ");
		handler.AppendFormatted(outputFps, "0.##");
		handler.AppendLiteral(" FPS");
		return string.Create(invariantCulture, ref handler);
	}

	public static string FormatOfficialBackend(double targetFps, double processingThroughputFps, double pipelineSpeed, long generatedFrames, bool hasEstimate, bool downgraded, string? language = null)
	{
		string value = Active(downgraded, language);
		DefaultInterpolatedStringHandler handler;
		IFormatProvider invariantCulture;
		if (pipelineSpeed > 0.0)
		{
			double value2 = Math.Clamp(targetFps * pipelineSpeed, 0.0, targetFps);
			invariantCulture = CultureInfo.InvariantCulture;
			IFormatProvider provider = invariantCulture;
			handler = new DefaultInterpolatedStringHandler(9, 3, invariantCulture);
			handler.AppendFormatted(value);
			handler.AppendLiteral("  ");
			handler.AppendFormatted(value2, "0.00");
			handler.AppendLiteral(" / ");
			handler.AppendFormatted(targetFps, "0.##");
			handler.AppendLiteral(" FPS");
			return string.Create(provider, ref handler);
		}
		invariantCulture = CultureInfo.InvariantCulture;
		IFormatProvider provider2 = invariantCulture;
		handler = new DefaultInterpolatedStringHandler(11, 2, invariantCulture);
		handler.AppendFormatted(value);
		handler.AppendLiteral("  -- / ");
		handler.AppendFormatted(targetFps, "0.##");
		handler.AppendLiteral(" FPS");
		return string.Create(provider2, ref handler);
	}

	public static string FormatCompact(JVFIMetrics metrics, bool downgraded, string? language = null)
	{
		string value = Active(downgraded, language);
		string text = Lang(language);
		string text2;
		if (text == "en")
		{
			text2 = "Timeline";
		}
		else
		{
			text2 = ((!(text == "ja")) ? "時間軸" : "タイムライン");
		}
		string value2 = text2;
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(10, 4, invariantCulture);
		handler.AppendFormatted(value);
		handler.AppendLiteral("  ");
		handler.AppendFormatted(value2);
		handler.AppendLiteral(" ");
		handler.AppendFormatted(metrics.TimelineFps, "0.00");
		handler.AppendLiteral(" / ");
		handler.AppendFormatted(metrics.TargetFps, "0.##");
		handler.AppendLiteral(" FPS");
		return string.Create(invariantCulture, ref handler);
	}

	public static string FormatDiagnostic(JVFIMetrics metrics, bool downgraded, string? language = null)
	{
		string value = Active(downgraded, language);
		string text = Lang(language);
		DefaultInterpolatedStringHandler handler;
		IFormatProvider invariantCulture;
		if (!(text == "en"))
		{
			if (text == "ja")
			{
				invariantCulture = CultureInfo.InvariantCulture;
				IFormatProvider provider = invariantCulture;
				handler = new DefaultInterpolatedStringHandler(52, 5, invariantCulture);
				handler.AppendFormatted(value);
				handler.AppendLiteral("\nタイムライン出力   ");
				handler.AppendFormatted(metrics.TimelineFps, "0.00");
				handler.AppendLiteral(" / ");
				handler.AppendFormatted(metrics.TargetFps, "0.##");
				handler.AppendLiteral(" FPS\n");
				handler.AppendLiteral("処理スループット     ");
				handler.AppendFormatted(metrics.ComputeFps, "0.00");
				handler.AppendLiteral(" FPS\n");
				handler.AppendLiteral("パイプライン速度     ");
				handler.AppendFormatted(metrics.PipelineSpeed, "0.00");
				handler.AppendLiteral("x");
				return string.Create(provider, ref handler);
			}
			invariantCulture = CultureInfo.InvariantCulture;
			IFormatProvider provider2 = invariantCulture;
			handler = new DefaultInterpolatedStringHandler(48, 5, invariantCulture);
			handler.AppendFormatted(value);
			handler.AppendLiteral("\n時間軸輸出      ");
			handler.AppendFormatted(metrics.TimelineFps, "0.00");
			handler.AppendLiteral(" / ");
			handler.AppendFormatted(metrics.TargetFps, "0.##");
			handler.AppendLiteral(" FPS\n");
			handler.AppendLiteral("引擎運算產能    ");
			handler.AppendFormatted(metrics.ComputeFps, "0.00");
			handler.AppendLiteral(" FPS\n");
			handler.AppendLiteral("管線速度        ");
			handler.AppendFormatted(metrics.PipelineSpeed, "0.00");
			handler.AppendLiteral("x");
			return string.Create(provider2, ref handler);
		}
		invariantCulture = CultureInfo.InvariantCulture;
		IFormatProvider provider3 = invariantCulture;
		handler = new DefaultInterpolatedStringHandler(77, 5, invariantCulture);
		handler.AppendFormatted(value);
		handler.AppendLiteral("\nTimeline output     ");
		handler.AppendFormatted(metrics.TimelineFps, "0.00");
		handler.AppendLiteral(" / ");
		handler.AppendFormatted(metrics.TargetFps, "0.##");
		handler.AppendLiteral(" FPS\n");
		handler.AppendLiteral("Processing throughput ");
		handler.AppendFormatted(metrics.ComputeFps, "0.00");
		handler.AppendLiteral(" FPS\n");
		handler.AppendLiteral("Pipeline speed      ");
		handler.AppendFormatted(metrics.PipelineSpeed, "0.00");
		handler.AppendLiteral("x");
		return string.Create(provider3, ref handler);
	}

	public static string FormatOfficialDiagnostic(double targetFps, double achievedFps, double processingThroughputFps, double pipelineSpeed, bool downgraded, string? language = null)
	{
		string value = Active(downgraded, language);
		double value2 = Math.Clamp(achievedFps, 0.0, targetFps);
		double value3 = Math.Max(0.0, processingThroughputFps);
		double value4 = Math.Max(0.0, pipelineSpeed);
		string text = Lang(language);
		DefaultInterpolatedStringHandler handler;
		IFormatProvider invariantCulture;
		if (!(text == "en"))
		{
			if (text == "ja")
			{
				invariantCulture = CultureInfo.InvariantCulture;
				IFormatProvider provider = invariantCulture;
				handler = new DefaultInterpolatedStringHandler(52, 5, invariantCulture);
				handler.AppendFormatted(value);
				handler.AppendLiteral("\nタイムライン出力   ");
				handler.AppendFormatted(value2, "0.00");
				handler.AppendLiteral(" / ");
				handler.AppendFormatted(targetFps, "0.##");
				handler.AppendLiteral(" FPS\n");
				handler.AppendLiteral("処理スループット     ");
				handler.AppendFormatted(value3, "0.00");
				handler.AppendLiteral(" FPS\n");
				handler.AppendLiteral("パイプライン速度     ");
				handler.AppendFormatted(value4, "0.00");
				handler.AppendLiteral("x");
				return string.Create(provider, ref handler);
			}
			invariantCulture = CultureInfo.InvariantCulture;
			IFormatProvider provider2 = invariantCulture;
			handler = new DefaultInterpolatedStringHandler(48, 5, invariantCulture);
			handler.AppendFormatted(value);
			handler.AppendLiteral("\n時間軸輸出      ");
			handler.AppendFormatted(value2, "0.00");
			handler.AppendLiteral(" / ");
			handler.AppendFormatted(targetFps, "0.##");
			handler.AppendLiteral(" FPS\n");
			handler.AppendLiteral("引擎運算產能    ");
			handler.AppendFormatted(value3, "0.00");
			handler.AppendLiteral(" FPS\n");
			handler.AppendLiteral("管線速度        ");
			handler.AppendFormatted(value4, "0.00");
			handler.AppendLiteral("x");
			return string.Create(provider2, ref handler);
		}
		invariantCulture = CultureInfo.InvariantCulture;
		IFormatProvider provider3 = invariantCulture;
		handler = new DefaultInterpolatedStringHandler(77, 5, invariantCulture);
		handler.AppendFormatted(value);
		handler.AppendLiteral("\nTimeline output     ");
		handler.AppendFormatted(value2, "0.00");
		handler.AppendLiteral(" / ");
		handler.AppendFormatted(targetFps, "0.##");
		handler.AppendLiteral(" FPS\n");
		handler.AppendLiteral("Processing throughput ");
		handler.AppendFormatted(value3, "0.00");
		handler.AppendLiteral(" FPS\n");
		handler.AppendLiteral("Pipeline speed      ");
		handler.AppendFormatted(value4, "0.00");
		handler.AppendLiteral("x");
		return string.Create(provider3, ref handler);
	}

	public static string GetEncoderLabel(string outputEncoder, string? language = null)
	{
		string text = outputEncoder.ToLowerInvariant();
		string text2;
		if (text.Contains("rkmpp", StringComparison.Ordinal))
		{
			text2 = "RKMPP";
		}
		else if (text.Contains("vaapi", StringComparison.Ordinal))
		{
			text2 = "VAAPI";
		}
		else if (text.Contains("qsv", StringComparison.Ordinal))
		{
			text2 = "QSV";
		}
		else if (text.Contains("nvenc", StringComparison.Ordinal))
		{
			text2 = "NVENC";
		}
		else if (text.Contains("amf", StringComparison.Ordinal))
		{
			text2 = "AMF";
		}
		else
		{
			text2 = (text.Contains("videotoolbox", StringComparison.Ordinal) ? "VideoToolbox" : "CPU");
		}
		bool flag = text2 != "CPU";
		string text3 = Lang(language);
		if (!(text3 == "en"))
		{
			if (text3 == "ja")
			{
				return flag ? (text2 + " ハードウェアエンコード") : "CPU ソフトウェアエンコード";
			}
			return flag ? (text2 + " 硬體編碼") : "CPU 軟體編碼";
		}
		return flag ? (text2 + " hardware encode") : "CPU software encode";
	}
}
