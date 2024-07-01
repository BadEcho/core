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
using System.Text.Json;
using BadEcho.Game.Pipeline.SpriteSheets;
using BadEcho.Serialization;
using Xunit;

namespace BadEcho.Game.Tests;

public class SpriteSheetPipelineTests
{
    private readonly SpriteSheetImporter _importer = new();
    private readonly SpriteSheetProcessor _processor = new();
    private readonly TestContentImporterContext _importerContext = new();
    private readonly TestContentProcessorContext _processorContext = new();

    [Fact]
    public void Deserialize_StickMan_ReturnsValid()
    {
        var fileContents = File.ReadAllText(GetAssetPath("StickMan.spritesheet"));

        var asset = JsonSerializer.Deserialize<SpriteSheetAsset>(
            fileContents,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new JsonIntFuncConverter<TimeSpan>(i => TimeSpan.FromMilliseconds(i), c => c.Milliseconds)
                }
            });

        Assert.NotNull(asset);
        Assert.Equal(4, asset.ColumnCount);
        Assert.Equal(1, asset.RowCount);
        Assert.NotEmpty(asset.Animations);
        Assert.Collection(asset.Animations,
                          s =>
                          {
                              Assert.Equal("Idle", s.Name);
                              Assert.Equal(0, s.StartFrame);
                              Assert.Equal(3, s.EndFrame);
                              Assert.Equal(200, s.Duration.Milliseconds);
                          });
    }
    
    [Fact]
    public void ImportProcess_StickMan_ReturnsValid()
    {
        SpriteSheetContent content = _importer.Import(GetAssetPath("StickMan.spritesheet"), _importerContext);

        content = _processor.Process(content, _processorContext);

        Assert.NotNull(content);
        Assert.NotNull(content.Asset);
    }

    private static string GetAssetPath(string assetName, [CallerFilePath] string rootPath = "")
        => $"{Path.GetDirectoryName(rootPath)}\\Content\\Images\\{assetName}";
}
