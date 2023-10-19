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

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game;

/// <summary>
/// Provides a <see cref="SpriteBatch"/> that can conduct sprite batch operations using preconfigured parameters.
/// </summary>
public sealed class ConfiguredSpriteBatch
{
    private readonly SpriteBatch _spriteBatch;
    private readonly SpriteSortMode _sortMode;
    private readonly BlendState _blendState;
    private readonly SamplerState _samplerState;
    private readonly DepthStencilState _depthStencilState;
    private readonly RasterizerState _rasterizerState;
    private readonly Matrix? _matrixTransform;

    private Effect? _effect;
    private IStandardEffect? _standardEffect;
    
    /// <summary>
    /// Initializes a new instance of the <see cref="ConfiguredSpriteBatch"/> class.
    /// </summary>
    /// <param name="spriteBatch">The <see cref="SpriteBatch"/> instance to configure.</param>
    /// <param name="sortMode">
    /// The drawing order for sprite and text drawing. This is <see cref="SpriteSortMode.Deferred"/> by default.
    /// </param>
    /// <param name="blendState">State of the blending. If null, <see cref="BlendState.AlphaBlend"/> is used.</param>
    /// <param name="samplerState">State of the sampler. If null, <see cref="SamplerState.LinearClamp"/> is used.</param>
    /// <param name="depthStencilState">
    /// State of the depth-stencil buffer. If null, <see cref="DepthStencilState.None"/> is used.
    /// </param>
    /// <param name="rasterizerState">
    /// State of the rasterization. If null, <see cref="RasterizerState.CullCounterClockwise"/> is used.
    /// </param>
    /// <param name="matrixTransform">
    /// An optional matrix used to transform the sprite geometry. If null, <see cref="Matrix.Identity"/> is used.
    /// </param>
    public ConfiguredSpriteBatch(SpriteBatch spriteBatch,
                                 SpriteSortMode sortMode = SpriteSortMode.Deferred,
                                 BlendState? blendState = null,
                                 SamplerState? samplerState = null,
                                 DepthStencilState? depthStencilState = null,
                                 RasterizerState? rasterizerState = null,
                                 Matrix? matrixTransform = null)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));

        _spriteBatch = spriteBatch;
        _sortMode = sortMode;
        _blendState = blendState ?? BlendState.AlphaBlend;
        _samplerState = samplerState ?? SamplerState.LinearClamp;
        _depthStencilState = depthStencilState ?? DepthStencilState.None;
        _rasterizerState = rasterizerState ?? RasterizerState.CullCounterClockwise;
        _matrixTransform = matrixTransform ?? Matrix.Identity;
    }

    /// <summary>
    /// Gets a value indicating if a batch operation has started.
    /// </summary>
    public bool BatchStarted
    { get; private set; }

    /// <summary>
    /// Gets the graphics device associated with this sprite batch.
    /// </summary>
    public GraphicsDevice GraphicsDevice
        => _spriteBatch.GraphicsDevice;

    /// <summary>
    /// Gets the custom <see cref="IStandardEffect"/> being used to override the default sprite effect, if any.
    /// </summary>
    public IStandardEffect? Effect
        => _standardEffect;

    /// <summary>
    /// Loads a custom <typeparamref name="TEffect"/> to override the default sprite effect.
    /// </summary>
    /// <typeparam name="TEffect">An <see cref="Effect"/> type implementing <see cref="IStandardEffect"/>.</typeparam>
    /// <param name="effect">A custom effect to override the default sprite effect.</param>
    public void LoadEffect<TEffect>(TEffect effect)
        where TEffect : Effect, IStandardEffect
    {
        _effect = effect;
        _standardEffect = effect;
    }

    /// <summary>
    /// Begins a new sprite batch operation by preparing the graphics device for drawing sprites.
    /// </summary>
    /// <remarks>
    /// This must be called before drawing any sprites.
    /// </remarks>
    public void Begin()
    {
        _spriteBatch.Begin(_sortMode,
                           _blendState,
                           _samplerState,
                           _depthStencilState,
                           _rasterizerState,
                           _effect,
                           _matrixTransform);

        BatchStarted = true;
    }

    /// <summary>
    /// Flushes the sprite batch to the screen, ending the batch operation.
    /// </summary>
    public void End()
    {
        _spriteBatch.End();

        BatchStarted = false;
    }

    /// <summary>
    /// Adds a sprite to the batch of sprites to be rendered.
    /// </summary>
    /// <param name="texture">The sprite texture.</param>
    /// <param name="destinationRectangle">A rectangle specifying, in screen coordinates, where the sprite will be drawn.</param>
    /// <param name="color">The color channel modulation to use. Use <see cref="Color.White"/> for full color with no tinting.</param>
    public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
        => _spriteBatch.Draw(texture, destinationRectangle, color);

    /// <summary>
    /// Adds a sprite to the batch of sprites to be rendered.
    /// </summary>
    /// <param name="texture">The sprite texture.</param>
    /// <param name="destinationRectangle">
    /// A rectangle specifying, in screen coordinates, where the sprite will be drawn. If this rectangle is not the
    /// same size <c>sourceRectangle</c> the sprite will be scaled to fit.
    /// </param>
    /// <param name="sourceRectangle">
    /// A rectangle specifying, in texels, which section of the rectangle to draw; otherwise, if null, the entire texture is drawn.
    /// </param>
    /// <param name="color">The color channel modulation to use. Use <see cref="Color.White"/> for full color with no tinting.</param>
    public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
        => _spriteBatch.Draw(texture, destinationRectangle, sourceRectangle, color);

    /// <summary>
    /// Adds a sprite to the batch of sprites to be rendered.
    /// </summary>
    /// <param name="texture">The sprite texture.</param>
    /// <param name="position">The location, in screen coordinates, where the sprite will be drawn.</param>
    /// <param name="sourceRectangle">
    /// A rectangle specifying, in texels, which section of the rectangle to draw; otherwise, if null, the entire texture is drawn.
    /// </param>
    /// <param name="color">The color channel modulation to use. Use <see cref="Color.White"/> for full color with no tinting.</param>
    /// <param name="rotation">The angle, in radians, to rotate the sprite around the origin.</param>
    /// <param name="origin">The origin of the sprite.</param>
    /// <param name="scale">Multiplier by which to scale the sprite width and height uniformly.</param>
    /// <param name="effects">Rotations to apply prior to rendering.</param>
    /// <param name="layerDepth">The sorting depth of the sprite, between 0 (front) and 1 (back).</param>
    public void Draw(Texture2D texture,
                     Vector2 position,
                     Rectangle? sourceRectangle,
                     Color color,
                     float rotation,
                     Vector2 origin,
                     float scale,
                     SpriteEffects effects,
                     float layerDepth)
    {
        _spriteBatch.Draw(texture, position, sourceRectangle, color, rotation, origin, scale, effects, layerDepth);
    }
}
