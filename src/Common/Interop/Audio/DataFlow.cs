// -----------------------------------------------------------------------
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
// -----------------------------------------------------------------------

namespace BadEcho.Interop.Audio;

/// <summary>
/// Specifies the direction in which audio data flows between an audio endpoint device and an application.
/// </summary>
internal enum DataFlow
{
    /// <summary>
    /// Audio rendering stream. Audio data flows from the application to the audio endpoint device,
    /// which renders the stream.
    /// </summary>
    Render,
    /// <summary>
    /// Audio capture stream. Audio data flows from the audio endpoint device that captures the stream,
    /// to the application.
    /// </summary>
    Capture,
    /// <summary>
    /// Audio rendering or capture stream. Audio data can flow either way.
    /// </summary>
    All
}
