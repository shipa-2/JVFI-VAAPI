using System;

namespace Jellyfin.Plugin.JVFI.Runtime;

/// <summary>Truthful snapshot of the selected official FFmpeg and accelerator capabilities.</summary>
public sealed class HardwareCapabilityReport
{
	public DateTimeOffset UpdatedAt { get; init; } = DateTimeOffset.UtcNow;

	public string ProbeStatus { get; init; } = "探測完成";

	public string FfmpegPath { get; init; } = string.Empty;

	public string FfmpegVersion { get; init; } = string.Empty;

	public string DecodeBackend { get; init; } = "CPU 軟體解碼";

	public string InterpolationBackend { get; init; } = "不可用";

	public string EncodeBackend { get; init; } = "CPU 軟體編碼";

	public InterpolationBackend RecommendedBackend { get; init; }

	public string Reason { get; init; } = string.Empty;

	public bool CanEnableFrameGeneration { get; init; }

	public bool HasFramerateFilter { get; init; }

	public bool HasMinterpolateFilter { get; init; }

	public bool HasVppQsvFilter { get; init; }

	public bool FramerateSelfTestPassed { get; init; }

	public bool MinterpolateSelfTestPassed { get; init; }

	public bool QsvFrcSelfTestPassed { get; init; }

	public bool HasOpenCl { get; init; }

	public bool HasVulkan { get; init; }

	public bool HasLibplacebo { get; init; }

	public bool HasRkmpp { get; init; }

	public bool HasVaapi { get; init; }

	public bool HasQsv { get; init; }

	public bool HasNvenc { get; init; }

	public bool HasAmf { get; init; }

	public bool HasVideoToolbox { get; init; }
}
