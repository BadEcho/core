//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows.Interop;
using BadEcho.Fenestra.Windows;
using BadEcho.Odin.Interop;
using BadEcho.Omnified.Vision.Properties;
using BadEcho.Omnified.Vision.ViewModels;

namespace BadEcho.Omnified.Vision.Windows
{
    /// <summary>
    /// Provides the main window for the Vision application.
    /// </summary>
    internal sealed class VisionWindow : Window<VisionViewModel>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VisionWindow"/> class.
        /// </summary>
        public VisionWindow()
            : base(Xaml.VisionWindowContent, Xaml.VisionWindowStyle)
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        /// <remarks>
        /// This ensures that Vision stays on the top of all other windows -- that's very much a requirement, being an overlay
        /// and all.
        /// </remarks>
        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);

            Topmost = true;
            Activate();
        }

        /// <inheritdoc/>
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            IntPtr handle = new WindowInteropHelper(this).Handle;
            var nativeWindow = new NativeWindow(handle);

            nativeWindow.MakeOverlay();
        }
    }
}
