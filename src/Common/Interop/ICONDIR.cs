//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Represents a "directory" of individual icon images.
/// </summary>
[StructLayout(LayoutKind.Sequential, Pack = 2)]
internal struct ICONDIR
{
    /// <summary>
    /// Whatever this is, it needs to be 0.
    /// </summary>
    public ushort idReserved;
    /// <summary>
    /// And whatever this is, it needs to be 1.
    /// </summary>
    public ushort idType;
    /// <summary>
    /// The number of icon images included.
    /// </summary>
    public ushort idCount;
    /// <summary>
    /// The first image entry.
    /// </summary>
    /// <remarks>
    /// This structure is aligned so that this immediately follows <see cref="idCount"/> in memory. There can be any number
    /// of additional icon entries following this one (the exact number, of course, described by <see cref="idCount"/>).
    /// </remarks>
    public ICONDIRENTRY idEntries;
}
