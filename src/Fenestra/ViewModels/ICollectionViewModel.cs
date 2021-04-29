//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Fenestra.ViewModels
{
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
        where TChildViewModel : class, IViewModel<TModel>
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
        /// Called when the results of a change operation have been committed to this collection view model's children so that it
        /// may prepare its contents for viewing.
        /// </summary>
        void OnCollectionChanged();
    }
}