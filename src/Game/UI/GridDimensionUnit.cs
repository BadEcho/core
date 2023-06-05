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

namespace BadEcho.Game.UI;

/// <summary>
/// Specifies the type of unit for a distance measurement of space within a <see cref="Grid"/>.
/// </summary>
public enum GridDimensionUnit
{
    /// <summary>
    /// The measurement is determined by the size properties of the content object.
    /// </summary>
    Auto,
    /// <summary>
    /// The measurement is determined using an expressed weighted proportion of available space.
    /// </summary>
    Proportional,
    /// <summary>
    /// The measurement is determined using an expressed absolute value in pixels.
    /// </summary>
    Absolute
}
