namespace Jellyfin.Plugin.JVFI.Models;

/// <summary>
/// Error report sent by the FFmpeg wrapper.
/// </summary>
public sealed class ErrorReportRequest
{
	/// <summary>Gets or sets the session id.</summary>
	public string SessionId { get; set; } = string.Empty;

	/// <summary>Gets or sets the active profile id when the error occurred.</summary>
	public string ProfileId { get; set; } = string.Empty;

	/// <summary>Gets or sets the FFmpeg exit code if known.</summary>
	public int? ExitCode { get; set; }

	/// <summary>Gets or sets a short machine-readable reason.</summary>
	public string Reason { get; set; } = string.Empty;

	/// <summary>Gets or sets a user-readable detail message.</summary>
	public string Detail { get; set; } = string.Empty;
}
