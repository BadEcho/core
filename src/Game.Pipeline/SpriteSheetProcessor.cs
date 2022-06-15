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

using BadEcho.Game.Pipeline.Properties;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Provides a processor of sprite sheet asset data for the content pipeline.
/// </summary>
[ContentProcessor(DisplayName = "Sprite Sheet Processor - Bad Echo")]
public sealed class SpriteSheetProcessor : TextureProcessor
{
    /// <inheritdoc/>
    public override TextureContent Process(TextureContent input, ContentProcessorContext context)
    {   
        Require.NotNull(input, nameof(input));
        
        if (input is not SpriteSheetContent sheetInput)
            throw new ArgumentException(Strings.NotSpriteSheetContent, nameof(input));

        ValidateAsset(sheetInput.Asset);

        // If no valid configuration was provided for the initial frame's row, we have it default to the first row.
        if (sheetInput.Asset.RowInitial <= 0)
            sheetInput.Asset.RowInitial = 1;

        // Let the texture processor handle all the texture-specific stuff before returning.
        return base.Process(input, context);
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
