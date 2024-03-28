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

using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.Effects;

/// <summary>
/// Provides <see cref="SpriteBatch"/> shaders that allow control over the alpha channel of all
/// sprites drawn in a batch.
/// </summary>
public sealed class AlphaSpriteEffect : Effect, IStandardEffect
{
    private EffectParameter _matrixParam;
    private EffectParameter _alphaParam;

    private Viewport _lastViewport;
    private Matrix? _lastTransform;
    private Matrix _viewProjection;

    /// <summary>
    /// Initializes a new instance of the <see cref="AlphaSpriteEffect"/> class.
    /// </summary>
    /// <param name="device">The graphics device used for sprite rendering.</param>
    public AlphaSpriteEffect(GraphicsDevice device)
        : base(device, Shaders.AlphaSpriteEffect)
    {
        CacheEffectParameters();

        Alpha = 1f;
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

    /// <inheritdoc/>
    public Matrix? MatrixTransform
    { get; set; }

    /// <inheritdoc/>
    public float Alpha
    {
        get => _alphaParam.GetValueSingle();
        set => _alphaParam.SetValue(value);
    }

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

        bool parametersChanged =
            viewport.Width != _lastViewport.Width || viewport.Height != _lastViewport.Height || _lastTransform != MatrixTransform;

        if (parametersChanged)
        {
            _viewProjection = (MatrixTransform ?? Matrix.Identity)
                .MultiplyBy2DProjection(viewport.Bounds.Size, GraphicsDevice.UseHalfPixelOffset);

            _lastViewport = viewport;
            _lastTransform = MatrixTransform;
        }

        _matrixParam.SetValue(_viewProjection);
    }
    
    [MemberNotNull(nameof(_matrixParam), nameof(_alphaParam))]
    private void CacheEffectParameters()
    {
        _matrixParam = Parameters[nameof(MatrixTransform)];
        _alphaParam = Parameters[nameof(Alpha)];
    }
}