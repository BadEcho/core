//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Odin;

/// <summary>
/// Provides a trampoline operation, which allows for a tail-recursive styled operation to be executed as if it was
/// implemented in a language supporting tail call optimizations.
/// </summary>
public sealed class Trampoline
{
    /// <summary>
    /// Executes a method that takes no arguments in a tail-recursive fashion until the end of the trampolined call chain has been
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

    /// <summary>
    /// Executes a method that takes a single argument in a tail-recursive fashion until the end of the trampolined call chain has
    /// been reached and a final result returned.
    /// </summary>
    /// <typeparam name="T">The type of result for the operation.</typeparam>
    /// <param name="method">The method to execute.</param>
    /// <param name="argument">The initial argument to be provided to the call chain.</param>
    /// <returns>The result of the tail-recursive operation.</returns>
    public static T Execute<T>(Func<T, Bounce<T>> method, T argument)
    {
        Require.NotNull(method, nameof(method));

        Bounce<T> bouncingMethod = Bounce.Continue(argument);

        while (!bouncingMethod.IsFinished)
        {
            bouncingMethod = method(bouncingMethod.Result);
        }

        return bouncingMethod.Result;
    }
}