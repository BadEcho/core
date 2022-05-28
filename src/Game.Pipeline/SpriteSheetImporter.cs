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
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Provides an importer of sprite sheet asset data for the content pipeline.
/// </summary>
[ContentImporter(".spritesheet", DisplayName = "Sprite Sheet Importer", DefaultProcessor = nameof(TextureProcessor))]
public sealed class SpriteSheetImporter : ContentImporter<SpriteSheetContent>
{
    /// <inheritdoc/>
    public override SpriteSheetContent Import(string filename, ContentImporterContext context)
    {
        var asset = JsonSerializer.Deserialize<SpriteSheetAsset>(filename);

        if (asset == null)
        {
            throw new ArgumentException(Strings.SpriteSheetNotFound.InvariantFormat(filename),
                                        nameof(filename));
        }

        var textureImporter =new TextureImporter();
        string textureFilename = Path.ChangeExtension(filename, asset.TextureFormat);

        Texture2DContent textureContent = (Texture2DContent) textureImporter.Import(textureFilename, context);

        var spriteSheetContent
            = new SpriteSheetContent(textureContent, asset) { Identity = new ContentIdentity(filename) };

        return spriteSheetContent;
    }
}
