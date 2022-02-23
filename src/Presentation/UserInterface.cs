//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Threading;
using BadEcho.Presentation.Extensions;
using BadEcho.Presentation.Properties;
using BadEcho.Logging;
using ThreadExceptionEventArgs = BadEcho.Threading.ThreadExceptionEventArgs;

namespace BadEcho.Presentation;

/// <summary>
/// Provides a management interface and entry point for a Bad Echo Presentation framework application (whether hosted or
/// standalone) which will ensure that the current application context is in a state such that it becomes appropriate for
/// using Bad Echo Presentation framework objects.
/// </summary>
public static class UserInterface
{
    private const int HRESULT_DISPATCHER_SHUTDOWN = unchecked((int) 0x80131509);
    private const string TEXT_BOX_VIEW_NAME = "TextBoxView";

    private static readonly ResourceKey _Footprint = new FootprintKey();

    /// <summary>
    /// Occurs when an exception is thrown by the Bad Echo Presentation framework application and not handled.
    /// </summary>
    public static event EventHandler<ThreadExceptionEventArgs>? UnhandledException;

    /// <summary>
    /// Runs the provided UI-related function in a context appropriate for hosting UI components.
    /// </summary>
    /// <param name="uiFunction">The function to run in a UI appropriate context.</param>
    /// <param name="isBlocking">
    /// Value indicating whether the calling thread should be blocked until the action is complete.
    /// </param>
    /// <remarks>
    /// <para>
    /// This function meets the concerns of many UI components by executing the function in a separate STA thread, optionally
    /// blocking the calling thread until the UI thread terminates.
    /// </para>
    /// <para>
    /// If <c>uiFunction</c> throws an exception, the incident will be relayed to the caller through the
    /// <see cref="UnhandledException"/> event. The STA thread will then immediately terminate.
    /// </para>
    /// </remarks>
    public static void RunUIFunction(Action uiFunction, bool isBlocking)
    {
        var staThread = new Thread(() => UIFunctionRunner(uiFunction));

        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start();

        if (isBlocking)
            staThread.Join();

        static void UIFunctionRunner(Action uiFunction)
        {
            try
            {
                uiFunction();

                Dispatcher.Run();
            }
            catch (InvalidOperationException invalidEx)
            {
                if (invalidEx.HResult != HRESULT_DISPATCHER_SHUTDOWN)
                    throw;
                    
                Logger.Debug(Strings.BadEchoDispatcherManuallyShutdown);
            }
            catch (EngineException engineEx)
            {
                if (!engineEx.IsProcessed)
                    Logger.Critical(Strings.BadEchoDispatcherError, engineEx.InnerException ?? engineEx);
            }
        }
    }

    /// <summary>
    /// Runs the provided UI-related function in a context appropriate for hosting UI components.
    /// </summary>
    /// <param name="uiFunction">The function to run in a UI appropriate context.</param>
    /// <remarks>
    /// <para>
    /// This function meets the concerns of many UI components by executing the function in a separate STA thread without
    /// blocking the calling thread.
    /// </para>
    /// <para>
    /// If <c>uiFunction</c> throws an exception, the incident will be relayed to the caller through the
    /// <see cref="UnhandledException"/> event. The STA thread will then immediately terminate.
    /// </para>
    /// </remarks>
    public static void RunUIFunction(Action uiFunction)
        => RunUIFunction(uiFunction, false);

    /// <summary>
    /// Ensures that an environment suitable for a Bad Echo Presentation framework application has been built by making sure that
    /// a properly configured <see cref="Application"/> session is loaded into the current process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the current application context lacks a live <see cref="Application"/> instance, one will be made. This current instance
    /// is then checked for the presence of a footprint left by Bad Echo Presentation framework upon building up a suitable environment.
    /// If this footprint is not there, then all Bad Echo Presentation framework and plugin-based resources are loaded into the current
    /// application, with a number of additional configuration steps following.
    /// </para>
    /// <para>
    /// This method should be invoked by all Bad Echo Presentation framework controls at the time of their initialization. There is no
    /// performance penalty to calling this method multiple times; initialization of the environment only occurs once.
    /// </para>
    /// </remarks>
    internal static void BuildEnvironment()
    {
        Application application
            = Application.Current ?? new Application
                                     {   // If no application has been initialized, then we're being hosted in a non-WPF process.
                                         // The lifetime of the process is tied to the underlying process itself, not to the closing
                                         // of one or more WPF windows. It makes sense, then, to keep the configured session running
                                         // to avoid having to rebuild the Bad Echo Presentation framework environment every time new
                                         // windows are required.
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

        application.DispatcherUnhandledException += HandleDispatcherUnhandledException;
    }

    /// <summary>
    /// Registers global event handlers for various routed events for the purpose of bringing the behavior of built-in UI elements in
    /// line with Bad Echo Presentation framework requirements.
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

    private static void HandleDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
    {
        var threadEventArgs = new ThreadExceptionEventArgs(e.Exception);
        bool isProcessed = false;

        EventHandler<ThreadExceptionEventArgs>? unhandledException = UnhandledException;

        if (unhandledException != null)
        {
            isProcessed = true;
            unhandledException(sender, threadEventArgs);
        }

        if (!threadEventArgs.Handled)
        {
            var dispatcher = (Dispatcher) sender;
            dispatcher.UnhandledException -= HandleDispatcherUnhandledException;

            if (e.Exception is EngineException {InnerException: { } innerException} engineException)
            {
                throw new EngineException(engineException.Message,
                                          innerException,
                                          isProcessed);
            }

            throw new EngineException(Strings.BadEchoDispatcherError, e.Exception, isProcessed);
        }

        e.Handled = true;
    }

    /// <summary>
    /// Provides a resource key used to make a Bad Echo Presentation framework processed application-scope resource dictionary.
    /// </summary>
    private sealed class FootprintKey : ResourceKey
    {
        /// <inheritdoc/>
        public override Assembly? Assembly
            => null;
    }
}