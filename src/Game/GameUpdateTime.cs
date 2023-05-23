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

namespace BadEcho.Game;

/// <summary>
/// Provides a standard snapshot of game timing configuration and state at the time game logic is being processed.
/// </summary>
public sealed class GameUpdateTime
{
    private readonly Microsoft.Xna.Framework.Game _game;
    private readonly GameTime _time;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameUpdateTime"/> class.
    /// </summary>
    /// <param name="game">The game currently running.</param>
    /// <param name="time">The elapsed game time since the last update.</param>
    public GameUpdateTime(Microsoft.Xna.Framework.Game game, GameTime time)
    {
        Require.NotNull(game, nameof(game));
        Require.NotNull(time, nameof(time));

        _game = game;
        _time = time;
    }

    /// <summary>
    /// The amount of elapsed game time since the last update.
    /// </summary>
    public TimeSpan ElapsedGameTime
        => _time.ElapsedGameTime;
    
    /// <summary>
    /// Gets the amount of game time since the start of the game.
    /// </summary>
    public TimeSpan TotalGameTime
        => _time.TotalGameTime;

    /// <summary>
    /// Gets a value indicating if the game loop is taking longer than its <see cref="TargetElapsedTime"/>.
    /// </summary>
    /// <remarks>
    /// If true, then the game loop can be considered to be running too slowly and should do something to "catch up".
    /// </remarks>
    public bool IsRunningSlowly
        => _time.IsRunningSlowly;

    /// <summary>
    /// Gets the maximum amount of time that Draw calls will be skipped and only Update calls are made when the
    /// game is running slowly.
    /// </summary>
    public TimeSpan MaxElapsedTime
        => _game.MaxElapsedTime;

    /// <summary>
    /// Gets the targeted time between frames when running with a fixed time step.
    /// </summary>
    public TimeSpan TargetElapsedTime
        => _game.TargetElapsedTime;
}
