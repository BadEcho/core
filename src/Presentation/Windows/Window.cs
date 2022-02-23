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

using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace BadEcho.Fenestra.Windows;

/// <summary>
/// Provides the basic window for the Fenestra framework, offering several capabilities in addition to what's offered
/// by WPF's own windows, as well as an entry point into the Fenestra framework.
/// </summary>
public class Window : System.Windows.Window, ICloseableContext, IHandlerBypassable
{
    /// <summary>
    /// Identifies the <see cref="IsOpen"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsOpenProperty
        = DependencyProperty.Register(nameof(IsOpen),
                                      typeof(bool),
                                      typeof(Window),
                                      new PropertyMetadata(false, OnIsOpenChanged));
    /// <summary>
    /// Identifies the <see cref="CloseCommand"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CloseCommandProperty
        = DependencyProperty.Register(nameof(CloseCommand),
                                      typeof(ICommand),
                                      typeof(Window),
                                      new PropertyMetadata(null));

    private bool _closingIsUncontrolled;

    /// <summary>
    /// Initializes a new instance of the <see cref="Window"/> class.
    /// </summary>
    public Window()
    {
        UserInterface.BuildEnvironment();

        Closing += HandleClosing;
        DataContextChanged += HandleDataContextChanged;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// The command bound to this is guaranteed to be executed in the event of an <c>uncontrolled</c> closing of this window;
    /// that is, a closing initiated by the user or some other process external to our own code elements. This makes this property
    /// a suitable place for any logic whose execution we require upon the window closing.
    /// </para>
    /// <para>
    /// This will be automatically bound to by any data context implementing the <see cref="ICloseableContext"/> interface,
    /// removing any need for said data context to have to retrieve and manipulate this particular window instance. While it is
    /// not required that the bound command actually facilitate the closing of the window, a data context can achieve just
    /// that through the manipulation of the <see cref="IsOpen"/> property by this command.
    /// </para>
    /// </remarks>
    public ICommand? CloseCommand
    { get; set; }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// Changing this property to false will result in a <c>controlled</c> closing of the window, meaning that the window will
    /// be closed without any need to execute the <see cref="CloseCommand"/>.
    /// </para>
    /// <para>
    /// This will be automatically bound to by any data context implementing the <see cref="ICloseableContext"/> interface,
    /// giving us the ability to close or open the window from a data context itself without having to retrieve and manipulate
    /// this particular window instance.
    /// </para>
    /// </remarks> 
    public bool IsOpen
    { get; set; }

    /// <summary>
    /// Manually displays the window.
    /// </summary>
    protected virtual void ShowWindow() 
        => Show();

    private static void OnIsOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var window = (Window) sender;

        if (window.IsHandlingBypassed())
            return;

        if ((bool) e.NewValue)
            window.ShowWindow();
        else
            window.BypassHandlers(window.CloseWindow);
    }

    private void CloseWindow()
    {
        if (!_closingIsUncontrolled)
            Close();

        ClearValue(CloseCommandProperty);
        ClearValue(IsOpenProperty);

        _closingIsUncontrolled = false;
    }

    private void HandleClosing(object? sender, CancelEventArgs e)
    {
        if (this.IsHandlingBypassed() || CloseCommand == null || !CloseCommand.CanExecute(this))
            return;

        _closingIsUncontrolled = true;

        CloseCommand.Execute(this);
    }

    private void HandleDataContextChanged(object? sender, DependencyPropertyChangedEventArgs e)
    {
        var closeCommandBinding
            = new Binding {Path = new PropertyPath(nameof(CloseCommand))};

        var isOpenBinding
            = new Binding {Path = new PropertyPath(nameof(IsOpen))};

        SetBinding(CloseCommandProperty, closeCommandBinding);
        SetBinding(IsOpenProperty, isOpenBinding);
    }
}