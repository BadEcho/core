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
        bool isActive = true;
        
        while (states.Count > 0)
        {   // We iterate through the states, from the top in the z-order to the bottom.
            var state = states.Pop();

            state.Update(updateTime, isActive);

            if (state.HasClosed)
                RemoveState(state); // This is the typical path for a gracefully exiting state's removal from the manager.
            else if (state.TransitionStatus is TransitionStatus.Entered or TransitionStatus.Entering && !state.IsModal)
            {
                // All others below the first state not marked as a modal popup will be considered to not be active.
                isActive = false;
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
            = _states.Where(s => s.TransitionStatus != TransitionStatus.Exited);

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

        // Associate this manager with the state.
        state.Load(this);

        _states.Add(state);
    }

    /// <summary>
    /// Removes the specified game state from the collection of loaded states, immediately disconnecting it from the screen.
    /// </summary>
    /// <param name="state">The game state to unload from this manager.</param>
    public void RemoveState(GameState state)
    {
        Require.NotNull(state, nameof(state));
        
        state.Dispose();

        _states.Remove(state);
    }

    /// <inheritdoc/>
    protected override void LoadContent()
    {
        base.LoadContent();

        _spriteBatch = new SpriteBatch(GraphicsDevice);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing && !_disposed)
        {
            _spriteBatch?.Dispose();
            
            foreach (GameState state in _states)
            {   
                state.Dispose();
            }

            _disposed = true;
        }

        base.Dispose(disposing);
    }
}
