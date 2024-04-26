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
using Microsoft.Xna.Framework.Graphics;

namespace BadEcho.Game.States;

/// <summary>
/// Provides a game component that manages the active states by performing tasks such as controlling their z-order
/// and visibility on the screen, as well as directing input from the user to the appropriate screen.
/// </summary>
public sealed class StateManager : DrawableGameComponent
{
    private readonly List<GameState> _states = [];

    private SpriteBatch? _spriteBatch;
    private bool _isLoaded;
    private bool _disposed;

    /// <summary>
    /// Initializes a new instance of the <see cref="StateManager"/> class.
    /// </summary>
    /// <param name="game">The game that the component should be attached to.</param>
    public StateManager(Microsoft.Xna.Framework.Game game)
        : base(game)
    { }

    /// <summary>
    /// Gets the states loaded into this manager.
    /// </summary>
    public IReadOnlyCollection<GameState> States
        => _states;

    /// <inheritdoc/>
    public override void Update(GameTime gameTime)
    {
        Require.NotNull(gameTime, nameof(gameTime));

        base.Update(gameTime);

        var states = new Stack<GameState>(_states);
        var updateTime = new GameUpdateTime(Game, gameTime);
        bool isTopmost = true;
        bool allowInput = Game.IsActive;

        while (states.Count > 0)
        {   // We iterate through the states, from the top in the z-order to the bottom.
            var state = states.Pop();

            state.IsTopmost = isTopmost;
            state.Update(updateTime);

            if (state.HasExited)
                RemoveState(state); // This is the typical path for a gracefully exiting state's removal from the manager.
            else if (state.ActivationStatus is ActivationStatus.Activated or ActivationStatus.Activating)
            {
                if (allowInput)
                {
                    state.ProcessInput();

                    allowInput = false;
                }

                // All others below the first state not marked as a modal popup will be transitioned to a deactivated state temporarily.
                if (!state.IsModal)
                    isTopmost = false;
            }
        }
    }

    /// <inheritdoc/>
    public override void Draw(GameTime gameTime)
    {
        Require.NotNull(gameTime, nameof(gameTime));

        base.Draw(gameTime);

        if (_spriteBatch == null)
            throw new InvalidOperationException(Strings.UninitializedGraphicsDevice);

        IEnumerable<GameState> visibleStates 
            = _states.Where(s => s.ActivationStatus != ActivationStatus.Deactivated);

        foreach (var visibleState in visibleStates)
        {
            visibleState.Draw(_spriteBatch);
        }
    }

    /// <summary>
    /// Adds the provided game state to the collection of loaded states, which will prepare it for being drawn to the screen.
    /// </summary>
    /// <param name="state">The game state to load into this manager.</param>
    public void AddState(GameState state)
    {
        Require.NotNull(state, nameof(state));
        state.Manager = this;

        // If the game component has been loaded and a graphics device is available, we'll activate the states.
        if (_isLoaded)
            state.Load(Game);

        _states.Add(state);
    }

    /// <summary>
    /// Removes the specified game state from the collection of loaded states, immediately disconnecting it from the screen.
    /// </summary>
    /// <param name="state">The game state to unload from this manager.</param>
    public void RemoveState(GameState state)
    {
        Require.NotNull(state, nameof(state));
        state.Manager = null;

        // If the screen was added and then removed prior to the component's initialization, there won't be any graphics resources
        // to unload.
        if (_isLoaded)
            state.Unload();

        _states.Remove(state);
    }

    /// <inheritdoc/>
    protected override void LoadContent()
    {
        base.LoadContent();

        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _isLoaded = true;

        // If any states were added prior to our initialization, we make sure they get loaded now.
        foreach (GameState state in _states)
        {
            state.Load(Game);
        }
    }

    /// <inheritdoc/>
    protected override void UnloadContent()
    {
        base.UnloadContent();

        foreach (GameState state in _states)
        {
            state.Unload();
        }
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _spriteBatch?.Dispose();

            _disposed = true;
        }

        base.Dispose(disposing);
    }
}
