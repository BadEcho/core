//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Text.Json;

namespace BadEcho.Omnified.Vision
{
    /// <summary>
    /// Provides configuration settings for the Vision application.
    /// </summary>
    internal sealed class VisionConfiguration
    {
        private const string SETTINGS_FILE = "settings.json";

        /// <summary>
        /// Gets or sets the path relative from Vision's base directory to the directory where all message files
        /// are being written to.
        /// </summary>
        /// <remarks>
        /// Message files are most often found in the directory containing the injected hacking code for the targeted
        /// binary. This must be set to that directory if we wish to gain vision into it.
        /// </remarks>
        public string MessageFilesDirectory { get; set; } = string.Empty;

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

            if (File.Exists(SETTINGS_FILE))
            {
                configuration = JsonSerializer.Deserialize<VisionConfiguration>(
                    File.ReadAllText(SETTINGS_FILE),
                    new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            }

            return configuration ?? new VisionConfiguration();
        }
    }
}
