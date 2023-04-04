//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Linq.Expressions;

namespace BadEcho.Presentation;

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
    /// <typeparam name="T">The type containing the dependency property.</typeparam>
    /// <param name="expression">An expression accessing the dependency property.</param>
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

        return RemoveDependencyPropertySuffix(body);
    }

    /// <summary>
    /// Returns the name of the dependency property being expressed so that its public accessors will be invoked by a XAML
    /// parser, assuming the dependency property is an attached property and such accessors exist.
    /// </summary>
    /// <typeparam name="T">The type containing the dependency property.</typeparam>
    /// <returns>
    /// The name of the dependency property expressed in <c>expression</c> such that, if registered as an attached property,
    /// all present public accessors will be invoked by a XAML parser.
    /// </returns>
    /// <remarks>
    /// <para>
    /// Attached properties are supposed to feature public accessors in the form of <c>GetNameOfProperty</c> and <c>SetNameOfProperty</c>.
    /// These accessors are commonly described, by Microsoft as well as others, as being absolute requirements for the complete and proper
    /// functioning of whatever attached property we're defining. One might assume, given that these accessors are described as essential,
    /// that any declarations of a particular attached property in XAML would result in said property's accessors being accessed
    /// at run-time.
    /// </para>
    /// <para>
    /// This is not the case, however, as attached properties used in XAML do not actually get their accessors invoked; this is done
    /// purposely as an optimization by the XAML parser. The only time they're actually invoked, outside of any calls we make to them
    /// in our own code, is at design-time by the XAML designer when data binding is occurring on the property. This optimization by the
    /// XAML parser can result in problems if the attached property has been designed such that proper operation requires invocation of an
    /// accessor prior to the property being referenced and used.
    /// </para>
    /// <para>
    /// For example, if the attached property is meant to be declared and configured using property element syntax as a collection,
    /// the collection must be initialized upon its first access by the XAML parser. The most straightforward way one might facilitate this
    /// would be to ensure it's initialized in its accessor. Because the accessors are never called by the parser, the property will remain
    /// null and an error will occur.
    /// </para>
    /// <para>
    /// Use of this method when registering an attached property will force the XAML parser to invoke its public accessor when it encounters
    /// the property's declaration in the XAML. Since accessors are usually not invoked for performance reasons, this method should only
    /// be used when accessor execution is unavoidably required.
    /// </para>
    /// </remarks>
    public static string ReadAccessorEnabledDependencyPropertyName<T>(Expression<Func<T>> expression)
    {
        Require.NotNull(expression, nameof(expression));

        var body = (MemberExpression) expression.Body;
        string nameWithoutSuffix = RemoveDependencyPropertySuffix(body);

        return $"Shadow{nameWithoutSuffix}";
    }

    private static string RemoveDependencyPropertySuffix(MemberExpression expression)
        => expression.Member.Name.EndsWith(SUFFIX_DEPENDENCY_PROPERTY, StringComparison.OrdinalIgnoreCase)
            ? expression.Member.Name[..^(SUFFIX_DEPENDENCY_PROPERTY.Length)]
            : expression.Member.Name;
}