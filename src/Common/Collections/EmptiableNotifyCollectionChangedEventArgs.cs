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

using System.Collections;
using System.Collections.Specialized;

namespace BadEcho.Collections;

/// <summary>
/// Provides data for the <see cref="INotifyCollectionChanged.CollectionChanged"/> event which uses empty instead of
/// null collections in the data exposed.
/// </summary>
public sealed class EmptiableNotifyCollectionChangedEventArgs : EventArgs
{
    private readonly NotifyCollectionChangedEventArgs _eventArgs;

    /// <summary>
    /// Initializes a new instance of the <see cref="EmptiableNotifyCollectionChangedEventArgs"/> class.
    /// </summary>
    /// <param name="eventArgs">The <see cref="NotifyCollectionChangedEventArgs"/> instance to wrap.</param>
    public EmptiableNotifyCollectionChangedEventArgs(NotifyCollectionChangedEventArgs eventArgs)
    {
        Require.NotNull(eventArgs, nameof(eventArgs));

        _eventArgs = eventArgs;
    }

    /// <inheritdoc cref="NotifyCollectionChangedEventArgs.Action"/>
    public NotifyCollectionChangedAction Action
        => _eventArgs.Action;

    /// <inheritdoc cref="NotifyCollectionChangedEventArgs.NewItems"/>
    public IList NewItems
        => _eventArgs.NewItems ?? Array.Empty<object>();

    /// <inheritdoc cref="NotifyCollectionChangedEventArgs.NewStartingIndex"/>
    public int NewStartingIndex
        => _eventArgs.NewStartingIndex;

    /// <inheritdoc cref="NotifyCollectionChangedEventArgs.OldItems"/>
    public IList OldItems
        => _eventArgs.OldItems ?? Array.Empty<object>();

    /// <inheritdoc cref="NotifyCollectionChangedEventArgs.OldStartingIndex"/>
    public int OldStartingIndex
        => _eventArgs.OldStartingIndex;
}