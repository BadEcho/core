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

using BadEcho.Game.Pipeline.Fonts;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Xunit;

namespace BadEcho.Game.Tests;

public class DistanceFieldFontPipelineTests
{
    private static int _AssetCount;

    private readonly DistanceFieldFontImporter _importer = new();
    private readonly DistanceFieldFontProcessor _processor = new();
    private readonly TestContentImporterContext _importerContext = new();
    private readonly TestContentProcessorContext _processorContext = new();

    [Fact]
    public void ImportProcess_LatoFont_ReturnsValid()
    {
        DistanceFieldFontContent content = _importer.Import(GetAssetPath("Lato.sdfont"), _importerContext);

        Assert.NotNull(content);
        Assert.NotNull(content.Asset);

        _processorContext.AssetPathFromOutput = "Fonts\\Lato.xnb";
        content = _processor.Process(content, _processorContext);
        _AssetCount++;

        Assert.Equal(4, content.Characteristics.DistanceRange);
        Assert.Equal(592, content.Characteristics.Width);
        Assert.Equal(592, content.Characteristics.Height);
        Assert.Equal(-0.80500000f, content.Characteristics.Ascender);
        Assert.Equal(0.19500000f, content.Characteristics.Descender);
        Assert.Equal(1.2, content.Characteristics.LineHeight, 5);

        Assert.NotEmpty(content.Glyphs);
        Assert.NotEmpty(content.Kernings);
    }

    [Fact]
    public void ImportProcess_LatoFont_ValidTextureAssetName()
    {
        DistanceFieldFontContent content = _importer.Import(GetAssetPath("Lato.sdfont"), _importerContext);
        
        _processorContext.AssetPathFromOutput = "Fonts\\Lato.xnb";
        _processorContext.AssetBuilt += (_, e) => Assert.EndsWith($"Lato-Regular-atlas_{_AssetCount++}", e.Data);
        _processor.Process(content, _processorContext);
    }

    [Fact]
    public void ImportProcess_SecondLatoFont_ValidTextureAssetName()
    {
        DistanceFieldFontContent content = _importer.Import(GetAssetPath("SecondLato.sdfont"), _importerContext);
        
        _processorContext.AssetPathFromOutput = "Fonts\\SecondLato.xnb";
        _processorContext.AssetBuilt += (_, e) => Assert.EndsWith($"Lato-Regular-atlas_{_AssetCount++}", e.Data);
        _processor.Process(content, _processorContext);
    }

    [Fact]
    public void ImportProcess_BothLatoFonts_ValidTextureAssetNames()
    {
        DistanceFieldFontContent content = _importer.Import(GetAssetPath("Lato.sdfont"), _importerContext);

        _processorContext.AssetPathFromOutput = "Fonts\\Lato.xnb";
        _processorContext.AssetBuilt += AssertFirstAssetBuilt;
        _processor.Process(content, _processorContext);
        _processorContext.AssetBuilt -= AssertFirstAssetBuilt;

        content = _importer.Import(GetAssetPath("SecondLato.sdfont"), _importerContext);
        
        _processorContext.AssetPathFromOutput = "Fonts\\SecondLato.xnb";
        _processorContext.AssetBuilt += AssertSecondAssetBuilt;
        _processor.Process(content, _processorContext);
        
        static void AssertFirstAssetBuilt(object? _, EventArgs<string> e)
        {
            Assert.EndsWith($"Lato-Regular-atlas_{_AssetCount++}", e.Data);
        }

        static void AssertSecondAssetBuilt(object? _, EventArgs<string> e)
        {
            Assert.EndsWith($"Lato-Regular-atlas_{_AssetCount++}", e.Data);
        }
    }

    [Fact]
    public void Deserialize_LatoSdfont_ReturnsValid()
    {
        var fileContents = File.ReadAllText(GetAssetPath("Lato.sdfont"));
        
        var asset = JsonSerializer.Deserialize<DistanceFieldFontAsset>(fileContents,
                                                                       new JsonSerializerOptions
                                                                       {
                                                                           PropertyNameCaseInsensitive = true
                                                                       });
        Assert.NotNull(asset);
        Assert.Equal("Lato-Regular.ttf", asset.FontPath);
        Assert.NotNull(asset.CharacterSet);
        Assert.Equal(64, asset.Resolution);
        Assert.Equal(4, asset.Range);
    }

    private static string GetAssetPath(string assetName, [CallerFilePath] string rootPath = "")
        => $"{Path.GetDirectoryName(rootPath)}\\Content\\Fonts\\{assetName}";
}
