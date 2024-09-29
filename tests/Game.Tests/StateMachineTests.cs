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
            = _builder.Add(TestState.That).TransitionsTo(TestState.This).WhenTrue(_ => true)
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
            = _builder.Add(TestState.That).TransitionsTo(TestState.This).WhenTrue(_ => true).After(TimeSpan.FromSeconds(3.0))
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
                        .TransitionsTo(TestState.This).WhenTrue(_ => false).After(TimeSpan.FromSeconds(3.0))
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
            = _builder.Add(TestState.Nothing)
                        .Executes(_ => firstCalled = true)
                        .TransitionsTo(TestState.This)
                            .WhenTrue(_ => true)
                      .Add(TestState.This)
                        .Executes(_ => secondCalled = true)
                        .TransitionsTo(TestState.That)
                            .WhenTrue(_ => true)
                      .Add(TestState.That)
                        .Executes(_ => thirdCalled = true)
                        .TransitionsTo(TestState.ThisAndThat)
                            .WhenTrue(_ => true)
                      .Add(TestState.ThisAndThat)
                        .Executes(_ => fourthCalled = true)
                        .TransitionsTo(TestState.Nothing)
                            .WhenTrue(_ => true)
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
                        .TransitionsTo(TestState.This).WhenTrue(_ => false)
                        .TransitionsTo(TestState.ThisAndThat).WhenTrue(_ => true)
                        .TransitionsTo(TestState.Nothing).WhenTrue(_ => false)
                      .Add(TestState.This)
                      .Add(TestState.ThisAndThat)
                      .Add(TestState.Nothing)
                      .Build(TestState.That);

        Assert.Equal(TestState.That, fsm.CurrentState.Identifier);

        fsm.Update(new GameUpdateTime(_game, new GameTime(TimeSpan.Zero, TimeSpan.Zero)));

        Assert.Equal(TestState.ThisAndThat, fsm.CurrentState.Identifier);
    }

    [Fact]
    public void Update_Component_ExecutesComponents()
    {
        bool firstCalled = false;
        bool secondCalled = false;

        StateMachine<TestState> fsm
            = _builder.Add(TestState.That)
                        .Executes(() => new ComponentStub(() => firstCalled = true))
                        .Executes(() => new ComponentStub(() => secondCalled = true))
                      .Build(TestState.That);
        
        var entity = new EntityStub();
        fsm.Update(entity, new GameUpdateTime(_game, new GameTime(TimeSpan.Zero, TimeSpan.Zero)));

        Assert.True(firstCalled);
        Assert.True(secondCalled);
    }

    [Fact]
    public void Update_ThisComponent_TransitionsToThat()
    {
        StateMachine<TestState> fsm
            = _builder.Add(TestState.This)
                          .Executes<DeadComponentStub>()
                          .TransitionsTo(TestState.That)
                            .WhenComponentsDone()
                      .Add(TestState.That)
                      .Build(TestState.This);

        var entity = new EntityStub();

        Assert.Equal(TestState.This, fsm.CurrentState.Identifier);

        fsm.Update(entity, new GameUpdateTime(_game, new GameTime(TimeSpan.Zero, TimeSpan.Zero)));

        Assert.Equal(TestState.That, fsm.CurrentState.Identifier);
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
                                                  .TransitionsTo(TestState.That).WhenTrue(_ => true)
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
                                                           .TransitionsTo(TestState.This).WhenTrue(_ => true)
                                                           .TransitionsTo(TestState.This).WhenTrue(_ => true)
                                                       .Build(TestState.That));
    }

    private enum TestState
    {
        Nothing,
        This,
        That,
        ThisAndThat
    }

    private sealed class ComponentStub : Component
    {
        private readonly Action _action;
        
        public ComponentStub(Action action)
        {
            _action = action;
        }

        public override bool Update(IEntity entity, GameUpdateTime time)
        {
            _action();

            return true;
        }
    }

    private sealed class DeadComponentStub : Component
    {
        public override bool Update(IEntity entity, GameUpdateTime time)
            => false;
    }
}
