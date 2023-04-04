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

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Represents a globally unique identifier.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal unsafe struct GUID
{
    /// <summary>
    /// Specifies the first 8 hexadecimal digits of the identifier.
    /// </summary>
    public uint Data1;
    /// <summary>
    /// Specifies the first group of 4 hexadecimal digits.
    /// </summary>
    public ushort Data2;
    /// <summary>
    /// Specifies the second group of 4 hexadecimal digits.
    /// </summary>
    public ushort Data3;
    /// <summary>
    /// Array of 8 bytes, with the first 2 bytes containing the third group of 4 hexadecimal
    /// digits and the remaining 6 bytes containing the final 12 hexadecimal digits. 
    /// </summary>
    public fixed byte Data4[8];
}