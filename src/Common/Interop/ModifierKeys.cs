//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Specifies keys that modify the action of another key when pressed together.
/// </summary>
[Flags]
public enum ModifierKeys
{
    /// <summary>
    /// No modifier keys.
    /// </summary>
    None = 0,
    /// <summary>
    /// The ALT key.
    /// </summary>
    Alt = 1,
    /// <summary>
    /// The CTRL key.
    /// </summary>
    Control = 2,
    /// <summary>
    /// The SHIFT key.
    /// </summary>
    Shift = 4,
    /// <summary>
    /// The Windows logo key.
    /// </summary>
    Windows = 8
}