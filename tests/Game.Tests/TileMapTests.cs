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

using BadEcho.Game.Tiles;
using Xunit;

namespace BadEcho.Game.Tests;

public class TileMapTests
{
    public TileMapTests()
    {
    }

    [Fact]
    public void Load_GrassFourTiles_NotNull()
        => ContentTestRunner.Run(g =>
                                 {
                                     TileMap map = g.Content.Load<TileMap>("Tiles\\GrassFourTiles");

                                     Assert.NotNull(map);
                                 });
    [Fact]
    public void Load_GrassFourTiles_HasTileLayer()
        => ContentTestRunner.Run(g =>
                                 {
                                     TileMap map = g.Content.Load<TileMap>("Tiles\\GrassFourTiles");

                                     var tileLayer = map.Layers.OfType<TileLayer>().FirstOrDefault();

                                     Assert.NotNull(tileLayer);
                                 });
}
