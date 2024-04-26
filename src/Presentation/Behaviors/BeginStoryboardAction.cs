//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2024 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Media.Animation;
using BadEcho.Presentation.Messaging;
using BadEcho.Presentation.Properties;

namespace BadEcho.Presentation.Behaviors;

/// <summary>
/// Provides an action that, when executed, will apply animations found in a <see cref="Storyboard"/> instance bound to
/// the dependency object this action is attached to.
/// </summary>
public sealed class BeginStoryboardAction : BehaviorAction<DependencyObject>
{
    /// <summary>
    /// Identifies the <see cref="Storyboard"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StoryboardProperty
        = DependencyProperty.Register(nameof(Storyboard),
                                      typeof(Storyboard),
                                      typeof(BeginStoryboardAction),
                                      new FrameworkPropertyMetadata(OnStoryboardChanged));
    /// <summary>
    /// Identifies the <see cref="Mediator"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty MediatorProperty 
        = DependencyProperty.Register(nameof(Mediator),
                                      typeof(Mediator),
                                      typeof(BeginStoryboardAction),
                                      new PropertyMetadata(OnMediatorChanged));

    private bool _hasPreviouslyExecuted;
    private bool _isMutuallyExclusive;
    private bool _animationsOnHold;
    private Storyboard? _activeStoryboard;

    /// <summary>
    /// Gets or sets the <see cref="Storyboard"/> that will have its animations applied when this action is executed.
    /// </summary>
    public Storyboard? Storyboard
    {
        get => (Storyboard?) GetValue(StoryboardProperty);
        set => SetValue(StoryboardProperty, value);
    }

    /// <summary>
    /// Gets or sets the <see cref="Mediator"/> used to send and receive animation hold requests.
    /// </summary>
    public Mediator? Mediator
    {
        get => (Mediator?) GetValue(MediatorProperty);
        set => SetValue(MediatorProperty, value);
    }

    /// <summary>
    /// Gets or sets a value indicating if no other animations can be started by other actions sharing a <see cref="Mediator"/>
    /// instance while one of this action's animations is running.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Setting this value to true will cause a message to go out on the <see cref="Mediator"/> that will instruct other
    /// <see cref="BeginStoryboardAction"/> instances to ignore execution requests whenever this action begins an animation.
    /// </para>
    /// <para>
    /// Once an animation started by this action has completed, another message will go out releasing the hold on animations
    /// from other actions.
    /// </para>
    /// <para>
    /// One thing to keep in mind, is that setting this to true will result in this action needing to subscribe to the
    /// <see cref="Timeline.Completed"/> event of the <see cref="Storyboard"/> instance. If the storyboard is frozen, and there is
    /// a good chance that it will be, the storyboard will need to be cloned.
    /// </para>
    /// <para>
    /// The main implication from cloning is that our storyboard will no longer be controllable from the outside, as only this action
    /// will have a reference to the running storyboard. Attempts to manipulate an ongoing animation by an external entity, such as
    /// stopping it, will fail. So, only set this to true if no other action needs to control the animation that this action starts.
    /// </para>
    /// </remarks>
    public bool IsMutuallyExclusive
    {
        get => _isMutuallyExclusive;
        set
        {
            bool changed = _isMutuallyExclusive != value;

            _isMutuallyExclusive = value;

            // The mutual exclusivity flag influences how we load the storyboard. So, if there was a change, we need to reload it.
            if (changed && Storyboard != null)
                LoadStoryboard(Storyboard);
        }
    }

    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// This action will only allow a single animation execution to occur at any given point in time. All requests to
    /// initiate another animation are ignored until the current animation sequence completes.
    /// </para>
    /// <para>
    /// This is done so that any animation designed to revert affected properties back to their original states is not interrupted
    /// in doing so, lest the original values for said properties become lost forever.
    /// </para>
    /// <para>
    /// The object we're attached to, if possible, will become the inheritance context for the Storyboard, allowing us
    /// to make use of Storyboards defined in separate ResourceDictionaries. 
    /// </para>
    /// <para>
    /// That being said, we should only be targeting properties that can actually be found within the very same dependency object
    /// we're attached to. If we wish to animate something outside the scope of said containing object, then simply attach another
    /// action-triggering behavior to the outside object with a storyboard only targeting those properties.
    /// </para>
    /// </remarks>
    public override bool Execute()
    {
        if (_activeStoryboard == null)
            return false;

        if (Mediator != null)
        {
            IEnumerable<bool> onHoldQuery = Mediator.BroadcastReceive<bool>(SystemMessages.AnimationHoldQuery);

            if (onHoldQuery.Any(onHold => onHold))
                return true;
        }

        if (TargetObject is not FrameworkElement containingObject)
            throw new InvalidOperationException(Strings.BeginStoryboardActionNeedsTarget);

        // Let any currently running animation complete before starting another one.
        // We can't check the storyboard's state until it has been started at least once.
        if (_hasPreviouslyExecuted && _activeStoryboard.GetCurrentState(containingObject) == ClockState.Active)
            return true;

        if (IsMutuallyExclusive)
        {
            _animationsOnHold = true;

            Mediator?.Broadcast(SystemMessages.CancelAnimationsRequested);
        }

        _activeStoryboard.Begin(containingObject, true);
        _hasPreviouslyExecuted = true;

        return true;
    }
        
    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore()
        => new BeginStoryboardAction();

    /// <inheritdoc/>
    protected override void OnDetaching()
    {
        base.OnDetaching();

        if (Mediator != null) 
            UnregisterMediator(Mediator);

        if (_activeStoryboard is { IsFrozen: false }) 
            _activeStoryboard.Completed -= HandleStoryboardCompleted;
    }

    private static void OnMediatorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        BeginStoryboardAction action = (BeginStoryboardAction) sender;

        if (e.OldValue is Mediator oldMediator)
            action.UnregisterMediator(oldMediator);

        if (e.NewValue is Mediator newMediator)
            action.RegisterMediator(newMediator);
    }
    
    private static void OnStoryboardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        BeginStoryboardAction action = (BeginStoryboardAction) sender;

        action.LoadStoryboard((Storyboard) e.NewValue);
    }

    private void LoadStoryboard(Storyboard? newStoryboard)
    {
        if (_activeStoryboard is { IsFrozen: false })
        {   
            _activeStoryboard.Completed -= HandleStoryboardCompleted;
            _activeStoryboard = null;
        }

        if (newStoryboard == null)
            return;

        _activeStoryboard = newStoryboard;

        if (IsMutuallyExclusive)
        {
            if (_activeStoryboard.IsFrozen)
                _activeStoryboard = _activeStoryboard.Clone();

            _activeStoryboard.Completed += HandleStoryboardCompleted;
        }
    }

    private void HandleStoryboardCompleted(object? sender, EventArgs e)
        => _animationsOnHold = false;

    private void RegisterMediator(Mediator mediator)
    {
        mediator.Register(SystemMessages.AnimationHoldQuery, MediateHoldAnimationsQuery);
    }

    private void UnregisterMediator(Mediator mediator)
    {
        mediator.Unregister(SystemMessages.AnimationHoldQuery, MediateHoldAnimationsQuery);
    }

    private bool MediateHoldAnimationsQuery()
        => _animationsOnHold;
}