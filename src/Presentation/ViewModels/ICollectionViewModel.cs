//-----------------------------------------------------------------------
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

using System.Windows.Threading;

namespace BadEcho.Presentation.ViewModels;

/// <summary>
/// Defines a view model that automates communication between a view and a collection of bound data.
/// </summary>
/// <typeparam name="TModel">
/// The type of data bound to the view model as part of a collection for display on a view.
/// </typeparam>
/// <typeparam name="TChildViewModel">
/// The type of view model that this collection view model generates for its children.
/// </typeparam>
public interface ICollectionViewModel<TModel, TChildViewModel> : IViewModel<TModel>, IAncestorViewModel<TChildViewModel>
    where TChildViewModel : class, IViewModel, IModelProvider<TModel>
{
    /// <summary>
    /// Binds the provided view model to this collection view model, allowing for the child's bound data to be represented
    /// in a view.
    /// </summary>
    /// <param name="viewModel">The <typeparamref name="TChildViewModel"/> object to bind to this collection view model.</param>
    void Bind(TChildViewModel viewModel);

    /// <summary>
    /// Unbinds the child view model from this collection view model, causing its bound data to no longer be represented in a
    /// view.
    /// </summary>
    /// <param name="viewModel">
    /// The <typeparamref name="TChildViewModel"/> object to unbind from this collection view model.
    /// </param>
    /// <returns>True <c>viewModel</c> was unbound; otherwise, false.</returns>
    bool Unbind(TChildViewModel viewModel);

    /// <summary>
    /// Generates a child view model for the provided data.
    /// </summary>
    /// <param name="model">The data to generate a child view model for.</param>
    /// <returns>A <typeparamref name="TChildViewModel"/> object with <c>model</c> bound to it.</returns>
    TChildViewModel CreateChild(TModel model);

    /// <summary>
    /// Updates an existing child view model to use the provided data.
    /// </summary>
    /// <param name="model">The data to update an existing child view model with.</param>
    void UpdateChild(TModel model);

    /// <summary>
    /// Changes the dispatcher currently in use by the view model to the provided one.
    /// </summary>
    /// <param name="dispatcher">The dispatcher that the view model should use to broadcast events.</param>
    /// <remarks>
    /// This method is meant to bring the mechanisms used by the view model to maintain its assigned collection into parity
    /// with the collection views the view model is communicating with. If those collection views are running on a different
    /// dispatcher than the one currently in use by the view model, then problems will typically occur.
    /// </remarks>
    void ChangeDispatcher(Dispatcher dispatcher);

    /// <summary>
    /// Searches for and returns the child view model responsible for representing the provided data within the collection
    /// view model.
    /// </summary>
    /// <typeparam name="TChildViewModelImpl">The specific type of child view model to look for.</typeparam>
    /// <param name="model">The bound data of the child view model to search for.</param>
    /// <returns>
    /// The <typeparamref name="TChildViewModelImpl"/> instance that <c>model</c> is bound to, or null if nothing was found.
    /// </returns>
    TChildViewModelImpl? FindChild<TChildViewModelImpl>(TModel model) where TChildViewModelImpl : TChildViewModel;
}