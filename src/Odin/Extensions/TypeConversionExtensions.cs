//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace BadEcho.Odin.Extensions
{
    /// <summary>
    /// Provides a set of static methods intended to aid in matters related to type conversion.
    /// </summary>
    public static class TypeConversionExtensions
    {
        /// <summary>
        /// Determines if an instance of this type can be assigned to a variable of another type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <param name="otherType">The type to compare with the current type.</param>
        /// <returns>
        /// True if an instance of <c>type</c> can be assigned to a variable of <c>otherType</c>; otherwise, false.
        /// </returns>
        public static bool IsA(this Type type, Type otherType)
        {
            if (otherType == null) 
                throw new ArgumentNullException(nameof(otherType));
            
            return otherType.IsAssignableFrom(type);
        }

        /// <summary>
        /// Determines if an instance of this type can be assigned to a variable of another type.
        /// </summary>
        /// <typeparam name="T">The type to compare with the current type.</typeparam>
        /// <param name="type">The current type.</param>
        /// <returns>
        /// True if an instance of <c>type</c> can be assigned to a variable of <typeparamref name="T"/>; otherwise, false.
        /// </returns>
        public static bool IsA<T>(this Type type)
        {
            return type.IsA(typeof(T));
        }
    }
}
