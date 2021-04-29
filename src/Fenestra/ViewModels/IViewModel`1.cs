//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Fenestra.ViewModels
{
    /// <summary>
    /// Defines a view abstraction that automates communication between a view and bound <typeparamref name="T"/>-typed
    /// data.
    /// </summary>
    /// <typeparam name="T">The type of data bound to the view model for display on a view.</typeparam>
    public interface IViewModel<T> : IViewModel
    {
        /// <summary>
        /// Gets data bound to the view model being actively emphasized for display on a view.
        /// </summary>
        T? ActiveModel { get; }

        /// <summary>
        /// Binds the provided data to the view model, allowing for it to be represented in a view.
        /// </summary>
        /// <param name="model">The data to bind to this view model.</param>brush 
        void Bind(T model);

        /// <summary>
        /// Unbinds data from this view model, causing it to no longer be represented in a view.
        /// </summary>
        /// <param name="model">
        /// The data to unbind from this view model, or <c>null</c> to unbind the current <see cref="ActiveModel"/>.
        /// </param>
        /// <returns>True if <c>model</c> was unbound; otherwise, false.</returns>
        bool Unbind(T? model);

        /// <summary>
        /// Unbinds the currently active model from the view model, causing it to no longer be represented in a view.
        /// </summary>
        /// <returns>True if the current <see cref="ActiveModel"/> was unbound; otherwise, false.</returns>
        bool Unbind();
    }
}