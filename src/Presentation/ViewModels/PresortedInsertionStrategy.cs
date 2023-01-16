//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensions;

namespace BadEcho.Presentation.ViewModels;

/// <summary>
/// Provides a strategy that will individually insert both single view models as well as presorted batches of view models into a
/// view model's inner collection such that all items are already sorted following the insertion operation.
/// </summary>
/// <remarks>
/// This strategy allows for collection view model sorts and batch insertions without requiring a collection reset notification, which
/// may be undesirable for certain scenarios. Removals are unaffected by this strategy; simple removal operations are performed.
/// </remarks>
/// <typeparam name="TChildViewModel">The type of view model generated as children of the collection.</typeparam>
/// <typeparam name="TKey">The type of key returned by the selector used to sort the children.</typeparam>
public sealed class PresortedInsertionStrategy<TChildViewModel, TKey> : ICollectionChangeStrategy<TChildViewModel>
    where TChildViewModel : class, IViewModel
{
    private readonly Func<TChildViewModel, TKey> _keySelector;
    private readonly bool _descending;

    /// <summary>
    /// Initializes a new instance of the <see cref="PresortedInsertionStrategy{TChildViewModel, TKey}"/> class.
    /// </summary>
    /// <param name="keySelector">A function to extract a key from an element, to use in the sorting operation.</param>
    public PresortedInsertionStrategy(Func<TChildViewModel, TKey> keySelector)
        : this (keySelector, false)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PresortedInsertionStrategy{TChildViewModel, TKey}"/> class.
    /// </summary>
    /// <param name="keySelector">A function to extract a key from an element, to use in the sorting operation.</param>
    /// <param name="descending">
    /// Value indicating if elements should be sorted in a descending order as opposed to an ascending order.
    /// </param>
    public PresortedInsertionStrategy(Func<TChildViewModel, TKey> keySelector, bool descending)
    {
        Require.NotNull(keySelector, nameof(keySelector));

        _keySelector = keySelector;
        _descending = descending;
    }

    /// <inheritdoc/>
    public void Add(IAncestorViewModel<TChildViewModel> collectionViewModel, TChildViewModel viewModel)
    {
        Require.NotNull(collectionViewModel, nameof(collectionViewModel));

        var allChildren
            = collectionViewModel.Children.Concat(viewModel.AsEnumerable());

        List<TChildViewModel> sortedChildren = Sort(allChildren).ToList();
        int index = sortedChildren.IndexOf(viewModel);

        collectionViewModel.Children.Insert(index, viewModel);
    }

    /// <inheritdoc/>
    public void AddRange(IAncestorViewModel<TChildViewModel> collectionViewModel, IEnumerable<TChildViewModel> viewModels)
    {
        Require.NotNull(collectionViewModel, nameof(collectionViewModel));

        viewModels = Sort(viewModels).ToList();

        List<TChildViewModel> originalChildren = collectionViewModel.Children.ToList();
            
        IEnumerable<TChildViewModel> allChildren
            = originalChildren.Concat(viewModels);

        List<TChildViewModel> sortedChildren = Sort(allChildren).ToList();

        var viewModelIndices = viewModels.Select(vm => sortedChildren.IndexOf(vm));
        var viewModelIndexPairs = viewModels.Zip(viewModelIndices);

        foreach (var (viewModel, index) in viewModelIndexPairs)
        {
            collectionViewModel.Children.Insert(index, viewModel);
        }
    }

    /// <inheritdoc/>
    public void Remove(IAncestorViewModel<TChildViewModel> collectionViewModel, TChildViewModel viewModel)
    {
        Require.NotNull(collectionViewModel, nameof(collectionViewModel));

        collectionViewModel.Children.Remove(viewModel);
    }

    /// <inheritdoc/>
    public void RemoveRange(IAncestorViewModel<TChildViewModel> collectionViewModel, IEnumerable<TChildViewModel> viewModels)
    {
        Require.NotNull(collectionViewModel, nameof(collectionViewModel));

        collectionViewModel.Children.RemoveRange(viewModels);
    }

    /// <inheritdoc/>
    public void TrimExcess(IAncestorViewModel<TChildViewModel> collectionViewModel, int countExceeded)
    {
        Require.NotNull(collectionViewModel, nameof(collectionViewModel));

        for (int i = 0; i < countExceeded; i++)
        {
            if (_descending)
                collectionViewModel.Children.RemoveAt(collectionViewModel.Children.Count - 1);
            else
                collectionViewModel.Children.RemoveAt(0);
        }
    }

    private IEnumerable<TChildViewModel> Sort(IEnumerable<TChildViewModel> collection)
    {
        return _descending
            ? collection.OrderByDescending(_keySelector)
            : collection.OrderBy(_keySelector);
    }
}