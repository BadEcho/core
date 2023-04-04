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

using System.Runtime.Serialization;

namespace BadEcho.Presentation;

/// <summary>
/// Provides an exception used to report severe errors resulting in the termination of a Bad Echo Presentation framework application.
/// </summary>
public sealed class EngineException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EngineException"/> class.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of this exception.</param>
    /// <param name="isProcessed">
    /// Value indicating if the inner exception already received attention by one or more external handlers.
    /// </param>
    public EngineException(string message, Exception innerException, bool isProcessed)
        : base(message, innerException)
    {
        IsProcessed = isProcessed;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="EngineException"/> class.
    /// </summary>
    /// <inheritdoc/>
    public EngineException(string message, Exception? innerException)
        : base(message, innerException)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EngineException"/> class.
    /// </summary>
    /// <inheritdoc/>
    public EngineException(string message)
        : base(message)
    { }

    /// <summary>
    /// Initializes a new instance of the <see cref="EngineException"/> class.
    /// </summary>
    public EngineException()
    { }

    /// <summary>
    /// Gets a value indicating if the inner exception already received attention by one or more external handlers.
    /// </summary>
    /// <remarks>
    /// Even if the exception is marked as being processed, it does not mean it was handled. It merely means it received at least
    /// some kind of diagnostic logging and reporting; actions resulting from the exception which need not be repeated. If an
    /// exception has truly been handled then it shouldn't be wrapped inside a <see cref="EngineException"/>, as this type signals
    /// errors severe enough to warrant termination of the application.
    /// </remarks>
    public bool IsProcessed
    { get; }

    /// <inheritdoc/>
    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        Require.NotNull(info, nameof(info));

        info.AddValue(nameof(IsProcessed), IsProcessed);

        base.GetObjectData(info, context);
    }
}