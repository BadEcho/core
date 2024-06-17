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

using System.Xml.Linq;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides configuration data for an animation frame belonging to tile's animation sequence.
/// </summary>
public sealed class FrameAsset
{
    private const string TILE_ID_ATTRIBUTE = "tileid";
    private const string DURATION_ATTRIBUTE = "duration";

    /// <summary>
    /// Initializes a new instance of the <see cref="FrameAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the animation frame's configuration.</param>
    public FrameAsset(XElement root)
    {
        Require.NotNull(root, nameof(root));

        TileId = (int?) root.Attribute(TILE_ID_ATTRIBUTE) ?? default;
        Duration = (int?) root.Attribute(DURATION_ATTRIBUTE) ?? default;
    }

    /// <summary>
    /// Gets the local identifier of the tile displayed by this frame.
    /// </summary>
    public int TileId
    { get; }

    /// <summary>
    /// Gets the amount of time, in milliseconds, this frame should be displayed before advancing to the next frame.
    /// </summary>
    public int Duration
    { get; }
}
