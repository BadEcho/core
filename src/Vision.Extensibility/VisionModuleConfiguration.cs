//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace BadEcho.Omnified.Vision.Extensibility
{
    /// <summary>
    /// Provides configuration settings for an individual Vision module.
    /// </summary>
    public sealed class VisionModuleConfiguration
    {
        /// <summary>
        /// Gets or sets the simple name of the module assembly.
        /// </summary>
        public string Name
        { get; init; } = string.Empty;

        /// <summary>
        /// Gets or sets the location for module's anchor point, overriding the default location defined by the module itself.
        /// </summary>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AnchorPointLocation? Location
        { get; init; }
    }
}
