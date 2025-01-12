//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2025 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Threading;

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
