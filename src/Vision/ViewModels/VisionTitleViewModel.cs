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

using BadEcho.Presentation.ViewModels;
using BadEcho.Vision.Properties;

namespace BadEcho.Vision.ViewModels;

/// <summary>
/// Provides a view model that facilitates the display of Vision's application title.
/// </summary>
internal sealed class VisionTitleViewModel : ViewModel
{
    /// <summary>
    /// Gets or sets the initial, primary text on rotation with the <see cref="SecondaryLogo"/> for display as Vision's
    /// application logo.
    /// </summary>
    public static string PrimaryLogo 
        => Strings.VisionTitleName;

    /// <summary>
    /// Gets or sets the secondary text on rotation with the <see cref="PrimaryLogo"/> for display as Vision's application
    /// logo.
    /// </summary>
    public static string SecondaryLogo
        => Strings.VisionTitleAddress;

    /// <inheritdoc/>
    public override void Disconnect()
    { }
}