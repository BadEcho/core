//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides an area in a tile map filled with image data.
/// </summary>
public sealed class ImageLayer : Layer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ImageLayer"/> class.  
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    /// <param name="isVisible">Value indicating if the layer data should actually be rendered.</param>
    /// <param name="opacity">The opacity of the layer and all of its contents.</param>
    /// <param name="offset">The offset, in terms of the layer's position, from the tile map's origin.</param>
    /// <param name="image">Gets the texture for the layer's image.</param>
    /// <param name="position">The drawing location of the image within the offset bounds of the layer.</param>
    public ImageLayer(string name, bool isVisible, float opacity, Vector2 offset, Texture2D image, Vector2 position)
        : base(name, isVisible, opacity, offset)
    {
        Image = image;
        Position = position;
    }

    /// <summary>
    /// Gets the texture for this layer's image.
    /// </summary>
    public Texture2D Image
    { get; }

    /// <summary>
    /// Gets the drawing location of the image within the offset bounds of this layer.
    /// </summary>
    public Vector2 Position
    { get; }
}
