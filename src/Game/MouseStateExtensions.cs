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

using Microsoft.Xna.Framework.Input;

namespace BadEcho.Game;

/// <summary>
/// Provides a set of static methods intended to aid in matters related to the state of the mouse.
/// </summary>
public static class MouseStateExtensions
{
    /// <summary>
    /// Returns the mouse buttons currently pressed according to this mouse state.
    /// </summary>
    /// <param name="mouseState">The mouse state value to read the pressed buttons from.</param>
    /// <returns>The mouse buttons currently pressed according to <c>mouseState</c>.</returns>
    public static IEnumerable<MouseButton> GetPressedButtons(this MouseState mouseState)
    {
        var pressedButtons = new List<MouseButton>();

        if (mouseState.LeftButton == ButtonState.Pressed)
            pressedButtons.Add(MouseButton.Left);

        if (mouseState.MiddleButton == ButtonState.Pressed)
            pressedButtons.Add(MouseButton.Middle);

        if (mouseState.RightButton == ButtonState.Pressed)
            pressedButtons.Add(MouseButton.Right);

        if (mouseState.XButton1 == ButtonState.Pressed)
            pressedButtons.Add(MouseButton.ExtendedFirst);

        if (mouseState.XButton2 == ButtonState.Pressed)
            pressedButtons.Add(MouseButton.ExtendedSecond);

        return pressedButtons;
    }
}
