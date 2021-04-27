//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace BadEcho.Odin
{
    /// <summary>
    /// Provides a trampoline operation, which allows for a tail-recursive styled operation to be executed as if it was
    /// implemented in a language supporting tail call optimizations.
    /// </summary>
    public sealed class Trampoline
    {
        /// <summary>
        /// Executes a method that takes no arguments in a recursive fashion until the end of the trampolined call chain has been
        /// reached.
        /// </summary>
        /// <param name="method">The method to execute.</param>
        public static void Execute(Func<Bounce> method)
        {
            Require.NotNull(method, nameof(method));

            Bounce bouncingMethod = Bounce.Continue();

            while (!bouncingMethod.IsFinished)
            {
                bouncingMethod = method();
            }
        }
    }
}