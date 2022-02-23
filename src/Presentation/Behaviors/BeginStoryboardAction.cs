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
using System.Windows.Media.Animation;

namespace BadEcho.Presentation.Behaviors;

/// <summary>
/// Provides an action that, when executed, will apply animations found in a <see cref="Storyboard"/> instance bound to
/// the dependency object this action is attached to.
/// </summary>
public sealed class BeginStoryboardAction : BehaviorAction<DependencyObject>
{
    private bool _isActive;
    private Storyboard? _writableStoryboard;

    /// <summary>
    /// Identifies the <see cref="Storyboard"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StoryboardProperty
        = DependencyProperty.Register(nameof(Storyboard),
                                      typeof(Storyboard),
                                      typeof(BeginStoryboardAction),
                                      new FrameworkPropertyMetadata(OnStoryboardChanged));
    /// <summary>
    /// Gets or sets the <see cref="Storyboard"/> that will have its animations applied when this action is executed.
    /// </summary>
    public Storyboard? Storyboard
    {
        get => (Storyboard?) GetValue(StoryboardProperty);
        set => SetValue(StoryboardProperty, value);
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
    /// </remarks>
    public override bool Execute()
    {
        if (_writableStoryboard == null)
            return false;

        if (_isActive)
            return true;

        // The object we're attached to, if possible, will become the inheritance context for the Storyboard, allowing us
        // to make use of Storyboards defined in separate ResourceDictionaries. 
        // That being said, we should only be targeting properties that can actually be found within the very same dependency object
        // we're attached to. If we wish to animate something outside the scope of said containing object, then simply attach another
        // action-triggering behavior to the outside object with a storyboard only targeting those properties.
        if (TargetObject is FrameworkElement containingObject)
            _writableStoryboard.Begin(containingObject);
        else
            _writableStoryboard.Begin();

        _isActive = true;

        return true;
    }
        
    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore()
        => new BeginStoryboardAction();

    private static void OnStoryboardChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        BeginStoryboardAction action = (BeginStoryboardAction) sender;

        action.LoadStoryboard((Storyboard) e.OldValue, (Storyboard) e.NewValue);
    }

    private void LoadStoryboard(Storyboard? oldStoryboard, Storyboard? newStoryboard)
    {
        if (oldStoryboard != null && _writableStoryboard != null)
        {
            _writableStoryboard.Completed -= HandleStoryboardCompleted;
            _writableStoryboard = null;
        }

        if (newStoryboard == null)
            return;

        // There is a good chance the Storyboard being bound to this action will be frozen. If that's the case,
        // we will be unable to subscribe to any of its events. We can work around this by cloning the Storyboard,
        // which gives us something whose events we can subscribe to, and will otherwise function completely
        // the same as the storyboard that's been bound to us.
        _writableStoryboard = newStoryboard.Clone();
        _writableStoryboard.Completed += HandleStoryboardCompleted;
    }

    private void HandleStoryboardCompleted(object? sender, EventArgs e) 
        => _isActive = false;
}