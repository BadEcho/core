//-----------------------------------------------------------------------
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
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Represents an individual icon image found in an <see cref="ICONDIR"/>.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
internal struct ICONDIRENTRY
{
    /// <summary>
    /// The width of the image. A width of 256 is represented by the value 0.
    /// </summary>
    public byte bWidth;
    /// <summary>
    /// The height of the image. A height of 256 is represented by the value 0.
    /// </summary>
    public byte bHeight;
    /// <summary>
    /// While this is supposed to represent the number of colors in the image, often this gets left at 0 with the 
    /// assumption that it'll be corrected by relevant Windows APIs (something that they do attempt, with varying success).
    /// </summary>
    public byte bColorCount;
    /// <summary>
    /// Must be set to 1, officially at least. In practice, this seems to almost always be 0.
    /// </summary>
    public byte bReserved;
    /// <summary>
    /// The number of color planes.
    /// </summary>
    public ushort wPlanes;
    /// <summary>
    /// The number of bits per pixel.
    /// </summary>
    public ushort wBitCount;
    /// <summary>
    /// The size in bytes of the actual image data.
    /// </summary>
    public uint dwBytesInRes;
    /// <summary>
    /// The location of the image data, relative to the start of the <see cref="ICONDIR"/>.
    /// </summary>
    public uint dwImageOffset;
}