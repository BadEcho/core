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

using System.Windows;
using System.Windows.Input;

namespace BadEcho.Presentation.Behaviors;

/// <summary>
/// Provides an action that, when executed, will execute a bound <see cref="ICommand"/> instance if possible.
/// </summary>
public sealed class CommandAction : BehaviorAction<DependencyObject>
{
    /// <summary>
    /// Identifies the <see cref="Command"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty CommandProperty
        = DependencyProperty.Register(nameof(Command),
                                      typeof(ICommand),
                                      typeof(CommandAction));

    /// <summary>
    /// Gets or sets the <see cref="ICommand"/> instance that will be executed when this action is executed.
    /// </summary>
    public ICommand? Command
    {
        get => (ICommand?) GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    /// <inheritdoc/>
    public override bool Execute()
    {
        if (Command == null)
            return false;

        if (!Command.CanExecute(null))
            return false;

        Command.Execute(null);

        return true;
    }

    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore() 
        => new CommandAction();
}