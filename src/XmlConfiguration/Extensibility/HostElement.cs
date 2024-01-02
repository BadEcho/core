//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Configuration;
using BadEcho.Extensibility.Configuration;

namespace BadEcho.XmlConfiguration.Extensibility;

/// <summary>
/// Provides a configuration element for settings options on all instances of the Bad Echo Extensibility framework's
/// plugin host.
/// </summary>
internal sealed class HostElement : ConfigurationElement
{
    private const string PLUGIN_DIRECTORY_ATTRIBUTE_SCHEMA = "pluginDirectory";

    private static readonly Lazy<ConfigurationPropertyCollection> _Properties
        = new(InitializeProperties, LazyThreadSafetyMode.PublicationOnly);

    /// <summary>
    /// Gets or sets the path relative from the current application context's base directory to the
    /// directory where all plugins are stored.
    /// </summary>
    /// <inheritdoc cref="IExtensibilityConfiguration.PluginDirectory"/>
    public string PluginDirectory
    {
        get => (string) base[PLUGIN_DIRECTORY_ATTRIBUTE_SCHEMA];
        set => base[PLUGIN_DIRECTORY_ATTRIBUTE_SCHEMA] = value;
    }

    /// <inheritdoc/>
    protected override ConfigurationPropertyCollection Properties
        => _Properties.Value;

    private static ConfigurationPropertyCollection InitializeProperties()
        => new()
           {
               new ConfigurationProperty(PLUGIN_DIRECTORY_ATTRIBUTE_SCHEMA,
                                         typeof(string),
                                         null,
                                         null,
                                         null,
                                         ConfigurationPropertyOptions.None)
           };
}