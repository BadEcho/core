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

namespace BadEcho.Threading;

/// <summary>
/// Provides data for thread exception events, allowing the handler to mark the exception as being handled.
/// </summary>
public sealed class ThreadExceptionEventArgs : EventArgs<Exception>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ThreadExceptionEventArgs"/> class.
    /// </summary>
    /// <param name="exception">The exception that occurred on another thread.</param>
    public ThreadExceptionEventArgs(Exception exception)
        : base(exception)
    { }

    /// <summary>
    /// Gets or sets a value indicating if the exception has been handled.
    /// </summary>
    public bool Handled
    { get; set; }
}