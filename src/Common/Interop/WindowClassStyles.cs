//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Specifies styling configurations for window classes that are assigned during window class registration.
/// </summary>
[Flags]
public enum WindowClassStyles 
{
    /// <summary>
    /// Saves, as a bitmap, the portion of the screen image obscured by a window this class, for fast restoration later.
    /// </summary>
    SaveBits = 0x800,
    /// <summary>
    /// Enables the drop shadow effect on a window.
    /// </summary>
    DropShadow = 0x20000
}
