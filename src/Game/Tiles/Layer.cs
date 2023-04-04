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

using Microsoft.Xna.Framework;

namespace BadEcho.Game.Tiles;

/// <summary>
/// Provides an area in a tile map filled with content.
/// </summary>
public abstract class Layer : Extensible
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Layer"/> class.
    /// </summary>
    /// <param name="name">The name of the layer.</param>
    /// <param name="isVisible">Value indicating if the layer data should actually be rendered.</param>
    /// <param name="opacity">The opacity of the layer and all of its contents.</param>
    /// <param name="offset">The offset, in terms of the layer's position, from the tile map's origin.</param>
    protected Layer(
        string name, bool isVisible, float opacity, Vector2 offset)
    {
        Name = name;
        IsVisible = isVisible;
        Opacity = opacity;
        Offset = offset;
    }

    /// <summary>
    /// Gets the name of this layer.
    /// </summary>
    public string Name
    { get; }

    /// <summary>
    /// Gets a value indicating if this layer's data should actually be rendered.
    /// </summary>
    public bool IsVisible
    { get; }

    /// <summary>
    /// Gets the opacity of this layer and all of its contents.
    /// </summary>
    public float Opacity
    { get; }

    /// <summary>
    /// Gets the offset, in terms of this layer's position, from the tile map's origin.
    /// </summary>
    public Vector2 Offset
    { get; }
}
