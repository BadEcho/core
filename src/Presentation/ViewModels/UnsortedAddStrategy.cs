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
/// Provides a strategy that will directly add and/or remove children from a view model's inner collection, with no
/// sorting, either individually or in batches.
/// </summary>
/// <typeparam name="TChildViewModel">The type of view model generated as children of the collection.</typeparam>
public sealed class UnsortedAddStrategy<TChildViewModel> : ICollectionChangeStrategy<TChildViewModel>
    where TChildViewModel : class, IViewModel
{
    /// <inheritdoc/>
    public void Add(IAncestorViewModel<TChildViewModel> collectionViewModel, TChildViewModel viewModel)
    {
        Require.NotNull(collectionViewModel, nameof(collectionViewModel));

        collectionViewModel.Items.Add(viewModel);
    }

    /// <inheritdoc/>
    public void AddRange(IAncestorViewModel<TChildViewModel> collectionViewModel, IEnumerable<TChildViewModel> viewModels)
    {
        Require.NotNull(collectionViewModel, nameof(collectionViewModel));

        collectionViewModel.Items.AddRange(viewModels);
    }

    /// <inheritdoc/>
    public void Remove(IAncestorViewModel<TChildViewModel> collectionViewModel, TChildViewModel viewModel)
    {
        Require.NotNull(collectionViewModel, nameof(collectionViewModel));

        collectionViewModel.Items.Remove(viewModel);
    }

    /// <inheritdoc/>
    public void RemoveRange(IAncestorViewModel<TChildViewModel> collectionViewModel, IEnumerable<TChildViewModel> viewModels)
    {
        Require.NotNull(collectionViewModel, nameof(collectionViewModel));

        collectionViewModel.Items.RemoveRange(viewModels);
    }

    /// <inheritdoc/>
    public void TrimExcess(IAncestorViewModel<TChildViewModel> collectionViewModel, int countExceeded)
    {
        Require.NotNull(collectionViewModel, nameof(collectionViewModel));

        for (int i = 0; i < countExceeded; i++)
        {
            collectionViewModel.Items.RemoveAt(0);
        }
    }
}