// -----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2026 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
// -----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace BadEcho.Interop;

/// <summary>
/// Provides interoperability with the Window Shell Light-Weight API.
/// </summary>
internal static partial class Shlwapi
{
    private const string LIBRARY_NAME = "shlwapi";

    /// <summary>
    /// Searches for and retrieves a file or protocol association-related string from the registry.
    /// </summary>
    /// <param name="flags">
    /// The flags used to control the search.
    /// </param>
    /// <param name="str">
    /// The <see cref="AssociationStringType"/> value that specifies the type of string to be returned.
    /// </param>
    /// <param name="pszAssoc">A null-terminated string used to determine the root key.</param>
    /// <param name="pszExtra">
    /// An optional null-terminated string with additional information about the location of the string, typically
    /// set to a Shell verb such as <c>open</c>.
    /// </param>
    /// <param name="pszOut">
    /// A buffer that, when this function returns successfully, receives the requested string. Set this parameter to
    /// <c>null</c> to retrieve the required buffer size.
    /// </param>
    /// <param name="pchOut">
    /// When calling the function, set to the number of characters in the <paramref name="pszOut"/> buffer. When the
    /// function returns successfully, the value is set to the number of characters actually placed in the buffer. If
    /// <paramref name="pszOut"/> is <c>null</c>, the function returns <c>S_FALSE</c> and this points to the required
    /// size, in characters, of the buffer.
    /// </param>
    /// <returns>A standard COM <see cref="ResultHandle"/> value.</returns>
    [LibraryImport(LIBRARY_NAME, EntryPoint = "AssocQueryStringW", StringMarshalling = StringMarshalling.Utf16)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    public static partial ResultHandle AssocQueryString(int flags, 
                                                        AssociationStringType str, 
                                                        string pszAssoc, 
                                                        string? pszExtra, 
                                                        char[] pszOut,
                                                        ref int pchOut);
}
