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

using BadEcho.Extensions;
using BadEcho.Game.Pipeline.Properties;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;

namespace BadEcho.Game.Pipeline;

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

        // If no valid configuration was provided for the initial frame's row, we have it default to the first row.
        if (input.Asset.RowInitial <= 0)
            input.Asset.RowInitial = 1;

        input.AddReference<Texture2DContent>(context, input.Asset.TexturePath, new OpaqueDataDictionary());

        context.Log(Strings.ProcessingFinished.InvariantFormat(input.Identity.SourceFilename));

        return input;
    }

    private static void ValidateAsset(SpriteSheetAsset asset)
    {
        if (asset.Rows <= 0)
            throw new InvalidOperationException(Strings.SheetHasNoRows);

        if (asset.Columns <= 0)
            throw new InvalidOperationException(Strings.SheetHasNoColumns);

        if (asset.RowUp > asset.Rows)
            throw new InvalidOperationException(Strings.SheetUpwardRowOutOfRange);

        if (asset.RowDown > asset.Rows)
            throw new InvalidOperationException(Strings.SheetDownwardRowOutOfRange);

        if (asset.RowLeft > asset.Rows)
            throw new InvalidOperationException(Strings.SheetLeftwardRowOutOfRange);

        if (asset.RowRight > asset.Rows)
            throw new InvalidOperationException(Strings.SheetRightwardRowOutOfRange);

        if (asset.RowInitial > asset.Rows)
            throw new InvalidOperationException(Strings.SheetInitialRowOutOfRange);
    }
}
