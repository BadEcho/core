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
    public void Load_GrasslandsCustomProperties_PropertiesValid()
    {
        TileSet tileSet = _content.Load<TileSet>("Tiles\\GrasslandsCustomProperties");

        Assert.True(tileSet.CustomProperties.Strings.ContainsKey("Something"));
        Assert.Equal("In The Way", tileSet.CustomProperties.Strings["Something"]);
    }
}
