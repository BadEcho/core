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

namespace BadEcho.Odin.Threading;

/// <summary>
/// Specifies the status of a thread executor operation.
/// </summary>
public enum ThreadExecutorOperationStatus
{
    /// <summary>
    /// The operation has not yet started.
    /// </summary>
    Pending,
    /// <summary>
    /// The operation was canceled.
    /// </summary>
    Canceled,
    /// <summary>
    /// The operation is running.
    /// </summary>
    Running,
    /// <summary>
    /// The operation has finished.
    /// </summary>
    Completed
}
