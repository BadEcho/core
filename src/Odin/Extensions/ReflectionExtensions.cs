//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BadEcho.Odin.Extensions
{
    /// <summary>
    /// Provides a set of static methods intended to aid in matters related to reflection.
    /// </summary>
    public static class ReflectionExtensions
    {
        private static readonly Dictionary<Type, ConstructorInfo> _TypeConstructorMap 
            = new();

        private static readonly object _ConstructorLock 
            = new();

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
                if (_TypeConstructorMap.ContainsKey(type))
                    return _TypeConstructorMap[type];

                ConstructorInfo? ctor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance,
                                                            Type.DefaultBinder,
                                                            parameterTypes,
                                                            Array.Empty<ParameterModifier>());
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
    }
}
