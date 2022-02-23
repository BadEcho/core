﻿//-----------------------------------------------------------------------
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

        collectionViewModel.Children.Add(viewModel);
    }

    /// <inheritdoc/>
    public void AddRange(IAncestorViewModel<TChildViewModel> collectionViewModel, IEnumerable<TChildViewModel> viewModels)
    {
        Require.NotNull(collectionViewModel, nameof(collectionViewModel));

        collectionViewModel.Children.AddRange(viewModels);
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
            collectionViewModel.Children.RemoveAt(0);
        }
    }
}