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

using Microsoft.Xna.Framework.Graphics;
using BadEcho.Game.Properties;
using Microsoft.Xna.Framework;
using BadEcho.Extensions;

namespace BadEcho.Game.Atlases;

/// <summary>
/// Provides a texture containing multiple smaller images organized as regions and arranged in an optimal fashion, allowing for
/// the selective drawing of said regions without the need to load and unload any additional textures.
/// </summary>
/// <remarks>
/// <para>
/// The terms "sprite sheet" and "texture atlas" are often used interchangeably; however, as far as the Bad Echo game framework
/// is concerned: they are two different things. A sprite sheet contains images arranged uniformly and tabularly, all related to
/// a single entity (such as an NPC sprite). A texture atlas contains images optimally packed together as close as possible, all
/// not necessarily corresponding to a single entity (such as different elements on a HUD interface).
/// </para>
/// <para>
/// Because a texture atlas packs potentially different-sized images together as much as possible, there is no guarantee of a
/// uniform layout. Due to this, regions must be attributed with their specific coordinates and dimensions so that they are
/// sourceable from the texture of the atlas.
/// </para>
/// </remarks>
public sealed class TextureAtlas
{
    private readonly Dictionary<string, TextureRegion> _regions = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="TextureAtlas"/> class.
    /// </summary>
    /// <param name="texture">The texture of the atlas that comprises its regions.</param>
    public TextureAtlas(Texture2D texture)
    {
        Require.NotNull(texture, nameof(texture));

        Texture = texture;
    }

    /// <summary>
    /// Gets the texture of the atlas that comprises its regions.
    /// </summary>
    public Texture2D Texture
    { get; }

    /// <summary>
    /// Gets the region identified by the specified name.
    /// </summary>
    /// <param name="name">The identifying name of the region to get.</param>
    /// <returns>The texture region identified by <paramref name="name"/>.</returns>
    public TextureRegion this[string name]
        => _regions[name];

    /// <summary>
    /// Creates a new region that sources the specified area of the atlas's texture.
    /// </summary>
    /// <param name="name">The identifying name for the region.</param>
    /// <param name="sourceArea">
    /// The bounding rectangle of the region in <see cref="Texture"/> that will be rendered when drawing the region.
    /// </param>
    public void AddRegion(string name, Rectangle sourceArea)
    {
        if (_regions.ContainsKey(name))
            throw new ArgumentException(Strings.AtlasAlreadyHasRegion.InvariantFormat(name), nameof(name));

        var region = new TextureRegion(Texture, sourceArea);
        
        _regions.Add(name, region);
    }

    /// <summary>
    /// Creates a new 9-slice region that sources the specified area of the atlas's texture.
    /// </summary>
    /// <param name="name">The identifying name for the region.</param>
    /// <param name="sourceArea">
    /// The bounding rectangle of the region in <see cref="Texture"/> that will be rendered when drawing the region.
    /// </param>
    /// <param name="nineSlicePadding">The space around the center slice, occupied by the other slices of the region.</param>
    public void AddRegion(string name, Rectangle sourceArea, Thickness nineSlicePadding)
    {
        if (_regions.ContainsKey(name))
            throw new ArgumentException(Strings.AtlasAlreadyHasRegion.InvariantFormat(name), nameof(name));

        var region = new NineSliceRegion(Texture, sourceArea, nineSlicePadding);

        _regions.Add(name, region);
    }
}