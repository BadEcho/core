//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Odin.Collections
{
    /// <summary>
    /// Specifies the type of action resulting in an event whose data is represented by a
    /// <see cref="CollectionPropertyChangedEventArgs"/> instance.
    /// </summary>
    public enum CollectionPropertyChangedAction
    {
        /// <summary>
        /// One or more items were added to the collection.
        /// </summary>
        Add,
        /// <summary>
        /// One or more items were removed from the collection.
        /// </summary>
        Remove,
        /// <summary>
        /// One or more items were replaced in the collection.
        /// </summary>
        Replace,
        /// <summary>
        /// One or more items were moved within the collection.
        /// </summary>
        Move,
        /// <summary>
        /// The content of the collection changed dramatically.
        /// </summary>
        Reset,
        /// <summary>
        /// The property value changed for an item in the collection.
        /// </summary>
        ItemProperty
    }
}
