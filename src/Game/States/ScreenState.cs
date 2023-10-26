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

using BadEcho.Game.UI;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a game state that hosts a rendering surface for drawing user interface elements.
/// </summary>
public sealed class ScreenState : GameState, IScreenManager
{
    private readonly UserInterface _userInterface;
    private readonly GraphicsDevice _device;
    private readonly Screen _screen;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenState"/> class.
    /// </summary>
    /// <param name="userInterface">The user interface the state will display.</param>
    /// <param name="device">The graphics device that will power the rendering surface.</param>
    public ScreenState(UserInterface userInterface, GraphicsDevice device)
    {
        Require.NotNull(userInterface, nameof(userInterface));
        Require.NotNull(device, nameof(device));
        
        _userInterface = userInterface;
        _screen = new Screen(device);
        _device = device;
        ActivationTime = TimeSpan.FromSeconds(0.5);
    }

    /// <inheritdoc/>
    protected override bool ClipDuringTransitions 
        => false;

    /// <inheritdoc/>
    protected override SpriteSortMode SortMode
        => SpriteSortMode.Immediate;

    /// <inheritdoc/>
    public override void Update(GameUpdateTime time)
    {
        _screen.Update();

        ContentOrigin = _screen.Content.LayoutBounds.Location;

        base.Update(time);
    }

    /// <inheritdoc />
    public void AddScreen(UserInterface userInterface)
        => AddScreen(userInterface, Transitions.None);

    /// <inheritdoc />
    public void AddScreen(UserInterface userInterface, Transitions transitions)
    {
        Manager?.AddState(new ScreenState(userInterface, _device)
                          {
                              StateTransitions = transitions
                          });
    }

    /// <inheritdoc />
    public void RemoveScreen() 
        => Manager?.RemoveState(this);

    /// <inheritdoc />
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
        => _screen.Draw(spriteBatch);

    /// <inheritdoc />
    protected override void LoadContent(ContentManager contentManager) 
        => _userInterface.Attach(_screen, contentManager, this);
}
