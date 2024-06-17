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
using BadEcho.Game.Tiles;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides configuration data for a tile belonging to a tile set.
/// </summary>
public sealed class TileAsset : ExtensibleAsset
{
    private const string ID_ATTRIBUTE = "id";
    private const string ANIMATION_ELEMENT = "animation";
    private const string FRAME_ELEMENT = "frame";
    private const string TILE_ID_ATTRIBUTE = "tileid";
    private const string DURATION_ATTRIBUTE = "duration";

    private readonly List<TileAnimationFrame> _animationFrames = [];
    
    /// <summary>
    /// Initializes a new instance of the <see cref="TileAsset"/> class.
    /// </summary>
    /// <param name="root">XML element for the tile's configuration.</param>
    public TileAsset(XElement root)
        : base(root)
    {
        Require.NotNull(root, nameof(root));

        Id = (int?) root.Attribute(ID_ATTRIBUTE) ?? default;

        XElement? imageElement = root.Element(XmlConstants.ImageElement);

        if (imageElement != null)
            Image = new ImageAsset(imageElement);

        XElement? animationElement = root.Element(ANIMATION_ELEMENT);

        if (animationElement != null)
        {
            foreach (XElement frameElement in animationElement.Elements(FRAME_ELEMENT))
            {
                int tileId = (int?) frameElement.Attribute(TILE_ID_ATTRIBUTE) ?? default;
                int duration = (int?) frameElement.Attribute(DURATION_ATTRIBUTE) ?? default;

                _animationFrames.Add(new TileAnimationFrame(tileId, duration));
            }
        }
    }

    /// <summary>
    /// Gets the local identifier of this tile within its tile set.
    /// </summary>
    public int Id
    { get; }

    /// <summary>
    /// Gets the image data for this tile's texture.
    /// </summary>
    /// <remarks>
    /// Image data will only be present here if the tile set this tile belongs to is a collection
    /// of images, as opposed to being based on a single image.
    /// </remarks>
    public ImageAsset? Image
    { get; }

    /// <summary>
    /// Gets a collection of animation frames in this tile's animation sequence, if one exists.
    /// </summary>
    public IReadOnlyCollection<TileAnimationFrame> AnimationFrames
        => _animationFrames;
}
