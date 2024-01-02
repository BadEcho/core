//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Represents a callback installed as a hook procedure.
/// </summary>
/// <param name="code">A code used to determine how to process the message.</param>
/// <param name="wParam">A message of a type dependent on the kind of hook procedure installed.</param>
/// <param name="lParam">A message of a type dependent on the kind of hook procedure installed.</param>
/// <returns>
/// Either the value returned by the next hook in the chain or a nonzero value if no further processing of the hook
/// should occur.
/// </returns>
public delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);