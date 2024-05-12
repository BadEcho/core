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

using System.Reflection;

namespace BadEcho.Extensions;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to reflection.
/// </summary>
public static class ReflectionExtensions
{
    private static readonly Dictionary<Type, ConstructorInfo> _TypeConstructorMap = [];

    private static readonly object _ConstructorLock 
        = new();

    /// <summary>
    /// Determines if null is a legal value for this type.
    /// </summary>
    /// <param name="type">The type to check for instance nullability.</param>
    /// <returns>True if instances of <c>type</c> can be null; otherwise, false.</returns>
    /// <remarks>
    /// <para>
    /// Be careful in the manner in which <c>type</c> is procured prior to invoking this method, as it may affect its accuracy.
    /// If the type was retrieved through a <c>typeof</c> expression, no issues will arise. If, however, <c>type</c> was
    /// obtained by invoking <see cref="object.GetType()"/>, then know that calling this method on a struct results in that
    /// struct getting boxed.
    /// </para>
    /// <para>
    /// This has implications for <see cref="Nullable{T}"/> objects, which, due to internal boxing logic,
    /// will have their underlying value types returned, not the <see cref="Nullable{T}"/> types themselves. Because a
    /// <see cref="Nullable{T}"/> object's underlying value type cannot be set to null, this method will end up returning an
    /// incorrect result for such objects.
    /// </para>
    /// </remarks>
    public static bool IsNullable(this Type type)
    {
        Require.NotNull(type, nameof(type));

        return !type.IsValueType || Nullable.GetUnderlyingType(type) != null;
    }

    /// <summary>
    /// Retrieves the default value for this type.
    /// </summary>
    /// <param name="type">The type to get the default value for.</param>
    /// <returns>The default value for <c>type</c>.</returns>
    /// <remarks>
    /// This method makes use of <see cref="IsNullable(Type)"/>; refer to that method's documentation for information on the
    /// accuracy of results based on the origin of <c>type</c>.
    /// </remarks>
    public static object? GetDefaultValue(this Type type)
    {
        Require.NotNull(type, nameof(type));

        return type.IsNullable() ? null : Activator.CreateInstance(type);
    }

    /// <summary>
    /// Finds a constructor for this type which accepts the parameter types in the specified array.
    /// </summary>
    /// <param name="type">The type to find a constructor for.</param>
    /// <param name="parameterTypes">
    /// A <see cref="Type"/> array that contains zero or more parameters that the constructor must support.
    /// </param>
    /// <returns>
    /// A reflected <see cref="ConstructorInfo"/> instance for <c>type</c> that accepts parameters whose types
    /// are found in <c>parameterTypes</c>, if found; otherwise, null.
    /// </returns>
    public static ConstructorInfo? FindConstructor(this Type type, params Type[] parameterTypes)
    {
        Require.NotNull(type, nameof(type));

        lock (_ConstructorLock)
        {
            if (_TypeConstructorMap.TryGetValue(type, out ConstructorInfo? cachedCtor))
                return cachedCtor;

            ConstructorInfo? ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                                                        Type.DefaultBinder,
                                                        parameterTypes,
                                                        []);
            if (ctor != null) 
                _TypeConstructorMap.Add(type, ctor);

            return ctor;
        }
    }

    /// <summary>
    /// Gets all possible properties found on this type.
    /// </summary>
    /// <param name="type">The type to pull the properties from.</param>
    /// <returns>A collection of <see cref="PropertyInfo"/> objects belonging to <c>type</c>.</returns>
    public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
    {
        Require.NotNull(type, nameof(type));

        return
            type.GetInterfaces()
                .Concat(new[] {type})
                .SelectMany(x => x.GetProperties());
    }

    /// <summary>
    /// Retrieves the custom attributes matching the specified type from an object decorated by those attributes.
    /// </summary>
    /// <typeparam name="T">The custom attribute type.</typeparam>
    /// <param name="attributeProvider">An object that supports custom attributes.</param>
    /// <returns>
    /// <para>
    /// The attributes matching the specified type of <typeparamref name="T"/>, if found; otherwise, an empty collection.
    /// </para>
    /// <para>
    /// While decorating an object with duplicate attributes is an impossibility, a <see cref="ICustomAttributeProvider"/> can
    /// return multiple matches if more than one attribute shares a common ancestor.
    /// </para>
    /// </returns>
    public static IEnumerable<T> GetAttributes<T>(this ICustomAttributeProvider attributeProvider)
        where T : Attribute
    {
        Require.NotNull(attributeProvider, nameof(attributeProvider));
            
        return attributeProvider.GetCustomAttributes(false).OfType<T>();
    }

    /// <summary>
    /// Retrieves the first custom attribute matching the specified type from an object decorated by the attribute.
    /// </summary>
    /// <typeparam name="T">The custom attribute type.</typeparam>
    /// <param name="attributeProvider">An object that supports custom attributes.</param>
    /// <returns>
    /// <para>
    /// The first attribute matching the specified type of <typeparamref name="T"/>, if found; otherwise, null.
    /// </para>
    /// <para>
    /// While decorating an object with duplicate attributes is an impossibility, a <see cref="ICustomAttributeProvider"/> can
    /// return multiple matches if more than one attribute shares a common ancestor.
    /// </para>
    /// </returns>
    public static T? GetAttribute<T>(this ICustomAttributeProvider attributeProvider)
        where T : Attribute
    {
        return attributeProvider.GetAttributes<T>().FirstOrDefault();
    }

    /// <summary>
    /// Returns the identifiers that do not include any part of an assembly's name from the sequence of identifiers that make
    /// up the <c>qualified-identifier</c> of the type's <c>namespace-declaration</c>.
    /// </summary>
    /// <param name="type">The type whose trailing namespace identifiers we're interested in.</param>
    /// <returns>
    /// The non-assembly related identifiers that make up the <c>qualified-identifier</c> of the <c>namespace-declaration</c>
    /// of <c>type</c>; or an empty string if <c>type</c> has no namespace. 
    /// </returns>
    public static string GetNonAssemblyNamespaceIdentifiers(this Type type)
    {
        Require.NotNull(type, nameof(type));

        // Technically if the assembly lacks a name, then there is nothing to remove from the namespace identifiers.
        string assemblyShortName = type.Assembly.GetName().Name ?? string.Empty;
        string[] assemblyIdentifiers = assemblyShortName.Split('.');

        return type.Namespace?.Split('.')
                   .Where(n => !assemblyIdentifiers.Contains(n))
                   .DefaultIfEmpty(string.Empty)
                   .Aggregate((x, y) => $"{x}.{y}")
            ?? string.Empty;
    }
}