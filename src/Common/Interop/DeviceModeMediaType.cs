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

namespace BadEcho.Interop;

/// <summary>
/// Specifies the type of media being printed on by a device.
/// </summary>
internal enum DeviceModeMediaType : uint
{
    /// <summary>
    /// The type of media being printed on is unset.
    /// </summary>
    Unset,
    /// <summary>
    /// Plain paper.
    /// </summary>
    Standard,
    /// <summary>
    /// Transparent film.
    /// </summary>
    Transparency,
    /// <summary>
    /// Glossy paper.
    /// </summary>
    Glossy
}
