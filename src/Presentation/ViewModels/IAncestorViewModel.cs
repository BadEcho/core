//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
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
/// Defines a view model which has the capability of having one or more children view models.
/// </summary>
/// <typeparam name="T">The type of view model that may descend from this view model.</typeparam>
public interface IAncestorViewModel<T> : IViewModel, IParent<T, AtomicObservableCollection<T>>
    where T : class, IViewModel
{ }