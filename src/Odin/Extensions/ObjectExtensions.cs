//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace BadEcho.Odin.Extensions
{
    /// <summary>
    /// Provides a set of static methods intended to simplify the use of any type of object.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Determines whether this object is equal to another object of type <typeparamref name="T"/> using
        /// a default equality comparer for the type.
        /// </summary>
        /// <typeparam name="T">The type of objects to compare.</typeparam>
        /// <param name="source">The current object.</param>
        /// <param name="other">The object to compare with the current object.</param>
        /// <returns>True if <c>source</c> and <c>other</c> are equal; otherwise, false.</returns>
        /// <remarks>
        /// <para>
        /// This extension method simplifies the act of checking whether two objects of a particular generic type
        /// parameter are equal.
        /// </para>
        /// <para>
        /// Comparing two generic values using <see cref="object.Equals(object)"/> will result in
        /// values being boxed if they are of a value type. Use of the equality operator to get around this many
        /// times will not work automatically as it requires constraining the generic type to a reference type
        /// (using the 'class' generic type constraint).
        ///</para>
        /// <para>
        /// Using this method to check for equality will not result in boxing if the generic type is a value type.
        /// It also greatly simplifies the whole process of determining object equality in a generic context.
        /// </para>
        /// <para>
        /// This extension method will not preclude an <see cref="IEquatable{T}"/> implementation's execution,
        /// as the default equality comparer used by this method will itself defer to an <see cref="IEquatable{T}"/>
        /// implementation if it is present.
        /// </para>
        /// </remarks>
        public static bool Equals<T>(this T? source, T? other) 
            => EqualityComparer<T>.Default.Equals(source, other);

        /// <summary>
        /// Calculates the hash code for this instance using one of the object's properties.
        /// </summary>
        /// <typeparam name="T">The type of the object the hash code is being calculated for.</typeparam>
        /// <typeparam name="T1">The type of the first property the hash code is based on.</typeparam>
        /// <param name="source">The object we're calculating the hash code for.</param>
        /// <param name="firstProperty">The first property to use in calculating the hash code.</param>
        /// <returns>A hash code for <c>source</c> based on the provided property values.</returns>
        /// <remarks>
        /// Generic type parameters are present for <c>source</c> and all other properties in order to avoid boxing as
        /// <see cref="object"/>.
        /// </remarks>
        public static int GetHashCode<T, T1>(this T source, T1 firstProperty)
            => HashCode.Combine(firstProperty);

        /// <summary>
        /// Calculates the hash code for this instance using one of the object's properties.
        /// </summary>
        /// <typeparam name="T">The type of the object the hash code is being calculated for.</typeparam>
        /// <typeparam name="T1">The type of the first property the hash code is based on.</typeparam>
        /// <typeparam name="T2">The type of the second property the hash code is based on.</typeparam>
        /// <param name="source">The object we're calculating the hash code for.</param>
        /// <param name="firstProperty">The first property to use in calculating the hash code.</param>
        /// <param name="secondProperty">The second property to use in calculating the hash code.</param>
        /// <returns>A hash code for <c>source</c> based on the provided property values.</returns>
        /// <remarks>
        /// Generic type parameters are present for <c>source</c> and all other properties in order to avoid boxing as
        /// <see cref="object"/>.
        /// </remarks>
        public static int GetHashCode<T, T1, T2>(this T source, T1 firstProperty, T2 secondProperty)
            => HashCode.Combine(firstProperty, secondProperty);
    }
}
