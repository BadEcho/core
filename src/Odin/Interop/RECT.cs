//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;

namespace BadEcho.Odin.Interop
{
    /// <summary>
    /// Provides a definition for the coordinates of the upper-left and lower-right corners of a rectangle.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        /// <summary>
        /// The x-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int Left;

        /// <summary>
        /// The y-coordinate of the upper-left corner of the rectangle.
        /// </summary>
        public int Top;

        /// <summary>
        /// The x-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int Right;

        /// <summary>
        /// The y-coordinate of the lower-right corner of the rectangle.
        /// </summary>
        public int Bottom;

        /// <summary>
        /// Gets the height of the rectangle.
        /// </summary>
        public int Height
            => Bottom - Top;

        /// <summary>
        /// Gets the width of the rectangle.
        /// </summary>
        public int Width
            => Right - Left;
    }
}
