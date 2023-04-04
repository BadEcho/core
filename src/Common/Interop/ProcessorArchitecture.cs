//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

namespace BadEcho.Interop;

/// <summary>
/// Specifies the processor architecture of the installed operating system.
/// </summary>
public enum ProcessorArchitecture
{
    /// <summary>
    /// Intel x86 processor architecture.
    /// </summary>
    Intel = 0,
    /// <summary>
    /// ARM 32-bit processor architecture.
    /// </summary>
    Arm = 5,
    /// <summary>
    /// IA-64 processor architecture.
    /// </summary>
    Itanium = 6,
    /// <summary>
    /// AMD x86-64 processor architecture.
    /// </summary>
    Amd64 = 9,
    /// <summary>
    /// ARM 64-bit processor architecture.
    /// </summary>
    Arm64 = 12,
    /// <summary>
    /// Unknown processor architecture.
    /// </summary>
    Unknown = 0xFFFF
}
