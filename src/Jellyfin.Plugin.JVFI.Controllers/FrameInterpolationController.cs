using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.JVFI.Configuration;
using Jellyfin.Plugin.JVFI.Models;
using Jellyfin.Plugin.JVFI.Runtime;
using Jellyfin.Plugin.JVFI.Services;
using MediaBrowser.Common.Plugins;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jellyfin.Plugin.JVFI.Controllers;

/// <summary>
/// API endpoints used by the frame interpolation wrapper and dashboard.
/// </summary>
[ApiController]
[Authorize]
[Route("FrameInterpolation")]
public sealed class FrameInterpolationController : ControllerBase
{
	private readonly FrameInterpolationStateStore _stateStore;

	private readonly HardwareCapabilityService _capabilities;

	public FrameInterpolationController(FrameInterpolationStateStore stateStore, HardwareCapabilityService capabilities)
	{
		_stateStore = stateStore;
		_capabilities = capabilities;
	}

	[HttpGet("Capabilities")]
	public ActionResult<HardwareCapabilityReport> GetCapabilities()
	{
		return Ok(_capabilities.Current);
	}

	[HttpPost("Capabilities/Refresh")]
	public async Task<ActionResult<HardwareCapabilityReport>> RefreshCapabilities(CancellationToken cancellationToken)
	{
		return Ok(await _capabilities.RefreshAsync(cancellationToken).ConfigureAwait(false));
	}

	[HttpGet("Config")]
	public ActionResult<FrameInterpolationConfigSnapshot> GetConfig()
	{
		var pluginConfiguration = Plugin.Instance?.Configuration;
		if (pluginConfiguration == null)
		{
			return NotFound();
		}

		return new FrameInterpolationConfigSnapshot
		{
			Enabled = pluginConfiguration.Enabled,
			DefaultProfileId = pluginConfiguration.DefaultProfileId,
			OutputEncoderMode = pluginConfiguration.OutputEncoderMode,
			OutputScaleHeight = pluginConfiguration.OutputScaleHeight,
			HudMode = pluginConfiguration.HudMode,
			HudPosition = pluginConfiguration.HudPosition,
			HudSeconds = pluginConfiguration.HudSeconds,
			DowngradeSpeedThreshold = pluginConfiguration.DowngradeSpeedThreshold,
			RecoverySpeedThreshold = pluginConfiguration.RecoverySpeedThreshold,
			AutoProfiles = pluginConfiguration.AutoProfiles,
			Profiles = pluginConfiguration.Profiles.Where(profile => profile.Enabled).ToArray(),
		};
	}

	[HttpGet("Sessions")]
	public ActionResult<IReadOnlyList<FrameInterpolationSession>> GetSessions()
	{
		return Ok(_stateStore.GetSessions());
	}

	[HttpPost("SessionStart")]
	public ActionResult<FrameInterpolationSession> StartSession([FromBody] SessionStartRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.SessionId))
		{
			return BadRequest("缺少工作階段 ID。");
		}

		if (string.IsNullOrWhiteSpace(request.ProfileId))
		{
			request.ProfileId = Plugin.Instance?.Configuration.DefaultProfileId ?? "auto60";
		}

		return Ok(_stateStore.Start(request));
	}

	[HttpPost("Status")]
	public ActionResult<FrameInterpolationSession> UpdateStatus([FromBody] StatusUpdateRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.SessionId))
		{
			return BadRequest("缺少工作階段 ID。");
		}

		var session = _stateStore.Update(request);
		if (session != null)
		{
			return Ok(session);
		}

		return NotFound();
	}

	[HttpPost("Error")]
	public ActionResult<FrameInterpolationSession> ReportError([FromBody] ErrorReportRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.SessionId))
		{
			return BadRequest("缺少工作階段 ID。");
		}

		return Ok(_stateStore.Fail(request));
	}
}
