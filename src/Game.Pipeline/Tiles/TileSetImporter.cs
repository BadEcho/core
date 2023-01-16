//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Xml.Linq;
using BadEcho.Extensions;
using BadEcho.Game.Pipeline.Properties;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace BadEcho.Game.Pipeline.Tiles;

/// <summary>
/// Provides an importer of tile set asset data for the content pipeline.
/// </summary>
[ContentImporter(".tsx", DisplayName = "Tile Set Importer - Bad Echo", DefaultProcessor = nameof(TileSetProcessor))]
public sealed class TileSetImporter : ContentImporter<TileSetContent>
{
    /// <inheritdoc />
    public override TileSetContent Import(string filename, ContentImporterContext context)
    {
        Require.NotNull(filename, nameof(filename));
        Require.NotNull(context, nameof(context));

        context.Log(Strings.ImportingTileSet.InvariantFormat(filename));

        XElement assetRoot = XElement.Load(filename);
        var asset = new TileSetAsset(assetRoot);

        if (asset.Image != null)
        {
            context.Log(Strings.ImportingDependency.InvariantFormat(asset.Image.Source));

            asset.Image.Source
                = Path.Combine(Path.GetDirectoryName(filename) ?? string.Empty, asset.Image.Source);

            context.AddDependency(asset.Image.Source);
        }

        context.Log(Strings.ImportingFinished.InvariantFormat(filename));

        return new TileSetContent(asset) { Identity = new ContentIdentity(filename) };
    }
}
