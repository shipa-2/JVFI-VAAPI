using System;

namespace Jellyfin.Plugin.JVFI.Models;

/// <summary>
/// Active or recent frame interpolation session.
/// </summary>
public sealed class FrameInterpolationSession
{
	/// <summary>Gets or sets the session id.</summary>
	public string SessionId { get; set; } = string.Empty;

	/// <summary>Gets or sets the Jellyfin user name or id.</summary>
	public string User { get; set; } = string.Empty;

	/// <summary>Gets or sets the device or client name.</summary>
	public string Device { get; set; } = string.Empty;

	/// <summary>Gets or sets the media item id.</summary>
	public string ItemId { get; set; } = string.Empty;

	/// <summary>Gets or sets the media item name.</summary>
	public string ItemName { get; set; } = string.Empty;

	/// <summary>Gets or sets the requested profile id.</summary>
	public string RequestedProfileId { get; set; } = string.Empty;

	/// <summary>Gets or sets the active profile id.</summary>
	public string ActiveProfileId { get; set; } = string.Empty;

	/// <summary>Gets or sets the detected input frame rate.</summary>
	public double InputFps { get; set; }

	/// <summary>Gets or sets the actual FFmpeg output encoder.</summary>
	public string OutputEncoder { get; set; } = string.Empty;

	/// <summary>Gets or sets the original source video width.</summary>
	public int SourceWidth { get; set; }

	/// <summary>Gets or sets the original source video height.</summary>
	public int SourceHeight { get; set; }

	/// <summary>Gets or sets the current output frame rate estimate.</summary>
	public double OutputFps { get; set; }

	/// <summary>Gets or sets FFmpeg's current processing rate in frames per second.</summary>
	public double ProcessingFps { get; set; }

	/// <summary>Gets or sets FFmpeg's current speed value.</summary>
	public double Speed { get; set; }

	/// <summary>Gets or sets the cumulative-average FFmpeg speed.</summary>
	public double AverageSpeed { get; set; }

	/// <summary>Gets or sets the estimated output video cadence.</summary>
	public double EstimatedOutputFps { get; set; }

	/// <summary>Gets or sets the declared output timeline cadence.</summary>
	public double TimelineFps { get; set; }

	/// <summary>Gets or sets native JVFI compute throughput.</summary>
	public double ComputeFps { get; set; }

	/// <summary>Gets or sets cumulative generated frame count.</summary>
	public long GeneratedFrames { get; set; }

	/// <summary>Gets or sets whether the generated frame counter is estimated rather than reported by a native backend.</summary>
	public bool GeneratedFramesEstimated { get; set; }

	/// <summary>Gets or sets output frame count observed or estimated for this processing session.</summary>
	public long OutputFrames { get; set; }

	/// <summary>Gets or sets cumulative processed media time represented by the frame counters.</summary>
	public double ProcessedMediaSeconds { get; set; }

	/// <summary>Gets or sets cumulative safe fallback frame count.</summary>
	public long FallbackFrames { get; set; }

	/// <summary>Gets or sets client renderer FPS only when a client reports it.</summary>
	public double? ClientRenderedFps { get; set; }

	/// <summary>Gets or sets client dropped frames only when a client reports it.</summary>
	public long? ClientDroppedFrames { get; set; }

	/// <summary>Gets or sets the output width.</summary>
	public int Width { get; set; }

	/// <summary>Gets or sets the output height.</summary>
	public int Height { get; set; }

	/// <summary>Gets or sets a value indicating whether the session downgraded.</summary>
	public bool Downgraded { get; set; }

	/// <summary>Gets or sets the current status text.</summary>
	public string Status { get; set; } = "啟動中";

	/// <summary>Gets or sets the current HUD text.</summary>
	public string HudText { get; set; } = string.Empty;

	/// <summary>Gets or sets the latest error reason.</summary>
	public string ErrorReason { get; set; } = string.Empty;

	/// <summary>Gets or sets when the session was created.</summary>
	public DateTimeOffset StartedAt { get; set; }

	/// <summary>Gets or sets when the session was last updated.</summary>
	public DateTimeOffset UpdatedAt { get; set; }
}
