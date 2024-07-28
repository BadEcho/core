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

namespace BadEcho.Game;

/// <summary>
/// Defines an object that occupies space.
/// </summary>
public interface ISpatial
{
    /// <summary>
    /// Gets the spatial shape of the object.
    /// </summary>
    /// <remarks>
    /// The spatial shape of the object acts as its boundary that, if crossed by another spatial object, could
    /// result in a collision between the two.
    /// </remarks>
    IShape Bounds { get; }
}
