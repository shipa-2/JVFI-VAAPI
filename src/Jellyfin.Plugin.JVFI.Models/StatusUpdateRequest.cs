namespace Jellyfin.Plugin.JVFI.Models;

/// <summary>
/// Runtime status update sent by the FFmpeg wrapper.
/// </summary>
public sealed class StatusUpdateRequest
{
	/// <summary>Gets or sets the session id.</summary>
	public string SessionId { get; set; } = string.Empty;

	/// <summary>Gets or sets the active profile id.</summary>
	public string ProfileId { get; set; } = string.Empty;

	/// <summary>Gets or sets the current output frame rate estimate.</summary>
	public double OutputFps { get; set; }

	/// <summary>Gets or sets FFmpeg's current processing rate in frames per second.</summary>
	public double ProcessingFps { get; set; }

	/// <summary>Gets or sets FFmpeg's current speed value, where 1.0 means real-time.</summary>
	public double Speed { get; set; }

	/// <summary>Gets or sets the cumulative-average FFmpeg speed used with cumulative frame rate.</summary>
	public double AverageSpeed { get; set; }

	/// <summary>Gets or sets the estimated output video cadence.</summary>
	public double EstimatedOutputFps { get; set; }

	/// <summary>Gets or sets the current output width.</summary>
	public int Width { get; set; }

	/// <summary>Gets or sets the current output height.</summary>
	public int Height { get; set; }

	/// <summary>Gets or sets a value indicating whether the wrapper downgraded from the requested profile.</summary>
	public bool Downgraded { get; set; }

	/// <summary>Gets or sets the actual FFmpeg output encoder.</summary>
	public string OutputEncoder { get; set; } = string.Empty;

	/// <summary>Gets or sets a short status message suitable for the video HUD.</summary>
	public string HudText { get; set; } = string.Empty;

	public double TimelineFps { get; set; }

	public double ComputeFps { get; set; }

	public long GeneratedFrames { get; set; }

	/// <summary>Gets or sets whether GeneratedFrames is estimated for an official FFmpeg backend.</summary>
	public bool GeneratedFramesEstimated { get; set; }

	/// <summary>Gets or sets estimated/observed output frame count for the current JVFI processing session.</summary>
	public long OutputFrames { get; set; }

	/// <summary>Gets or sets cumulative processed media time represented by the frame counters.</summary>
	public double ProcessedMediaSeconds { get; set; }

	public long FallbackFrames { get; set; }

	public double? ClientRenderedFps { get; set; }

	public long? ClientDroppedFrames { get; set; }
}
