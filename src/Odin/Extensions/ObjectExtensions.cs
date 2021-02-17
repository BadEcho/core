//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
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
        public static bool Equals<T>(this T source, T? other) 
            => EqualityComparer<T>.Default.Equals(source, other);
    }
}
