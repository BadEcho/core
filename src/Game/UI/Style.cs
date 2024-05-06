using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Marshalling;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

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
    private readonly List<Action<T>> _setters = new();

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
        if (propertyAccess.NodeType != ExpressionType.MemberAccess)
        {   // If the expression is neither accessing nor converting a member, then it is an invalid expression for a style.
            if (propertyAccess.NodeType != ExpressionType.Convert)
                throw new ArgumentException();

            propertyAccess = ((UnaryExpression) propertyAccess).Operand;

            if (propertyAccess.NodeType != ExpressionType.MemberAccess)
                throw new ArgumentException();
        }

        MemberInfo member = ((MemberExpression) propertyAccess).Member;

        // Only properties are meant to be styled.
        if (member.MemberType != MemberTypes.Property)
            throw new ArgumentException();

        propertyAccess = Expression.Property(control, (PropertyInfo) member);

        Expression<Action<T>> setterExpression
            = Expression.Lambda<Action<T>>(propertyAccess, control);

        _setters.Add(setterExpression.Compile());
    }

    public void ApplyTo(T control)
    {
        foreach (var a in _setters)
        {
            a(control);
        }
    }
    
    public IEnumerator GetEnumerator()
    {
        throw new NotImplementedException();
    }
}