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

namespace BadEcho.Collections;

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