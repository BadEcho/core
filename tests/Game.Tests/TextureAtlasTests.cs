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

using BadEcho.Game.Atlases;
using Microsoft.Xna.Framework;
using Xunit;

namespace BadEcho.Game.Tests;

public class TextureAtlasTests : IClassFixture<ContentManagerFixture>
{
    private readonly Microsoft.Xna.Framework.Content.ContentManager _content;

    public TextureAtlasTests(ContentManagerFixture contentFixture)
        => _content = contentFixture.Content;

    [Fact]
    public void Load_BlackShuttleGrass_NotNull()
    {
        TextureAtlas atlas = _content.Load<TextureAtlas>($"Atlases\\BlackShuttleGrass");

        Assert.NotNull(atlas);
    }

    [Fact]
    public void Load_BlackShuttleGrass_ValidRegions()
    {
        TextureAtlas atlas = _content.Load<TextureAtlas>($"Atlases\\BlackShuttleGrass");

        var shuttle = atlas["shuttle"];

        Assert.NotNull(shuttle);
        Assert.Equal(new Rectangle(0, 0, 58, 90), shuttle.SourceArea);

        var grasslands = atlas["Grasslands"];

        Assert.NotNull(grasslands);
        Assert.Equal(new Rectangle(0, 90, 80, 32), grasslands.SourceArea);
        var nineSliceGrasslands = Assert.IsType<NineSliceRegion>(grasslands);
        Assert.Equal(new Thickness(20, 8, 30, 12), nineSliceGrasslands.Padding);

        var black = atlas["black"];

        Assert.NotNull(black);
        Assert.Equal(new Rectangle(58, 0, 16, 16), black.SourceArea);
    }
}
