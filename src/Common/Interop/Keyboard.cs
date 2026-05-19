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

namespace BadEcho.Interop;

/// <summary>
/// Provides information regarding a keyboard device.
/// </summary>
public static class Keyboard
{
    /// <summary>
    /// Determines if the specified key is currently pressed.
    /// </summary>
    /// <param name="key">The specified key.</param>
    /// <returns>True if <c>key</c> is pressed; otherwise, false.</returns>
    public static bool IsPressed(VirtualKey key)
    {
        short state = User32.GetKeyState(key);

        return (state & 0x8000) != 0;
    }
}
