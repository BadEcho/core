//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under a
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using System.ComponentModel;
using BadEcho.Extensions;
using BadEcho.Game.Pipeline.Properties;

namespace BadEcho.Game.Pipeline;

/// <summary>
/// Provides a processor of bounded texture asset data for the content pipeline.
/// </summary>
/// <remarks>
/// There is no content importer type for bounded textures; rather, use the normal built-in texture importer, and then
/// designate this type as the processor so that spatial characteristics can be specified via this processor's parameters.
/// </remarks>
[ContentProcessor(DisplayName = "Bounded Texture Processor - Bad Echo")]
public sealed class BoundedTextureProcessor : ContentProcessor<TextureContent, BoundedTextureContent>
{
    /// <summary>
    /// Gets or sets a <see cref="ShapeType"/> value that specifies the texture's spatial shape type.
    /// </summary>
    [DisplayName("Bounds Shape Type")]
    public ShapeType BoundsShapeType
    { get; set; }

    /// <summary>
    /// Gets or sets the width of the texture's spatial shape.
    /// </summary>
    /// <remarks>
    /// The way this parameter is handled depends on the value <see cref="BoundsShapeType"/> is set to:
    /// <list type="bullet">
    /// <item>
    /// <para><see cref="ShapeType.RectangleSource"/>:</para>
    /// <para>
    /// This value is ignored, as the texture's own dimensions are used.
    /// </para>
    /// </item>
    /// <item>
    /// <para><see cref="ShapeType.RectangleCustom"/>:</para>
    /// <para>
    /// This value will be used as the spatial rectangle's width. While values equal 0 are valid, be aware
    /// that it'll prevent any collisions from happening as rectangles are endpoint exclusive.
    /// </para>
    /// </item>
    /// <item>
    /// <para><see cref="ShapeType.CircleCustom"/>:</para>
    /// <para>
    /// This value will be used as the spatial circle's width. It needs to be equal to <see cref="BoundsHeight"/>
    /// as all lines connecting two points on the edge of the circle through the center must have the same length;
    /// therefore, the circle must have the same width and height.
    /// </para>
    /// </item>
    /// </list> 
    /// </remarks>
    [DisplayName("Bounds Size: Width")]
    public float BoundsWidth
    { get; set; }

    /// <summary>
    /// Gets or sets the height of the texture's spatial shape.
    /// </summary>
    /// <remarks>
    /// The way this parameter is handled depends on the value <see cref="BoundsShapeType"/> is set to:
    /// <list type="bullet">
    /// <item>
    /// <para><see cref="ShapeType.RectangleSource"/>:</para>
    /// <para>
    /// This value is ignored, as the texture's own dimensions are used.
    /// </para>
    /// </item>
    /// <item>
    /// <para><see cref="ShapeType.RectangleCustom"/>:</para>
    /// <para>
    /// This value will be used as the spatial rectangle's height. While values equal 0 are valid, be aware
    /// that it'll prevent any collisions from happening as rectangles are endpoint exclusive.
    /// </para>
    /// </item>
    /// <item>
    /// <para><see cref="ShapeType.CircleCustom"/>:</para>
    /// <para>
    /// This value will be used as the spatial circle's height. It needs to be equal to <see cref="BoundsWidth"/>
    /// as all lines connecting two points on the edge of the circle through the center must have the same length;
    /// therefore, the circle must have the same width and height.
    /// </para>
    /// </item>
    /// </list> 
    /// </remarks>
    [DisplayName("Bounds Size: Height")]
    public float BoundsHeight
    { get; set; }

    /// <summary>
    /// Gets or sets the color used when color keying for a texture is enabled.
    /// </summary>
    [DefaultValue(typeof(Color), "255,0,255,255")]
    [DisplayName("Color Key Color")]
    public Color ColorKeyColor 
    { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether color keying of a texture is enabled.
    /// </summary>
    [DefaultValue(true)]
    [DisplayName("Color Key Enabled")]
    public bool ColorKeyEnabled
    { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if a full chain of mipmaps are generated from the source texture.
    /// </summary>
    [DisplayName("Generate Mipmaps")]
    public bool GenerateMipmaps
    { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether alpha premultiply of textures is enabled.
    /// </summary>
    [DefaultValue(true)]
    [DisplayName("Premultiply Alpha")]
    public bool PremultiplyAlpha
    { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether resizing of a texture is enabled.
    /// </summary>
    [DisplayName("Resize to Power of Two")]
    public bool ResizeToPowerOfTwo
    { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="TextureProcessorOutputFormat"/> value that specifies the output's texture format.
    /// </summary>
    [DisplayName("Texture Format")]
    public TextureProcessorOutputFormat TextureFormat
    { get; set; }

    /// <inheritdoc />
    public override BoundedTextureContent Process(TextureContent input, ContentProcessorContext context)
    {
        Require.NotNull(input, nameof(input));
        Require.NotNull(context, nameof(context));

        context.Log(Strings.ProcessingBoundedTexture.InvariantFormat(input.Identity.SourceFilename));

        var processorParameters = new OpaqueDataDictionary
                                  {
                                      { nameof(ColorKeyColor), ColorKeyColor },
                                      { nameof(ColorKeyEnabled), ColorKeyEnabled },
                                      { nameof(GenerateMipmaps), GenerateMipmaps },
                                      { nameof(PremultiplyAlpha), PremultiplyAlpha },
                                      { nameof(ResizeToPowerOfTwo), ResizeToPowerOfTwo },
                                      { nameof(TextureFormat), TextureFormat }
                                  };
        
        var processedInput =
            context.Convert<TextureContent, TextureContent>(input, nameof(TextureProcessor), processorParameters);

        context.Log(Strings.BoundedTextureCharacteristics.InvariantFormat(BoundsShapeType, BoundsWidth, BoundsHeight));

        SizeF boundsSize = CalculateBoundsSize(processedInput);

        context.Log(Strings.ProcessingFinished.InvariantFormat(input.Identity.SourceFilename));
        
        return new BoundedTextureContent(BoundsShapeType, boundsSize, processedInput);
    }

    private SizeF CalculateBoundsSize(TextureContent texture)
    {
        switch (BoundsShapeType)
        {
            case ShapeType.RectangleSource:
                BitmapContent firstMipmapLevel = texture.Faces[0][0];

                return new SizeF(firstMipmapLevel.Width, firstMipmapLevel.Height);

            case ShapeType.RectangleCustom:
                return new SizeF(BoundsWidth, BoundsHeight);

            case ShapeType.CircleCustom:
                if (!BoundsHeight.ApproximatelyEquals(BoundsWidth))
                    throw new PipelineException(Strings.CircleNeedsSameWidthHeight);

                return new SizeF(BoundsWidth, BoundsHeight);
                
            default:
                throw new NotSupportedException(Strings.BoundedTextureUnsupportedShapeType.InvariantFormat(BoundsShapeType));
        }
    }
}
