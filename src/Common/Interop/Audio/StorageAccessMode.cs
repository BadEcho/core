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
/// Specifies the mode to open a property store in.
/// </summary>
internal enum StorageAccessMode
{
    /// <summary>
    /// Read access.
    /// </summary>
    Read,
    /// <summary>
    /// Write access.
    /// </summary>
    Write,
    /// <summary>
    /// Read/write access.
    /// </summary>
    ReadWrite
}
