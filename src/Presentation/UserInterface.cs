//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
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

    private static readonly object _ApplicationLock 
        = new();

    private static App? _Application;

    /// <summary>
    /// Occurs when an unhandled exception is thrown by the Bad Echo Presentation framework application.
    /// </summary>
    public static event EventHandler<ThreadExceptionEventArgs>? UnhandledException
    {
        add
        {
            if (_Application != null)
                _Application.UnhandledException += value;
        }
        remove
        {
            if (_Application != null)
                _Application.UnhandledException -= value;
        }
    }
    
    /// <summary>
    /// Runs the provided UI-related function in a context appropriate for hosting UI components.
    /// </summary>
    /// <param name="uiFunction">The function to run in a UI appropriate context.</param>
    /// <param name="isBlocking">
    /// Value indicating whether the calling thread should be blocked until the action is complete.
    /// </param>
    /// <param name="runDispatcher">
    /// Value indicating if the <see cref="Dispatcher"/> needs to be explicitly run, typically because
    /// <paramref name="uiFunction"/> does not do so itself.
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
    public static void RunUIFunction(Action uiFunction, bool isBlocking, bool runDispatcher)
    {
        var staThread = new Thread(() => UIFunctionRunner(uiFunction, runDispatcher));

        staThread.SetApartmentState(ApartmentState.STA);
        staThread.Start();

        if (isBlocking)
            staThread.Join();

        static void UIFunctionRunner(Action uiFunction, bool runDispatcher)
        {
            try
            {
                uiFunction();

                if (runDispatcher)
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
        => RunUIFunction(uiFunction, false, true);

    /// <summary>
    /// Ensures that an environment suitable for a Bad Echo Presentation framework application has been built by making sure that
    /// a properly configured <see cref="Application"/> session is loaded into the current process.
    /// </summary>
    /// <remarks>
    /// <para>
    /// If the current application context lacks a live <see cref="Application"/> instance, one will be made. This current instance
    /// is then further encapsulated and managed by the <see cref="App"/>checked for the presence of a footprint left by Bad Echo Presentation framework upon building up a suitable environment.
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
        lock (_ApplicationLock)
        {
            if (_Application != null)
                return;

            Application application
                = Application.Current ?? new Application
                                         {
                                             // If no application has been initialized, then we're being hosted in a non-WPF process.
                                             // The lifetime of the process is tied to the underlying process itself, not to the closing
                                             // of one or more WPF windows. It makes sense, then, to keep the configured session running
                                             // to avoid having to rebuild the Bad Echo Presentation framework environment every time new
                                             // windows are required.
                                             ShutdownMode = ShutdownMode.OnExplicitShutdown
                                         };

            _Application = new App(application);
        }
    }
}