using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Text.RegularExpressions.Generated;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Adds a capability-gated interpolation backend to Jellyfin-generated FFmpeg commands.</summary>
public static partial class FfmpegCommandTransformer
{
	/// <summary>Transforms a video transcode command without changing unrelated arguments.</summary>
	public static FfmpegTransformResult Transform(string commandLine, FfmpegTransformRequest request)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(commandLine, "commandLine");
		ArgumentNullException.ThrowIfNull(request, "request");
		HardwarePipeline hardwarePipeline = DetectPipeline(commandLine);
		Match match = VideoCodecRegex().Match(commandLine);
		string text = (match.Success ? match.Groups[1].Value : string.Empty);
		if (VideoCopyRegex().IsMatch(commandLine))
		{
			return new FfmpegTransformResult(Applied: false, commandLine, hardwarePipeline, "影片仍是直接串流，未進入視訊轉碼。", text);
		}
		if (!match.Success)
		{
			return new FfmpegTransformResult(Applied: false, commandLine, hardwarePipeline, "命令中沒有視訊編碼器，已保留原始播放方式。");
		}
		OutputEncoderSelection outputEncoderSelection = OutputEncoderSelector.Select(request.EncoderMode, hardwarePipeline, request.JellyfinHardwarePipeline, text);
		string commandLine2 = RewriteVideoEncoder(commandLine, match, outputEncoderSelection.Encoder);
		commandLine2 = EnsureHardwareInitialization(commandLine2, outputEncoderSelection.Pipeline, request.JellyfinQsvDevice);
		if (outputEncoderSelection.Pipeline == HardwarePipeline.Software)
		{
			commandLine2 = EnsureSoftwareDecodePipeline(commandLine2);
		}

