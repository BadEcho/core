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

using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a set of configured properties that can be applied to a control.
/// </summary>
/// <typeparam name="T">The type of control targeted by this style.</typeparam>
/// <remarks>
/// A style contains one or more property setters, each configured with the control property being set and the
/// value to set said property to. It is expressed as a sequence of control member accesses and the values to
/// assign them.
/// </remarks>
/// <example>The following code defines a style for buttons on an options screen:
/// <code><![CDATA[Style<Button> optionButtonStyle =
/// [
///     (b => b.HorizontalAlignment, HorizontalAlignment.Center),
///     (b => b.VerticalAlignment, VerticalAlignment.Center),
///     (b => b.Background, new Brush(Color.Black)),
///     (b => b.Border, new Brush(Color.White)),
///     (b => b.HoveredBackground, new Brush(Color.Gray)),
///     (b => b.PressedBackground, new Brush(Color.DarkGray)),
///     (b => b.BorderThickness, new Thickness(1)),
///     (b => b.Padding, new Thickness(10)),
///     (b => b.Margin, new Thickness(10))
/// ];]]>
/// </code>
/// <para>
/// A style like the one above allows us to unify the look of related buttons without having to copy and paste the same
/// boilerplate styling code for each button. The following shows how we can apply the style in a declarative fashion
/// when initializing a control:
/// </para>
/// <code>
/// var resetDefaultsButton 
///     = new Button { Style = optionButtonStyle };
/// </code>
/// <para>
/// Being able to apply styles declaratively like above simplifies their use and keeps our code nice and tidy.
/// </para>
/// </example>
[CollectionBuilder(typeof(StyleBuilder), nameof(StyleBuilder.Create))]
public sealed class Style<T>
    where T : IControl
{
    private readonly List<(Expression<Func<T, object?>>, object)> _setterExpressions;
    private readonly List<Action<T>> _setters;

    internal Style(List<(Expression<Func<T, object?>>, object)> setterExpressions, 
                   List<Action<T>> setters)
    {
        _setterExpressions = setterExpressions;
        _setters = setters;
    }

    /// <summary>
    /// Returns an enumerator that iterates through this style's property setter expressions.
    /// </summary>
    /// <returns>An enumerator for this style.</returns>
    /// <remarks>
    /// This exists merely to satisfy the compiler when initializing instances of this type via collection expressions.
    /// Iteration of a style is not an expected use case and implementation of a collection-related interface is not required
    /// for collection expression support.
    /// </remarks>
    [MustDisposeResource]
    public IEnumerator<(Expression<Func<T, object?>>, object)> GetEnumerator()
        => _setterExpressions.GetEnumerator();

    /// <summary>
    /// Applies this style to the provided control.
    /// </summary>
    /// <param name="control">The control to apply this style to.</param>
    public void ApplyTo(T control)
    {
        foreach (Action<T> setter in _setters)
        {
            setter(control);
        }
    }
}