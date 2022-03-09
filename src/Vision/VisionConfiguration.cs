//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Threading;
using BadEcho.Presentation.Serialization;
using BadEcho.Configuration;
using BadEcho.Vision.Extensibility;

namespace BadEcho.Vision;

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
    public ExtensionDataStore<VisionModuleConfiguration> Modules 
    { get; set; }  = new();

    /// <inheritdoc/>
    public Dispatcher? Dispatcher { get; set; }
}