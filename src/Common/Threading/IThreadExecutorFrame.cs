//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
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