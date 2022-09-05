﻿//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Threading;
using BadEcho.Presentation.Configuration;
using BadEcho.Presentation.Extensions;
using BadEcho.Configuration;
using BadEcho.Extensibility.Hosting;
using BadEcho.Interop;
using BadEcho.Vision.Windows;

namespace BadEcho.Vision;

/// <summary>
/// Provides the Vision application.
/// </summary>
public partial class App
{
    private static void ApplyConfiguration(Window window, IConfigurationProvider configurationProvider)
    {
        PresentationConfiguration configuration
            = configurationProvider.GetConfiguration<PresentationConfiguration>();

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
            = PluginHost.LoadFromProcess<IConfigurationProvider>();

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