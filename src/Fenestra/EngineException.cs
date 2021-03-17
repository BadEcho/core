//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Runtime.Serialization;
using BadEcho.Odin;

namespace BadEcho.Fenestra
{
    /// <summary>
    /// Provides an exception used to report severe errors resulting in the termination of a Fenestra-based application.
    /// </summary>
    [Serializable]
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
        public EngineException(string message, Exception innerException)
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
        /// Initializes a new instance of the <see cref="EngineException"/> with serialized data.
        /// </summary>
        /// <inheritdoc/>
        private EngineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            IsProcessed = info.GetBoolean(nameof(IsProcessed));
        }

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
}