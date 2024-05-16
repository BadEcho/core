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

using BadEcho.Game.Properties;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a game state that acts as a backdrop behind any other active states. 
/// </summary>
/// <remarks>
/// Unlike how other states behave, it will not begin to deactivate if not at the top of the z-order, as it is meant to serve
/// as a background (something it cannot do if it deactivates and becomes hidden).
/// </remarks>
public sealed class BackgroundState : GameState
{
    private readonly string _backgroundAssetPath;

    private Texture2D? _texture;
    private bool _disposed;
      
    /// <summary>
    /// Initializes a new instance of the <see cref="BackgroundState"/> class.
    /// </summary>
    /// <param name="backgroundAssetPath">The relative content path to the asset that will be loaded as the background's texture.</param>
    public BackgroundState(string backgroundAssetPath)
    {
        _backgroundAssetPath = backgroundAssetPath;
        ActivationTime = TimeSpan.FromSeconds(1.0);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Because this state is always meant to be visible in the background, this override prevents deactivation when another state
    /// is added on top of it in the z-order.
    /// </remarks>
    public override void Update(GameUpdateTime time, bool isActive) 
        => base.Update(time, true);

    /// <inheritdoc/>
    protected override void LoadContent(ContentManager contentManager) 
        => _texture = contentManager.Load<Texture2D>(_backgroundAssetPath);

    /// <inheritdoc/>
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
    {
        Require.NotNull(spriteBatch, nameof(spriteBatch));

        Viewport viewport = spriteBatch.GraphicsDevice.Viewport;

        if (_texture == null)
            throw new InvalidOperationException(Strings.GameStateResourcesNotLoaded);

        spriteBatch.Draw(_texture,
                         new Rectangle(0, 0, viewport.Width, viewport.Height),
                         Color.White);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _texture?.Dispose();

            _disposed = true;
        }

        base.Dispose(disposing);
    }
}
