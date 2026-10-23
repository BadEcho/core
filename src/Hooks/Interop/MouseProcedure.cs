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
/// <param name="mouseEvent">An enumeration value specifying the type of mouse input event.</param>
/// <param name="x">The x-coordinate of the cursor.</param>
/// <param name="y">The y-coordinate of the cursor.</param>
/// <param name="data">
/// Additional information about the event.
/// For <see cref="MouseEvent.Wheel"/> and <see cref="MouseEvent.HorizontalWheel"/>, this is the distance the wheel was rotated,
/// in multiples or fractions of 120; positive values indicate rotation forward (away from the user) or to the right.
/// For <see cref="MouseEvent.XButtonDown"/> and <see cref="MouseEvent.XButtonUp"/>, this is 1 for the first X button or 2 for the
/// second.
/// For all other events, this is zero.
/// </param>
/// <returns>The result of processing the mouse input message.</returns>
public delegate ProcedureResult MouseProcedure(MouseEvent mouseEvent, int x, int y, int data);
