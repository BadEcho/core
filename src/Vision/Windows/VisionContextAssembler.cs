//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
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
            var viewModel = new VisionViewModel();
            var modules = PluginHost.Load<IVisionModule>();

            foreach (var module in modules)
            {
                var moduleHost = new ModuleHost(module);

                viewModel.Bind(moduleHost);
            }

            return viewModel;
        }
    }
}
