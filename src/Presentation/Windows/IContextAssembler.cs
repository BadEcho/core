//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Threading;

namespace BadEcho.Presentation.Windows;

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