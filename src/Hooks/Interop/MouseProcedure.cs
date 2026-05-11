// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
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
/// Represents a callback that processes mouse input messages.
/// </summary>
/// <param name="mouseEvent"></param>
/// <param name="x">The x-coordinate of the cursor.</param>
/// <param name="y">The y-coordinate of the cursor.</param>
/// <returns>The result of processing the mouse input message.</returns>
public delegate ProcedureResult MouseProcedure(MouseEvent mouseEvent, int x, int y);
