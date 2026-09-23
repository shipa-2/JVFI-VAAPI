namespace Jellyfin.Plugin.JVFI.Models;

/// <summary>
/// Request sent when a wrapper starts handling a Jellyfin transcode session.
/// </summary>
public sealed class SessionStartRequest
{
	/// <summary>Gets or sets the Jellyfin play session id or wrapper-generated id.</summary>
	public string SessionId { get; set; } = string.Empty;

	/// <summary>Gets or sets the Jellyfin user name or id.</summary>
	public string User { get; set; } = string.Empty;

	/// <summary>Gets or sets the Jellyfin client or device name.</summary>
	public string Device { get; set; } = string.Empty;

	/// <summary>Gets or sets the media item id.</summary>
	public string ItemId { get; set; } = string.Empty;

	/// <summary>Gets or sets the media item name.</summary>
	public string ItemName { get; set; } = string.Empty;

	/// <summary>Gets or sets the requested interpolation profile id.</summary>
	public string ProfileId { get; set; } = string.Empty;

	/// <summary>Gets or sets the detected source frame rate.</summary>
	public double InputFps { get; set; }

	/// <summary>Gets or sets the actual FFmpeg output encoder.</summary>
	public string OutputEncoder { get; set; } = string.Empty;

	/// <summary>Gets or sets the source video width.</summary>
	public int SourceWidth { get; set; }

	/// <summary>Gets or sets the source video height.</summary>
	public int SourceHeight { get; set; }
}
