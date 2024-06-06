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

using System.Diagnostics;
using BadEcho.Game.States;
using Microsoft.Xna.Framework;
using Xunit;

namespace BadEcho.Game.Tests;

/// <suppressions>
/// ReSharper disable AccessToDisposedClosure
/// </suppressions>
public class StateTests
{
    private const double DRIFT_TOLERANCE = 125;

    [Fact]
    public void AddState_Default_Entered()
    {
        var game = new TestGame();
        var stateManager = new StateManager(game);

        TestGameState? gameState = null;

        game.Components.Add(stateManager);
        game.Initialized += (_, _) =>
        {
            gameState = new TestGameState(game);
            stateManager.AddState(gameState);
        };

        game.ExitCondition = () => gameState?.TransitionStatus == TransitionStatus.Entered;
        game.Run();
        game.Dispose();

        Assert.Contains(gameState, stateManager.States);
        Assert.True(gameState?.TransitionStatus == TransitionStatus.Entered);
    }

    [Theory]
    [InlineData(2.0)]
    [InlineData(1.5)]
    [InlineData(0.5)]
    [InlineData(4)]
    public void AddState_ConfiguredTransitionTime_TransitionsInTime(double transitionTimeSeconds)
    {
        TimeSpan transitionTime = TimeSpan.FromSeconds(transitionTimeSeconds);
        var stopwatch = new Stopwatch();
        var game = new TestGame();
        var stateManager = new StateManager(game);

        TestGameState? gameState = null;

        game.Components.Add(stateManager);
        game.Initialized += (_, _) =>
        {
            gameState = new TestGameState(game)
                        {
                            TransitionTime = transitionTime,
                            StateTransitions = Transitions.Fade | Transitions.MoveLeft | Transitions.Rotate | Transitions.Zoom,
                            PowerCurve = 4
                            
                        };
            stopwatch.Start();
            stateManager.AddState(gameState);
        };

        game.Exiting += (_, _) => stopwatch.Stop();

        game.ExitCondition = () => gameState?.TransitionStatus == TransitionStatus.Entered;
        game.Run();
        game.Dispose();

        double drift = Math.Abs(stopwatch.Elapsed.Subtract(transitionTime).TotalMilliseconds);

        Assert.True(drift < DRIFT_TOLERANCE, $"Expected Transition Time: {transitionTime} Actual Transition Time: {stopwatch.Elapsed}");
    }

    [Fact]
    public void AddState_Background_StaysEntered()
    {
        var game = new TestGame();
        var stateManager = new StateManager(game);

        TestGameState? gameState = null;
        BackgroundState? backgroundState = null;

        game.Components.Add(stateManager);
        game.Initialized += (_, _) =>
        {
            backgroundState = new BackgroundState(game, "Images\\Circle")
                              {
                                  StateTransitions = Transitions.MoveDown | Transitions.Zoom
                              };
            stateManager.AddState(backgroundState);
            gameState = new TestGameState(game) {TransitionTime = TimeSpan.FromSeconds(2), StateTransitions = Transitions.MoveUp};
            stateManager.AddState(gameState);
        };

        game.ExitCondition = () => gameState?.TransitionStatus == TransitionStatus.Entered;
        game.Run();
        game.Dispose();

        Assert.True(backgroundState?.TransitionStatus == TransitionStatus.Entered);
    }

    [Fact]
    public void AddState_TwoDirections_NoMovement()
    {
        var game = new TestGame();
        var stateManager = new StateManager(game);

        BackgroundState? gameState = null;

        game.Components.Add(stateManager);
        game.Initialized += (_, _) =>
        {
            gameState = new BackgroundState(game, "Images\\Circle")
                              {
                                  StateTransitions = Transitions.MoveDown | Transitions.MoveRight
                              };
            stateManager.AddState(gameState);
        };

        game.ExitCondition = () => gameState?.TransitionStatus == TransitionStatus.Entered;
        game.Run();
        game.Dispose();

        Assert.True(gameState?.TransitionStatus == TransitionStatus.Entered);
    }

    [Fact]
    public void AddState_TwoStates_FirstExited()
    {
        var game = new TestGame();
        var stateManager = new StateManager(game);

        TestGameState? firstState = null;
        TestGameState? secondState = null;

        game.Components.Add(stateManager);
        game.Initialized += (_, _) =>
        {
            firstState = new TestGameState(game);
            firstState.Entered += (_, _) =>
            {
                secondState = new TestGameState(game);
                stateManager.AddState(secondState);
            };
            stateManager.AddState(firstState);
        };

        game.ExitCondition = () =>
            secondState?.TransitionStatus == TransitionStatus.Entered 
            && firstState?.TransitionStatus == TransitionStatus.Exited;

        game.Run();
        game.Dispose();

        Assert.True(firstState?.TransitionStatus == TransitionStatus.Exited);
        Assert.True(secondState?.TransitionStatus == TransitionStatus.Entered);
    }

    [Fact]
    public void AddState_Screen_Entered()
    {
        var game = new TestGame();
        var stateManager = new StateManager(game);

        TestScreenState? gameState = null;

        game.Components.Add(stateManager);
        game.Initialized += (_, _) =>
        {
            gameState = new TestScreenState(game)
                        {
                            TransitionTime = TimeSpan.FromSeconds(2), StateTransitions = Transitions.MoveRight
                        };
            
            stateManager.AddState(gameState);
        };

        game.ExitCondition = () => gameState?.TransitionStatus == TransitionStatus.Entered;
        game.Run();
        game.Dispose();

        Assert.True(gameState?.TransitionStatus == TransitionStatus.Entered);
    }

    [Fact]
    public void Close_Default_Exited()
    {
        var game = new TestGame();
        var stateManager = new StateManager(game);

        TestGameState? gameState = null;
        bool entered = false;

        game.Components.Add(stateManager);
        game.Initialized += (_, _) =>
        {
            gameState = new TestGameState(game);

            gameState.Entered += (_, _) =>
            {
                entered = true;
                gameState.Close();
            };

            stateManager.AddState(gameState);
        };

        game.ExitCondition = () => entered && gameState?.TransitionStatus == TransitionStatus.Exited;
        game.Run();
        game.Dispose();

        Assert.True(gameState?.TransitionStatus == TransitionStatus.Exited);
    }

    [Fact]
    public void Draw_Uninitialized_ThrowsException()
    {
        var game = new TestGame();
        var stateManager = new StateManager(game);

        Assert.Throws<InvalidOperationException>(() => stateManager.Draw(new GameTime()));
    }

    [Fact]
    public void Dispose_Twice_NoException()
    {
        using var game = new TestGame();
        var gameState = new TestGameState(game);

        gameState.Dispose();
        gameState.Dispose();
    }
}
