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

using BadEcho.Threading;

namespace BadEcho.Interop;

/// <summary>
/// Provides a representation of a message-only window executor frame.
/// </summary>
internal sealed class MessageOnlyExecutorFrame : IThreadExecutorFrame
{
    private readonly IThreadExecutor _executor;
    private readonly bool _exitUponRequest;
    private bool _shouldContinue;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageOnlyExecutorFrame"/> class.
    /// </summary>
    /// <param name="executor">The executor powering the frame.</param>
    /// <param name="exitUponRequest">
    /// A value indicating if this frame will exit when all frames are requested to exit.
    /// </param>
    public MessageOnlyExecutorFrame(IThreadExecutor executor, bool exitUponRequest)
    {
        _executor = executor;
        _exitUponRequest = exitUponRequest;
        _shouldContinue = true;
    }

    /// <inheritdoc/>
    public bool ShouldContinue
    {
        get
        {
            bool shouldContinue = _shouldContinue;

            if (shouldContinue && _exitUponRequest && _executor.IsShutdownStarted) 
                shouldContinue = false;

            return shouldContinue;
        }
        set
        {
            _shouldContinue = value;
            // An empty message is posted so the message pump will wake up if needed to it can check the state of the frame.
            _executor.BeginInvoke(() => { }, null);
        }
    }
}
