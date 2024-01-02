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

using BadEcho.Extensions;
using BadEcho.Game.Pipeline.Properties;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace BadEcho.Game.Pipeline.SpriteSheets;

/// <summary>
/// Provides a processor of sprite sheet asset data for the content pipeline.
/// </summary>
[ContentProcessor(DisplayName = "Sprite Sheet Processor - Bad Echo")]
public sealed class SpriteSheetProcessor : ContentProcessor<SpriteSheetContent, SpriteSheetContent>
{
    /// <inheritdoc/>
    public override SpriteSheetContent Process(SpriteSheetContent input, ContentProcessorContext context)
    {   
        Require.NotNull(input, nameof(input));
        Require.NotNull(context, nameof(context));

        context.Log(Strings.ProcessingSpriteSheet.InvariantFormat(input.Identity.SourceFilename));

        ValidateAsset(input.Asset);
        
        input.AddReference<Texture2DContent>(context, input.Asset.TexturePath, new OpaqueDataDictionary());

        context.Log(Strings.ProcessingFinished.InvariantFormat(input.Identity.SourceFilename));

        return input;
    }

    private static void ValidateAsset(SpriteSheetAsset asset)
    {
        if (asset.RowCount <= 0)
            throw new PipelineException(Strings.SheetHasNoRows);

        if (asset.ColumnCount <= 0)
            throw new PipelineException(Strings.SheetHasNoColumns);

        if (asset.RowUp >= asset.RowCount)
            throw new PipelineException(Strings.SheetUpwardRowOutOfRange);

        if (asset.RowDown >= asset.RowCount)
            throw new PipelineException(Strings.SheetDownwardRowOutOfRange);

        if (asset.RowLeft >= asset.RowCount)
            throw new PipelineException(Strings.SheetLeftwardRowOutOfRange);

        if (asset.RowRight >= asset.RowCount)
            throw new PipelineException(Strings.SheetRightwardRowOutOfRange);

        if (asset.RowInitial >= asset.RowCount)
            throw new PipelineException(Strings.SheetInitialRowOutOfRange);
    }
}
