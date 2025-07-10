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
/// Specifies a virtual key, which may be anything from an actual keyboard key to a button on the mouse.
/// </summary>
public enum VirtualKey
{
    /// <summary>
    /// A key that does not exist. The complete absence of a key. A key with no name, no purpose, and no hope.
    /// </summary>
    None = 0,
    /// <summary>
    /// The L key.
    /// </summary>
    L = 0x4C,
    /// <summary>
    /// The Z key.
    /// </summary>
    Z = 0x5A,
    /// <summary>
    /// The F11 key.
    /// </summary>
    F11 = 0x7A,
    /// <summary>
    /// The F12 key.
    /// </summary>
    F12 = 0x7B
}