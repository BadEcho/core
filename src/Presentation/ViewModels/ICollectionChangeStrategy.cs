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

namespace BadEcho.Presentation.ViewModels;

/// <summary>
/// Defines the strategy behind the way a view model's children are added to and removed from the view model's inner collection.
/// </summary>
/// <typeparam name="TChildViewModel">The type of view model generated as children of the collection.</typeparam>
public interface ICollectionChangeStrategy<TChildViewModel>
    where TChildViewModel : class, IViewModel
{
    /// <summary>
    /// Adds a single view model as a child of a view model's collection.
    /// </summary>
    /// <param name="collectionViewModel">A collection view model to add a single view model to.</param>
    /// <param name="viewModel">The view model to add as a child.</param>
    void Add(IAncestorViewModel<TChildViewModel> collectionViewModel, TChildViewModel viewModel);

    /// <summary>
    /// Adds a sequence of view models as children of a view model's collection.
    /// </summary>
    /// <param name="collectionViewModel">A collection view model to add a sequence of view models to.</param>
    /// <param name="viewModels">The sequence of view models to add as children.</param>
    void AddRange(IAncestorViewModel<TChildViewModel> collectionViewModel, IEnumerable<TChildViewModel> viewModels);

    /// <summary>
    /// Removes a single view model from a view model's collection.
    /// </summary>
    /// <param name="collectionViewModel">A collection view model to remove a single view model from.</param>
    /// <param name="viewModel">The view model to remove.</param>
    void Remove(IAncestorViewModel<TChildViewModel> collectionViewModel, TChildViewModel viewModel);

    /// <summary>
    /// Removes a sequence of view models from a view model's collection.
    /// </summary>
    /// <param name="collectionViewModel">A collection view model to remove a sequence of view models from.</param>
    /// <param name="viewModels">The view models to remove.</param>
    void RemoveRange(IAncestorViewModel<TChildViewModel> collectionViewModel, IEnumerable<TChildViewModel> viewModels);

    /// <summary>
    /// Removes all items exceeding the allowed capacity of the view model's collection.
    /// </summary>
    /// <param name="collectionViewModel">A collection view model to trim.</param>
    /// <param name="countExceeded">The number of child view models causing the overcapacity.</param>
    void TrimExcess(IAncestorViewModel<TChildViewModel> collectionViewModel, int countExceeded);
}