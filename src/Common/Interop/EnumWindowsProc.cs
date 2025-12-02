// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Represents a callback invoked by the <see cref="User32.EnumWindows"/> function.
/// </summary>
/// <param name="hWnd">A handle to a top-level window.</param>
/// <param name="lParam">The application-defined value given in <see cref="User32.EnumWindows"/>.</param>
/// <returns>Value indicating whether enumeration should continue.</returns>
internal delegate bool EnumWindowsProc(nint hWnd, nint lParam);
