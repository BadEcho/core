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

namespace BadEcho.Hooks;

/// <summary>
/// Specifies the state of a key.
/// </summary>
public enum KeyState
{
    /// <summary>
    /// The key is pressed.
    /// </summary>
    Down,
    /// <summary>
    /// The key is released.
    /// </summary>
    Up
}