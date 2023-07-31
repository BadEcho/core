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
/// Provides a <see cref="SpriteBatch"/> effect that allows control over the alpha channel of all sprites drawn
/// in a batch.
/// </summary>
public sealed class AlphaSpriteEffect : Effect
{
    private EffectParameter _matrixParam;
    private EffectParameter _alphaParam;

    private Viewport _lastViewport;
    private Matrix _projection;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlphaSpriteEffect"/> class.
    /// </summary>
    /// <param name="device">The graphics device used for sprite rendering.</param>
    public AlphaSpriteEffect(GraphicsDevice device)
        : base(device, Properties.Effects.AlphaSpriteEffect)
    {
        CacheEffectParameters();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AlphaSpriteEffect"/> class.
    /// </summary>
    /// <param name="cloneSource">The <see cref="AlphaSpriteEffect"/> instance to clone.</param>
    private AlphaSpriteEffect(AlphaSpriteEffect cloneSource)
        : base(cloneSource)
    {
        CacheEffectParameters();
    }

    /// <summary>
    /// Gets or sets an optional matrix used to transform the sprite geometry.
    /// </summary>
    /// <remarks>
    /// A <see cref="Matrix.Identity"/> value is used if this is null.
    /// </remarks>
    public Matrix? MatrixTransform
    { get; set; }

    /// <summary>
    /// Gets or sets the transparency of all sprites drawn in a batch.
    /// </summary>
    /// <remarks>
    /// This is set to be fully opaque by default.
    /// </remarks>
    public float Alpha
    { get; set; } = 1f;

    /// <summary>
    /// Creates a clone of the current <see cref="AlphaSpriteEffect"/> instance.
    /// </summary>
    /// <returns>A cloned <see cref="Effect"/> instance of this.</returns>
    public override Effect Clone()
        => new AlphaSpriteEffect(this);

    /// <summary>
    /// Lazily computes derived parameter values immediately before applying the effect.
    /// </summary>
    protected override void OnApply()
    {
        Viewport viewport = GraphicsDevice.Viewport;

        if (viewport.Width != _lastViewport.Width || viewport.Height != _lastViewport.Height)
        {
            Matrix.CreateOrthographicOffCenter(0, viewport.Width, viewport.Height, 0, 0, -1, out _projection);

            if (GraphicsDevice.UseHalfPixelOffset)
            {
                _projection.M41 -= 0.5f * _projection.M11;
                _projection.M42 -= 0.5f * _projection.M22;
            }

            _lastViewport = viewport;
        }

        if (MatrixTransform.HasValue)
            _matrixParam.SetValue(MatrixTransform.GetValueOrDefault() * _projection);
        else
            _matrixParam.SetValue(_projection);

        _alphaParam.SetValue(Alpha);
    }
    
    [MemberNotNull(nameof(_matrixParam), nameof(_alphaParam))]
    private void CacheEffectParameters()
    {
        _matrixParam = Parameters[nameof(MatrixTransform)];
        _alphaParam = Parameters[nameof(Alpha)];
    }
}