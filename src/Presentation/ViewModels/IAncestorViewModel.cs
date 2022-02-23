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

namespace BadEcho.Presentation.ViewModels;

/// <summary>
/// Defines a view model which has the capability of having one or more children view models.
/// </summary>
/// <typeparam name="T">The type of view model that may descend from this view model.</typeparam>
public interface IAncestorViewModel<T> : IViewModel, IParent<T, AtomicObservableCollection<T>>
    where T : class, IViewModel
{ }