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

namespace BadEcho.Presentation.Messaging;

/// <summary>
/// Provides messages sent or received through a mediator that expose general-purpose functionality across Fenestra
/// components.
/// </summary>
public static class SystemMessages
{
    /// <summary>
    /// Gets a message requesting all animations currently playing on supporting controls are to be canceled, with the
    /// affected controls being set back to their initial, unanimated state.
    /// </summary>
    public static MediatorMessage CancelAnimationsRequested
    { get; } = new (nameof(CancelAnimationsRequested), typeof(Action));
}