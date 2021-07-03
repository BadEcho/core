//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Odin.Extensibility;

namespace BadEcho.Omnified.Vision.Extensibility
{
    /// <summary>
    /// Defines a snap-in module granting vision to Omnified data.
    /// </summary>
    public interface IVisionModule : IFilterableFamily
    {
        /// <summary>
        /// Gets the default location of the module's anchor point.
        /// </summary>
        AnchorPointLocation DefaultLocation { get; }

        /// <summary>
        /// Gets the direction the module will grow from its anchor point.
        /// </summary>
        GrowthDirection GrowthDirection { get; }

        /// <summary>
        /// Gets the name of the file containing messages for the Vision module.
        /// </summary>
        string MessageFile { get; }

        /// <summary>
        /// Gets a value indicating if only new messages should be processed, as opposed to the entire file, whenever
        /// the Vision module's file is updated.
        /// </summary>
        bool ProcessNewMessagesOnly { get; }
    }
}
