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
/// Specifies the role that the system has assigned to an audio endpoint device.
/// </summary>
internal enum Role
{
    /// <summary>
    /// Games, system notification sounds, and voice commands.
    /// </summary>
    Console,
    /// <summary>
    /// Music, movies, narration, and live music recording.
    /// </summary>
    Multimedia,
    /// <summary>
    /// Voice communications.
    /// </summary>
    Communications
}
