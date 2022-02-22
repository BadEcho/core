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

using BadEcho.Threading;

namespace BadEcho.Interop;

/// <summary>
/// Provides a representation of a message-only window executor frame.
/// </summary>
internal sealed class MessageOnlyExecutorFrame : IThreadExecutorFrame
{
    private readonly IThreadExecutor _executor;
    private bool _shouldContinue;

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageOnlyExecutorFrame"/> class.
    /// </summary>
    /// <param name="executor">The executor powering the frame.</param>
    public MessageOnlyExecutorFrame(IThreadExecutor executor)
    {
        _shouldContinue = true;
        _executor = executor;
    }

    /// <inheritdoc/>
    public bool ExitUponRequest 
    { get; set; }

    /// <inheritdoc/>
    public bool ShouldContinue
    {
        get
        {
            bool shouldContinue = _shouldContinue;

            if (shouldContinue && ExitUponRequest && _executor.IsShutdownStarted) 
                shouldContinue = false;

            return shouldContinue;
        }
        set
        {
            _shouldContinue = value;
            // An empty message is posted so the message pump will wake up if needed to it can check the state of the frame.
            _executor.BeginInvoke(() => { }, false, null);
        }
    }
}
