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

using System.Windows.Threading;

namespace BadEcho.Fenestra.Windows;

/// <summary>
/// Defines an assembler of a window's data context.
/// </summary>
/// <typeparam name="T">The type of data context used by the window.</typeparam>
public interface IContextAssembler<out T>
{
    /// <summary>
    /// Assembles a new data context for a window.
    /// </summary>
    /// <param name="dispatcher">The dispatcher which the window is running on.</param>
    /// <returns>The assembled data context.</returns>
    T Assemble(Dispatcher dispatcher);
}