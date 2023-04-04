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

namespace BadEcho;

/// <summary>
/// Defines a component which is able to have one or more children.
/// </summary>
/// <typeparam name="TChild">The type of children the parent is able to have.</typeparam>
/// <typeparam name="TCollection">The specific type of <see cref="ICollection{T}"/> containing the children.</typeparam>
public interface IParent<TChild, out TCollection>
    where TCollection : ICollection<TChild>
{
    /// <summary>
    /// Gets a collection of all of the children that exist within this <see cref="IParent{TChild, TCollection}"/> object.
    /// </summary>
    TCollection Children { get; }
}