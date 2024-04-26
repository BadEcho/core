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
using System.Windows.Markup;
using BadEcho.Presentation.Extensions;
using BadEcho.Presentation.Properties;

namespace BadEcho.Presentation.Windows;

/// <summary>
/// Provides a basic window for the Bad Echo Presentation framework that additionally offers support for a specific type of data
/// context.
/// </summary>
/// <typeparam name="T">The type of data context used by the window.</typeparam>
/// <remarks>
/// <para>
/// Because this class requires a generic type parameter to be specified, any direct descendant of this type will be unable
/// to support markup-compiled XAML (i.e., the normal practice of having a separate *.xaml file where the user interface is
/// defined). This is because WPF still lacks support for markup-compiled WPF 2009 language features, which offer the
/// <c>x:TypeArguments</c> directive.
/// </para>
/// <para>
/// Bad Echo Presentation framework <see cref="Window{T}"/> types support the use of loose XAML as opposed to markup-compiled XAML
/// when defining their interfaces. Instead of requiring XAML that defines the window document in its entirety, all that is required
/// is XAML for the window's <see cref="ContentControl.Content"/> property and optionally the <see cref="FrameworkElement.Style"/> property.
/// </para>
/// </remarks>
public class Window<T> : Window, IComponentConnector
{
    private readonly string _contentXaml;
    private readonly string? _styleXaml;

    private bool _contentLoaded;

    /// <summary>
    /// Initializes the <see cref="Window{T}"/> class.
    /// </summary>
    static Window() =>
        DataContextProperty.OverrideMetadata(typeof(Window<T>), new FrameworkPropertyMetadata(OnDataContextChanged));

    /// <summary>
    /// Initializes a new instance of the <see cref="Window{T}"/> class.
    /// </summary>
    /// <param name="contentXaml">Loose XAML defining the <see cref="ContentControl.Content"/> of the window.</param>
    public Window(string contentXaml)
        : this(contentXaml, null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="Window{T}"/> class.
    /// </summary>
    /// <param name="contentXaml">Loose XAML defining the <see cref="ContentControl.Content"/> of the window.</param>
    /// <param name="styleXaml">Loose XAML defining the <see cref="FrameworkElement.Style"/> of the window.</param>
    public Window(string contentXaml, string? styleXaml)
    {
        _contentXaml = contentXaml;
        _styleXaml = styleXaml;
    }

    /// <summary>
    /// Gets or sets the typed data context for an element when it participated in data binding.
    /// </summary>
    public new T DataContext
    {
        get => (T) GetValue(DataContextProperty);
        set => SetValue(DataContextProperty, value);
    }

    /// <summary>
    /// Constructs the data context for this window using the provided context assembler.
    /// </summary>
    /// <param name="contextAssembler">An assembler of the data context used by this window.</param>
    public async void AssembleContext(IContextAssembler<T> contextAssembler)
    {
        VerifyAccess();
        Require.NotNull(contextAssembler, nameof(contextAssembler));

        T dataContext = await Task.Run(() => contextAssembler.Assemble(Dispatcher))
                                  .ConfigureAwait(true);
        DataContext = dataContext;
    }
        
    /// <inheritdoc/>
    public void Connect(int connectionId, object target) 
        => _contentLoaded = true;

    /// <inheritdoc/>
    public void InitializeComponent()
    {
        if (_contentLoaded)
            return;

        _contentLoaded = true;

        if (_styleXaml != null)
            Style = _styleXaml.ParseLooseXaml<Style>();

        Content = _contentXaml.ParseLooseXaml();
    }

    private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (e.NewValue == null)
            return;
            
        if (e.NewValue is not T)
            throw new InvalidOperationException(Strings.IncompatibleDataContextType);
    }
}