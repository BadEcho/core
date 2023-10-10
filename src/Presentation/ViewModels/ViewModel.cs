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

using System.ComponentModel;
using System.Runtime.CompilerServices;
using BadEcho.Extensions;

namespace BadEcho.Presentation.ViewModels;

/// <summary>
/// Provides a base view abstraction that automates communication between a view and bound data.
/// </summary>
public abstract class ViewModel : IViewModel
{
    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc/>
    public abstract void Disconnect();

    /// <summary>
    /// Sets a property's backing field to the provided value, notifying the view of a change in value if one occurred. 
    /// </summary>
    /// <typeparam name="T">The property value's type.</typeparam>
    /// <param name="field">A reference to the backing field for the property.</param>
    /// <param name="value">The value being assigned.</param>
    /// <param name="propertyName">Optional. The name of the property whose value is being assigned.</param>
    /// <returns>True if a change in value has occurred; otherwise, false.</returns>
    protected bool NotifyIfChanged<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (field.Equals<T>(value))
            return false;

        field = value;

        OnPropertyChanged(propertyName);
            
        return true;
    }
        
    /// <summary>
    /// Raises the <see cref="PropertyChanged"/> event, notifying the view of changes to a binding source.
    /// </summary>
    /// <param name="propertyName">Optional. The name of the property whose value changed.</param>
    /// <remarks>If no value is provided for <c>propertyName</c>, the name of the caller to this method will be used.</remarks>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}