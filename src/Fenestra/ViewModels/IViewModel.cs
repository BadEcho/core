//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.ComponentModel;

namespace BadEcho.Fenestra.ViewModels
{
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
}
