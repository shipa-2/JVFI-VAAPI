using System;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Selects a broadly compatible H.264 encoder for the requested hardware.</summary>
public static class OutputEncoderSelector
{
	public static HardwarePipeline FromJellyfinHardware(string? hardwareAccelerationType)
	{
		return hardwareAccelerationType?.ToLowerInvariant() switch
		{
			"rkmpp" => HardwarePipeline.Rkmpp, 
			"vaapi" => HardwarePipeline.Vaapi, 
			"qsv" => HardwarePipeline.Qsv, 
			"nvenc" => HardwarePipeline.Cuda, 
			"amf" => HardwarePipeline.Amf, 
			"videotoolbox" => HardwarePipeline.VideoToolbox, 
			_ => HardwarePipeline.Software, 
		};
	}

	public static OutputEncoderSelection Select(OutputEncoderMode mode, HardwarePipeline detectedPipeline, HardwarePipeline jellyfinPipeline, string originalEncoder)
	{
		switch (mode)
		{
		case OutputEncoderMode.FollowJellyfin:
			return new OutputEncoderSelection(originalEncoder, DetectEncoderPipeline(originalEncoder));
		case OutputEncoderMode.Software:
			return new OutputEncoderSelection("libx264", HardwarePipeline.Software);
		case OutputEncoderMode.AutoHardware:
		{
			HardwarePipeline hardwarePipeline = DetectEncoderPipeline(originalEncoder);
			if (hardwarePipeline != HardwarePipeline.Software)
			{
				return new OutputEncoderSelection(originalEncoder, hardwarePipeline);
			}
			HardwarePipeline pipeline2 = (IsHardwarePipeline(jellyfinPipeline) ? jellyfinPipeline : (IsHardwarePipeline(detectedPipeline) ? detectedPipeline : HardwarePipeline.Software));
			string text2 = MapEncoder(pipeline2);
			if (text2 != null)
			{
				return new OutputEncoderSelection(text2, pipeline2);
			}
			return new OutputEncoderSelection(originalEncoder, hardwarePipeline);
		}
		default:
		{
			HardwarePipeline pipeline = MapMode(mode);
			string text = MapEncoder(pipeline);
			if (text != null)
			{
				return new OutputEncoderSelection(text, pipeline);
			}
			return new OutputEncoderSelection(originalEncoder, DetectEncoderPipeline(originalEncoder));
		}
		}
	}

	private static bool IsHardwarePipeline(HardwarePipeline pipeline)
	{
		if ((uint)(pipeline - 1) <= 5u)
		{
			return true;
		}
		return false;
	}

	private static HardwarePipeline MapMode(OutputEncoderMode mode)
	{
		return mode switch
		{
			OutputEncoderMode.Rkmpp => HardwarePipeline.Rkmpp, 
			OutputEncoderMode.Vaapi => HardwarePipeline.Vaapi, 
			OutputEncoderMode.Qsv => HardwarePipeline.Qsv, 
			OutputEncoderMode.Nvenc => HardwarePipeline.Cuda, 
			OutputEncoderMode.Amf => HardwarePipeline.Amf, 
			OutputEncoderMode.VideoToolbox => HardwarePipeline.VideoToolbox, 
			_ => HardwarePipeline.Unknown, 
		};
	}

	private static string? MapEncoder(HardwarePipeline pipeline)
	{
		return pipeline switch
		{
			HardwarePipeline.Rkmpp => "h264_rkmpp", 
			HardwarePipeline.Vaapi => "h264_vaapi", 
			HardwarePipeline.Qsv => "h264_qsv", 
			HardwarePipeline.Cuda => "h264_nvenc", 
			HardwarePipeline.Amf => "h264_amf", 
			HardwarePipeline.VideoToolbox => "h264_videotoolbox", 
			_ => null, 
		};
	}

	private static HardwarePipeline DetectEncoderPipeline(string encoder)
	{
		string text = encoder.ToLowerInvariant();
		if (text.Contains("rkmpp", StringComparison.Ordinal))
		{
			return HardwarePipeline.Rkmpp;
		}
		if (text.Contains("vaapi", StringComparison.Ordinal))
		{
			return HardwarePipeline.Vaapi;
		}
		if (text.Contains("qsv", StringComparison.Ordinal))
		{
			return HardwarePipeline.Qsv;
		}
		if (text.Contains("nvenc", StringComparison.Ordinal))
		{
			return HardwarePipeline.Cuda;
		}
		if (text.Contains("amf", StringComparison.Ordinal))
		{
			return HardwarePipeline.Amf;
		}
		if (text.Contains("videotoolbox", StringComparison.Ordinal))
		{
			return HardwarePipeline.VideoToolbox;
		}
		return HardwarePipeline.Software;
	}
}
