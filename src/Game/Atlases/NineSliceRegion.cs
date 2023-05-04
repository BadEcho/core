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

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace BadEcho.Game.Atlases;

/// <summary>
/// Provides an image-bounding region of a texture atlas, able to be drawn independently from other images
/// found in the atlas and resized using 9-slice scaling.
/// </summary>
public sealed class NineSliceRegion : TextureRegion
{
    private readonly TextureRegion _topLeft,
                                   _topCenter,
                                   _topRight,
                                   _centerLeft,
                                   _center,
                                   _centerRight,
                                   _bottomLeft,
                                   _bottomCenter,
                                   _bottomRight;
    /// <summary>
    /// Initializes a new instance of the <see cref="NineSliceRegion"/> class.
    /// </summary>
    /// <param name="atlasTexture">The texture of the atlas that contains this region.</param>
    /// <param name="sourceArea">
    /// The bounding rectangle of the region in <paramref name="atlasTexture"/> that will be rendered when drawing this.
    /// </param>
    /// <param name="padding">The space around the center slice, occupied by the other slices of the region.</param>
    public NineSliceRegion(Texture2D atlasTexture, Rectangle sourceArea, Thickness padding)
        : base(atlasTexture, sourceArea)
    {
        Padding = padding;

        int centerWidth = SourceArea.Width - Padding.Left - Padding.Right;
        int centerHeight = SourceArea.Height - Padding.Top - Padding.Bottom;
        
        int leftX = SourceArea.X;
        int centerX = leftX + Padding.Left;
        int rightX = centerX + centerWidth;
        int topY = SourceArea.Y;
        int centerY = topY + Padding.Top;
        int bottomY = centerY + centerHeight;

        // Create the sliced regions, column by column. The slice dimensions should have been properly validated during the processing
        // stage of writing to the content pipeline, so we assume all regions are valid (no zero- or negative-spaced regions).
        _topLeft = new TextureRegion(AtlasTexture, new Rectangle(leftX, topY, Padding.Left, Padding.Top));
        _centerLeft = new TextureRegion(AtlasTexture, new Rectangle(leftX, centerY, Padding.Left, centerHeight));
        _bottomLeft = new TextureRegion(AtlasTexture, new Rectangle(leftX, bottomY, Padding.Left, Padding.Bottom));
        _topCenter = new TextureRegion(AtlasTexture, new Rectangle(centerX, topY, centerWidth, Padding.Top));
        _center = new TextureRegion(AtlasTexture, new Rectangle(centerX, centerY, centerWidth, centerHeight));
        _bottomCenter = new TextureRegion(AtlasTexture, new Rectangle(centerX, bottomY, centerWidth, Padding.Bottom));
        _topRight = new TextureRegion(AtlasTexture, new Rectangle(rightX, topY, Padding.Right, Padding.Top));
        _centerRight = new TextureRegion(AtlasTexture, new Rectangle(rightX, centerY, Padding.Right, centerHeight));
        _bottomRight = new TextureRegion(AtlasTexture, new Rectangle(rightX, bottomY, Padding.Right, Padding.Bottom));
    }

    /// <summary>
    /// Gets the space around the center slice, occupied by the other slices of the region.
    /// </summary>
    public Thickness Padding
    { get; }

    /// <inheritdoc/>
    public override void Draw(SpriteBatch spriteBatch, Rectangle targetArea)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));

        Thickness targetPadding = CalculateTargetPadding(targetArea);
        int centerWidth = targetArea.Width - targetPadding.Left - targetPadding.Right;
        int centerHeight = targetArea.Height - targetPadding.Top - targetPadding.Bottom;

        int leftX = targetArea.X;
        int centerX = leftX + targetPadding.Left;
        int rightX = centerX + centerWidth;
        int topY = targetArea.Y;
        int centerY = topY + targetPadding.Top;
        int bottomY = centerY + centerHeight;

        // With 9-slice scaling, different slices are scaled differently. Corner slices will always maintain their height and width.
        _topLeft.Draw(spriteBatch, new Rectangle(leftX, topY, targetPadding.Left, targetPadding.Top));
        _topRight.Draw(spriteBatch, new Rectangle(rightX, topY, targetPadding.Right, targetPadding.Top));
        _bottomLeft.Draw(spriteBatch, new Rectangle(leftX, bottomY, targetPadding.Left, targetPadding.Bottom));
        _bottomRight.Draw(spriteBatch, new Rectangle(rightX, bottomY, targetPadding.Right, targetPadding.Bottom));

        // Vertical slices (which go from top to bottom) only scale vertically.
        _centerLeft.Draw(spriteBatch, new Rectangle(leftX, centerY, targetPadding.Left, centerHeight));
        _centerRight.Draw(spriteBatch, new Rectangle(rightX, centerY, targetPadding.Right, centerHeight));

        // Horizontal slices (which go from left to right) only scale horizontally.
        _topCenter.Draw(spriteBatch, new Rectangle(centerX, topY, centerWidth, targetPadding.Top));
        _bottomCenter.Draw(spriteBatch, new Rectangle(centerX, bottomY, centerWidth, targetPadding.Bottom));

        // Only the center slice will have both of its dimensions scaled.
        _center.Draw(spriteBatch, new Rectangle(centerX, centerY, centerWidth, centerHeight));
    }

    private Thickness CalculateTargetPadding(Rectangle targetArea)
    {
        int left = Padding.Left;
        int right = Padding.Right;
        int top = Padding.Top;
        int bottom = Padding.Bottom;

        // The exterior slices always should maintain either their width, height, or both, depending on their position;
        // however, if the target bounding rectangle is too small to accomodate a particular dimension, then the affected slices
        // will be resized proportionally to the overall region's source size.
        if (left + right > targetArea.Width)
        {
            double sourceTargetRatio = (double)SourceArea.Width / targetArea.Width;

            left = (int)(left * sourceTargetRatio);
            right = (int)(right * sourceTargetRatio);
        }

        if (top + bottom > targetArea.Height)
        {
            double sourceTargetRatio = (double)SourceArea.Height / targetArea.Height;

            top = (int)(top * sourceTargetRatio);
            bottom = (int)(bottom * sourceTargetRatio);
        }

        return new Thickness(left, top, right, bottom);
    }
}
