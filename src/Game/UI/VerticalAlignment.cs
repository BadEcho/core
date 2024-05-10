//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Game.UI;

/// <summary>
/// Specifies how a child control is vertically positioned within its layout allocation.
/// </summary>
public enum VerticalAlignment
{
    /// <summary>
    /// The control is stretched to fill its layout allocation.
    /// </summary>
    /// <remarks>An explicitly set <see cref="Control{T}.Height"/> takes precedence over this value.</remarks>
    Stretch,
    /// <summary>
    /// The control is aligned to the top of its layout allocation.
    /// </summary>
    Top,
    /// <summary>
    /// The control is aligned to the center of its layout allocation.
    /// </summary>
    Center,
    /// <summary>
    /// The control is aligned to the bottom of its layout allocation.
    /// </summary>
    Bottom
}
