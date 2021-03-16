//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using BadEcho.Fenestra.Extensions;

namespace BadEcho.Fenestra
{
    /// <summary>
    /// Provides a management interface and entry point for a Fenestra-powered application (whether hosted or standalone) which
    /// will ensure that the current application context is in a state such that it becomes appropriate for using Fenestra-based
    /// objects.
    /// </summary>
    public static class UserInterface
    {
        private const string TEXT_BOX_VIEW_NAME = "TextBoxView";

        private static readonly ResourceKey _Footprint = new FootprintKey();

        /// <summary>
        /// Ensures that an environment suitable for a Fenestra-powered application has been built by making sure that a properly
        /// configured <see cref="Application"/> session is loaded into the current process.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the current application context lacks a live <see cref="Application"/> instance, one will be made. This current instance
        /// is then checked for the presence of a footprint left by Fenestra upon building up a suitable environment. If this footprint
        /// is not there, then all Fenestra-based and plugin-based resources are loaded into the current application, with a number
        /// of additional configuration steps following.
        /// </para>
        /// <para>
        /// This method should be invoked by all Fenestra-based controls at the time of their initialization. There is no performance
        /// penalty to calling this method multiple times; initialization of the environment only occurs once.
        /// </para>
        /// </remarks>
        internal static void BuildEnvironment()
        {
            Application application
                = Application.Current ?? new Application
                                         {   // If no application has been initialized, then we're being hosted in a non-WPF process.
                                             // The lifetime of the process is tied to the underlying process itself, not to the closing
                                             // of one or more WPF windows. It makes sense, then, to keep the configured session running
                                             // to avoid having to rebuild the Fenestra environment every time new windows are required.
                                             ShutdownMode = ShutdownMode.OnExplicitShutdown
                                         };

            if (application.Resources.Contains(_Footprint))
                return;
            
            application.Resources.Add(_Footprint, null);
            // Microsoft applies an optimization that prevents default styles found three or more levels deep within merged dictionaries
            // from being applied if there are no default styles declared in the upper two levels of said resource dictionaries. A default
            // style is created and added to the root dictionary as a workaround for this potentially problematic optimization.
            application.Resources.Add(typeof(FrameworkElement), new Style(typeof(FrameworkElement)));
            
            application.ImportResources();
            RegisterClassHandlers();
        }

        /// <summary>
        /// Registers global event handlers for various routed events for the purpose of bringing the behavior of built-in UI elements in
        /// line with Fenestra framework requirements.
        /// </summary>
        /// <remarks>
        /// Application-wide changes to native WPF controls are effectuated here. Currently, several routed events belonging to the
        /// <see cref="TextBox"/> control have handlers registered that together result in text boxes mimicking the text-highlighting behavior
        /// seen from elements such as the Chrome browser's address bar. A selection behavior I find rather suitable for most text box
        /// instances.
        /// </remarks>
        private static void RegisterClassHandlers()
        {
            EventManager.RegisterClassHandler(typeof(TextBox),
                                              UIElement.PreviewMouseLeftButtonDownEvent,
                                              new MouseButtonEventHandler(OnTextBoxLeftButtonDown),
                                              true);

            EventManager.RegisterClassHandler(typeof(TextBox),
                                              UIElement.GotKeyboardFocusEvent,
                                              new RoutedEventHandler(OnTextBoxGotKeyboardFocus),
                                              true);
        }

        /// <summary>
        /// Called in response to left clicking a text box, prior to the event reaching the text box, in order to prevent
        /// the default handler from deselecting the currently selected text as well as text selected through subsequent event
        /// handlers.
        /// </summary>
        private static void OnTextBoxLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            TextBox textBox = (TextBox) sender;

            if (textBox.IsKeyboardFocusWithin)
                return;

            // We want this behavior only to apply to standard TextBox controls.
            if (TEXT_BOX_VIEW_NAME != e.OriginalSource.GetType().Name)
                return;
            // Text will be forcibly deselected if we let the standard event handlers run.
            e.Handled = true;

            // Because the standard event handlers no longer run, we must set focus ourselves, otherwise the text box becomes unclickable.
            textBox.Invoke(() => textBox.Focus(), DispatcherPriority.Input);
        }

        /// <summary>
        /// Called in response to a text box receiving keyboard focus in order to immediately select all text, unless a selection has
        /// already been made.
        /// </summary>
        private static void OnTextBoxGotKeyboardFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox) e.OriginalSource;

            textBox.Invoke(textBox.SelectAll, DispatcherPriority.Input);
        }

        /// <summary>
        /// Provides a resource key used to make a Fenestra processed application-scope resource dictionary.
        /// </summary>
        private sealed class FootprintKey : ResourceKey
        {
            /// <inheritdoc/>
            public override Assembly? Assembly
                => null;
        }
    }
}