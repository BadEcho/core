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

using System.Buffers;

namespace BadEcho.Interop;

/// <summary>
/// Provides interoperability with Windows Explorer.
/// </summary>
public static class Explorer
{
    private const string VERB_EXPLORE = "explore";
    private const string VERB_OPEN = "open";

    /// <summary>
    /// Explores the folder referred to by the specified path.
    /// </summary>
    /// <param name="path">The full or relative path to the folder to explore.</param>
    /// <returns>The result of invoking the Windows Shell.</returns>
    public static ShellResult OpenFolder(string path)
    {
        int result = Shell32.ShellExecute(nint.Zero, VERB_EXPLORE, path, null, null, 1);

        return result > 32 ? ShellResult.Success : (ShellResult) result;
    }

    /// <summary>
    /// Opens the file or folder referred to by the specified path.
    /// </summary>
    /// <param name="path">The full or relative path to the file or folder to open.</param>
    /// <returns>The result of invoking the Windows Shell.</returns>
    public static ShellResult Open(string path)
        => Open(path, null);

    /// <summary>
    /// Opens the file or folder referred to by the specified path.
    /// </summary>
    /// <param name="path">The full or relative path to the file or folder to open.</param>
    /// <param name="parameters">The parameters to pass if the file is an executable.</param>
    /// <returns>The result of invoking the Windows Shell.</returns>
    public static ShellResult Open(string path, string? parameters)
    {
        int result = Shell32.ShellExecute(nint.Zero, VERB_OPEN, path, parameters, null, 1);

        return result > 32 ? ShellResult.Success : (ShellResult) result;
    }

    /// <summary>
    /// Searches and retrieves the path to the executable associated with the specified file or protocol.
    /// </summary>
    /// <param name="searchKey">The file or protocol to get the associate executable for.</param>
    /// <returns></returns>
    public static string? GetAssociatedExecutable(string searchKey)
    {
        // MAX_PATH plus the additional null terminator the native method will add.
        int pathSize = 261;

        // ArrayPool is recommended when dealing with caller-allocated string buffers modified by a native callee.
        char[] pathBuffer = ArrayPool<char>.Shared.Rent(pathSize);
        string? path = null;

        try
        {
            ResultHandle result =
                Shlwapi.AssocQueryString(0, AssociationStringType.Executable, searchKey, null, pathBuffer, ref pathSize);

            if (result == ResultHandle.Success)
                path = new string(pathBuffer, 0, pathSize - 1); // Remove the null terminator.
        }
        finally
        {
            ArrayPool<char>.Shared.Return(pathBuffer);
        }

        return path;
    }
}