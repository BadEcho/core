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
        
        input.AddReference<Texture2DContent>(context, input.Asset.TexturePath, []);

        context.Log(Strings.ProcessingFinished.InvariantFormat(input.Identity.SourceFilename));

        return input;
    }

    private static void ValidateAsset(SpriteSheetAsset asset)
    {
        if (asset.RowCount <= 0)
            throw new PipelineException(Strings.SheetHasNoRows);

        if (asset.ColumnCount <= 0)
            throw new PipelineException(Strings.SheetHasNoColumns);

        int finalFrame = 0;
        HashSet<string> animationNames = [];

        foreach (SpriteAnimationAsset animation in asset.Animations)
        {
            if (animation.StartFrame > animation.EndFrame)
                throw new PipelineException(Strings.SheetInvalidAnimationSequence);

            if (animation.EndFrame > finalFrame)
                finalFrame = animation.EndFrame;

            if (!animationNames.Add(animation.Name))
                throw new PipelineException(Strings.SheetNonUniqueAnimationNames);
        }

        if (asset.RowCount * asset.ColumnCount < finalFrame + 1)
            throw new PipelineException(Strings.SheetTooManyFrames);

        if (asset.InitialFrame > finalFrame)
            throw new PipelineException(Strings.SheetInitialFrameOutOfRange);
    }
}
