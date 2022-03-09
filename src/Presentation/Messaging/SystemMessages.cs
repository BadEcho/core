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

namespace BadEcho.Presentation.Messaging;

/// <summary>
/// Provides messages sent or received through a mediator that expose general-purpose functionality across Bad Echo Presentation
/// framework components.
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