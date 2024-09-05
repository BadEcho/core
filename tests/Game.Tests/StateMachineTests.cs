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

using BadEcho.Game.AI;
using Microsoft.Xna.Framework;
using Xunit;
using Xunit.Sdk;

namespace BadEcho.Game.Tests;

public class StateMachineTests
{
    private readonly TestGame _game = new();
    private readonly StateMachineBuilder<TestState> _builder = new();

    [Fact]
    public void Build_That_IsInitialState()
    {
        StateMachine<TestState> fsm = _builder.Add(TestState.Nothing)
                                              .Add(TestState.That)
                                              .Add(TestState.This)
                                              .Add(TestState.ThisAndThat)
                                              .Build(TestState.That);

        Assert.Equal(TestState.That, fsm.CurrentState.Identifier);
    }

    [Fact]
    public void Update_ThatOneCondition_TransitionsToThis()
    {
        StateMachine<TestState> fsm
            = _builder.Add(TestState.That).TransitionTo(TestState.This).IfTrue(() => true)
                      .Add(TestState.This)
                      .Build(TestState.That);

        Assert.Equal(TestState.That, fsm.CurrentState.Identifier);

        fsm.Update(new GameUpdateTime(_game, new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(200))));

        Assert.Equal(TestState.This, fsm.CurrentState.Identifier);
    }

    [Fact]
    public void Update_ThatOneTimedCondition_TransitionsToThisAfterTime()
    {
        StateMachine<TestState> fsm
            = _builder.Add(TestState.That).TransitionTo(TestState.This).IfTrue(() => true).After(TimeSpan.FromSeconds(3.0))
                      .Add(TestState.This)
                      .Build(TestState.That);

        Assert.Equal(TestState.That, fsm.CurrentState.Identifier);

        var time = new GameUpdateTime(_game, new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(500)));

        for (int i = 0; i < 5; i++)
        {
            fsm.Update(time);
            Assert.NotEqual(TestState.This, fsm.CurrentState.Identifier);
        }

        fsm.Update(time);

        Assert.Equal(TestState.This, fsm.CurrentState.Identifier);
    }

    [Fact]
    public void Update_ThatOneTimedFalseCondition_NoTransitionAfterTime()
    {
        StateMachine<TestState> fsm 
            = _builder.Add(TestState.That)
                     .TransitionTo(TestState.This).IfTrue(() => false).After(TimeSpan.FromSeconds(3.0))
                     .Add(TestState.This)
                     .Build(TestState.That);

        Assert.Equal(TestState.That, fsm.CurrentState.Identifier);

        var time = new GameUpdateTime(_game, new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(500)));

        for (int i = 0; i < 20; i++)
        {
            fsm.Update(time);
            Assert.NotEqual(TestState.This, fsm.CurrentState.Identifier);
        }

        Assert.Equal(TestState.That, fsm.CurrentState.Identifier);
    }

    [Fact]
    public void Update_FourStatesNoConditions_AllUpdatesCalled()
    {
        bool firstCalled = false, secondCalled = false, thirdCalled = false, fourthCalled = false;

        StateMachine<TestState> fsm
            = _builder.Add(TestState.Nothing).Executes(_ => firstCalled = true).TransitionTo(TestState.This).IfTrue(() => true)
                     .Add(TestState.This).Executes(_ => secondCalled = true).TransitionTo(TestState.That).IfTrue(() => true)
                     .Add(TestState.That).Executes(_ => thirdCalled = true).TransitionTo(TestState.ThisAndThat).IfTrue(() => true)
                     .Add(TestState.ThisAndThat).Executes(_ => fourthCalled = true).TransitionTo(TestState.Nothing).IfTrue(() => true)
                     .Build(TestState.That);

        var time = new GameUpdateTime(_game, new GameTime(TimeSpan.Zero, TimeSpan.FromMilliseconds(500)));

        fsm.Update(time);
        fsm.Update(time);
        fsm.Update(time);
        fsm.Update(time);

        Assert.True(firstCalled);
        Assert.True(secondCalled);
        Assert.True(thirdCalled);
        Assert.True(fourthCalled);
    }

    [Fact]
    public void Update_ThatThreeTargets_TransitionsToThisAndThat()
    {
        StateMachine<TestState> fsm
            = _builder.Add(TestState.That)
                      .TransitionTo(TestState.This).IfTrue(() => false)
                      .TransitionTo(TestState.ThisAndThat).IfTrue(() => true)
                      .TransitionTo(TestState.Nothing).IfTrue(() => false)
                      .Add(TestState.This)
                      .Add(TestState.ThisAndThat)
                      .Add(TestState.Nothing)
                      .Build(TestState.That);

        Assert.Equal(TestState.That, fsm.CurrentState.Identifier);

        fsm.Update(new GameUpdateTime(_game, new GameTime(TimeSpan.Zero, TimeSpan.Zero)));

        Assert.Equal(TestState.ThisAndThat, fsm.CurrentState.Identifier);
    }

    [Fact]
    public void Init_InitialStateNotRegistered_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _builder.Add(TestState.That).Build(TestState.This));
    }

    [Fact]
    public void Init_StateNoIdentifier_ThrowsException()
    {
        Assert.Throws<InvalidOperationException>(() => new State<string>(new StateModel<string>()));
    }

    [Fact]
    public void Init_StateTransitionNoTarget_ThrowsExceptions()
    {
        var model = new StateModel<string> { Identifier = "hello" };
        
        model.Transitions.Add(new TransitionModel<string>());

        Assert.Throws<InvalidOperationException>(() => new State<string>(model));
    }

    [Fact]
    public void Update_ThisToThat_EnterExitMethodsCalled()
    {
        bool enterCalled = false, exitCalled = false;

        StateMachine<TestState> fsm = _builder.Add(TestState.This)
                                              .OnEnter(_ => enterCalled = true)
                                              .OnExit(_ => exitCalled = true)
                                              .TransitionTo(TestState.That).IfTrue(() => true)
                                              .Add(TestState.That)
                                              .Build(TestState.This);

        fsm.Update(new GameUpdateTime(_game, new GameTime(TimeSpan.Zero, TimeSpan.Zero)));

        Assert.True(enterCalled);
        Assert.True(exitCalled);
    }

    [Fact]
    public void Add_DuplicateStates_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _builder.Add(TestState.That).Add(TestState.That).Build(TestState.That));
    }

    [Fact]
    public void Add_DuplicateTransitions_ThrowsException()
    {
        Assert.Throws<ArgumentException>(() => _builder.Add(TestState.That)
                                                       .TransitionTo(TestState.This).IfTrue(() => true)
                                                       .TransitionTo(TestState.This).IfTrue(() => true)
                                                       .Build(TestState.That));
    }

    private enum TestState
    {
        Nothing,
        This,
        That,
        ThisAndThat
    }
}
