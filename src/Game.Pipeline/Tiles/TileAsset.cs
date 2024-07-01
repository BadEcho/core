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
using Microsoft.Xna.Framework;

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

                _animationFrames.Add(
                    new TileAnimationFrame(tileId, TimeSpan.FromMilliseconds(duration)));
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
    /// <para>
    /// Image data will only be present here if the tile set this tile belongs to is a collection
    /// of images, as opposed to being based on a single image.
    /// </para>
    /// <para>
    /// Even if image data is present here, it will not be written to the content pipeline. Instead,
    /// it will be used to generate an all-encompassing packed texture that will be associated with
    /// the tile set itself.
    /// </para>
    /// </remarks>
    public ImageAsset? Image
    { get; }

    /// <summary>
    /// Gets or sets the explicit bounding rectangle of the region of the texture associated with this tile that
    /// will be rendered when drawing this tile.
    /// </summary>
    /// <remarks>
    /// This will only be present if the tile belongs to a tile set based on a collection of images. This is because
    /// a new (packed) texture will have been generated based on the individual images, and we require additional
    /// guidance in determining what area of the new texture to source when rendering this tile.
    /// </remarks>
    public Rectangle? SourceArea
    { get; set; }

    /// <summary>
    /// Gets a collection of animation frames in this tile's animation sequence, if one exists.
    /// </summary>
    public IReadOnlyCollection<TileAnimationFrame> AnimationFrames
        => _animationFrames;
}
