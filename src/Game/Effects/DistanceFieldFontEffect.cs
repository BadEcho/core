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

using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Effects;

/// <summary>
/// Provides parameters and the shaders that use them which are needed to render multi-channel signed distance field font text.
/// </summary>
public sealed class DistanceFieldFontEffect : Effect, ITextureEffect
{
    private EffectParameter _worldViewProjectionParam;
    private EffectParameter _atlasSizeParam;
    private EffectParameter _distanceRangeParam;
    private EffectParameter _textureParam;

    /// <summary>
    /// Initializes a new instance of the <see cref="DistanceFieldFontEffect"/> class.
    /// </summary>
    /// <param name="device">The graphics device used for rendering.</param>
    public DistanceFieldFontEffect(GraphicsDevice device) 
        : base(device, Shaders.DistanceFieldFontEffect)
    {
        CacheEffectParameters();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DistanceFieldFontEffect"/> class.
    /// </summary>
    /// <param name="cloneSource">The <see cref="DistanceFieldFontEffect"/> instance to clone.</param>
    private DistanceFieldFontEffect(DistanceFieldFontEffect cloneSource)
        : base(cloneSource)
    {
        CacheEffectParameters();
    }

    /// <summary>
    /// Gets the name of the technique designed to render larger text optimally.
    /// </summary>
    public static string LargeTextTechnique
        => "DistanceFieldFont";

    /// <summary>
    /// Gets the name of the technique designed to render smaller text optimally.
    /// </summary>
    public static string SmallTextTechnique
        => "SmallDistanceFieldFont";

    /// <summary>
    /// Gets the name of the technique designed to render outlined larger text optimally.
    /// </summary>
    public static string LargeStrokedTextTechnique
        => "StrokedDistanceFieldFont";

    /// <summary>
    /// Gets the name of the technique designed to render outlined smaller text optimally.
    /// </summary>
    public static string SmallStrokedTextTechnique
        => "StrokedSmallDistanceFieldFont";

    /// <summary>
    /// Gets or sets the world, view, and projection transformation matrix.
    /// </summary>
    public Matrix WorldViewProjection
    {
        get => _worldViewProjectionParam.GetValueMatrix();
        set => _worldViewProjectionParam.SetValue(value);
    }

    /// <summary>
    /// Gets or sets the width and height of the atlas texture.
    /// </summary>
    public Vector2 AtlasSize
    {
        get => _atlasSizeParam.GetValueVector2();
        set => _atlasSizeParam.SetValue(value);
    }

    /// <summary>
    /// Gets or sets the distance field range in pixels.
    /// </summary>
    public float DistanceRange
    {
        get => _distanceRangeParam.GetValueSingle();
        set => _distanceRangeParam.SetValue(value);
    }

    /// <summary>
    /// Gets or sets the font atlas texture.
    /// </summary>
    public Texture2D Texture
    {
        get => _textureParam.GetValueTexture2D();
        set => _textureParam.SetValue(value);
    }

    /// <summary>
    /// Creates a clone of the current <see cref="DistanceFieldFontEffect"/> instance.
    /// </summary>
    /// <returns>A cloned <see cref="Effect"/> instance of this.</returns>
    public override Effect Clone()
        => new DistanceFieldFontEffect(this);

    [MemberNotNull(nameof(_worldViewProjectionParam),
                   nameof(_atlasSizeParam),
                   nameof(_distanceRangeParam),
                   nameof(_textureParam))]
    private void CacheEffectParameters()
    {
        _worldViewProjectionParam = Parameters[nameof(WorldViewProjection)];
        _atlasSizeParam = Parameters[nameof(AtlasSize)];
        _distanceRangeParam = Parameters[nameof(DistanceRange)];
        _textureParam = Parameters[nameof(Texture)];
    }
}
