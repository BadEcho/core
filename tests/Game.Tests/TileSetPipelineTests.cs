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

using System.Runtime.CompilerServices;
using BadEcho.Game.Pipeline.Tiles;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Xunit;

namespace BadEcho.Game.Tests;

public class TileSetPipelineTests
{
    private readonly TileSetImporter _importer = new();
    private readonly TileSetProcessor _processor = new();
    private readonly TestContentImporterContext _importerContext = new();
    private readonly TestContentProcessorContext _processorContext = new();

    [Fact]
    public void ImportProcess_Grasslands_ReturnsValid()
    {
        TileSetContent content = _importer.Import(GetAssetPath("Grasslands.tsx"), _importerContext);

        Assert.NotNull(content);
        Assert.NotNull(content.Asset);
        Assert.NotNull(content.Asset.Image);
        Assert.Empty(content.Asset.Tiles);

        content = _processor.Process(content, _processorContext);

        Assert.NotNull(content.Asset.Image);

        ExternalReference<Texture2DContent> imageReference
            = content.GetReference<Texture2DContent>(content.Asset.Image.Source);

        Assert.NotNull(imageReference);
    }

    [Fact]
    public void ImportProcess_CompositeGrass_ReturnsValid()
    {
        TileSetContent content = _importer.Import(GetAssetPath("CompositeGrass.tsx"), _importerContext);

        Assert.NotNull(content);
        Assert.NotNull(content.Asset);
        Assert.Null(content.Asset.Image);
        Assert.NotEmpty(content.Asset.Tiles);

        Assert.Collection(content.Asset.Tiles,
                          t => Assert.NotNull(t.Image),
                          t => Assert.NotNull(t.Image),
                          t => Assert.NotNull(t.Image),
                          t => Assert.NotNull(t.Image));

        content = _processor.Process(content, _processorContext);

        foreach (TileAsset tile in content.Asset.Tiles)
        {
            Assert.NotNull(tile.Image);

            ExternalReference<Texture2DContent> imageReference
                = content.GetReference<Texture2DContent>(tile.Image.Source);

            Assert.NotNull(imageReference);
        }
    }

    private static string GetAssetPath(string assetName, [CallerFilePath] string rootPath = "")
        => $"{Path.GetDirectoryName(rootPath)}\\Content\\Tiles\\{assetName}";
}
