//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using BadEcho.Omnified.Vision.Extensibility;

namespace BadEcho.Omnified.Vision
{
    /// <summary>
    /// Provides configuration settings for the Vision application.
    /// </summary>
    internal sealed class VisionConfiguration : IVisionConfiguration
    {
        /// <inheritdoc/>
        public string MessageFilesDirectory 
        { get; set; } = string.Empty;

        /// <inheritdoc/>
        public double LeftAnchorVerticalOffset 
        { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, VisionModuleConfiguration> Modules 
        { get; init; }  = new Dictionary<string, VisionModuleConfiguration>();

        /// <summary>
        /// Loads an instance of the configuration for this application as it exists on disk.
        /// </summary>
        /// <returns>
        /// A <see cref="VisionConfiguration"/> instance populated with values sourced from the local configuration file,
        /// if one exists; otherwise, the instance will simply contain default property values.
        /// </returns>
        public static VisionConfiguration Load()
        {
            VisionConfiguration? configuration = null;

            if (File.Exists(App.SettingsFile))
            {
                configuration = JsonSerializer.Deserialize<VisionConfiguration>(
                    File.ReadAllText(App.SettingsFile),
                    new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            }

            return configuration ?? new VisionConfiguration();
        }
    }
}
