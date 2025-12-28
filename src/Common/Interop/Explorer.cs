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
/// Provides interoperability with Windows Explorer.
/// </summary>
public static class Explorer
{
    private const string VERB_EXPLORE = "explore";

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
}