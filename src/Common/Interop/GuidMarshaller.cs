//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices.Marshalling;

namespace BadEcho.Interop;

/// <summary>
/// Provides a marshaller for globally unique identifiers.
/// </summary>
[CustomMarshaller(typeof(Guid), MarshalMode.Default, typeof(GuidMarshaller))]
internal static unsafe class GuidMarshaller
{
    /// <summary>
    /// Converts a managed <see cref="Guid"/> value to its unmanaged counterpart.
    /// </summary>
    /// <param name="managed">A managed value of a globally unique identifier.</param>
    /// <returns>A <see cref="GUID"/> equivalent of <c>managed</c>.</returns>
    public static GUID ConvertToUnmanaged(Guid managed)
    {
        byte[] data = managed.ToByteArray();

        fixed (byte* pData = data)
        {
            return *(GUID*)pData;
        }
    }

    /// <summary>
    /// Converts an unmanaged <see cref="GUID"/> value to its managed counterpart.
    /// </summary>
    /// <param name="unmanaged">An unmanaged value of a globally unique identifier.</param>
    /// <returns>A <see cref="Guid"/> equivalent of <c>unmanaged</c>.</returns>
    public static Guid ConvertToManaged(GUID unmanaged)
        => new(unmanaged.Data1,
               unmanaged.Data2,
               unmanaged.Data3,
               unmanaged.Data4[0],
               unmanaged.Data4[1],
               unmanaged.Data4[2],
               unmanaged.Data4[3],
               unmanaged.Data4[4],
               unmanaged.Data4[5],
               unmanaged.Data4[6],
               unmanaged.Data4[7]);
}
