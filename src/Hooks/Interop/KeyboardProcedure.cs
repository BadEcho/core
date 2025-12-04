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

using BadEcho.Interop;

namespace BadEcho.Hooks.Interop;

/// <summary>
/// Represents a callback that processes keyboard input messages.
/// </summary>
/// <param name="state">An enumeration value specifying the state of the key.</param>
/// <param name="key">An enumeration value specifying the key that generated the message.</param>
/// <returns>The result of processing the keyboard input message.</returns>
public delegate ProcedureResult KeyboardProcedure(KeyState state, VirtualKey key);