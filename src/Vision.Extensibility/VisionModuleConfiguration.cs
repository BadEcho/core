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

namespace BadEcho.Vision.Extensibility;

/// <summary>
/// Provides configuration settings for an individual Vision module.
/// </summary>
public class VisionModuleConfiguration
{
    /// <summary>
    /// Gets the simple name of the module assembly.
    /// </summary>
    public string Name
    { get; init; } = string.Empty;

    /// <summary>
    /// Gets the location for the module's anchor point, overriding the default location defined by the module itself.
    /// </summary>
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AnchorPointLocation? Location
    { get; init; }

    /// <summary>
    /// Gets the maximum number of messages the module will process at any given time.
    /// </summary>
    /// <remarks>
    /// A default value of 0 will result in no limit of messages being processed.
    /// </remarks>
    public int MaxMessages
    { get; init; }
}