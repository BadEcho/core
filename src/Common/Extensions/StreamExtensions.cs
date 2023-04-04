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

namespace BadEcho.Extensions;

/// <summary>
/// Provides a set of static methods that aid in matters related to input/output streams.
/// </summary>
public static class StreamExtensions
{
    /// <summary>
    /// Opens a text file, reads all the text in the file, and then closes the file.
    /// </summary>
    /// <param name="info">A <see cref="FileInfo"/> instance wrapping the path to the file we want to read.</param>
    /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
    /// <returns>A string containing all the text in the file.</returns>
    /// <remarks>
    /// This is almost analogous to <see cref="File.ReadAllText(string)"/>, the main difference that this method allows
    /// us to specify a <see cref="FileShare"/> value, giving us the ability to open files that other processes are writing to
    /// (something which will result in an <see cref="IOException"/> if attempted with <see cref="File.ReadAllText(string)"/>).
    /// </remarks>
    public static string ReadAllText(this FileInfo info, FileShare share)
        => ReadAllText(info, share, 0);

    /// <summary>
    /// Opens a text file, reads all text found starting at the given offset position, and then closes the file.
    /// </summary>
    /// <param name="info">A <see cref="FileInfo"/> instance wrapping the path to the file we want to read.</param>
    /// <param name="share">A <see cref="FileShare"/> value specifying the type of access other threads have to the file.</param>
    /// <param name="offset">The point relative from the beginning of the file from which to start reading./</param>
    /// <returns>A string containing all the text in the file.</returns>
    /// <remarks>
    /// This is almost analogous to <see cref="File.ReadAllText(string)"/>, the main difference that this method allows
    /// us to specify a <see cref="FileShare"/> value, as well as a position to begin reading from, giving us the ability to open files
    /// at precise locations that other processes are writing to (something which will result in an <see cref="IOException"/> if
    /// attempted with <see cref="File.ReadAllText(string)"/>).
    /// </remarks>
    public static string ReadAllText(this FileInfo info, FileShare share, long offset)
    {
        Require.NotNull(info, nameof(info));

        if (!info.Exists)
            return string.Empty;

        using (var file = info.Open(FileMode.Open, FileAccess.Read, share))
        {
            file.Seek(offset, SeekOrigin.Begin);

            using (var reader = new StreamReader(file))
            {
                return reader.ReadToEnd();
            }
        }
    }

    /// <summary>
    /// Writes the stream's contents to a byte array.
    /// </summary>
    /// <param name="source">The stream to convert to a byte array.</param>
    /// <returns>The contents of <c>source</c> as an array of bytes.</returns>
    public static byte[] ToArray(this Stream source)
    {
        Require.NotNull(source, nameof(source));

        using (var memoryStream = new MemoryStream())
        {
            source.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}