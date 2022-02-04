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

using System.Windows.Input;

namespace BadEcho.Fenestra;

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