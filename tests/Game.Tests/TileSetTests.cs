//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Game.Tiles;
using Microsoft.Xna.Framework;
using Xunit;

namespace BadEcho.Game.Tests;

public class TileSetTests : IClassFixture<ContentManagerFixture>
{
    private readonly Microsoft.Xna.Framework.Content.ContentManager _content;

    public TileSetTests(ContentManagerFixture contentFixture)
        => _content = contentFixture.Content;

    [Theory]
    [InlineData("Grasslands")]
    [InlineData("GrasslandsCustomProperties")]
    [InlineData("CompositeGrass")]
    public void Load_NotNull(string tileSetName)
    {
        TileSet tileSet = _content.Load<TileSet>($"Tiles\\{tileSetName}");
        
        Assert.NotNull(tileSet);
    }

    [Fact]
    public void Load_Grasslands_SizeValid()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\Grasslands");

        Assert.Equal(16, tileSet.TileSize.Width);
        Assert.Equal(16, tileSet.TileSize.Height);
    }

    [Fact]
    public void Load_Grasslands_CountValid()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\Grasslands");

        Assert.Equal(10, tileSet.TileCount);
    }

    [Fact]
    public void Load_Grasslands_LastIdValid()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\Grasslands");

        Assert.Equal(9, tileSet.LastId);
    }

    [Fact]
    public void Load_GrasslandsCustomProperties_PropertiesValid()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\GrasslandsCustomProperties");

        Assert.True(tileSet.CustomProperties.Strings.ContainsKey("Something"));
        Assert.Equal("In The Way", tileSet.CustomProperties.Strings["Something"]);
    }

    [Fact]
    public void Load_GrasslandsCustomProperties_TilePropertiesValid()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\GrasslandsCustomProperties");

        var firstProperties = tileSet.GetTileCustomProperties(0);
        var secondProperties = tileSet.GetTileCustomProperties(1);
        var thirdProperties = tileSet.GetTileCustomProperties(2);
        var fourthProperties = tileSet.GetTileCustomProperties(3);

        Assert.NotNull(firstProperties);
        Assert.NotNull(secondProperties);
        Assert.NotNull(thirdProperties);
        Assert.NotNull(thirdProperties);
        Assert.NotNull(fourthProperties);

        Assert.True(secondProperties.Strings.ContainsKey("Something"));
        Assert.Equal("Hello", secondProperties.Strings["Something"]);

        Assert.True(fourthProperties.Strings.ContainsKey("SomethingElse"));
        Assert.Equal("Hmm?", fourthProperties.Strings["SomethingElse"]);
    }

    [Fact]
    public void Load_CompositeGrass_CountValid()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\CompositeGrass");

        Assert.Equal(4, tileSet.TileCount);
    }

    [Fact]
    public void Load_CompositeGrass_LastIdValid()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\CompositeGrass");

        Assert.Equal(3, tileSet.LastId);
    }

    [Fact]
    public void Load_CompositeGrass_TextureSizeValid()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\CompositeGrass");

        Assert.Equal(32 * 32, tileSet.Texture.Width * tileSet.Texture.Height);
    }

    [Fact]
    public void Load_NonContiguous_CountAndLastIdValid()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\NonContiguousCompositeGrass");
        
        Assert.Equal(3, tileSet.TileCount);
        Assert.Equal(3, tileSet.LastId);
    }

    [Fact]
    public void GetSourceArea_CompositeGrass_PackedTilesValid()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\CompositeGrass");

        Assert.Equal(new Rectangle(0, 0, 16, 16), tileSet.GetTileSourceArea(0));
        Assert.Equal(new Rectangle(16, 0, 16, 16), tileSet.GetTileSourceArea(1));
        Assert.Equal(new Rectangle(0, 16, 16, 16), tileSet.GetTileSourceArea(2));
        Assert.Equal(new Rectangle(16, 16, 16, 16), tileSet.GetTileSourceArea(3));
    }

    [Fact]
    public void GetTileAnimationFrames_Grasslands_IsEmpty()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\Grasslands");

        var animationFrames = tileSet.GetTileAnimationFrames(0);

        Assert.Empty(animationFrames);
    }

    [Fact]
    public void GetTileAnimationFrames_AnimatedGrass_IsNotEmpty()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\AnimatedGrass");

        var animationFrames = tileSet.GetTileAnimationFrames(0);

        Assert.NotEmpty(animationFrames);
    }
}
