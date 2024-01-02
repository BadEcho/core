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

namespace BadEcho.Presentation;

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