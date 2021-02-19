//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace BadEcho.Odin.Extensibility.Configuration
{
    /// <summary>
    /// Defines configuration settings for Odin's Extensibility framework.
    /// </summary>
    public interface IExtensibilityConfiguration
    {
        /// <summary>
        /// Gets the path relative from the current application context's base directory to the directory
        /// where all plugins are stored.
        /// </summary>
        /// <remarks>
        /// If this setting is not set then it will be assumed that all plugins are stored in the base directory
        /// of the current application context.
        /// </remarks>
        string PluginDirectory { get; }

        /// <summary>
        /// Gets the collection of contracts being segmented by call-routable plugins.
        /// </summary>
        IEnumerable<IContractConfiguration> SegmentedContracts { get; }
    }
}
