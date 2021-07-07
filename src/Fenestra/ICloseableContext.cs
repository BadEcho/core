//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Input;

namespace BadEcho.Fenestra
{
    /// <summary>
    /// Defines a data context or other entity able to influence the state of a Fenestra window.
    /// </summary>
    public interface ICloseableContext
    {
        /// <summary>
        /// Gets or sets a command which acts as a fail-safe for the execution of required logic during the closing operation of
        /// a window.
        /// </summary>
        ICommand? CloseCommand { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the visibility of this context.
        /// </summary>
        bool IsOpen { get; set; }
    }
}
