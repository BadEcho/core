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
/// Specifies the result of invoking the Windows Shell.
/// </summary>
public enum ShellResult
{
    /// <summary>
    /// The operation was successful.
    /// </summary>
    Success = 0,
    /// <summary>
    /// The specified file was not found.
    /// </summary>
    FileNotFound = 2,
    /// <summary>
    /// The specified path was not found.
    /// </summary>
    PathNotFound,
    /// <summary>
    /// The operating system denied access to the specified file.
    /// </summary>
    AccessDenied = 5,
    /// <summary>
    /// There was not enough memory to complete the operation.
    /// </summary>
    OutOfMemory = 8,
    /// <summary>
    /// The executable file is invalid.
    /// </summary>
    BadFormat = 11,
    /// <summary>
    /// A sharing violation occurred.
    /// </summary>
    SharingViolation = 26,
    /// <summary>
    /// The file name association is incomplete or invalid.
    /// </summary>
    InvalidFileNameAssociation,
    /// <summary>
    /// The DDE transaction could not be completed because the request timed out.
    /// </summary>
    DdeTimeout,
    /// <summary>
    /// The DDE transaction failed.
    /// </summary>
    DdeFail,
    /// <summary>
    /// The DDE transaction could not be completed because other DDE transactions were being processed.
    /// </summary>
    DdeBusy,
    /// <summary>
    /// THer eis no application associated with the given file name extension.
    /// </summary>
    NoAssociatedApplication,
    /// <summary>
    /// The specified DLL was not found.
    /// </summary>
    DllNotFound
}
