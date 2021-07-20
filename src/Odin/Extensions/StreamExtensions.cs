//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.IO;

namespace BadEcho.Odin.Extensions
{
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
        /// use to specify a <see cref="FileShare"/> value, giving us the ability to open files that other processes are writing to
        /// (something which will result in an <see cref="IOException"/> if attempted with <see cref="File.ReadAllText(string)"/>).
        /// </remarks>
        public static string ReadAllText(this FileInfo info, FileShare share)
        {
            Require.NotNull(info, nameof(info));

            if (!info.Exists)
                return string.Empty;

            using (var file = info.Open(FileMode.Open, FileAccess.Read, share))
            {
                using (var reader = new StreamReader(file))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
