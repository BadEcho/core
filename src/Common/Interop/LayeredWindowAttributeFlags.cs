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
/// Specifies flags that result in actions taken when setting a layer window's attributes.
/// </summary>
[Flags]
internal enum LayeredWindowAttributeFlags
{
    /// <summary>
    /// No action taken.
    /// </summary>
    None = 0x0,
    /// <summary>
    /// Use the supplied alpha value to determine the opacity of the window.
    /// </summary>
    Alpha = 0x1,
    /// <summary>
    /// Use the supplied color key as the transparency color.
    /// </summary>
    ColorKey = 0x2
}
