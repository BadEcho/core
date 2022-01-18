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

using System.Windows.Threading;
using BadEcho.Fenestra.Windows;
using BadEcho.Odin.Configuration;
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Omnified.Vision.Extensibility;
using BadEcho.Omnified.Vision.ViewModels;

namespace BadEcho.Omnified.Vision.Windows;

/// <summary>
/// Provides an assembler of a Vision main window's data context.
/// </summary>
internal sealed class VisionContextAssembler : IContextAssembler<VisionViewModel>
{
    private readonly VisionViewModel _viewModel = new();
    
    private Dispatcher? _dispatcher;
    private bool _isAssembled;
        
    /// <inheritdoc/>
    public VisionViewModel Assemble(Dispatcher dispatcher)
    {
        if (_isAssembled)
            return _viewModel;

        _dispatcher = dispatcher;

        IConfigurationProvider configurationProvider
            = PluginHost.LoadRequirement<IConfigurationProvider>(true);

        configurationProvider.ConfigurationChanged += HandleConfigurationChanged;

        ApplyConfiguration(configurationProvider);

        _isAssembled = true;

        return _viewModel;
    }

    private void ApplyConfiguration(IConfigurationProvider configurationProvider)
    {
        VisionConfiguration configuration
            = configurationProvider.GetConfiguration<VisionConfiguration>();

        configuration.Dispatcher = _dispatcher;

        _viewModel.Disconnect();
        _viewModel.ApplyConfiguration(configuration);

        var titleModuleHost = ModuleHost.ForTitle(configuration);

        _viewModel.Bind(titleModuleHost);

        var modules
            = PluginHost.ArmedLoad<IVisionModule, IVisionConfiguration>(configuration);

        foreach (var module in modules)
        {
            var moduleHost = new ModuleHost(module, configuration);
                
            _viewModel.Bind(moduleHost);
        }
    }

    private void HandleConfigurationChanged(object? sender, EventArgs e)
    {
        if (sender is not IConfigurationProvider configurationProvider)
            return;

        ApplyConfiguration(configurationProvider);
    }
}