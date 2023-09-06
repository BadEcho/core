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

using BadEcho.Game.Pipeline.Fonts;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Xunit;

namespace BadEcho.Game.Tests;

public class DistanceFieldFontPipelineTests
{
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

        content = _processor.Process(content, _processorContext);

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
