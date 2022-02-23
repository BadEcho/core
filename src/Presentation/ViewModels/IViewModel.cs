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

using System.ComponentModel;

namespace BadEcho.Presentation.ViewModels;

/// <summary>
/// Defines a view abstraction that automates communication between a view and bound data.
/// </summary>
public interface IViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Disconnects the view model from any data being modeled.
    /// </summary>
    void Disconnect();
}