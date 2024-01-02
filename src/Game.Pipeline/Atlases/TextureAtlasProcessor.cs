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

using Microsoft.Xna.Framework.Content.Pipeline;
using BadEcho.Game.Pipeline.Properties;
using BadEcho.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace BadEcho.Game.Pipeline.Atlases;

/// <summary>
/// Provides a processor of texture atlas asset data for the content pipeline.
/// </summary>
[ContentProcessor(DisplayName = "Texture Atlas Processor - Bad Echo")]
public sealed class TextureAtlasProcessor : ContentProcessor<TextureAtlasContent, TextureAtlasContent>
{
    /// <inheritdoc />
    public override TextureAtlasContent Process(TextureAtlasContent input, ContentProcessorContext context)
    {
        Require.NotNull(input, nameof(input));
        Require.NotNull(context, nameof(context));

        context.Log(Strings.ProcessingTextureAtlas.InvariantFormat(input.Identity.SourceFilename));

        ValidateAsset(input.Asset);

        input.AddReference<Texture2DContent>(context, input.Asset.TexturePath, new OpaqueDataDictionary());

        context.Log(Strings.ProcessingFinished.InvariantFormat(input.Identity.SourceFilename));

        return input;
    }

    private static void ValidateAsset(TextureAtlasAsset asset)
    {
        foreach (TextureRegionAsset region in asset.Regions)
        {
            if (region.NineSliceArea.IsEmpty)
                continue;

            Rectangle nineSliceArea = region.NineSliceArea;

            if (nineSliceArea.X <= 0)
                throw new PipelineException(Strings.AtlasRegionNoLeftSlices.InvariantFormat(region.Name));

            if (nineSliceArea.Width <= 0)
                throw new PipelineException(Strings.AtlasRegionNoCenterVerticalSlices.InvariantFormat(region.Name));

            if (region.SourceArea.Width <= nineSliceArea.Width + nineSliceArea.X)
                throw new PipelineException(Strings.AtlasRegionNoRightSlices.InvariantFormat(region.Name));

            if (nineSliceArea.Y <= 0)
                throw new PipelineException(Strings.AtlasRegionNoTopSlices.InvariantFormat(region.Name));

            if (nineSliceArea.Height <= 0)
                throw new PipelineException(Strings.AtlasRegionNoCenterHorizontalSlices.InvariantFormat(region.Name));

            if (region.SourceArea.Height <= nineSliceArea.Height + nineSliceArea.Y)
                throw new PipelineException(Strings.AtlasRegionNoBottomSlices.InvariantFormat(region.Name));
        }
    }
}
