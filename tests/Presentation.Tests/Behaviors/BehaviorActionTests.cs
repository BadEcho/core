//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using BadEcho.Presentation.Behaviors;
using Xunit;

namespace BadEcho.Presentation.Tests.Behaviors;

public class BehaviorActionTests
{
    [Fact]
    public void ExecuteActions_Empty_ReturnsTrue()
    {
        var actions = new BehaviorActionCollection<DependencyObject>();

        Assert.True(actions.ExecuteActions());
    }

    [Fact]
    public void ExecuteActions_TrueCondition_ReturnsTrue()
    {
        var command = new DelegateCommand(_ => { });

        var actions = new BehaviorActionCollection<DependencyObject>
                      {
                          new ConditionAction {IsEnabled = true},
                          new CommandAction {Command = command}
                      };

        Assert.True(actions.ExecuteActions());
    }

    [Fact]
    public void ExecuteActions_TrueCondition_AllExecuted()
    {
        bool commandExecuted = false;

        var command = new DelegateCommand(_ => commandExecuted = true);

        var actions = new BehaviorActionCollection<DependencyObject>
                      {
                          new ConditionAction {IsEnabled = true},
                          new CommandAction {Command = command}
                      };

        actions.ExecuteActions();

        Assert.True(commandExecuted);
    }

    [Fact]
    public void ExecuteActions_FalseCondition_ReturnsFalse()
    {
        var command = new DelegateCommand(_ => { });

        var actions = new BehaviorActionCollection<DependencyObject>
                      {
                          new ConditionAction {IsEnabled = false},
                          new CommandAction {Command = command}
                      };

        Assert.False(actions.ExecuteActions());
    }

    [Fact]
    public void ExecuteActions_FalseCondition_NotAllExecuted()
    {
        bool commandExecuted = false;

        var command = new DelegateCommand(_ => commandExecuted = true);

        var actions = new BehaviorActionCollection<DependencyObject>
                      {
                          new ConditionAction {IsEnabled = false},
                          new CommandAction {Command = command}
                      };

        actions.ExecuteActions();

        Assert.False(commandExecuted);
    }
}