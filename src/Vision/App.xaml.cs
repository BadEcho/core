//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using BadEcho.Fenestra.Configuration;
using BadEcho.Fenestra.Extensions;
using BadEcho.Odin.Interop;
using BadEcho.Omnified.Vision.Windows;

namespace BadEcho.Omnified.Vision
{
    /// <summary>
    /// Provides the Vision application.
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Gets the name of the file containing configuration settings for Vision.
        /// </summary>
        internal static string SettingsFile
            => "settings.json";

        private void HandleStartup(object sender, StartupEventArgs e)
        {
            var window = new VisionWindow();
            var contextAssembler = new VisionContextAssembler();

            window.AssembleContext(contextAssembler);

            ApplyConfiguration(window);

            window.Show();
        }

        private static void ApplyConfiguration(Window window)
        {
            FenestraConfiguration? configuration = null;

            if (File.Exists(SettingsFile))
            {
                configuration = JsonSerializer.Deserialize<FenestraConfiguration>(
                    File.ReadAllText(SettingsFile),
                    new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});
            }

            if (configuration == null)
                return;

            var launchDisplay = Display.Devices.Skip(configuration.LaunchDisplay)
                                       .First();

            window.MoveToDisplay(launchDisplay);
        }
    }
}
