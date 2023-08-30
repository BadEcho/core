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
using BadEcho.Game.Pipeline.Properties;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace BadEcho.Game.Pipeline.Fonts;

/// <summary>
/// Provides an importer of multi-channel signed distance field font asset data for the content pipeline.
/// </summary>
[ContentImporter(".sdfont", DisplayName = "Distance Field Font Importer - Bad Echo", DefaultProcessor = nameof(DistanceFieldFontProcessor))]
public sealed class DistanceFieldFontImporter : ContentImporter<DistanceFieldFontContent>
{
    /// <inheritdoc/>
    public override DistanceFieldFontContent Import(string filename, ContentImporterContext context)
    {
        Require.NotNull(filename, nameof(filename));
        Require.NotNull(context, nameof(context));

        context.Log(Strings.ImportingDistanceFieldFont.InvariantFormat(filename));

        var fileContents = File.ReadAllText(filename);
        var asset = JsonSerializer.Deserialize<DistanceFieldFontAsset?>(fileContents,
                                                                        new JsonSerializerOptions
                                                                        {
                                                                            PropertyNameCaseInsensitive = true
                                                                        });
        if (asset == null)
            throw new ArgumentException(Strings.DistanceFieldFontIsNull.InvariantFormat(filename), nameof(filename));

        context.Log(Strings.ImportingDependency.InvariantFormat(asset.FontPath));

        asset.FontPath
            = Path.Combine(Path.GetDirectoryName(filename) ?? string.Empty, asset.FontPath);

        string name = Path.GetFileNameWithoutExtension(asset.FontPath);

        context.AddDependency(asset.FontPath);
        context.Log(Strings.ImportingFinished.InvariantFormat(filename));

        return new DistanceFieldFontContent(asset) { Identity = new ContentIdentity(filename), Name = name };
    }
}
