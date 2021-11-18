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

using System;
using System.Collections.Generic;
using System.Linq;
using BadEcho.Odin;
using BadEcho.Odin.Extensions;

namespace BadEcho.Fenestra.ViewModels
{
    /// <summary>
    /// Provides a strategy that will individually insert both single view models as well as presorted batches of view models into a
    /// view model's inner collection such that all items are already sorted following the insertion operation.
    /// </summary>
    /// <remarks>
    /// This strategy allows for collection view model sorts and batch insertions without requiring a collection reset notification, which
    /// may be undesirable for certain scenarios. Removals are unaffected by this strategy; simple removal operations are performed.
    /// </remarks>
    /// <typeparam name="TChildViewModel"></typeparam>
    /// <typeparam name="TKey"></typeparam>
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

        private IEnumerable<TChildViewModel> Sort(IEnumerable<TChildViewModel> collection)
        {
            return _descending
                ? collection.OrderByDescending(_keySelector)
                : collection.OrderBy(_keySelector);
        }
    }
}
