﻿//-----------------------------------------------------------------------
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

using System.Collections.Generic;

namespace BadEcho.Fenestra.ViewModels
{
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
        /// <param name="collectionViewModel">
        /// A view model which has the capability of having one or more children view models.
        /// </param>
        /// <param name="viewModel">The view model to add as a child.</param>
        void Add(IAncestorViewModel<TChildViewModel> collectionViewModel, TChildViewModel viewModel);

        /// <summary>
        /// Adds a sequence of items as children of a view model's collection.
        /// </summary>
        /// <param name="collectionViewModel">
        /// A view model which has the capability of having one or more children view models.
        /// </param>
        /// <param name="viewModels">The sequence of view models to add as children.</param>
        void AddRange(IAncestorViewModel<TChildViewModel> collectionViewModel, IEnumerable<TChildViewModel> viewModels);

        /// <summary>
        /// Removes a single view model from a view model's collection.
        /// </summary>
        /// <param name="collectionViewModel">
        /// A view model which has the capability of having one or more children view models.
        /// </param>
        /// <param name="viewModel">The view model to remove.</param>
        void Remove(IAncestorViewModel<TChildViewModel> collectionViewModel, TChildViewModel viewModel);

        /// <summary>
        /// Removes a sequence of view models from a view model's collection.
        /// </summary>
        /// <param name="collectionViewModel">
        /// A view model which has the capability of having one or more children view models.
        /// </param>
        /// <param name="viewModels">The view models to remove.</param>
        void RemoveRange(IAncestorViewModel<TChildViewModel> collectionViewModel, IEnumerable<TChildViewModel> viewModels);
    }
}