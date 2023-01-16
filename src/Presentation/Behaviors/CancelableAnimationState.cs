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

using System.Windows;
using System.Windows.Media.Animation;
using BadEcho.Presentation.Messaging;

namespace BadEcho.Presentation.Behaviors;

/// <summary>
/// Provides attachable animation state data to a <see cref="CancelableAnimationBehavior"/> instance controlling it.
/// </summary>
/// <remarks>
/// This is meant to be provided to a <see cref="CancelableAnimationBehavior"/> instance using property element syntax in order
/// to provide both messaging support as well as a means to specify which storyboard we aim to make cancelable.
/// </remarks>
public sealed class CancelableAnimationState : AttachableComponent<DependencyObject>
{
    /// <summary>
    /// Identifies the <see cref="Mediator"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MediatorProperty =
        DependencyProperty.Register(nameof(Mediator),
                                    typeof(Mediator),
                                    typeof(CancelableAnimationState),
                                    new PropertyMetadata(OnMediatorChanged));
    /// <summary>
    /// Identifies the <see cref="Storyboard"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StoryboardProperty =
        DependencyProperty.Register(nameof(Storyboard),
                                    typeof(Storyboard),
                                    typeof(CancelableAnimationState));
    /// <summary>
    /// Occurs when a request to cancel the playback of <see cref="Storyboard"/> has been made.
    /// </summary>
    public event EventHandler? AnimationCanceling;

    /// <summary>
    /// Gets or sets the storyboard whose playback on the dependency object this component is attached to will be made cancelable.
    /// </summary>
    public Storyboard? Storyboard
    {
        get => (Storyboard) GetValue(StoryboardProperty); 
        set => SetValue(StoryboardProperty, value);
    }

    /// <summary>
    /// Gets or sets the mediator used to receive animation cancellation requests.
    /// </summary>
    public Mediator? Mediator
    {
        get => (Mediator) GetValue(MediatorProperty);
        set => SetValue(MediatorProperty, value);
    }
    
    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore()
        => new CancelableAnimationState();

    private static void OnMediatorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        var animationState = (CancelableAnimationState) sender;

        if (e.OldValue is Mediator oldMediator) 
            animationState.UnregisterMediator(oldMediator);

        if (e.NewValue is Mediator newMediator)
            animationState.RegisterMediator(newMediator);
    }
    
    private void RegisterMediator(Mediator mediator)
        => mediator.Register(SystemMessages.CancelAnimationsRequested, MediateCancelAnimationsRequest);

    private void UnregisterMediator(Mediator mediator)
        => mediator.Unregister(SystemMessages.CancelAnimationsRequested, MediateCancelAnimationsRequest);

    private void MediateCancelAnimationsRequest()
    { 
        AnimationCanceling?.Invoke(TargetObject, EventArgs.Empty);
    }
}
