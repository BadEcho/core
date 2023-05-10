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

namespace BadEcho.Game;

/// <summary>
/// Defines a visual component encompassing an area of known size, able to be drawn to the screen.
/// </summary>
public interface IVisualRegion : IVisual
{
    /// <summary>
    /// Gets the size of the visual region's source.
    /// </summary>
    Size Size { get; }
}
