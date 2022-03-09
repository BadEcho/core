//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Input;
using BadEcho.Presentation.Properties;

namespace BadEcho.Presentation.Commands;

/// <summary>
/// Provides a command that executes a method encapsulated by a delegate.
/// </summary>
public sealed class DelegateCommand : ICommand
{
    private readonly Action<object?> _action;
    private readonly Predicate<object?> _canExecute = _ => true;

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
    /// </summary>
    /// <param name="action">The action to invoke when this command is executed.</param>
    public DelegateCommand(Action<object?> action)
        : this(action, null)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="DelegateCommand"/> class.
    /// </summary>
    /// <param name="action">The action to invoke when this command is executed.</param>
    /// <param name="canExecute">The condition that must be met in order for execution to occur.</param>
    public DelegateCommand(Action<object?> action, Predicate<object?>? canExecute)
    {
        _action = action;
            
        if (canExecute != null)
            _canExecute = canExecute;
    }

    /// <inheritdoc/>
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value;
        remove => CommandManager.RequerySuggested -= value;
    }

    /// <inheritdoc/>
    public bool CanExecute(object? parameter)
        => _canExecute(parameter);

    /// <inheritdoc/>
    public void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
            throw new InvalidOperationException(Strings.CommandCannotExecute);

        _action(parameter);
    }
}