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
/// Specifies a common error code value returned by the <c>GetLastError</c> Win32 function.
/// </summary>
public enum ErrorCode
{
    /// <summary>
    /// No error occurred.
    /// </summary>
    Success = 0,
    /// <summary>
    /// The window handle is invalid.
    /// </summary>
    InvalidWindowHandle = 1400
}
