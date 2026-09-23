using System;
using Jellyfin.Plugin.JVFI.Patching;
using Jellyfin.Plugin.JVFI.Services;
using MediaBrowser.Controller;
using MediaBrowser.Controller.Plugins;
using Microsoft.Extensions.DependencyInjection;

namespace Jellyfin.Plugin.JVFI;

/// <summary>
/// Registers services used by the plugin.
/// </summary>
public sealed class PluginServiceRegistrator : IPluginServiceRegistrator
{
	/// <inheritdoc />
	public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
	{
		ServiceCollectionServiceExtensions.AddSingleton<FrameInterpolationStateStore>(serviceCollection);
		ServiceCollectionServiceExtensions.AddSingleton<HudFileStore>(serviceCollection);
		ServiceCollectionServiceExtensions.AddSingleton<MetricsFileStore>(serviceCollection);
		ServiceCollectionServiceExtensions.AddSingleton<AdaptiveProfileStore>(serviceCollection);
		ServiceCollectionServiceExtensions.AddSingleton<ConcurrentViewingLimiter>(serviceCollection);
		ServiceCollectionServiceExtensions.AddSingleton<HardwareCapabilityService>(serviceCollection);
		ServiceCollectionServiceExtensions.AddSingleton<JVFIPatcher>(serviceCollection);
		ServiceCollectionHostedServiceExtensions.AddHostedService<HardwareCapabilityService>(serviceCollection, (Func<IServiceProvider, HardwareCapabilityService>)((IServiceProvider provider) => ServiceProviderServiceExtensions.GetRequiredService<HardwareCapabilityService>(provider)));
		ServiceCollectionHostedServiceExtensions.AddHostedService<JVFIStartupService>(serviceCollection);
		ServiceCollectionHostedServiceExtensions.AddHostedService<PlaybackReservationMonitor>(serviceCollection);
		ServiceCollectionHostedServiceExtensions.AddHostedService<TranscodingStatusMonitor>(serviceCollection);
	}
}
