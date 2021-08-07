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

using System;

namespace BadEcho.Odin.Extensions
{
    /// <summary>
    /// Provides a set of static methods to aid in matters related to <see cref="Exception"/> objects.
    /// </summary>
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Finds and returns the deepest inner exception found in the provided exception object.
        /// </summary>
        /// <param name="exception">A <see cref="Exception"/> object to retrieve the innermost exception from.</param>
        /// <returns>The innermost <see cref="Exception"/> object found in <c>exception</c>.</returns>
        public static Exception FindInnermostException(this Exception exception)
        {
            Require.NotNull(exception, nameof(exception));

            return Trampoline.Execute(ExceptionDelver, exception);

            static Bounce<Exception> ExceptionDelver(Exception currentException)
            {
                Exception? innerException = currentException.InnerException;
                
                return innerException == null ? Bounce.Finish(currentException) : Bounce.Continue(innerException);
            }
        }
    }
}
