//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;

namespace BadEcho.Omnified.Vision.Extensibility
{
    /// <summary>
    /// Defines configuration settings for the Vision application.
    /// </summary>
    public interface IVisionConfiguration
    {
        /// <summary>
        /// Gets the path relative from Vision's base directory to the directory where all message files are being written to.
        /// </summary>
        /// <remarks>
        /// Message files are most often found in the directory containing the injected hacking code for the targeted binary.
        /// If this is set to such a directory containing hacking code for a targeted binary, then we shall gain vision
        /// into that which is targeted.
        /// </remarks>
        string MessageFilesDirectory { get; }

        /// <summary>
        /// Gets a dictionary containing the names of plugin assemblies paired with their individual configurations.
        /// </summary>
        IDictionary<string, VisionModuleConfiguration> Modules
        { get; } 
    }
}
