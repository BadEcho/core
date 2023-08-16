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

using Microsoft.Xna.Framework.Content.Pipeline;
using BadEcho.Game.Pipeline.Properties;
using BadEcho.Extensions;
using System.Text.Json;

namespace BadEcho.Game.Pipeline.Atlases;

/// <summary>
/// Provides an importer of texture atlas asset data for the content pipeline.
/// </summary>
[ContentImporter(".atlas", DisplayName = "Texture Atlas Importer - Bad Echo", DefaultProcessor = nameof(TextureAtlasProcessor))]
public sealed class TextureAtlasImporter : ContentImporter<TextureAtlasContent>
{
    /// <inheritdoc />
    public override TextureAtlasContent Import(string filename, ContentImporterContext context)
    {
        Require.NotNull(filename, nameof(filename));
        Require.NotNull(context, nameof(context));

        context.Log(Strings.ImportingTextureAtlas.InvariantFormat(filename));

        var fileContents = File.ReadAllBytes(filename);
        var asset = JsonSerializer.Deserialize<TextureAtlasAsset?>(fileContents,
                                                                   new JsonSerializerOptions
                                                                   {
                                                                       PropertyNameCaseInsensitive = true,
                                                                       IncludeFields = true
                                                                   });
        if (asset == null)
            throw new ArgumentException(Strings.AtlasIsNull.InvariantFormat(filename), nameof(filename));

        context.Log(Strings.ImportingDependency.InvariantFormat(asset.TexturePath));

        asset.TexturePath
            = Path.Combine(Path.GetDirectoryName(filename) ?? string.Empty, asset.TexturePath);

        context.AddDependency(asset.TexturePath);

        context.Log(Strings.ImportingFinished.InvariantFormat(filename));

        return new TextureAtlasContent(asset) { Identity = new ContentIdentity(filename) };
    }
}
