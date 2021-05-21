//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Fenestra
{
    /// <summary>
    /// Defines a provider of modeled data for display on a view.
    /// </summary>
    /// <typeparam name="T">The type of data provided for display on a view.</typeparam>
    public interface IModelProvider<out T>
    {
        /// <summary>
        /// Gets the data being actively emphasized for display on a view.
        /// </summary>
        T? ActiveModel { get; }
    }
}
