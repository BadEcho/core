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

using BadEcho.Omnified.Vision.Extensibility;

namespace BadEcho.Omnified.Vision.Apocalypse;

/// <summary>
/// Provides configuration settings for the Apocalypse Vision module.
/// </summary>
public sealed class ApocalypseModuleConfiguration : VisionModuleConfiguration
{
    /// <summary>
    /// Gets the maximum width constraint of elements responsible for displaying Apocalypse event messages.
    /// </summary>
    public double EffectMessageMaxWidth
    { get; init; } = double.NaN;
}