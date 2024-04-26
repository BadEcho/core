//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using BadEcho.Configuration;
using BadEcho.Extensibility.Hosting;
using BadEcho.Extensions;
using BadEcho.Interop;
using BadEcho.Logging;
using BadEcho.Presentation.Extensions;
using BadEcho.Presentation.Properties;
using ThreadExceptionEventArgs = BadEcho.Threading.ThreadExceptionEventArgs;

namespace BadEcho.Presentation;

/// <summary>
/// Provides Bad Echo's own encapsulation of a Windows Presentation Foundation application.
/// </summary>
/// <remarks>
/// <para>
/// This class is provided an <see cref="Application"/> instance at startup to be configured such that an environment
/// suitable for a Bad Echo Presentation framework application is ensured.
/// </para>
/// <para>
/// This class loads all Bad Echo Presentation framework and plugin-based resources are into the current application, followed
/// by a number of additional configuration steps.
/// </para>
/// </remarks>
internal sealed class App
{
    private const string TEXT_BOX_VIEW_NAME = "TextBoxView";

    private Transform? _defaultTransform;

    /// <summary>
    /// Occurs when an exception is thrown by the Bad Echo Presentation framework application and not handled.
    /// </summary>
    public event EventHandler<ThreadExceptionEventArgs>? UnhandledException;

    public App(Application application)
    {
        Require.NotNull(application, nameof(application));

        // Microsoft applies an optimization that prevents default styles found three or more levels deep within merged dictionaries
        // from being applied if there are no default styles declared in the upper two levels of said resource dictionaries. A default
        // style is created and added to the root dictionary as a workaround for this potentially problematic optimization.
        application.Resources.Add(typeof(FrameworkElement), new Style(typeof(FrameworkElement)));

        ImportResources(application);

        RegisterClassHandlers();

        application.DispatcherUnhandledException += HandleDispatcherUnhandledException;
        application.Startup += HandleStartup;
    }

    /// <summary>
    /// Imports and loads all Bad Echo Presentation framework resources into the <see cref="Application"/> instance.
    /// </summary>
    /// <param name="application">The current WPF application object to import resources into.</param>
    /// <remarks>
    /// <para>
    /// For both performance and convenience related reasons, merging dictionaries at a user control level should be avoided.
    /// Instead, merge resources just once at the <see cref="Application"/> level. Doing so makes all of these resources available
    /// to all controls present within the assembly for linking to as static resources.
    /// </para>
    /// <para>
    /// Naturally, this becomes difficult when operating within the context of a library (as the Bad Echo Presentation framework
    /// itself is), as no notion of an <see cref="Application"/> definition exists within a library. The difficulty increases
    /// doubly so when considering plugins that export graphical elements to the WPF application.
    ///</para>
    ///<para>
    /// Not only do these plugins lack an <see cref="Application"/> definition, but they typically also lack a reference from the
    /// application hosting them (which is a rather onerous and ridiculous requirement anyway that would only tightly couple the
    /// host to the plugin).
    /// This means the host application has no way of accessing the resources if it wanted to explicitly merge them into its own
    /// application-scope resource dictionary itself.
    /// </para>
    /// <para>
    /// The method here provides us with the means to get all of a library's resources merged into an application-scoped resource
    /// dictionary in a simple manner. All that is required by the library to engage in this process is to export a
    /// <see cref="IResourceProvider"/> implementation that points to a root resource dictionary pointing to all other resources
    /// required by the assembly.
    /// </para>
    /// </remarks>
    private static void ImportResources(Application application)
    {
        var resourceProviders = PluginHost.Load<IResourceProvider>();

        foreach (var resourceProvider in resourceProviders)
        {
            object component = Application.LoadComponent(resourceProvider.ResourceUri);

            if (component is not ResourceDictionary componentDictionary)
                throw new InvalidOperationException(Strings.ResourceUriNotResourceDictionary);

            application.Resources.MergedDictionaries.Add(componentDictionary);
        }
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
    /// Called in response to left-clicking a text box, prior to the event reaching the text box, in order to prevent
    /// the default handler from deselecting the currently selected text as well as text selected through subsequent event
    /// handlers.
    /// </summary>
    private static void OnTextBoxLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        TextBox textBox = (TextBox)sender;

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
        TextBox textBox = (TextBox)e.OriginalSource;

        textBox.Invoke(textBox.SelectAll, DispatcherPriority.Input);
    }

