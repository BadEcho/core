//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
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
/// Defines a view abstraction that automates communication between a view and bound <typeparamref name="T"/>-typed
/// data.
/// </summary>
/// <typeparam name="T">The type of data bound to the view model for display on a view.</typeparam>
public interface IViewModel<T> : IViewModel, IModelProvider<T>
{
    /// <summary>
    /// Checks if the provided data is bound to this view model.
    /// </summary>
    /// <param name="model">The data to check for whether it is bound to this view model.</param>
    /// <returns>True if <c>model</c> is bound; otherwise, false.</returns>
    bool IsBound(T model);

    /// <summary>
    /// Binds the provided data to the view model, allowing for it to be represented in a view.
    /// </summary>
    /// <param name="model">The data to bind to this view model.</param>brush 
    void Bind(T model);

    /// <summary>
    /// Binds the provided sequence of data to the view model, allowing for them to be represented in a view.
    /// </summary>
    /// <param name="models">The sequence of data to bind to this view model.</param>
    void Bind(IEnumerable<T> models);

    /// <summary>
    /// Unbinds data from this view model, causing it to no longer be represented in a view.
    /// </summary>
    /// <param name="model">
    /// The data to unbind from this view model, or <c>null</c> to unbind the current <see cref="IModelProvider{T}.ActiveModel"/>.
    /// </param>
    /// <returns>True if <c>model</c> was unbound; otherwise, false.</returns>
    bool Unbind(T? model);

    /// <summary>
    /// Unbinds the data found in the provided sequence from this view model, causing them to no longer be represented
    /// in a view.
    /// </summary>
    /// <param name="models">
    /// The sequence of data to unbind from this view model.
    /// </param>
    void Unbind(IEnumerable<T> models);

    /// <summary>
    /// Unbinds the currently active model from the view model, causing it to no longer be represented in a view.
    /// </summary>
    /// <returns>True if the current <see cref="IModelProvider{T}.ActiveModel"/> was unbound; otherwise, false.</returns>
    bool Unbind();
}