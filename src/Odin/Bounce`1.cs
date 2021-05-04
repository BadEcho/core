//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Odin
{
    /// <summary>
    /// Provides a bouncing method used in a trampolining recursive operation that accepts and returns a single
    /// result.
    /// </summary>
    /// <typeparam name="T">The type of result accepted and returned by the bouncing method.</typeparam>
    public sealed class Bounce<T> : Bounce
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bounce{T}"/> class.
        /// </summary>
        /// <param name="result">The result being passed to this bouncing method.</param>
        internal Bounce(T result) 
            => Result = result;

        /// <summary>
        /// Gets the current result that was passed to the bounced method.
        /// </summary>
        public T Result
        { get; }
    }
}