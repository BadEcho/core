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
        public override bool Execute()
        {
            if (Storyboard == null)
                return false;
            
            Storyboard.Begin();
            return true;
        }

        /// <inheritdoc/>
        protected override Freezable CreateInstanceCore()
            => new BeginStoryboardAction();
    }
}
