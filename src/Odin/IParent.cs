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

namespace BadEcho.Odin;

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