//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2022 Bad Echo LLC. All rights reserved.
//
//		Bad Echo Technologies are licensed under a
//		Creative Commons Attribution-NonCommercial 4.0 International License.
//
//		See accompanying file LICENSE.md or a copy at:
//		http://creativecommons.org/licenses/by-nc/4.0/
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Threading;
using BadEcho.Configuration;

namespace BadEcho.Omnified.Vision.Extensibility;

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
    /// Gets the location for the Vision application title's anchor point.
    /// </summary>
    AnchorPointLocation TitleLocation { get; }

    /// <summary>
    /// Gets the outer margin of modules anchored to the left side of the screen.
    /// </summary>
    Thickness LeftAnchorMargin { get; }

    /// <summary>
    /// Gets the outer margin of modules anchored to the center of the screen.
    /// </summary>
    Thickness CenterAnchorMargin { get; }

    /// <summary>
    /// Gets the outer margin of modules anchored to the right of the screen.
    /// </summary>
    Thickness RightAnchorMargin { get; }

    /// <summary>
    /// Gets a store of extension data for specific Vision modules.
    /// </summary>
    ExtensionDataStore<VisionModuleConfiguration> Modules { get; }

    /// <summary>
    /// Gets or sets the dispatcher Vision modules should execute dispatcher-sensitive operations on.
    /// </summary>
    Dispatcher? Dispatcher { get; set; }
}