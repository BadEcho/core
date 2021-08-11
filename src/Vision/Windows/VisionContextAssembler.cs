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
using BadEcho.Odin.Extensibility.Hosting;
using BadEcho.Omnified.Vision.Extensibility;
using BadEcho.Omnified.Vision.ViewModels;

namespace BadEcho.Omnified.Vision.Windows
{
    /// <summary>
    /// Provides an assembler of a Vision main window's data context.
    /// </summary>
    internal sealed class VisionContextAssembler : IContextAssembler<VisionViewModel>
    {
        /// <inheritdoc/>
        public VisionViewModel Assemble(Dispatcher dispatcher)
        {
            var configuration = VisionConfiguration.Load();
            var viewModel = new VisionViewModel(configuration);

            var modules
                = PluginHost.ArmedLoad<IVisionModule, IVisionConfiguration>(configuration);

            foreach (var module in modules)
            {
                var moduleHost = new ModuleHost(module, configuration);

                viewModel.Bind(moduleHost);
            }

            return viewModel;
        }
    }
}
