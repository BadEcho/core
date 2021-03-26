//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq.Expressions;
using BadEcho.Odin;

namespace BadEcho.Fenestra
{
    /// <summary>
    /// Provides a reader for expressions involving XAML-related code elements, for instances where the built-in <c>nameof</c>
    /// expression is not sufficient.
    /// </summary>
    public static class NameOf
    {
        private const string SUFFIX_DEPENDENCY_PROPERTY = "Property";

        /// <summary>
        /// Returns the name of the dependency property being expressed as it would appear in a XAML context.
        /// </summary>
        /// <param name="expression"></param>
        /// <typeparam name="T">The type containing the dependency property.</typeparam>
        /// <returns>
        /// The name of the dependency property expressed in <c>expression</c> in a format which is acceptable in a XAML context.
        /// </returns>
        /// <remarks>
        /// This method of reading the dependency property name is most appropriate to use when we need to provide the name for
        /// an attached dependency property during its registration. Because, more often than not, there is no CLR property
        /// counterpart for an attached property, no suitable code element otherwise exists to use with <c>nameof</c>.
        /// </remarks>
        public static string ReadDependencyPropertyName<T>(Expression<Func<T>> expression)
        {
            Require.NotNull(expression, nameof(expression));

            var body = (MemberExpression) expression.Body;

            return body.Member.Name.EndsWith(SUFFIX_DEPENDENCY_PROPERTY, StringComparison.OrdinalIgnoreCase)
                ? body.Member.Name[..^(SUFFIX_DEPENDENCY_PROPERTY.Length)]
                : body.Member.Name;
        }
    }
}