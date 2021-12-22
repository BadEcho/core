//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Odin.Interop;

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
