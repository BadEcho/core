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

using System.Text.Json;
using BadEcho.Extensions;
using Microsoft.Xna.Framework.Content.Pipeline;
using BadEcho.Game.Pipeline.Properties;

namespace BadEcho.Game.Pipeline.SpriteSheets;

/// <summary>
/// Provides an importer of sprite sheet asset data for the content pipeline.
/// </summary>
[ContentImporter(".spritesheet", DisplayName = "Sprite Sheet Importer - Bad Echo", DefaultProcessor = nameof(SpriteSheetProcessor))]
public sealed class SpriteSheetImporter : ContentImporter<SpriteSheetContent>
{
    private static readonly JsonSerializerOptions _AssetFileOptions = new()
                                                                      {
                                                                          PropertyNameCaseInsensitive = true
                                                                      };
    /// <inheritdoc/>
    public override SpriteSheetContent Import(string filename, ContentImporterContext context)
    {
        Require.NotNull(filename, nameof(filename));
        Require.NotNull(context, nameof(context));
         
        context.Log(Strings.ImportingSpriteSheet.InvariantFormat(filename));

        var fileContents = File.ReadAllText(filename);
        var asset = JsonSerializer.Deserialize<SpriteSheetAsset?>(fileContents,
                                                                  _AssetFileOptions);
        if (asset == null)
            throw new ArgumentException(Strings.SheetIsNull.InvariantFormat(filename), nameof(filename));

        context.Log(Strings.ImportingDependency.InvariantFormat(asset.TexturePath));

        asset.TexturePath
            = Path.Combine(Path.GetDirectoryName(filename) ?? string.Empty, asset.TexturePath);

        context.AddDependency(asset.TexturePath);

        context.Log(Strings.ImportingFinished.InvariantFormat(filename));

        return new SpriteSheetContent(asset) { Identity = new ContentIdentity(filename) };
    }
}
