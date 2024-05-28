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

using BadEcho.Game.States;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a self-contained user interface game state that loads and deploys packaged controls onto a provided
/// <see cref="Screen"/> instance.
/// </summary>
public abstract class ScreenState : GameState
{
    private readonly Screen _screen;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenState"/> class.
    /// </summary>
    /// <param name="device">The graphics device that will power the rendering surface.</param>
    protected ScreenState(GraphicsDevice device)
    {
        Require.NotNull(device, nameof(device));

        Device = device;
        _screen = new Screen(device);
    }

    /// <inheritdoc/>
    protected override bool ClipDuringTransitions
        => false;

    /// <inheritdoc/>
    protected override SpriteSortMode SortMode
        => SpriteSortMode.Immediate;

    /// <summary>
    /// Gets the graphics device powering the rendering surface.
    /// </summary>
    protected GraphicsDevice Device
    { get; }

    /// <inheritdoc/>
    protected sealed override void LoadContent(ContentManager contentManager)
        => _screen.Content = LoadControls(contentManager);
    
    /// <inheritdoc/>
    protected override void UpdateCore(GameUpdateTime time, bool isActive)
    {
        _screen.Update();

        ContentOrigin = _screen.Content.LayoutBounds.Location;
    }

    /// <inheritdoc/>
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
        => _screen.Draw(spriteBatch);

    /// <summary>
    /// Initializes and returns a layout panel containing this user interface's controls.
    /// </summary>
    /// <param name="contentManager">The content manager used to load any dependencies for the controls of this interface.</param>
    /// <returns>An <see cref="IPanel"/> instance containing this user interface's controls.</returns>
    protected abstract IPanel LoadControls(ContentManager contentManager);
}