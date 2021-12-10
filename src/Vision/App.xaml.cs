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

using System.Windows;
using System.Windows.Threading;
using BadEcho.Fenestra.Configuration;
using BadEcho.Fenestra.Extensions;
using BadEcho.Odin.Configuration;
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Odin.Interop;
using BadEcho.Omnified.Vision.Windows;

namespace BadEcho.Omnified.Vision
{
    /// <summary>
    /// Provides the Vision application.
    /// </summary>
    public partial class App
    {
        private static void ApplyConfiguration(Window window, IConfigurationProvider configurationProvider)
        {
            FenestraConfiguration configuration
                = configurationProvider.GetConfiguration<FenestraConfiguration>();

            var launchDisplay = Display.Devices.Skip(configuration.LaunchDisplay)
                                       .First();

            window.MoveToDisplay(launchDisplay);
        }

        /// <summary>
        /// Initializes a <see cref="VisionWindow"/> instance to act as Vision's <see cref="Application.MainWindow"/>, and then fires
        /// off a context assembler purposed for loading the Vision application environment.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="StartupEventArgs"/> instance containing the event data.</param>
        private void HandleStartup(object sender, StartupEventArgs e)
        {
            var window = new VisionWindow();
            var contextAssembler = new VisionContextAssembler();

            window.AssembleContext(contextAssembler);

            IConfigurationProvider configurationProvider
                = PluginHost.LoadRequirement<IConfigurationProvider>(true);

            configurationProvider.ConfigurationChanged += HandleConfigurationChanged;

            ApplyConfiguration(window, configurationProvider);
            window.Show();
        }

        private void HandleConfigurationChanged(object? sender, EventArgs e)
        {
            if (sender is not IConfigurationProvider configurationProvider)
                return;
            
            this.Invoke(ReapplyConfiguration, DispatcherPriority.Background);
            
            void ReapplyConfiguration()
            {
                if (MainWindow != null)
                    ApplyConfiguration(MainWindow, configurationProvider);
            }
        }
    }
}
