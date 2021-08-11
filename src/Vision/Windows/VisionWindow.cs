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

using System;
using System.Windows.Interop;
using BadEcho.Fenestra.Windows;
using BadEcho.Odin;
using BadEcho.Odin.Interop;
using BadEcho.Omnified.Vision.Properties;
using BadEcho.Omnified.Vision.ViewModels;
using ModifierKeys = BadEcho.Odin.Interop.ModifierKeys;

namespace BadEcho.Omnified.Vision.Windows
{
    /// <summary>
    /// Provides the main window for the Vision application.
    /// </summary>
    internal sealed class VisionWindow : Window<VisionViewModel>
    {
        private NativeWindow? _native;
        
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

            _native = new NativeWindow(handle, new PresentationWindowWrapper(handle));

            _native.HotKeyPressed += HandleHotKeyPressed;

            _native.RegisterHotKey(0, ModifierKeys.Shift, VirtualKey.L);
            _native.MakeOverlay();
        }

        /// <inheritdoc/>
        protected override void OnClosed(EventArgs e)
        {
            _native?.UnregisterHotKey(0);

            base.OnClosed(e);
        }

        private void ToggleVisibility()
        {
            Visibility = Visibility == System.Windows.Visibility.Visible
                ? System.Windows.Visibility.Collapsed
                : System.Windows.Visibility.Visible;
        }

        private void HandleHotKeyPressed(object? sender, EventArgs<int> e)
        {
            if (e.Data == 0)
                ToggleVisibility();
        }
    }
}