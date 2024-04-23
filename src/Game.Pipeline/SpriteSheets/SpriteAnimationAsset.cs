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

namespace BadEcho.Game.Pipeline.SpriteSheets;

/// <summary>
/// Provides configuration data for an animation of a sprite.
/// </summary>
public sealed class SpriteAnimationAsset
{
    /// <summary>
    /// Gets the name of the animation.
    /// </summary>
    public string Name
    { get; init; } = string.Empty;

    /// <summary>
    /// Gets the index of the first frame in the animation.
    /// </summary>
    public int StartFrame
    { get; init; }

    /// <summary>
    /// Gets the index of the last frame in the animation.
    /// </summary>
    public int EndFrame
    { get; init; }
}
