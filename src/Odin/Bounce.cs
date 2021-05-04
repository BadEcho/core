//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Odin
{
    /// <summary>
    /// Provides a bouncing method used in a trampolined recursive operation.
    /// </summary>
    public class Bounce
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Bounce"/> class.
        /// </summary>
        internal Bounce()
        { }

        /// <summary>
        /// Gets a value indicating if the method is done bouncing, marking the end of the trampolined operation.
        /// </summary>
        public bool IsFinished
        { get; private init; }

        /// <summary>
        /// Bounces the next method in the call chain.
        /// </summary>
        /// <returns>The <see cref="Bounce"/> for the next method in the call chain.</returns>
        public static Bounce Continue()
            => new();

        /// <summary>
        /// Bounces the next method in the call chain.
        /// </summary>
        /// <typeparam name="T">The type of result accepted and returned by the bouncing method.</typeparam>
        /// <param name="result">The result to pass to the next method in the call chain.</param>
        /// <returns>The <see cref="Bounce{T}"/> for the next method in the call chain.</returns>
        public static Bounce<T> Continue<T>(T result)
            => new(result);

        /// <summary>
        /// Bounces the final method in the call chain, marking the end of the trampolined operation.
        /// </summary>
        /// <returns>The final <see cref="Bounce"/> in the call chain.</returns>
        public static Bounce Finish()
            => new() {IsFinished = true};

        /// <summary>
        /// Bounces the final method in the call chain, marking the end of the trampolined operation.
        /// </summary>
        /// <typeparam name="T">The type of result accepted and returned by the bouncing method.</typeparam>
        /// <param name="result">The result to pass to the final method in the call chain.</param>
        /// <returns>THe <see cref="Bounce{T}"/> for the final method in the call chain.</returns>
        public static Bounce<T> Finish<T>(T result)
            => new(result) {IsFinished = true};
    }
}