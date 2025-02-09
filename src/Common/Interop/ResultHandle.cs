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
using BadEcho.Properties;

namespace BadEcho.Interop;

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
    /// <summary>
    /// Indicates that no such interface is supported. Corresponds to E_NOINTERFACE.
    /// </summary>
    NoInterface = unchecked((int)0x80004002),
    /// <summary>
    /// Indicates that the operation was aborted. Corresponds to E_ABORT.
    /// </summary>
    Abort = unchecked((int)0x80004004),
    /// <summary>
    /// Indicates an unspecified failure. Corresponds to E_FAIL.
    /// </summary>
    Failure = unchecked((int)0x80004005),
    /// <summary>
    /// Indicates that a specified element either could not be located or simply does not exist. 
    /// Corresponds to TYPE_E_ELEMENTNOTFOUND.
    /// </summary>
    TypeElementNotFound = unchecked((int)0x8002802B),
    /// <summary>
    /// Indicates that a requested resource is read-only. Corresponds to STG_E_ACCESSDENIED.
    /// </summary>
    AccessDenied = unchecked((int)0x80030005),
    /// <summary>
    /// Indicates that the class is not registered in the registry. Corresponds to REGDB_E_CLASSNOTREG.
    /// </summary>
    ClassNotRegistered = unchecked((int)0x80040154),
    /// <summary>
    /// Indicates that there was a general error in reading the registry. Corresponds to REGDB_E_READREGDB.
    /// </summary>
    ErrorReadingRegistry = unchecked((int)0x80040150),
    /// <summary>
    /// Indicates that a specified object could not be found. Corresponds to NO_OBJECT
    /// </summary>
    NoObject = unchecked((int)0x800401E5),
    /// <summary>
    /// Indicates that invalid arguments were provided. Corresponds to E_INVALIDARG.
    /// </summary>
    InvalidArguments = unchecked((int)0x80070057),
    /// <summary>
    /// Indicates that we're out of memory. Corresponds to E_OUTOFMEMORY.
    /// </summary>
    OutOfMemory = unchecked((int)0x8007000E),
    /// <summary>
    /// Indicates that a specified element either could not be located or simply does not exist.
    /// Corresponds to E_ELEMENTNOTFOUND.
    /// </summary>
    ElementNotFound = unchecked((int)0x80070490),
    /// <summary>
    /// Indicates that an in-process server was unable to complete the registration of all the type
    /// libraries used by its classes.
    /// </summary>
    TypeLibraryRegistrationFailure = unchecked((int)0x80040200),
    /// <summary>
    /// Indicates that an in-process server was unable to complete the registration of all the object classes.
    /// </summary>
    ObjectClassRegistrationFailure = unchecked((int)0x80040201),
    /// <summary>
    /// Indicates that a horrible error occurred (one whose true name should never be shown to a user lest
    /// widespread panic be the goal!).
    /// </summary>
    CatastrophicFailure = unchecked((int)0x8000ffff)
}

/// <summary>
/// Provides a set of static methods that simplify the use of <see cref="ResultHandle"/> values.
/// </summary>
public static class ResultHandleExtensions
{
    /// <summary>
    /// Determines if the result handle is indicative of failure.
    /// </summary>
    /// <param name="hResult">The result handle to check.</param>
    /// <returns>True if <c>hResult</c> indicates failure; otherwise false.</returns>
    public static bool Failed(this ResultHandle hResult)
        => hResult < 0;

    /// <summary>
    /// Converts the <see cref="ResultHandle"/> value into a corresponding <see cref="Exception"/> object.
    /// </summary>
    /// <param name="hResult">A failed result handle.</param>
    /// <returns>An exception object representing the converted <c>hResult</c>.</returns>
    public static Exception GetException(this ResultHandle hResult)
        => Marshal.GetExceptionForHR((int)hResult)
            ?? throw new ArgumentException(Strings.NoExceptionFromSuccessfulResult, nameof(hResult));
}