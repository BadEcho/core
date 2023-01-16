//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
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