//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensions;
using BadEcho.Properties;

namespace BadEcho.Extensibility.Configuration;

/// <summary>
/// Defines configuration settings for the Bad Echo Extensibility framework.
/// </summary>
public interface IExtensibilityConfiguration
{
    /// <summary>
    /// Gets the default plugin directory path used if <see cref="PluginDirectory"/> is not set.
    /// </summary>
    /// <remarks>
    /// If the default plugin directory is used due to <see cref="PluginDirectory"/> not being specified and the resulting
    /// full path does not refer to an existing directory, then plugins will be loaded from the base directory of the
    /// current application context instead. 
    /// </remarks>
    static string DefaultPluginDirectory => "plugins";

    /// <summary>
    /// Gets the path relative from the current application context's base directory to the directory
    /// where all plugins are stored.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If this setting is not set then the path found in <see cref="DefaultPluginDirectory"/> is used instead.
    /// Setting this to an empty string will result in plugins being loaded from the base directory of the current
    /// application context.
    /// </para>
    /// <para>
    /// Providing a value for this setting sets an expectation that the specified directory exists. If this setting is specified
    /// and the resulting full path does not refer to an existing directory, then an error will occur.
    /// </para>
    /// </remarks>
    string? PluginDirectory { get; }

    /// <summary>
    /// Gets the collection of contracts being segmented by call-routable plugins.
    /// </summary>
    IEnumerable<IContractConfiguration> SegmentedContracts { get; }

    /// <summary>
    /// Gets the full path to where plugins are stored based on this configuration.
    /// </summary>
    /// <returns>
    /// <para>
    /// The full path to where plugins are stored based on this configuration. This will be based on combining either the
    /// <see cref="PluginDirectory"/> setting (if that was set) or the <see cref="DefaultPluginDirectory"/> setting (if it
    /// was not) as a relative path to the base directory of the current application context.
    /// </para>
    /// <para>
    /// If <see cref="PluginDirectory"/> was not specified and the full path resulting from <see cref="DefaultPluginDirectory"/>
    /// does not refer to an existing directory, then plugins will be loaded from the base directory of the current application
    /// context instead.
    /// </para>
    /// </returns>
    /// <exception cref="DirectoryNotFoundException">
    /// <see cref="PluginDirectory"/> was specified, but the resulting full path does not refer to an existing directory.
    /// </exception>
    string GetFullPathToPlugins()
    {
        if (PluginDirectory == null)
        {
            string defaultPath = Path.GetFullPath(DefaultPluginDirectory, AppContext.BaseDirectory);

            return !Directory.Exists(defaultPath) ? AppContext.BaseDirectory : defaultPath;
        }

        string path = Path.GetFullPath(PluginDirectory, AppContext.BaseDirectory);

        if (!Directory.Exists(path))
        {
            throw new DirectoryNotFoundException(Strings.ExtensibilityConfigurationDirectoryNotFound
                                                        .InvariantFormat(path));
        }

        return path;
    }
}