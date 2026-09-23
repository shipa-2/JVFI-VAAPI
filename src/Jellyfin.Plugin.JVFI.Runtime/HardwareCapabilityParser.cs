using System;
using System.Linq;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Converts official FFmpeg evidence and runtime tests into a capability report.</summary>
public static class HardwareCapabilityParser
{
	public static HardwareCapabilityReport Parse(string ffmpegPath, string versionOutput, string filterOutput, bool framerateSelfTestPassed, bool minterpolateSelfTestPassed, bool qsvFrcSelfTestPassed, HardwarePipeline jellyfinPipeline = HardwarePipeline.Unknown)
	{
		string text = (versionOutput + "\n" + filterOutput).ToLowerInvariant();
		bool flag = ContainsFilter(filterOutput, "framerate");
		bool flag2 = ContainsFilter(filterOutput, "minterpolate");
		bool flag3 = ContainsFilter(filterOutput, "vpp_qsv");
		bool flag4 = text.Contains("rkmpp", StringComparison.Ordinal);
		bool hasOpenCl = text.Contains("opencl", StringComparison.Ordinal);
		bool flag5 = text.Contains("vaapi", StringComparison.Ordinal);
		bool flag6 = text.Contains("qsv", StringComparison.Ordinal);
		bool flag7 = text.Contains("nvenc", StringComparison.Ordinal);
		bool flag8 = text.Contains("amf", StringComparison.Ordinal);
		bool flag9 = text.Contains("videotoolbox", StringComparison.Ordinal);
		bool hasVulkan = text.Contains("vulkan", StringComparison.Ordinal);
		bool hasLibplacebo = text.Contains("libplacebo", StringComparison.Ordinal);
		InterpolationBackend interpolationBackend = ((framerateSelfTestPassed & flag) ? InterpolationBackend.OfficialFramerate : InterpolationBackend.None);
		string interpolationBackend2 = ((interpolationBackend == InterpolationBackend.OfficialFramerate) ? "官方 FFmpeg framerate（Universal Hybrid）" : "不可用");
		string reason = ((interpolationBackend == InterpolationBackend.OfficialFramerate) ? "官方 jellyfin-ffmpeg framerate runtime self-test 已通過；JVFI 使用統一的 Hardware Decode/VPP + CPU framerate + Hardware Encode 混合路徑。" : "官方 jellyfin-ffmpeg 的 framerate self-test 未通過，保留 Jellyfin 原始播放路徑。");
		return new HardwareCapabilityReport
		{
			FfmpegPath = ffmpegPath,
			FfmpegVersion = FirstLine(versionOutput),
			DecodeBackend = SelectSelectedCodecLabel(jellyfinPipeline, flag4, flag5, flag6, flag7, flag8, flag9, decoding: true),
			InterpolationBackend = interpolationBackend2,
			EncodeBackend = SelectSelectedCodecLabel(jellyfinPipeline, flag4, flag5, flag6, flag7, flag8, flag9, decoding: false),
			RecommendedBackend = interpolationBackend,
			Reason = reason,
			CanEnableFrameGeneration = (interpolationBackend != InterpolationBackend.None),
			HasFramerateFilter = flag,
			HasMinterpolateFilter = flag2,
			HasVppQsvFilter = flag3,
			FramerateSelfTestPassed = (flag & framerateSelfTestPassed),
			MinterpolateSelfTestPassed = (flag2 & minterpolateSelfTestPassed),
			QsvFrcSelfTestPassed = (flag3 & flag6 & qsvFrcSelfTestPassed),
			HasOpenCl = hasOpenCl,
			HasVulkan = hasVulkan,
			HasLibplacebo = hasLibplacebo,
			HasRkmpp = flag4,
			HasVaapi = flag5,
			HasQsv = flag6,
			HasNvenc = flag7,
			HasAmf = flag8,
			HasVideoToolbox = flag9
		};
	}

	public static HardwareCapabilityReport Failed(string reason, string ffmpegPath = "")
	{
		return new HardwareCapabilityReport
		{
			ProbeStatus = "探測失敗",
			FfmpegPath = ffmpegPath,
			Reason = "硬體能力探測失敗：" + reason + "。已停用補幀並保留 Jellyfin 原始播放路徑。"
		};
	}

	private static bool ContainsFilter(string filters, string name)
	{
		return filters.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).SelectMany((string line) => line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries)).Any((string token) => string.Equals(token, name, StringComparison.OrdinalIgnoreCase));
	}

	private static string SelectSelectedCodecLabel(HardwarePipeline selected, bool rkmpp, bool vaapi, bool qsv, bool nvenc, bool amf, bool videotoolbox, bool decoding)
	{
		string text = (decoding ? "解碼" : "編碼");
		switch (selected)
		{
		case HardwarePipeline.Rkmpp:
			if (rkmpp)
			{
				return "RKMPP 硬體" + text;
			}
			break;
		case HardwarePipeline.Qsv:
			if (qsv)
			{
				return "QSV 硬體" + text;
			}
			break;
		case HardwarePipeline.Vaapi:
			if (vaapi)
			{
				return "VAAPI 硬體" + text;
			}
			break;
		case HardwarePipeline.Cuda:
			if (nvenc)
			{
				return "NVIDIA 硬體" + text;
			}
			break;
		case HardwarePipeline.Amf:
			if (amf)
			{
				return "AMF 硬體" + text;
			}
			break;
		case HardwarePipeline.VideoToolbox:
			if (videotoolbox)
			{
				return "VideoToolbox 硬體" + text;
			}
			break;
		case HardwarePipeline.Software:
			return "CPU 軟體" + text;
		}
		return SelectCodecLabel(rkmpp, vaapi, qsv, nvenc, amf, videotoolbox, decoding);
	}

	private static string SelectCodecLabel(bool rkmpp, bool vaapi, bool qsv, bool nvenc, bool amf, bool videotoolbox, bool decoding)
	{
		string text = (decoding ? "解碼" : "編碼");
		if (rkmpp)
		{
			return "RKMPP 硬體" + text;
		}
		if (qsv)
		{
			return "QSV 硬體" + text;
		}
		if (vaapi)
		{
			return "VAAPI 硬體" + text;
		}
		if (nvenc)
		{
			return "NVIDIA 硬體" + text;
		}
		if (amf)
		{
			return "AMF 硬體" + text;
		}
		if (videotoolbox)
		{
			return "VideoToolbox 硬體" + text;
		}
		return "CPU 軟體" + text;
	}

	private static string FirstLine(string value)
	{
		return value.Split(new char[2] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault()?.Trim() ?? string.Empty;
	}
}
