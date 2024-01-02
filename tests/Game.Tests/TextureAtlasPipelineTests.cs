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

using BadEcho.Game.Pipeline.Atlases;
using System.Runtime.CompilerServices;
using Xunit;

namespace BadEcho.Game.Tests;

public class TextureAtlasPipelineTests
{
    private readonly TextureAtlasImporter _importer = new();
    private readonly TextureAtlasProcessor _processor = new();
    private readonly TestContentImporterContext _importerContext = new();
    private readonly TestContentProcessorContext _processorContext = new();

    [Fact]
    public void ImportProcess_BlackShuttleGrass_HasThreeRegions()
    {
        TextureAtlasContent content = _importer.Import(GetAssetPath("BlackShuttleGrass.atlas"), _importerContext);

        Assert.NotNull(content);
        Assert.NotNull(content.Asset);

        content = _processor.Process(content, _processorContext);

        Assert.Equal(3, content.Asset.Regions.Count);
    }

    private static string GetAssetPath(string assetName, [CallerFilePath] string rootPath = "")
        => $"{Path.GetDirectoryName(rootPath)}\\Content\\Atlases\\{assetName}";
}
