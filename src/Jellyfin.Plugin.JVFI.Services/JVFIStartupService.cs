using System;
using System.Threading;
using System.Threading.Tasks;
using Jellyfin.Plugin.JVFI.Configuration;
using Jellyfin.Plugin.JVFI.Patching;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.JVFI.Services;

/// <summary>Owns the runtime patch lifecycle.</summary>
public sealed class JVFIStartupService : IHostedService
{
	private readonly JVFIPatcher _patcher;

	private readonly ILogger<JVFIStartupService> _logger;

	public JVFIStartupService(JVFIPatcher patcher, ILogger<JVFIStartupService> logger)
	{
		_patcher = patcher;
		_logger = logger;
	}

	public Task StartAsync(CancellationToken cancellationToken)
	{
		Plugin instance = Plugin.Instance;
		if (instance != null && PluginConfigurationNormalizer.Normalize(((BasePlugin<PluginConfiguration>)(object)instance).Configuration))
		{
			((BasePlugin<PluginConfiguration>)(object)instance).UpdateConfiguration((BasePluginConfiguration)(object)((BasePlugin<PluginConfiguration>)(object)instance).Configuration);
			LoggerExtensions.LogInformation((ILogger)(object)_logger, "JVFI：已將舊版解析度降級設定更新為 v1.6 原解析度品質降級鏈。", Array.Empty<object>());
		}
		PatchReport patchReport = _patcher.Apply();
		LoggerExtensions.LogInformation((ILogger)(object)_logger, "JVFI：啟動檢查完成，狀態={Status}，訊息={Message}", new object[2]
		{
			patchReport.Compatible ? "可用" : "停用",
			patchReport.Message
		});
		return Task.CompletedTask;
	}

	public Task StopAsync(CancellationToken cancellationToken)
	{
		_patcher.Remove();
		return Task.CompletedTask;
	}
}
