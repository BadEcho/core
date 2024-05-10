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

using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using BadEcho.Extensions;
using BadEcho.Game.Properties;

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
public sealed class Style<T> : IEnumerable
    where T : IControl
{
    private readonly List<Action<T>> _setters = [];

    IEnumerator IEnumerable.GetEnumerator()
        => _setters.GetEnumerator();

    /// <summary>
    /// Adds a setter that applies a property value to this style.
    /// </summary>
    /// <param name="setter">
    /// An expression accessing the property being styled coupled with the value to apply to said property.
    /// </param>
    /// <remarks>
    /// <para>
    /// This allows collection initializers and expressions to be used to create an instance of this class,
    /// in addition to providing a means to add further setters post-initialization. It will convert the
    /// expression into an <see cref="Action{TControl}"/> that will assign the configured value to the
    /// property.
    /// </para>
    /// </remarks>
    public void Add((Expression<Func<T, object?>> Property, object Value) setter)
    {
        Require.NotNull(setter, nameof(setter));
        
        ParameterExpression control = Expression.Parameter(typeof(T), nameof(control));
        Expression propertyAccess = setter.Property.Body;
        
        // Expressions accessing properties that return value types will have their member access information
        // stuffed inside a conversion UnaryExpression (on account of the necessary boxing).
        if (propertyAccess.NodeType == ExpressionType.Convert) 
            propertyAccess = ((UnaryExpression) propertyAccess).Operand;

        // If we don't have a member access expression at this point, then it is an invalid expression for a style.
        if (propertyAccess.NodeType != ExpressionType.MemberAccess)
            throw new ArgumentException(Strings.StyleSetterNotMemberAccess);

        MemberInfo member = ((MemberExpression) propertyAccess).Member;

        // Only properties are meant to be set by the style.
        if (member.MemberType != MemberTypes.Property)
            throw new ArgumentException(Strings.StyleSetterNonPropertyAccess);

        // Only the target control type is meant to be styled.
        if (member.DeclaringType != null && !typeof(T).IsA(member.DeclaringType))
            throw new ArgumentException(Strings.StyleSetterNonLocalProperty);

        PropertyInfo property = (PropertyInfo) member;
        propertyAccess = Expression.Property(control, property);

        Expression propertySetter 
            = Expression.Assign(propertyAccess, Expression.Constant(setter.Value, property.PropertyType));

        Expression<Action<T>> delegateExpression
            = Expression.Lambda<Action<T>>(propertySetter, control);

        _setters.Add(delegateExpression.Compile());
    }

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