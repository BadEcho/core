//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using BadEcho.Omnified.Vision.Windows;

namespace BadEcho.Omnified.Vision
{
    /// <summary>
    /// Provides the Vision application.
    /// </summary>
    public partial class App
    {
        private void HandleStartup(object sender, StartupEventArgs e)
        {
            var window = new VisionWindow();
            var contextAssembler = new VisionContextAssembler();

            window.AssembleContext(contextAssembler);

            window.Show();
        }
    }
}
