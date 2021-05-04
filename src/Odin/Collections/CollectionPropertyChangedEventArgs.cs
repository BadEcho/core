//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;

namespace BadEcho.Odin.Collections
{
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
        public CollectionPropertyChangedEventArgs(CollectionPropertyChangedAction action)
            => Action = action;

        /// <summary>
        /// Gets the action that caused the event.
        /// </summary>
        public CollectionPropertyChangedAction Action
        { get; }

        /// <summary>
        /// Gets the name of the collection item's modified property at the source of the event.
        /// </summary>
        public string? PropertyName
        { get; }
    }
}
