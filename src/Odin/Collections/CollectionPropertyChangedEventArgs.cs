//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;

namespace BadEcho.Odin.Collections;

/// <summary>
/// Provides data for a change in either a collection's composition or a property value of one of the items belonging
/// to said collection.
/// </summary>
public sealed class CollectionPropertyChangedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionPropertyChangedEventArgs"/> class.
    /// </summary>
    /// <param name="propertyName">The name of the collection item's modified property at the source of the event.</param>
    public CollectionPropertyChangedEventArgs(string propertyName)
        : this(CollectionPropertyChangedAction.ItemProperty)
    {
        Require.NotNullOrEmpty(propertyName, nameof(propertyName));

        PropertyName = propertyName;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionPropertyChangedEventArgs"/> class.
    /// </summary>
    /// <param name="action">The action that caused the event.</param>
    /// <param name="newItems">The list of new items involved in the change.</param>
    /// <param name="oldItems">The list of items affected by a replacement, removal, or movement of members within a collection.</param>
    public CollectionPropertyChangedEventArgs(CollectionPropertyChangedAction action, IList? newItems, IList? oldItems)
        : this(action)
    {
        if (newItems != null)
            NewItems = newItems;

        if (oldItems != null)
            OldItems = oldItems;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CollectionPropertyChangedEventArgs"/> class.
    /// </summary>
    /// <param name="action">The action that caused the event.</param>
    private CollectionPropertyChangedEventArgs(CollectionPropertyChangedAction action) 
        => Action = action;

    /// <summary>
    /// Gets the action that caused the event.
    /// </summary>
    public CollectionPropertyChangedAction Action
    { get; }

    /// <summary>
    /// Gets the list of new items involved in the change.
    /// </summary>
    public IList NewItems
    { get; } = new ArrayList();

    /// <summary>
    /// Gets the list of items affected by a replacement, removal, or movement of members within a collection.
    /// </summary>
    public IList OldItems
    { get; } = new ArrayList();

    /// <summary>
    /// Gets the name of the collection item's modified property at the source of the event.
    /// </summary>
    public string? PropertyName
    { get; }
}