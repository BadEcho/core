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

namespace BadEcho.Game.Tiles;

/// <summary>
/// Specifies the way a map is oriented and the manner in which its containing objects are projected.
/// </summary>
public enum MapOrientation  
{
    /// <summary>
    /// The map is oriented such that all projection lines are orthogonal to the projection plane.
    /// </summary>
    Orthogonal,
    /// <summary>
    /// The map is oriented such that it makes equal angles with all planes of an object, meaning the angle between all axes
    /// are 120 degrees.
    /// </summary>
    Isometric
}
