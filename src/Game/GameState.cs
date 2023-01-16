//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>

using Microsoft.Xna.Framework;

namespace BadEcho.Game;

/// <summary>
/// Provides the current state of a game at a given point in time.
/// </summary>
public sealed class GameState
{
    private readonly Microsoft.Xna.Framework.Game _game;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameState"/> class.
    /// </summary>
    /// <param name="game">The game currently running.</param>
    /// <param name="time">The elapsed time since the last call to the method receiving the game state.</param>
    public GameState(Microsoft.Xna.Framework.Game game, GameTime time)
    {
        Require.NotNull(game, nameof(game));
        Require.NotNull(time, nameof(time));

        _game = game;
        Time = time;
    }

    /// <summary>
    /// Gets the elapsed time since the last call to the method receiving this game state was made.
    /// </summary>
    public GameTime Time
    { get; }

    /// <summary>
    /// Gets the amount of time that the game thread will sleep per loop iteration when the game loses focus.
    /// </summary>
    public TimeSpan InactiveSleepTime
        => _game.InactiveSleepTime;

    /// <summary>
    /// Gets a value indicating if the game is the focused application.
    /// </summary>
    public bool IsActive
        => _game.IsActive;

    /// <summary>
    /// Gets a value indicating if the game is running with a fixed time between frames.
    /// </summary>
    public bool IsFixedTimeStep
        => _game.IsFixedTimeStep;

    /// <summary>
    /// Gets a value indicating if the mouse cursor is visible on the game screen.
    /// </summary>
    public bool IsMouseVisible
        => _game.IsMouseVisible;

    /// <summary>
    /// Gets the startup parameters for the game.
    /// </summary>
    public LaunchParameters LaunchParameters
        => _game.LaunchParameters;

    /// <summary>
    /// Gets the maximum amount of time that Draw calls will be skipped and only Update calls made when the game
    /// is running slowly.
    /// </summary>
    public TimeSpan MaxElapsedTime
        => _game.MaxElapsedTime;

    /// <summary>
    /// Gets a container holding service providers attached to the game.
    /// </summary>
    public GameServiceContainer Services
        => _game.Services;

    /// <summary>
    /// Gets the targeted time between frames when running with a fixed time step.
    /// </summary>
    public TimeSpan TargetElapsedTime
        => _game.TargetElapsedTime;

    /// <summary>
    /// Gets the system window that the game is displayed on.
    /// </summary>
    public GameWindow Window
        => _game.Window;
}
