//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Runtime.InteropServices;
using BadEcho.Odin.Properties;

namespace BadEcho.Odin.Interop
{
    /// <summary>
    /// Specifies an opaque result handle returned by a function.
    /// </summary>
    public enum ResultHandle
    {
        /// <summary>
        /// The operation was successful.
        /// </summary>
        /// <remarks>Corresponds to S_OK.</remarks>
        Success = 0,
        /// <summary>
        /// The operation returned false.
        /// </summary>
        /// <remarks>Corresponds to S_FALSE. Function should still be considered successful.</remarks>
        False = 1,
    }

    /// <summary>
    /// Provides a set of static methods that simplify the use of <see cref="ResultHandle"/> values.
    /// </summary>
    public static class ResultHandleExtensions
    {
        /// <summary>
        /// Converts the <see cref="ResultHandle"/> value into a corresponding <see cref="Exception"/> object.
        /// </summary>
        /// <param name="hResult">A failed result handle.</param>
        /// <returns>An exception object representing the converted <c>hResult</c>.</returns>
        public static Exception GetException(this ResultHandle hResult)
        {
            var exception = Marshal.GetExceptionForHR((int) hResult);

            if (exception == null)
                throw new ArgumentException(Strings.NoExceptionFromSuccessfulResult, nameof(hResult));

            return exception;
        }
    }
}
