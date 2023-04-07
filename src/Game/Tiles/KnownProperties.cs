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

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides a set of known custom property names used by tile-related content.
/// </summary>
public static class KnownProperties
{
    /// <summary>
    /// Gets the name for the boolean custom property on tile layers that indicates whether its tiles will cause
    /// collisions for entities attempting to cross over them. 
    /// </summary>
    public static string Collidable
        => nameof(Collidable);
}
