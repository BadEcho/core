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

using BadEcho.Extensions;
using System.Linq.Expressions;
using System.Reflection;
using BadEcho.Game.Properties;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a means for the <see cref="Style{T}"/> type to opt in to collection expression support.
/// </summary>
public static class StyleBuilder
{
    /// <summary>
    /// Constructs a <see cref="Style{T}"/> from a span of property setters.
    /// </summary>
    /// <typeparam name="T">The type of control to be targeted by the style.</typeparam>
    /// <param name="values">
    /// A span of property setter expressions, each expressing the control property being set and the value to set said 
    /// property to.
    /// </param>
    /// <returns>A <see cref="Style{T}"/> configured to apply the provided property setter <c>values</c>.</returns>
    public static Style<T> Create<T>(ReadOnlySpan<(Expression<Func<T, object?>> Property, object Value)> values)
        where T : IControl
    {
        List<(Expression<Func<T, object?>> Property, object Value)> setterExpressions = [];
        List<Action<T>> setters = [];

        foreach (var setterExpression in values)
        {
            setterExpressions.Add(setterExpression);
            setters.Add(CreateSetter(setterExpression));
        }

        return new Style<T>(setterExpressions, setters);
    }

    private static Action<T> CreateSetter<T>((Expression<Func<T, object?>> Property, object Value) setterExpression)
    {
        ParameterExpression control = Expression.Parameter(typeof(T), nameof(control));
        Expression propertyAccess = setterExpression.Property.Body;
        
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
            = Expression.Assign(propertyAccess, Expression.Constant(setterExpression.Value, property.PropertyType));

        Expression<Action<T>> delegateExpression
            = Expression.Lambda<Action<T>>(propertySetter, control);

        return delegateExpression.Compile();
    }
}