		commandLine2 = EnsureFilterHwDeviceForVaapiFilters(commandLine2);
		commandLine2 = EnsureMinimum4KBitrate(commandLine2, request, outputEncoderSelection.Encoder);
		if (!TryBuildInterpolationFilter(commandLine2, request, hardwarePipeline, out string _, out string reason))
		{
			return new FfmpegTransformResult(Applied: false, commandLine, hardwarePipeline, reason, text);
		}
		commandLine2 = PrepareHardwareFramesForSoftwareInterpolation(commandLine2, request, hardwarePipeline, outputEncoderSelection.Pipeline, outputEncoderSelection.Encoder);
		string filter2 = BuildFilter(commandLine2, request, hardwarePipeline, outputEncoderSelection.Pipeline, outputEncoderSelection.Encoder);
		string commandLine3 = TryAppendQuotedFilter(commandLine2, "-filter_complex", filter2, complex: true) ?? TryAppendQuotedFilter(commandLine2, "-vf", filter2, complex: false) ?? AddVideoFilter(commandLine2, filter2);
		commandLine3 = AddOutputFrameRate(commandLine3, request.TargetRate);
		if (!string.IsNullOrWhiteSpace(request.ProgressFile))
		{
			string text2 = request.ProgressFile.Replace("\\", "/").Replace("\"", "\\\"");
			commandLine3 = "-stats_period 0.05 -progress \"" + text2 + "\" " + commandLine3;
		}
		return new FfmpegTransformResult(Applied: true, commandLine3, hardwarePipeline, $"已套用 Universal Hybrid {request.TargetRate} FPS；interpolation={request.Backend}。", outputEncoderSelection.Encoder, outputEncoderSelection.Pipeline, outputEncoderSelection.Fallback);
	}

	/// <summary>Detects the active hardware family without relying on one vendor-specific command shape.</summary>
	public static HardwarePipeline DetectPipeline(string commandLine)
	{
		string text = commandLine.ToLowerInvariant();
		if (text.Contains("rkmpp", StringComparison.Ordinal) || text.Contains("rkrga", StringComparison.Ordinal))
		{
			return HardwarePipeline.Rkmpp;
		}
		if (text.Contains("_qsv", StringComparison.Ordinal) || text.Contains(" qsv", StringComparison.Ordinal))
		{
			return HardwarePipeline.Qsv;
		}
		if (text.Contains("_vaapi", StringComparison.Ordinal) || text.Contains(" vaapi", StringComparison.Ordinal))
		{
			return HardwarePipeline.Vaapi;
		}
		if (text.Contains("_nvenc", StringComparison.Ordinal) || text.Contains("_cuda", StringComparison.Ordinal) || text.Contains(" cuda", StringComparison.Ordinal))
		{
			return HardwarePipeline.Cuda;
		}
		if (text.Contains("_amf", StringComparison.Ordinal))
		{
			return HardwarePipeline.Amf;
		}
		if (text.Contains("_videotoolbox", StringComparison.Ordinal))
		{
			return HardwarePipeline.VideoToolbox;
		}
		return HardwarePipeline.Software;
	}

	private static string BuildFilter(string commandLine, FfmpegTransformRequest request, HardwarePipeline inputPipeline, HardwarePipeline outputPipeline, string outputEncoder)
	{
		if (!TryBuildInterpolationFilter(commandLine, request, inputPipeline, out string filter, out string reason))
		{
			throw new InvalidOperationException(reason);
		}
		List<string> list = new List<string>();
		InterpolationBackend backend = request.Backend;
		bool flag = (((uint)(backend - 1) <= 1u || backend == InterpolationBackend.Duplicate) ? true : false);
		bool flag2 = flag;
		string text = (flag2 ? ResolveInterpolationWorkingPixelFormat(commandLine, request, outputEncoder, outputPipeline) : null);
		string left = null;
		if (flag2 && NeedsHardwareDownload(commandLine))
		{
			list.Add("hwdownload");
			string text2 = ResolveHardwareDownloadPixelFormat(commandLine, request);
			list.Add("format=" + text2);
			left = text2;
			if (text != null && !PixelFormatsEquivalent(left, text))
			{
				list.Add("format=" + text);
				left = text;
			}
		}
		else if (flag2 && text != null)
		{
			list.Add("format=" + text);
			left = text;
		}
		if (request.MaxWidth > 0)
		{
			IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
			DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(19, 1, invariantCulture);
			handler.AppendLiteral("scale='min(");
			handler.AppendFormatted(request.MaxWidth);
			handler.AppendLiteral(",iw)':-2");
			list.Add(string.Create(invariantCulture, ref handler));
		}
		list.Add(filter);
		if (request.ShowHud && !string.IsNullOrWhiteSpace(request.HudTextFile))
		{
			list.Add(BuildDrawText(request));
		}
		if (flag2)
		{
			string text3 = ResolveEncoderInputPixelFormat(commandLine, request, outputEncoder, outputPipeline);
			if (text3 != null && !PixelFormatsEquivalent(left, text3))
			{
				list.Add("format=" + text3);
				left = text3;
			}
			if ((outputPipeline != HardwarePipeline.Software && outputPipeline != HardwarePipeline.Unknown) || 1 == 0)
			{
				string hardwareUpload = GetHardwareUpload(outputPipeline);
				if (hardwareUpload != null)
				{
					list.Add(hardwareUpload);
				}
			}
		}
		return string.Join(',', list);
	}

	private static bool TryBuildInterpolationFilter(string commandLine, FfmpegTransformRequest request, HardwarePipeline inputPipeline, out string filter, out string reason)
	{
		filter = string.Empty;
		reason = string.Empty;
		string text = request.TargetRate.ToFfmpegExpression();
		if (request.Backend != InterpolationBackend.None && IsProtectedHdr(commandLine, request))
		{
			reason = "偵測到 HDR / HLG / Dolby Vision，0.7.6 本地測試版仍保留原始 Jellyfin HDR 路徑，不套用補幀。";
			return false;
		}
		switch (request.Backend)
		{
		case InterpolationBackend.OfficialMinterpolate:
		{
			if (inputPipeline == HardwarePipeline.Rkmpp)
			{
				reason = "RKMPP 的 software minterpolate 尚未通過端到端即時播放驗證；目前使用官方 framerate 相容 backend，避免 HLS 啟動停滯。";
				return false;
			}
			string value = request.Quality switch
			{
				JVFIQualityProfile.Extreme => "epzs", 
				JVFIQualityProfile.Quality => "epzs", 
				JVFIQualityProfile.Eco => "epzs", 
				_ => "epzs", 
			};
			int value2 = request.Quality switch
			{
				JVFIQualityProfile.Extreme => 24, 
				JVFIQualityProfile.Quality => 20, 
				JVFIQualityProfile.Eco => 16, 
				_ => 24, 
			};
			int value3 = 16;
			string value4 = ((request.Quality == JVFIQualityProfile.Extreme) ? "aobmc" : "obmc");
			int value5 = ((request.Quality == JVFIQualityProfile.Extreme) ? 1 : 0);
			filter = $"minterpolate=fps={text}:mi_mode=mci:mc_mode={value4}:me_mode=bilat:me={value}:mb_size={value3}:search_param={value2}:vsbmc={value5}:scd=fdiff:scd_threshold=10";
			return true;
		}
		case InterpolationBackend.OfficialFramerate:
			filter = "framerate=fps=" + text;
			return true;
		case InterpolationBackend.Duplicate:
			filter = "fps=fps=" + text + ":round=near";
			return true;
		case InterpolationBackend.IntelQsvFrc:
			if (inputPipeline != HardwarePipeline.Qsv)
			{
				reason = "Intel QSV FRC 只允許在已確認的 QSV pipeline 使用。";
				return false;
			}
			filter = "vpp_qsv=framerate=" + text;
			return true;
		case InterpolationBackend.NativeCpu:
		case InterpolationBackend.NativeExternal:
			reason = "Native Motion Core 尚未建立 Official FFmpeg ↔ Native Engine Data Plane，未修改 FFmpeg 命令。";
			return false;
		default:
			reason = "沒有通過 capability probe 的補幀 backend，保留 Jellyfin 原始播放路徑。";
			return false;
		}
	}

	private static bool IsProtectedHdr(string commandLine, FfmpegTransformRequest request)
	{
		if (request.IsHdr)
		{
			return true;
		}
		return HdrSignalRegex().IsMatch(commandLine);
	}

	private static string ResolveHardwareDownloadPixelFormat(string commandLine, FfmpegTransformRequest request)
	{
		Match match = HardwareFilterPixelFormatRegex().Matches(commandLine).Cast<Match>().LastOrDefault();
		if (match != null)
		{
			return NormalizeHardwarePixelFormat(match.Groups[1].Value);
		}
		if (!IsHighBitDepthPixelFormat(request.SourcePixelFormat))
		{
			return "nv12";
		}
		return "p010le";
	}

	private static string? ResolveInterpolationWorkingPixelFormat(string commandLine, FfmpegTransformRequest request, string outputEncoder, HardwarePipeline outputPipeline)
	{
		if (!IsHighBitDepthPixelFormat(request.SourcePixelFormat) || OutputRequiresHighBitDepth(commandLine, request, outputEncoder))
		{
			return null;
		}
		if (IsH264Encoder(outputEncoder))
		{
			if ((outputPipeline != HardwarePipeline.Software && outputPipeline != HardwarePipeline.Unknown) || 1 == 0)
			{
				return "nv12";
			}
			return "yuv420p";
		}
		if ((outputPipeline != HardwarePipeline.Software && outputPipeline != HardwarePipeline.Unknown) || 1 == 0)
		{
			return "nv12";
		}
		return null;
	}

	private static string? ResolveEncoderInputPixelFormat(string commandLine, FfmpegTransformRequest request, string outputEncoder, HardwarePipeline outputPipeline)
	{
		bool flag = ((outputPipeline == HardwarePipeline.Software || outputPipeline == HardwarePipeline.Unknown) ? true : false);
		bool flag2 = !flag;
		if (IsH264Encoder(outputEncoder))
		{
			if (flag2)
			{
				return "nv12";
			}
			if (!IsHighBitDepthPixelFormat(request.SourcePixelFormat))
			{
				return null;
			}
			return "yuv420p";
		}
		if (!flag2)
		{
			return null;
		}
		if (OutputRequiresHighBitDepth(commandLine, request, outputEncoder))
		{
			string text = ResolveExplicitHighBitDepthOutputPixelFormat(commandLine);
			if (text == null)
			{
				if (!IsTenBitPixelFormat(request.SourcePixelFormat))
				{
					return null;
				}
				text = "p010le";
			}
			return text;
		}
		return "nv12";
	}

	private static bool OutputRequiresHighBitDepth(string commandLine, FfmpegTransformRequest request, string outputEncoder)
	{
		if (!IsHighBitDepthPixelFormat(request.SourcePixelFormat) || IsH264Encoder(outputEncoder))
		{
			return false;
		}
		if (ResolveExplicitHighBitDepthOutputPixelFormat(commandLine) != null)
		{
			return true;
		}
		int lastInput = commandLine.LastIndexOf(" -i ", StringComparison.Ordinal);
		return Main10ProfileRegex().Matches(commandLine).Cast<Match>().Any((Match match) => match.Index > lastInput);
	}

	private static string? ResolveExplicitHighBitDepthOutputPixelFormat(string commandLine)
	{
		int lastInput = commandLine.LastIndexOf(" -i ", StringComparison.Ordinal);
		Match match = OutputPixelFormatRegex().Matches(commandLine).Cast<Match>().LastOrDefault((Match match3) => match3.Index > lastInput);
		if (match != null && IsHighBitDepthPixelFormat(match.Groups[1].Value))
		{
			return NormalizeHardwarePixelFormat(match.Groups[1].Value);
		}
		Match match2 = HardwareFilterPixelFormatRegex().Matches(commandLine).Cast<Match>().LastOrDefault((Match match3) => match3.Index > lastInput);
		if (match2 != null && IsHighBitDepthPixelFormat(match2.Groups[1].Value))
		{
			return NormalizeHardwarePixelFormat(match2.Groups[1].Value);
		}
		return null;
	}

	private static bool IsH264Encoder(string encoder)
	{
		if (!encoder.Contains("h264", StringComparison.OrdinalIgnoreCase))
		{
			return encoder.Contains("libx264", StringComparison.OrdinalIgnoreCase);
		}
		return true;
	}

	private static bool PixelFormatsEquivalent(string? left, string? right)
	{
		if (left == null || right == null)
		{
			return false;
		}
		return string.Equals(NormalizeHardwarePixelFormat(left), NormalizeHardwarePixelFormat(right), StringComparison.OrdinalIgnoreCase);
	}

	private static bool IsHighBitDepthPixelFormat(string pixelFormat)
	{
		if (!string.IsNullOrWhiteSpace(pixelFormat))
		{
			return HighBitDepthPixelFormatRegex().IsMatch(pixelFormat);
		}
		return false;
	}

	private static string NormalizeHardwarePixelFormat(string pixelFormat)
	{
		string text = pixelFormat.ToLowerInvariant();
		if (!(text == "p010"))
		{
			if (text == "p012")
			{
				return "p012le";
			}
			return text;
		}
		return "p010le";
	}

	private static string PrepareHardwareFramesForSoftwareInterpolation(string commandLine, FfmpegTransformRequest request, HardwarePipeline inputPipeline, HardwarePipeline outputPipeline, string outputEncoder)
	{
		string text = EnsureDownloadableHardwareFrames(commandLine);
		if (!UsesSoftwareInterpolation(request.Backend) || !IsTenBitPixelFormat(request.SourcePixelFormat) || !OutputRequiresHighBitDepth(text, request, outputEncoder))
		{
			return text;
		}
		return inputPipeline switch
		{
			HardwarePipeline.Rkmpp => RewriteEightBitHardwareFilter(text, RkrgaEightBitFormatRegex(), "p010"), 
			HardwarePipeline.Qsv => RewriteEightBitHardwareFilter(text, QsvEightBitFormatRegex(), "p010"), 
			HardwarePipeline.Vaapi => RewriteEightBitHardwareFilter(text, VaapiEightBitFormatRegex(), "p010"), 
			HardwarePipeline.Cuda => RewriteEightBitHardwareFilter(text, CudaEightBitFormatRegex(), "p010le"), 
			_ => text, 
		};
	}

	private static bool UsesSoftwareInterpolation(InterpolationBackend backend)
	{
		if ((uint)(backend - 1) <= 1u || backend == InterpolationBackend.Duplicate)
		{
			return true;
		}
		return false;
	}

	private static bool IsTenBitPixelFormat(string pixelFormat)
	{
		if (!string.IsNullOrWhiteSpace(pixelFormat))
		{
			return TenBit420PixelFormatRegex().IsMatch(pixelFormat);
		}
		return false;
	}

	private static string RewriteEightBitHardwareFilter(string commandLine, Regex regex, string hardwarePixelFormat)
	{
		return regex.Replace(commandLine, (Match match) => match.Groups[1].Value + hardwarePixelFormat);
	}

	private static string EnsureDownloadableHardwareFrames(string commandLine)
	{
		if (!NeedsHardwareDownload(commandLine))
		{
			return commandLine;
		}
		return RkrgaFilterRegex().Replace(commandLine, (Match match) => match.Value.Replace(":afbc=1", ":afbc=0", StringComparison.OrdinalIgnoreCase));
	}

	private static bool NeedsHardwareDownload(string commandLine)
	{
		string text = commandLine.ToLowerInvariant();
		if (text.Contains("hwdownload", StringComparison.Ordinal))
		{
			return false;
		}
		if (!text.Contains("scale_vaapi", StringComparison.Ordinal) && !text.Contains("tonemap_vaapi", StringComparison.Ordinal) && !text.Contains("scale_qsv", StringComparison.Ordinal) && !text.Contains("vpp_qsv", StringComparison.Ordinal) && !text.Contains("scale_cuda", StringComparison.Ordinal) && !text.Contains("overlay_cuda", StringComparison.Ordinal) && !text.Contains("vpp_rkrga", StringComparison.Ordinal) && !text.Contains("-hwaccel_output_format vaapi", StringComparison.Ordinal) && !text.Contains("-hwaccel_output_format qsv", StringComparison.Ordinal) && !text.Contains("-hwaccel_output_format cuda", StringComparison.Ordinal))
		{
			return text.Contains("-hwaccel_output_format drm_prime", StringComparison.Ordinal);
		}
		return true;
	}

	private static string? GetHardwareUpload(HardwarePipeline pipeline)
	{
		switch (pipeline)
		{
		case HardwarePipeline.Cuda:
			return "hwupload_cuda";
		case HardwarePipeline.Vaapi:
			return "hwupload_vaapi";
		case HardwarePipeline.Qsv:
			return "hwupload";
		default:
			return null;
		}
	}

	private static string RewriteVideoEncoder(string commandLine, Match codecMatch, string outputEncoder)
	{
		if (string.Equals(codecMatch.Groups[1].Value, outputEncoder, StringComparison.OrdinalIgnoreCase))
		{
			return commandLine;
		}
		string text = commandLine.Substring(0, codecMatch.Groups[1].Index);
		int num = codecMatch.Groups[1].Index + codecMatch.Groups[1].Length;
		string input = text + outputEncoder + commandLine.Substring(num, commandLine.Length - num);
		return EncoderSpecificOptionRegex().Replace(input, string.Empty);
	}

	private static string EnsureHardwareInitialization(string commandLine, HardwarePipeline pipeline, string jellyfinQsvDevice)
	{
		if (pipeline == HardwarePipeline.Rkmpp && !commandLine.Contains("-init_hw_device rkmpp", StringComparison.OrdinalIgnoreCase))
		{
			return "-init_hw_device rkmpp=rk " + commandLine;
		}
		if (pipeline == HardwarePipeline.Qsv && !string.IsNullOrWhiteSpace(jellyfinQsvDevice) && !commandLine.Contains("-qsv_device ", StringComparison.OrdinalIgnoreCase) && !commandLine.Contains("-init_hw_device qsv", StringComparison.OrdinalIgnoreCase))
		{
			string text = jellyfinQsvDevice.Replace("\"", "\\\"", StringComparison.Ordinal);
			commandLine = "-qsv_device \"" + text + "\" " + commandLine;
		}
		if (pipeline == HardwarePipeline.Qsv && commandLine.Contains("-qsv_device ", StringComparison.OrdinalIgnoreCase) && !commandLine.Contains("-filter_hw_device ", StringComparison.OrdinalIgnoreCase))
		{
		 commandLine = "-filter_hw_device __qsv_device " + commandLine;
		}
		return commandLine;
	}

	/// <summary>
	/// Jellyfin 12 sets <c>-filter_hw_device vk</c> for libplacebo. VAAPI filters (<c>scale_vaapi</c>, <c>hwupload</c>) must use <c>va</c>.
	/// </summary>
	private static string EnsureFilterHwDeviceForVaapiFilters(string commandLine)
	{
		if (!commandLine.Contains("-init_hw_device vaapi=va", StringComparison.OrdinalIgnoreCase))
		{
			return commandLine;
		}

		return Regex.Replace(
			commandLine,
			"-filter_hw_device\\s+vk\\b",
			"-filter_hw_device va",
			RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
	}

	/// <summary>
	/// Software encode (libx264) with CPU framerate must not touch the GPU — hybrid VAAPI decode caused amdgpu context loss on AMD.
	/// </summary>
	private static string EnsureSoftwareDecodePipeline(string commandLine)
	{
		string text = commandLine;
		text = Regex.Replace(text, "-hwaccel\\s+vaapi\\s+", string.Empty, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		text = Regex.Replace(text, "-hwaccel_output_format\\s+vaapi\\s+", string.Empty, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		text = Regex.Replace(text, "-filter_hw_device\\s+\\S+\\s*", string.Empty, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		text = Regex.Replace(text, "-init_hw_device\\s+vulkan=vk@dr\\s+", string.Empty, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		text = Regex.Replace(text, "-init_hw_device\\s+vaapi=va@dr\\s+", string.Empty, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		text = Regex.Replace(text, "-init_hw_device\\s+drm=dr:/dev/dri/renderD128\\s+", string.Empty, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		text = Regex.Replace(text, "scale_vaapi=format=nv12", "format=yuv420p", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		text = Regex.Replace(text, "scale_vaapi=[^,\"]+", "format=yuv420p", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
		return text;
	}

	private static string EnsureMinimum4KBitrate(string commandLine, FfmpegTransformRequest request, string outputEncoder)
	{
		if (request.TargetFps < 50.0 || request.SourceWidth <= 0 || !outputEncoder.StartsWith("h264", StringComparison.OrdinalIgnoreCase))
		{
			return commandLine;
		}
		int num = ((request.MaxWidth > 0) ? Math.Min(request.SourceWidth, request.MaxWidth) : request.SourceWidth);
		int num2;
		if (num <= 1280)
		{
			num2 = ((num > 854) ? request.Minimum720pBitrateMbps : request.Minimum480pBitrateMbps);
		}
		else
		{
			num2 = ((num > 1920) ? request.Minimum4KBitrateMbps : request.Minimum1080pBitrateMbps);
		}
		int num3 = num2;
		if (num3 <= 0)
		{
			return commandLine;
		}
		long num4 = (long)num3 * 1000000L;
		return RaiseNumericOption(RaiseNumericOption(RaiseNumericOption(commandLine, VideoBitrateRegex(), "-b:v", num4), MaximumBitrateRegex(), "-maxrate", num4), BufferSizeRegex(), "-bufsize", num4 * 2);
	}

	private static string RaiseNumericOption(string commandLine, Regex regex, string option, long minimum)
	{
		Match match = regex.Match(commandLine);
		if (!match.Success || !long.TryParse(match.Groups[1].Value, NumberStyles.None, CultureInfo.InvariantCulture, out var result) || result >= minimum)
		{
			return commandLine;
		}
		IFormatProvider invariantCulture = CultureInfo.InvariantCulture;
		DefaultInterpolatedStringHandler handler = new DefaultInterpolatedStringHandler(1, 2, invariantCulture);
		handler.AppendFormatted(option);
		handler.AppendLiteral(" ");
		handler.AppendFormatted(minimum);
		string text = string.Create(invariantCulture, ref handler);
		string text2 = commandLine.Substring(0, match.Index);
		int num = match.Index + match.Length;
		return text2 + text + commandLine.Substring(num, commandLine.Length - num);
	}

	private static string BuildDrawText(FfmpegTransformRequest request)
	{
		string text = request.HudTextFile.Replace("\\", "/", StringComparison.Ordinal).Replace(":", "\\:", StringComparison.Ordinal).Replace("'", "\\'", StringComparison.Ordinal);
		return "drawtext=font='Noto Sans CJK TC':textfile='" + text + "':reload=1:fontcolor=0xF9FAFB:fontsize='min(max(h/27.5\\,30)\\,48)':box=1:boxcolor=0x111827@0.82:boxborderw=10:borderw=0:shadowcolor=black@0.35:shadowx=1:shadowy=1:" + request.HudPosition switch
		{
			"TopRight" => "x=w-tw-16:y=16", 
			"BottomLeft" => "x=16:y=h-th-16", 
			"BottomRight" => "x=w-tw-16:y=h-th-16", 
			_ => "x=16:y=16", 
		};
	}

	private static string? TryAppendQuotedFilter(string commandLine, string option, string filter, bool complex)
	{
		string text = option + " \"";
		int num = commandLine.IndexOf(text, StringComparison.Ordinal);
		if (num < 0)
		{
			return null;
		}
		int num2 = num + text.Length;
		int num3 = FindClosingQuote(commandLine, num2);
		if (num3 < 0)
		{
			return null;
		}
		int num4 = num2;
		string text2 = commandLine.Substring(num4, num3 - num4);
		string text3 = (complex ? AppendToFinalComplexChain(text2, filter) : (text2 + "," + filter));
		string text4 = commandLine.Substring(0, num2);
		num4 = num3;
		return text4 + text3 + commandLine.Substring(num4, commandLine.Length - num4);
	}

	private static string AppendToFinalComplexChain(string graph, string filter)
	{
		int num = graph.LastIndexOf(';') + 1;
		string text = graph;
		int num2 = num;
		string input = text.Substring(num2, text.Length - num2);
		Match match = OutputLabelRegex().Match(input);
		if (!match.Success)
		{
			return graph + "," + filter;
		}
		int num3 = num + match.Index;
		string text2 = graph.Substring(0, num3);
		text = graph;
		num2 = num3;
		return text2 + "," + filter + text.Substring(num2, text.Length - num2);
	}

	private static int FindClosingQuote(string value, int start)
	{
		for (int i = start; i < value.Length; i++)
		{
			if (value[i] == '"' && (i == 0 || value[i - 1] != '\\'))
			{
				return i;
			}
		}
		return -1;
	}

	private static string AddVideoFilter(string commandLine, string filter)
	{
		int num = commandLine.LastIndexOf(" -f ", StringComparison.Ordinal);
		if (num < 0)
		{
			return commandLine + " -vf \"" + filter + "\"";
		}
		return commandLine.Insert(num, " -vf \"" + filter + "\"");
	}

	private static string AddOutputFrameRate(string commandLine, FpsRational targetFps)
	{
		int lastInput = commandLine.LastIndexOf(" -i ", StringComparison.Ordinal);
		Match match = OutputRateRegex().Matches(commandLine).Cast<Match>().FirstOrDefault((Match match2) => match2.Index > lastInput);
		string text = "-r:v:0 " + targetFps.ToFfmpegExpression();
		if (match != null)
		{
			string text2 = commandLine.Substring(0, match.Index);
			int num = match.Index + match.Length;
			return text2 + text + commandLine.Substring(num, commandLine.Length - num);
		}
		int num2 = commandLine.LastIndexOf(" -f ", StringComparison.Ordinal);
		if (num2 < 0)
		{
			return commandLine + " " + text;
		}
		return commandLine.Insert(num2, " " + text);
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?:-codec:v(?::0)?|-c:v(?::0)?)\\s+copy(?:\\s|$)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Match '-'.<br />
	/// ○ Match a character in the set [Cc].<br />
	/// ○ Match with 2 alternative expressions.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Oo].<br />
	///         ○ Match a character in the set [Dd].<br />
	///         ○ Match a character in the set [Ee].<br />
	///         ○ Match a character in the set [Cc].<br />
	///         ○ Match ':'.<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Optional (greedy).<br />
	///             ○ Match the string ":0".<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match ':'.<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Optional (greedy).<br />
	///             ○ Match the string ":0".<br />
	/// ○ Match a whitespace character atomically at least once.<br />
	/// ○ Match a character in the set [Cc].<br />
	/// ○ Match a character in the set [Oo].<br />
	/// ○ Match a character in the set [Pp].<br />
	/// ○ Match a character in the set [Yy].<br />
	/// ○ Match with 2 alternative expressions, atomically.<br />
	///     ○ Match a whitespace character.<br />
	///     ○ Match if at the end of the string or if before an ending newline.<br />
	/// </code>
	/// </remarks>
	private static Regex VideoCopyRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VideoCopyRegex_0.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?:-codec:v(?::0)?|-c:v(?::0)?)\\s+([^\\s\\"]+)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Match '-'.<br />
	/// ○ Match a character in the set [Cc].<br />
	/// ○ Match with 2 alternative expressions.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Oo].<br />
	///         ○ Match a character in the set [Dd].<br />
	///         ○ Match a character in the set [Ee].<br />
	///         ○ Match a character in the set [Cc].<br />
	///         ○ Match ':'.<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Optional (greedy).<br />
	///             ○ Match the string ":0".<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match ':'.<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Optional (greedy).<br />
	///             ○ Match the string ":0".<br />
	/// ○ Match a whitespace character greedily at least once.<br />
	/// ○ 1st capture group.<br />
	///     ○ Match a character in the set [^"\s] atomically at least once.<br />
	/// </code>
	/// </remarks>
	private static Regex VideoCodecRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VideoCodecRegex_1.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>-r:v(?::0)?\\s+\\S+</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Match '-'.<br />
	/// ○ Match a character in the set [Rr].<br />
	/// ○ Match ':'.<br />
	/// ○ Match a character in the set [Vv].<br />
	/// ○ Optional (greedy).<br />
	///     ○ Match the string ":0".<br />
	/// ○ Match a whitespace character greedily at least once.<br />
	/// ○ Match any character other than a whitespace character atomically at least once.<br />
	/// </code>
	/// </remarks>
	private static Regex OutputRateRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__OutputRateRegex_2.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?&lt;!\\S)-b:v(?::0)?\\s+(\\d+)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Zero-width negative lookbehind.<br />
	///     ○ Match any character other than a whitespace character right-to-left.<br />
	/// ○ Match '-'.<br />
	/// ○ Match a character in the set [Bb].<br />
	/// ○ Match ':'.<br />
	/// ○ Match a character in the set [Vv].<br />
	/// ○ Optional (greedy).<br />
	///     ○ Match the string ":0".<br />
	/// ○ Match a whitespace character atomically at least once.<br />
	/// ○ 1st capture group.<br />
	///     ○ Match a Unicode digit atomically at least once.<br />
	/// </code>
	/// </remarks>
	private static Regex VideoBitrateRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VideoBitrateRegex_3.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?&lt;!\\S)-bufsize(?::v(?::0)?)?\\s+(\\d+)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Zero-width negative lookbehind.<br />
	///     ○ Match any character other than a whitespace character right-to-left.<br />
	/// ○ Match '-'.<br />
	/// ○ Match a character in the set [Bb].<br />
	/// ○ Match a character in the set [Uu].<br />
	/// ○ Match a character in the set [Ff].<br />
	/// ○ Match a character in the set [Ss].<br />
	/// ○ Match a character in the set [Ii].<br />
	/// ○ Match a character in the set [Zz].<br />
	/// ○ Match a character in the set [Ee].<br />
	/// ○ Optional (greedy).<br />
	///     ○ Match ':'.<br />
	///     ○ Match a character in the set [Vv].<br />
	///     ○ Optional (greedy).<br />
	///         ○ Match the string ":0".<br />
	/// ○ Match a whitespace character atomically at least once.<br />
	/// ○ 1st capture group.<br />
	///     ○ Match a Unicode digit atomically at least once.<br />
	/// </code>
	/// </remarks>
	private static Regex BufferSizeRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__BufferSizeRegex_4.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?&lt;!\\S)-maxrate(?::v(?::0)?)?\\s+(\\d+)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Zero-width negative lookbehind.<br />
	///     ○ Match any character other than a whitespace character right-to-left.<br />
	/// ○ Match '-'.<br />
	/// ○ Match a character in the set [Mm].<br />
	/// ○ Match a character in the set [Aa].<br />
	/// ○ Match a character in the set [Xx].<br />
	/// ○ Match a character in the set [Rr].<br />
	/// ○ Match a character in the set [Aa].<br />
	/// ○ Match a character in the set [Tt].<br />
	/// ○ Match a character in the set [Ee].<br />
	/// ○ Optional (greedy).<br />
	///     ○ Match ':'.<br />
	///     ○ Match a character in the set [Vv].<br />
	///     ○ Optional (greedy).<br />
	///         ○ Match the string ":0".<br />
	/// ○ Match a whitespace character atomically at least once.<br />
	/// ○ 1st capture group.<br />
	///     ○ Match a Unicode digit atomically at least once.<br />
	/// </code>
	/// </remarks>
	private static Regex MaximumBitrateRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__MaximumBitrateRegex_5.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(\\[[^\\[\\]]+\\])$</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ 1st capture group.<br />
	///     ○ Match '['.<br />
	///     ○ Match a character in the set [^[]] atomically at least once.<br />
	///     ○ Match ']'.<br />
	/// ○ Match if at the end of the string or if before an ending newline.<br />
	/// </code>
	/// </remarks>
	private static Regex OutputLabelRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__OutputLabelRegex_6.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>\\s+-(?:svtav1-params(?::\\d+)?|preset(?::v(?::\\d+)?)?|profile:v(?::\\d+)?|level(?::v(?::\\d+)?)?)\\s+[^\\s"]+</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Match a whitespace character atomically at least once.<br />
	/// ○ Match '-'.<br />
	/// ○ Match with 3 alternative expressions.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Ss].<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match a character in the set [Tt].<br />
	///         ○ Match a character in the set [Aa].<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match the string "1-".<br />
	///         ○ Match a character in the set [Pp].<br />
	///         ○ Match a character in the set [Aa].<br />
	///         ○ Match a character in the set [Rr].<br />
	///         ○ Match a character in the set [Aa].<br />
	///         ○ Match a character in the set [Mm].<br />
	///         ○ Match a character in the set [Ss].<br />
	///         ○ Optional (greedy).<br />
	///             ○ Match ':'.<br />
	///             ○ Match a Unicode digit atomically at least once.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Pp].<br />
	///         ○ Match a character in the set [Rr].<br />
	///         ○ Match with 2 alternative expressions.<br />
	///             ○ Match a sequence of expressions.<br />
	///                 ○ Match a character in the set [Ee].<br />
	///                 ○ Match a character in the set [Ss].<br />
	///                 ○ Match a character in the set [Ee].<br />
	///                 ○ Match a character in the set [Tt].<br />
	///                 ○ Optional (greedy).<br />
	///                     ○ Match ':'.<br />
	///                     ○ Match a character in the set [Vv].<br />
	///                     ○ Optional (greedy).<br />
	///                         ○ Match ':'.<br />
	///                         ○ Match a Unicode digit greedily at least once.<br />
	///             ○ Match a sequence of expressions.<br />
	///                 ○ Match a character in the set [Oo].<br />
	///                 ○ Match a character in the set [Ff].<br />
	///                 ○ Match a character in the set [Ii].<br />
	///                 ○ Match a character in the set [Ll].<br />
	///                 ○ Match a character in the set [Ee].<br />
	///                 ○ Match ':'.<br />
	///                 ○ Match a character in the set [Vv].<br />
	///                 ○ Optional (greedy).<br />
	///                     ○ Match ':'.<br />
	///                     ○ Match a Unicode digit atomically at least once.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Ll].<br />
	///         ○ Match a character in the set [Ee].<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match a character in the set [Ee].<br />
	///         ○ Match a character in the set [Ll].<br />
	///         ○ Optional (greedy).<br />
	///             ○ Match ':'.<br />
	///             ○ Match a character in the set [Vv].<br />
	///             ○ Optional (greedy).<br />
	///                 ○ Match ':'.<br />
	///                 ○ Match a Unicode digit greedily at least once.<br />
	/// ○ Match a whitespace character greedily at least once.<br />
	/// ○ Match a character in the set [^"\s] atomically at least once.<br />
	/// </code>
	/// </remarks>
	private static Regex EncoderSpecificOptionRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__EncoderSpecificOptionRegex_7.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?&lt;!\\S)-pix_fmt(?::v(?::0)?)?\\s+([a-z0-9_]+)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Zero-width negative lookbehind.<br />
	///     ○ Match any character other than a whitespace character right-to-left.<br />
	/// ○ Match '-'.<br />
	/// ○ Match a character in the set [Pp].<br />
	/// ○ Match a character in the set [Ii].<br />
	/// ○ Match a character in the set [Xx].<br />
	/// ○ Match '_'.<br />
	/// ○ Match a character in the set [Ff].<br />
	/// ○ Match a character in the set [Mm].<br />
	/// ○ Match a character in the set [Tt].<br />
	/// ○ Optional (greedy).<br />
	///     ○ Match ':'.<br />
	///     ○ Match a character in the set [Vv].<br />
	///     ○ Optional (greedy).<br />
	///         ○ Match the string ":0".<br />
	/// ○ Match a whitespace character atomically at least once.<br />
	/// ○ 1st capture group.<br />
	///     ○ Match a character in the set [0-9A-Z_a-z\u212A] atomically at least once.<br />
	/// </code>
	/// </remarks>
	private static Regex OutputPixelFormatRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__OutputPixelFormatRegex_8.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?&lt;!\\S)-profile:v(?::0)?\\s+(?:main10|main12|rext)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Zero-width negative lookbehind.<br />
	///     ○ Match any character other than a whitespace character right-to-left.<br />
	/// ○ Match '-'.<br />
	/// ○ Match a character in the set [Pp].<br />
	/// ○ Match a character in the set [Rr].<br />
	/// ○ Match a character in the set [Oo].<br />
	/// ○ Match a character in the set [Ff].<br />
	/// ○ Match a character in the set [Ii].<br />
	/// ○ Match a character in the set [Ll].<br />
	/// ○ Match a character in the set [Ee].<br />
	/// ○ Match ':'.<br />
	/// ○ Match a character in the set [Vv].<br />
	/// ○ Optional (greedy).<br />
	///     ○ Match the string ":0".<br />
	/// ○ Match a whitespace character atomically at least once.<br />
	/// ○ Match with 2 alternative expressions, atomically.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Aa].<br />
	///         ○ Match a character in the set [Ii].<br />
	///         ○ Match a character in the set [Nn].<br />
	///         ○ Match '1'.<br />
	///         ○ Match a character in the set [02].<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Ee].<br />
	///         ○ Match a character in the set [Xx].<br />
	///         ○ Match a character in the set [Tt].<br />
	/// </code>
	/// </remarks>
	private static Regex Main10ProfileRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__Main10ProfileRegex_9.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?:-color_trc(?::\\w+)?\\s+(?:smpte2084|smpte-2084|arib-std-b67)|-color_transfer(?::\\w+)?\\s+(?:smpte2084|smpte-2084|arib-std-b67)|zscale=[^,\\s"']*?(?:transfer|t)=(?:smpte2084|smpte-2084|arib-std-b67))</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Match with 2 alternative expressions, atomically.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Cc].<br />
	///         ○ Match a character in the set [Oo].<br />
	///         ○ Match a character in the set [Ll].<br />
	///         ○ Match a character in the set [Oo].<br />
	///         ○ Match a character in the set [Rr].<br />
	///         ○ Match '_'.<br />
	///         ○ Match a character in the set [Tt].<br />
	///         ○ Match a character in the set [Rr].<br />
	///         ○ Match with 2 alternative expressions, atomically.<br />
	///             ○ Match a sequence of expressions.<br />
	///                 ○ Match an empty string.<br />
	///                 ○ Optional (greedy).<br />
	///                     ○ Match ':'.<br />
	///                     ○ Match a word character atomically at least once.<br />
	///                 ○ Match a whitespace character atomically at least once.<br />
	///                 ○ Match with 2 alternative expressions, atomically.<br />
	///                     ○ Match a sequence of expressions.<br />
	///                         ○ Match an empty string.<br />
	///                         ○ Match a character in the set [Mm].<br />
	///                         ○ Match a character in the set [Pp].<br />
	///                         ○ Match a character in the set [Tt].<br />
	///                         ○ Match a character in the set [Ee].<br />
	///                         ○ Match with 2 alternative expressions, atomically.<br />
	///                             ○ Match the string "2084".<br />
	///                             ○ Match the string "-2084".<br />
	///                     ○ Match a sequence of expressions.<br />
	///                         ○ Match an empty string.<br />
	///                         ○ Match a character in the set [Rr].<br />
	///                         ○ Match a character in the set [Ii].<br />
	///                         ○ Match a character in the set [Bb].<br />
	///                         ○ Match '-'.<br />
	///                         ○ Match a character in the set [Ss].<br />
	///                         ○ Match a character in the set [Tt].<br />
	///                         ○ Match a character in the set [Dd].<br />
	///                         ○ Match '-'.<br />
	///                         ○ Match a character in the set [Bb].<br />
	///                         ○ Match the string "67".<br />
	///             ○ Match a sequence of expressions.<br />
	///                 ○ Match an empty string.<br />
	///                 ○ Match a character in the set [Nn].<br />
	///                 ○ Match a character in the set [Ss].<br />
	///                 ○ Match a character in the set [Ff].<br />
	///                 ○ Match a character in the set [Ee].<br />
	///                 ○ Match a character in the set [Rr].<br />
	///                 ○ Optional (greedy).<br />
	///                     ○ Match ':'.<br />
	///                     ○ Match a word character atomically at least once.<br />
	///                 ○ Match a whitespace character atomically at least once.<br />
	///                 ○ Match with 2 alternative expressions, atomically.<br />
	///                     ○ Match a sequence of expressions.<br />
	///                         ○ Match an empty string.<br />
	///                         ○ Match a character in the set [Mm].<br />
	///                         ○ Match a character in the set [Pp].<br />
	///                         ○ Match a character in the set [Tt].<br />
	///                         ○ Match a character in the set [Ee].<br />
	///                         ○ Match with 2 alternative expressions, atomically.<br />
	///                             ○ Match the string "2084".<br />
	///                             ○ Match the string "-2084".<br />
	///                     ○ Match a sequence of expressions.<br />
	///                         ○ Match an empty string.<br />
	///                         ○ Match a character in the set [Rr].<br />
	///                         ○ Match a character in the set [Ii].<br />
	///                         ○ Match a character in the set [Bb].<br />
	///                         ○ Match '-'.<br />
	///                         ○ Match a character in the set [Ss].<br />
	///                         ○ Match a character in the set [Tt].<br />
	///                         ○ Match a character in the set [Dd].<br />
	///                         ○ Match '-'.<br />
	///                         ○ Match a character in the set [Bb].<br />
	///                         ○ Match the string "67".<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Ss].<br />
	///         ○ Match a character in the set [Cc].<br />
	///         ○ Match a character in the set [Aa].<br />
	///         ○ Match a character in the set [Ll].<br />
	///         ○ Match a character in the set [Ee].<br />
	///         ○ Match '='.<br />
	///         ○ Match a character in the set [^"',\s] lazily any number of times.<br />
	///         ○ Match with 2 alternative expressions.<br />
	///             ○ Match a sequence of expressions.<br />
	///                 ○ Match a character in the set [Tt].<br />
	///                 ○ Match a character in the set [Rr].<br />
	///                 ○ Match a character in the set [Aa].<br />
	///                 ○ Match a character in the set [Nn].<br />
	///                 ○ Match a character in the set [Ss].<br />
	///                 ○ Match a character in the set [Ff].<br />
	///                 ○ Match a character in the set [Ee].<br />
	///                 ○ Match a character in the set [Rr].<br />
	///             ○ Match a character in the set [Tt].<br />
	///         ○ Match '='.<br />
	///         ○ Match with 2 alternative expressions, atomically.<br />
	///             ○ Match a sequence of expressions.<br />
	///                 ○ Match an empty string.<br />
	///                 ○ Match a character in the set [Mm].<br />
	///                 ○ Match a character in the set [Pp].<br />
	///                 ○ Match a character in the set [Tt].<br />
	///                 ○ Match a character in the set [Ee].<br />
	///                 ○ Match with 2 alternative expressions, atomically.<br />
	///                     ○ Match the string "2084".<br />
	///                     ○ Match the string "-2084".<br />
	///             ○ Match a sequence of expressions.<br />
	///                 ○ Match an empty string.<br />
	///                 ○ Match a character in the set [Rr].<br />
	///                 ○ Match a character in the set [Ii].<br />
	///                 ○ Match a character in the set [Bb].<br />
	///                 ○ Match '-'.<br />
	///                 ○ Match a character in the set [Ss].<br />
	///                 ○ Match a character in the set [Tt].<br />
	///                 ○ Match a character in the set [Dd].<br />
	///                 ○ Match '-'.<br />
	///                 ○ Match a character in the set [Bb].<br />
	///                 ○ Match the string "67".<br />
	/// </code>
	/// </remarks>
	private static Regex HdrSignalRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HdrSignalRegex_10.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?:10|12|14|16)(?:le|be)?$</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Match '1'.<br />
	/// ○ Match a character in the set [0246].<br />
	/// ○ Optional (greedy).<br />
	///     ○ Match with 2 alternative expressions.<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Ee].<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Ee].<br />
	/// ○ Match if at the end of the string or if before an ending newline.<br />
	/// </code>
	/// </remarks>
	private static Regex HighBitDepthPixelFormatRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HighBitDepthPixelFormatRegex_11.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?:p010|yuv420p10)(?:le|be)?$</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Match with 2 alternative expressions.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match the string "010".<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Uu].<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match the string "420".<br />
	///         ○ Match a character in the set [Pp].<br />
	///         ○ Match the string "10".<br />
	/// ○ Optional (greedy).<br />
	///     ○ Match with 2 alternative expressions.<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Ee].<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Ee].<br />
	/// ○ Match if at the end of the string or if before an ending newline.<br />
	/// </code>
	/// </remarks>
	private static Regex TenBit420PixelFormatRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__TenBit420PixelFormatRegex_12.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>((?:vpp|scale)_rkrga=[^,\\s"']*?format=)(?:nv12|yuv420p)(?=[: ,"']|$)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ 1st capture group.<br />
	///     ○ Match with 2 alternative expressions.<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Pp] exactly 2 times.<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Cc].<br />
	///             ○ Match a character in the set [Aa].<br />
	///             ○ Match a character in the set [Ll].<br />
	///             ○ Match a character in the set [Ee].<br />
	///     ○ Match '_'.<br />
	///     ○ Match a character in the set [Rr].<br />
	///     ○ Match a character in the set [Kk\u212A].<br />
	///     ○ Match a character in the set [Rr].<br />
	///     ○ Match a character in the set [Gg].<br />
	///     ○ Match a character in the set [Aa].<br />
	///     ○ Match '='.<br />
	///     ○ Match a character in the set [^"',\s] lazily any number of times.<br />
	///     ○ Match a character in the set [Ff].<br />
	///     ○ Match a character in the set [Oo].<br />
	///     ○ Match a character in the set [Rr].<br />
	///     ○ Match a character in the set [Mm].<br />
	///     ○ Match a character in the set [Aa].<br />
	///     ○ Match a character in the set [Tt].<br />
	///     ○ Match '='.<br />
	/// ○ Match with 2 alternative expressions.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match the string "12".<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Uu].<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match the string "420".<br />
	///         ○ Match a character in the set [Pp].<br />
	/// ○ Zero-width positive lookahead.<br />
	///     ○ Match with 2 alternative expressions, atomically.<br />
	///         ○ Match a character in the set [ "',:].<br />
	///         ○ Match if at the end of the string or if before an ending newline.<br />
	/// </code>
	/// </remarks>
	private static Regex RkrgaEightBitFormatRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__RkrgaEightBitFormatRegex_13.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>((?:vpp|scale)_qsv=[^,\\s"']*?format=)(?:nv12|yuv420p)(?=[: ,"']|$)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ 1st capture group.<br />
	///     ○ Match with 2 alternative expressions.<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Pp] exactly 2 times.<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Cc].<br />
	///             ○ Match a character in the set [Aa].<br />
	///             ○ Match a character in the set [Ll].<br />
	///             ○ Match a character in the set [Ee].<br />
	///     ○ Match '_'.<br />
	///     ○ Match a character in the set [Qq].<br />
	///     ○ Match a character in the set [Ss].<br />
	///     ○ Match a character in the set [Vv].<br />
	///     ○ Match '='.<br />
	///     ○ Match a character in the set [^"',\s] lazily any number of times.<br />
	///     ○ Match a character in the set [Ff].<br />
	///     ○ Match a character in the set [Oo].<br />
	///     ○ Match a character in the set [Rr].<br />
	///     ○ Match a character in the set [Mm].<br />
	///     ○ Match a character in the set [Aa].<br />
	///     ○ Match a character in the set [Tt].<br />
	///     ○ Match '='.<br />
	/// ○ Match with 2 alternative expressions.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match the string "12".<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Uu].<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match the string "420".<br />
	///         ○ Match a character in the set [Pp].<br />
	/// ○ Zero-width positive lookahead.<br />
	///     ○ Match with 2 alternative expressions, atomically.<br />
	///         ○ Match a character in the set [ "',:].<br />
	///         ○ Match if at the end of the string or if before an ending newline.<br />
	/// </code>
	/// </remarks>
	private static Regex QsvEightBitFormatRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__QsvEightBitFormatRegex_14.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>((?:scale|tonemap)_vaapi=[^,\\s"']*?format=)(?:nv12|yuv420p)(?=[: ,"']|$)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ 1st capture group.<br />
	///     ○ Match with 2 alternative expressions.<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Cc].<br />
	///             ○ Match a character in the set [Aa].<br />
	///             ○ Match a character in the set [Ll].<br />
	///             ○ Match a character in the set [Ee].<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Oo].<br />
	///             ○ Match a character in the set [Nn].<br />
	///             ○ Match a character in the set [Ee].<br />
	///             ○ Match a character in the set [Mm].<br />
	///             ○ Match a character in the set [Aa].<br />
	///             ○ Match a character in the set [Pp].<br />
	///     ○ Match '_'.<br />
	///     ○ Match a character in the set [Vv].<br />
	///     ○ Match a character in the set [Aa] exactly 2 times.<br />
	///     ○ Match a character in the set [Pp].<br />
	///     ○ Match a character in the set [Ii].<br />
	///     ○ Match '='.<br />
	///     ○ Match a character in the set [^"',\s] lazily any number of times.<br />
	///     ○ Match a character in the set [Ff].<br />
	///     ○ Match a character in the set [Oo].<br />
	///     ○ Match a character in the set [Rr].<br />
	///     ○ Match a character in the set [Mm].<br />
	///     ○ Match a character in the set [Aa].<br />
	///     ○ Match a character in the set [Tt].<br />
	///     ○ Match '='.<br />
	/// ○ Match with 2 alternative expressions.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match the string "12".<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Uu].<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match the string "420".<br />
	///         ○ Match a character in the set [Pp].<br />
	/// ○ Zero-width positive lookahead.<br />
	///     ○ Match with 2 alternative expressions, atomically.<br />
	///         ○ Match a character in the set [ "',:].<br />
	///         ○ Match if at the end of the string or if before an ending newline.<br />
	/// </code>
	/// </remarks>
	private static Regex VaapiEightBitFormatRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__VaapiEightBitFormatRegex_15.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>((?:scale|overlay)_cuda=[^,\\s"']*?format=)(?:nv12|yuv420p)(?=[: ,"']|$)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ 1st capture group.<br />
	///     ○ Match with 2 alternative expressions.<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Cc].<br />
	///             ○ Match a character in the set [Aa].<br />
	///             ○ Match a character in the set [Ll].<br />
	///             ○ Match a character in the set [Ee].<br />
	///         ○ Match a sequence of expressions.<br />
	///             ○ Match an empty string.<br />
	///             ○ Match a character in the set [Vv].<br />
	///             ○ Match a character in the set [Ee].<br />
	///             ○ Match a character in the set [Rr].<br />
	///             ○ Match a character in the set [Ll].<br />
	///             ○ Match a character in the set [Aa].<br />
	///             ○ Match a character in the set [Yy].<br />
	///     ○ Match '_'.<br />
	///     ○ Match a character in the set [Cc].<br />
	///     ○ Match a character in the set [Uu].<br />
	///     ○ Match a character in the set [Dd].<br />
	///     ○ Match a character in the set [Aa].<br />
	///     ○ Match '='.<br />
	///     ○ Match a character in the set [^"',\s] lazily any number of times.<br />
	///     ○ Match a character in the set [Ff].<br />
	///     ○ Match a character in the set [Oo].<br />
	///     ○ Match a character in the set [Rr].<br />
	///     ○ Match a character in the set [Mm].<br />
	///     ○ Match a character in the set [Aa].<br />
	///     ○ Match a character in the set [Tt].<br />
	///     ○ Match '='.<br />
	/// ○ Match with 2 alternative expressions.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match the string "12".<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Uu].<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match the string "420".<br />
	///         ○ Match a character in the set [Pp].<br />
	/// ○ Zero-width positive lookahead.<br />
	///     ○ Match with 2 alternative expressions, atomically.<br />
	///         ○ Match a character in the set [ "',:].<br />
	///         ○ Match if at the end of the string or if before an ending newline.<br />
	/// </code>
	/// </remarks>
	private static Regex CudaEightBitFormatRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__CudaEightBitFormatRegex_16.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?:vpp_rkrga|scale_rkrga|scale_vaapi|tonemap_vaapi|scale_qsv|vpp_qsv|scale_cuda|overlay_cuda)=[^,\\s"']*?format=([a-z0-9_]+)</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Match with 7 alternative expressions.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match a character in the set [Pp] exactly 2 times.<br />
	///         ○ Match '_'.<br />
	///         ○ Match a character in the set [Rr].<br />
	///         ○ Match a character in the set [Kk\u212A].<br />
	///         ○ Match a character in the set [Rr].<br />
	///         ○ Match a character in the set [Gg].<br />
	///         ○ Match a character in the set [Aa].<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Ss].<br />
	///         ○ Match a character in the set [Cc].<br />
	///         ○ Match a character in the set [Aa].<br />
	///         ○ Match a character in the set [Ll].<br />
	///         ○ Match a character in the set [Ee].<br />
	///         ○ Match '_'.<br />
	///         ○ Match with 2 alternative expressions.<br />
	///             ○ Match a sequence of expressions.<br />
	///                 ○ Match an empty string.<br />
	///                 ○ Match a character in the set [Kk\u212A].<br />
	///                 ○ Match a character in the set [Rr].<br />
	///                 ○ Match a character in the set [Gg].<br />
	///                 ○ Match a character in the set [Aa].<br />
	///             ○ Match a sequence of expressions.<br />
	///                 ○ Match an empty string.<br />
	///                 ○ Match a character in the set [Aa] exactly 2 times.<br />
	///                 ○ Match a character in the set [Pp].<br />
	///                 ○ Match a character in the set [Ii].<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Tt].<br />
	///         ○ Match a character in the set [Oo].<br />
	///         ○ Match a character in the set [Nn].<br />
	///         ○ Match a character in the set [Ee].<br />
	///         ○ Match a character in the set [Mm].<br />
	///         ○ Match a character in the set [Aa].<br />
	///         ○ Match a character in the set [Pp].<br />
	///         ○ Match '_'.<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match a character in the set [Aa] exactly 2 times.<br />
	///         ○ Match a character in the set [Pp].<br />
	///         ○ Match a character in the set [Ii].<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Ss].<br />
	///         ○ Match a character in the set [Cc].<br />
	///         ○ Match a character in the set [Aa].<br />
	///         ○ Match a character in the set [Ll].<br />
	///         ○ Match a character in the set [Ee].<br />
	///         ○ Match '_'.<br />
	///         ○ Match a character in the set [Qq].<br />
	///         ○ Match a character in the set [Ss].<br />
	///         ○ Match a character in the set [Vv].<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match a character in the set [Pp] exactly 2 times.<br />
	///         ○ Match '_'.<br />
	///         ○ Match a character in the set [Qq].<br />
	///         ○ Match a character in the set [Ss].<br />
	///         ○ Match a character in the set [Vv].<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Ss].<br />
	///         ○ Match a character in the set [Cc].<br />
	///         ○ Match a character in the set [Aa].<br />
	///         ○ Match a character in the set [Ll].<br />
	///         ○ Match a character in the set [Ee].<br />
	///         ○ Match '_'.<br />
	///         ○ Match a character in the set [Cc].<br />
	///         ○ Match a character in the set [Uu].<br />
	///         ○ Match a character in the set [Dd].<br />
	///         ○ Match a character in the set [Aa].<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match a character in the set [Oo].<br />
	///         ○ Match a character in the set [Vv].<br />
	///         ○ Match a character in the set [Ee].<br />
	///         ○ Match a character in the set [Rr].<br />
	///         ○ Match a character in the set [Ll].<br />
	///         ○ Match a character in the set [Aa].<br />
	///         ○ Match a character in the set [Yy].<br />
	///         ○ Match '_'.<br />
	///         ○ Match a character in the set [Cc].<br />
	///         ○ Match a character in the set [Uu].<br />
	///         ○ Match a character in the set [Dd].<br />
	///         ○ Match a character in the set [Aa].<br />
	/// ○ Match '='.<br />
	/// ○ Match a character in the set [^"',\s] lazily any number of times.<br />
	/// ○ Match a character in the set [Ff].<br />
	/// ○ Match a character in the set [Oo].<br />
	/// ○ Match a character in the set [Rr].<br />
	/// ○ Match a character in the set [Mm].<br />
	/// ○ Match a character in the set [Aa].<br />
	/// ○ Match a character in the set [Tt].<br />
	/// ○ Match '='.<br />
	/// ○ 1st capture group.<br />
	///     ○ Match a character in the set [0-9A-Z_a-z\u212A] atomically at least once.<br />
	/// </code>
	/// </remarks>
	private static Regex HardwareFilterPixelFormatRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__HardwareFilterPixelFormatRegex_17.Instance;
	}

	/// <remarks>
	/// Pattern:<br />
	/// <code>(?:vpp|scale)_rkrga=[^,\\s"']+</code><br />
	/// Options:<br />
	/// <code>RegexOptions.IgnoreCase</code><br />
	/// Explanation:<br />
	/// <code>
	/// ○ Match with 2 alternative expressions.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Pp] exactly 2 times.<br />
	///     ○ Match a sequence of expressions.<br />
	///         ○ Match an empty string.<br />
	///         ○ Match a character in the set [Cc].<br />
	///         ○ Match a character in the set [Aa].<br />
	///         ○ Match a character in the set [Ll].<br />
	///         ○ Match a character in the set [Ee].<br />
	/// ○ Match '_'.<br />
	/// ○ Match a character in the set [Rr].<br />
	/// ○ Match a character in the set [Kk\u212A].<br />
	/// ○ Match a character in the set [Rr].<br />
	/// ○ Match a character in the set [Gg].<br />
	/// ○ Match a character in the set [Aa].<br />
	/// ○ Match '='.<br />
	/// ○ Match a character in the set [^"',\s] atomically at least once.<br />
	/// </code>
	/// </remarks>
	private static Regex RkrgaFilterRegex()
	{
		return _003CRegexGenerator_g_003EFEAA9A0DF6B65E33C4CF7E3C0FBE4CA3656505B8B5E3F6A5AF62AF2A8C73034F2__RkrgaFilterRegex_18.Instance;
	}
}
