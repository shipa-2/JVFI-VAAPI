using System;
using System.Collections.Generic;
using System.Globalization;
using Jellyfin.Plugin.JVFI.Configuration;
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;

namespace Jellyfin.Plugin.JVFI;

/// <summary>
/// Jellyfin plugin entrypoint for frame interpolation control.
/// </summary>
public sealed class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
{
	/// <summary>
	/// The stable plugin identifier.
	/// </summary>
	public static readonly Guid PluginId = Guid.Parse("b8d11789-88f4-4230-befd-9550952780ec");

	/// <inheritdoc />
	public override string Name => "Jellyfin Video Frame Interpolation";

	/// <inheritdoc />
	public override Guid Id => PluginId;

	/// <summary>
	/// Gets the current plugin instance.
	/// </summary>
	public static Plugin? Instance { get; private set; }

	/// <summary>
	/// Initializes a new instance of the <see cref="T:Jellyfin.Plugin.JVFI.Plugin" /> class.
	/// </summary>
	/// <param name="applicationPaths">Application paths.</param>
	/// <param name="xmlSerializer">XML serializer.</param>
	public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
		: base(applicationPaths, xmlSerializer)
	{
		Instance = this;
	}

	/// <inheritdoc />
	public IEnumerable<PluginPageInfo> GetPages()
	{
		//IL_0000: Unknown result type (might be due to invalid IL or missing references)
		//IL_0005: Unknown result type (might be due to invalid IL or missing references)
		//IL_0011: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Expected Obj, but got Unknown
		return new _003C_003Ez__ReadOnlySingleElementList<PluginPageInfo>(new PluginPageInfo
		{
			Name = ((BasePlugin)this).Name,
			EmbeddedResourcePath = string.Format(CultureInfo.InvariantCulture, "{0}.Configuration.configPage.html", ((object)this).GetType().Namespace)
		});
	}
}
