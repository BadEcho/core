//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Media.Animation;

namespace BadEcho.Fenestra.Behaviors
{
    /// <summary>
    /// Provides an action that, when executed, will apply animations found in a bound <see cref="Storyboard"/> instance to
    /// the dependency object this action is attached to.
    /// </summary>
    public sealed class BeginStoryboardAction : BehaviorAction<DependencyObject>
    {
        private bool _isActive;

        /// <summary>
        /// Identifies the <see cref="Storyboard"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StoryboardProperty
            = DependencyProperty.Register(nameof(Storyboard),
                                          typeof(Storyboard),
                                          typeof(BeginStoryboardAction));
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
            if (Storyboard == null)
                return false;

            if (_isActive)
                return true;

            // Remember, Storyboard is a Freezable, so subscribing to one of its events results in a PropertyChangeCallback.
            // This makes the idea of subscribing and unsubscribing to events inside such a callback quite untenable.
            // Therefore, we subscribe and unsubscribe to events in response to requests for animation.
            Storyboard.Completed += HandleStoryboardCompleted;

            // The object we're attached to, if possible, will become the inheritance context for the Storyboard, allowing us
            // to make use of Storyboards defined in separate ResourceDictionaries. 
            // That being said, we should only be targeting properties that can actually be found within the very same dependency object
            // we're attached to. If we wish to animate something outside the scope of said containing object, then simply attach another
            // action-triggering behavior to the outside object with a storyboard only targeting those properties.
            if (TargetObject is FrameworkElement containingObject)
                Storyboard.Begin(containingObject);
            else
                Storyboard.Begin();

            _isActive = true;

            return true;
        }
        
        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
            => new BeginStoryboardAction();

        private void HandleStoryboardCompleted(object? sender, System.EventArgs e)
        {
            _isActive = false;
            
            if (Storyboard != null)
                Storyboard.Completed -= HandleStoryboardCompleted;
        }
    }
}
