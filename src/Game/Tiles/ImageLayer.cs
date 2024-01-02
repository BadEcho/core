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
    /// <param name="image">Gets the texture for the layer's image.</param>
    /// <param name="customProperties">The image layer's custom properties.</param>
    public ImageLayer(string name, Texture2D image, CustomProperties customProperties)
        : base(name, customProperties)
    {
        Image = image;
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
    { get; init; }
}
