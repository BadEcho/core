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

using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Windows;
using BadEcho.Fenestra.Serialization;
using BadEcho.Omnified.Vision.Extensibility;

namespace BadEcho.Omnified.Vision
{
    /// <summary>
    /// Provides configuration settings for the Vision application.
    /// </summary>
    internal sealed class VisionConfiguration : IVisionConfiguration
    {
        /// <inheritdoc/>
        public string MessageFilesDirectory 
        { get; set; } = string.Empty;

        [JsonConverter(typeof(JsonThicknessConverter))]
        public Thickness LeftAnchorMargin
        { get; set; }

        [JsonConverter(typeof(JsonThicknessConverter))]
        public Thickness CenterAnchorMargin
        { get; set; }

        [JsonConverter(typeof(JsonThicknessConverter))]
        public Thickness RightAnchorMargin
        { get; set; }

        /// <inheritdoc/>
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AnchorPointLocation TitleLocation
        { get; set; }

        /// <inheritdoc/>
        public IDictionary<string, VisionModuleConfiguration> Modules 
        { get; init; }  = new Dictionary<string, VisionModuleConfiguration>();
    }
}
