//-----------------------------------------------------------------------
// <copyright>
//		Created by Matt Weber <matt@badecho.com>
//		Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under the
//		GNU Affero General Public License v3.0.
//
//		See accompanying file LICENSE.md or a copy at:
//		https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows.Media;

namespace BadEcho.Presentation.Configuration;

/// <summary>
/// Provides configuration settings for a Bad Echo Presentation framework application.
/// </summary>
public sealed class PresentationConfiguration
{
    /// <summary>
    /// Get the index of the monitor that the main window of the Bad Echo Presentation framework application should be
    /// initially launched on.
    /// </summary>
    /// <remarks>
    /// A monitor's index corresponds to where the monitor is in the arrangement defined in the user's display settings, with
    /// the lowest index being the leftmost monitor and the highest index being the rightmost.
    /// </remarks>
    public int LaunchDisplay
    { get; init; }

    /// <summary>
    /// Gets the x-axis scale factor applied to the overall application.
    /// </summary>
    /// <remarks>
    /// Use this if there is a desire to immediately affect the size of all rendered elements on the x-axis in the Presentation
    /// application. Leaving this setting unset (or setting it to exactly <c>1.0</c>) will result in no scaling being applied.
    /// </remarks>
    public double ScaleX
    { get; init; }

    /// <summary>
    /// Gets the y-axis scale factor applied to the overall application.
    /// </summary>
    /// <remarks>
    /// Use this if there is a desire to immediately affect the size of all rendered elements on the y-axis in the Presentation
    /// application. Leaving this setting unset (or setting it to exactly <c>1.0</c>) will result in no scaling being applied.
    /// </remarks>
    public double ScaleY
    { get; init; }

    /// <summary>
    /// Gets a value indicating if an attempt to force software rendering for the current process should be made.
    /// </summary>
    /// <remarks>
    /// Setting this to true will take care of setting the <see cref="RenderOptions.ProcessRenderMode"/> property so that rendering
    /// is not hardware accelerated. Bear in mind that this merely specifies our rendering preference, and that other parts of WPF
    /// may override this preference and ultimately decide the actual rendering mode.
    /// </remarks>
    public bool ForceSoftwareRendering
    { get; init; }
}