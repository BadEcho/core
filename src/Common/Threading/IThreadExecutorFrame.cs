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

namespace BadEcho.Threading;

/// <summary>
/// Defines a representation of a thread executor frame.
/// </summary>
public interface IThreadExecutorFrame
{
    /// <summary>
    /// Gets or sets a value indicating if this frame will exit when all frames are requested to exit.
    /// </summary>
    bool ExitUponRequest { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if this frame should continue processing.
    /// </summary>
    bool ShouldContinue { get; set; }
}