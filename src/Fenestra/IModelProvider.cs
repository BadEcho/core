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

namespace BadEcho.Fenestra;

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