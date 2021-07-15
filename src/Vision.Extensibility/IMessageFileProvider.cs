//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using BadEcho.Odin;

namespace BadEcho.Omnified.Vision.Extensibility
{
    /// <summary>
    /// Defines a provider of a Vision module's message file.
    /// </summary>
    public interface IMessageFileProvider
    {
        /// <summary>
        /// Occurs when new messages are available for processing.
        /// </summary>
        event EventHandler<EventArgs<string>>? NewMessages;

        /// <summary>
        /// Gets the complete set of messages that have posted to the message file.
        /// </summary>
        string? CurrentMessages { get; }
    }
}
