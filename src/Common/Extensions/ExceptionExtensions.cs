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

namespace BadEcho.Extensions;

/// <summary>
/// Provides a set of static methods to aid in matters related to <see cref="Exception"/> objects.
/// </summary>
public static class ExceptionExtensions
{
    /// <summary>
    /// Finds and returns the deepest inner exception found in the provided exception.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> instance to retrieve the innermost exception from.</param>
    /// <returns>The innermost <see cref="Exception"/> instance found in <c>exception</c>.</returns>
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

    /// <summary>
    /// Gets the fully qualified name of the method that threw the exception.
    /// </summary>
    /// <param name="exception">The <see cref="Exception"/> instance to retrieve the full target site name for.</param>
    /// <returns>The fully qualified name of the method that threw <c>exception</c>.</returns>
    public static string GetTargetSiteFullName(this Exception exception)
    {
        Require.NotNull(exception, nameof(exception));

        if (exception.TargetSite == null)
            return string.Empty;

        string typeNamePrefix = exception.TargetSite.ReflectedType != null
            ? $"{exception.TargetSite.ReflectedType.FullName}."
            : string.Empty;

        return $"{typeNamePrefix}{exception.TargetSite.Name}";
    }
}