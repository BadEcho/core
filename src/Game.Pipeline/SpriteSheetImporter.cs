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

using System.Text.Json;
using BadEcho.Extensions;
using Microsoft.Xna.Framework.Content.Pipeline;
using BadEcho.Game.Pipeline.Properties;

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Provides an importer of sprite sheet asset data for the content pipeline.
/// </summary>
[ContentImporter(".spritesheet", DisplayName = "Sprite Sheet Importer - Bad Echo", DefaultProcessor = nameof(SpriteSheetProcessor))]
public sealed class SpriteSheetImporter : ContentImporter<SpriteSheetContent>
{
    /// <inheritdoc/>
    public override SpriteSheetContent Import(string filename, ContentImporterContext context)
    {
        Require.NotNull(context, nameof(context));

        var fileContents = File.ReadAllText(filename); 
        var asset = JsonSerializer.Deserialize<SpriteSheetAsset?>(fileContents,
                                                                 new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (asset == null)
        {
            throw new ArgumentException(Strings.SheetIsNull.InvariantFormat(filename),
                                        nameof(filename));
        }

        var spriteSheetContent
            = new SpriteSheetContent(asset) { Identity = new ContentIdentity(filename) };

        context.AddDependency(asset.TexturePath);

        return spriteSheetContent;
    }
}
