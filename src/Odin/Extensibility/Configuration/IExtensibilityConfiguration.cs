//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;

namespace BadEcho.Odin.Extensibility.Configuration
{
    /// <summary>
    /// Defines configuration settings for Odin's Extensibility framework.
    /// </summary>
    public interface IExtensibilityConfiguration
    {
        /// <summary>
        /// Gets the default plugin directory path used if <see cref="PluginDirectory"/> is not set.
        /// </summary>
        static string DefaultPluginDirectory => "plugins";

        /// <summary>
        /// Gets the path relative from the current application context's base directory to the directory
        /// where all plugins are stored.
        /// </summary>
        /// <remarks>
        /// If this setting is not set then the path found in <see cref="DefaultPluginDirectory"/> is used instead.
        /// Setting this to an empty string will result in plugins being loaded from the base directory of the current
        /// application context.
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
        /// The full path to where plugins are stored based on this configuration. This will be based on combining either the
        /// <see cref="PluginDirectory"/> setting (if that was set) or the <see cref="DefaultPluginDirectory"/> setting (if it
        /// was not) as a relative path to the base directory of the current application context.
        /// </returns>
        string GetFullPathToPlugins()
            => Path.GetFullPath(PluginDirectory ?? DefaultPluginDirectory, AppContext.BaseDirectory);
    }
}