    private static void ApplyDisplayConfiguration(PresentationConfiguration configuration, Window mainWindow)
    {
        Display launchDisplay = Display.Devices.Skip(configuration.LaunchDisplay)
                                       .First();

        mainWindow.MoveToDisplay(launchDisplay);
    }

    private static bool IsScaleApplicable(double scale)
        => scale > 0 && !scale.ApproximatelyEquals(1.0);

    private void ApplyConfiguration(IConfigurationProvider configurationProvider)
    {
        Window? mainWindow = Application.Current?.MainWindow;

        if (mainWindow == null)
            return;

        PresentationConfiguration configuration
            = configurationProvider.GetConfiguration<PresentationConfiguration>();

        ApplyDisplayConfiguration(configuration, mainWindow);
        ApplyScalingConfiguration(configuration, mainWindow);

        RenderOptions.ProcessRenderMode 
            = configuration.ForceSoftwareRendering ? RenderMode.SoftwareOnly : RenderMode.Default;
    }

    private void ApplyScalingConfiguration(PresentationConfiguration configuration, Window mainWindow)
    {
        if (mainWindow.Content is not FrameworkElement content)
        {
            Logger.Debug(Strings.NoWindowContentToScale);
            return;
        }

        if (_defaultTransform != null) 
            content.LayoutTransform = _defaultTransform;

        if (!IsScaleApplicable(configuration.ScaleX) && !IsScaleApplicable(configuration.ScaleY))
            return;

        _defaultTransform = content.LayoutTransform.Clone();
        ScaleTransform scaleTransform = new();
        
        content.LayoutTransform = content.LayoutTransform switch
        {   // An unset LayoutTransform defaults to a MatrixTransform with an identity matrix set. This can be safely replaced with our transform.
            MatrixTransform matrixTransformation when matrixTransformation.Value == Matrix.Identity => scaleTransform,
            // If the LayoutTransform is a group, we simply add our new scale transform to it.
            TransformGroup transformGroup => AddToExistingGroup(transformGroup),
            // Otherwise, We will preserve the current transform in a newly created group along with the scale transform we're constructing.
            _ => new TransformGroup
                 {
                     Children = { content.LayoutTransform, scaleTransform }
                 }
        };

        if (IsScaleApplicable(configuration.ScaleX))
            scaleTransform.ScaleX = configuration.ScaleX;

        if (IsScaleApplicable(configuration.ScaleY))
            scaleTransform.ScaleY = configuration.ScaleY;

        TransformGroup AddToExistingGroup(TransformGroup transformGroup)
        {
            transformGroup.Children.Add(scaleTransform);
            return transformGroup;
        }
    }

    private void HandleConfigurationChanged(object? sender, EventArgs e)
    {
        if (sender is not IConfigurationProvider configurationProvider)
            return;

        Application.Current?.Invoke(ReapplyConfiguration, DispatcherPriority.Background);

        void ReapplyConfiguration()
        {
            ApplyConfiguration(configurationProvider);
        }
    }

    private void HandleStartup(object sender, StartupEventArgs e)
    {
        if (!PluginHost.IsSupportedByProcess<IConfigurationProvider>())
            return;

        IConfigurationProvider configurationProvider
            = PluginHost.LoadFromProcess<IConfigurationProvider>();

        configurationProvider.ConfigurationChanged += HandleConfigurationChanged;

        ApplyConfiguration(configurationProvider);
    }

    private void HandleDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
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
            var dispatcher = (Dispatcher)sender;
            dispatcher.UnhandledException -= HandleDispatcherUnhandledException;

            if (e.Exception is EngineException { InnerException: { } innerException } engineException)
            {
                throw new EngineException(engineException.Message,
                                          innerException,
                                          isProcessed);
            }

            throw new EngineException(Strings.BadEchoDispatcherError, e.Exception, isProcessed);
        }

        e.Handled = true;
    }
}
