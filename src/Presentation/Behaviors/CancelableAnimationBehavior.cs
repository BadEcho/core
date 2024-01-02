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

namespace BadEcho.Presentation.Behaviors;

/// <summary>
/// Provides a behavior that, when attached to a target dependency object, allows for the immediate cancellation of an animation
/// running on it. 
/// </summary>
public sealed class CancelableAnimationBehavior : CompoundBehavior<FrameworkElement, CancelableAnimationState>
{
    /// <summary>
    /// Identifies the attached property that gets or sets this behavior's <see cref="CancelableAnimationState"/> data,
    /// which ultimately specifies the storyboard this behavior will be canceling if requested to do so.
    /// </summary>
    public static readonly DependencyProperty StateProperty 
        = RegisterAttachment();

    /// <summary>
    /// Gets the value of the <see cref="StateProperty"/> attached property for a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="source">The dependency object from which the property value is read.</param>
    /// <returns>The <see cref="CancelableAnimationState"/> associated with <c>source</c>.</returns>
    public static CancelableAnimationState GetState(DependencyObject source)
    {
        Require.NotNull(source, nameof(source));

        return GetAttachment(source, StateProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="StateProperty"/> attached property on a given <see cref="DependencyObject"/>.
    /// </summary>
    /// <param name="source">The dependency object to which the property is written.</param>
    /// <param name="value">The <see cref="CancelableAnimationState"/> to set.</param>
    public static void SetState(DependencyObject source, CancelableAnimationState value)
    {
        Require.NotNull(source, nameof(source));

        source.SetValue(StateProperty, value);
    }

    /// <inheritdoc/>
    protected override void OnValueAssociated(FrameworkElement targetObject, CancelableAnimationState newValue)
    {
        base.OnValueAssociated(targetObject, newValue);

        newValue.AnimationCanceling += HandleAnimationCanceling;
    }

    /// <inheritdoc/>
    protected override void OnValueDisassociated(FrameworkElement targetObject, CancelableAnimationState oldValue)
    {
        base.OnValueDisassociated(targetObject, oldValue);

        oldValue.AnimationCanceling -= HandleAnimationCanceling;
    }

    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore()
        => new CancelableAnimationBehavior();

    private static DependencyProperty RegisterAttachment()
    {
        var behavior = new CancelableAnimationBehavior();

        return DependencyProperty.RegisterAttached(
            NameOf.ReadAccessorEnabledDependencyPropertyName(() => StateProperty),
            typeof(CancelableAnimationState),
            typeof(CancelableAnimationBehavior),
            behavior.DefaultMetadata);
    }

    private static void HandleAnimationCanceling(object? sender, EventArgs e)
    {
        if (sender == null)
            return;

        var targetObject = (FrameworkElement)sender;

        CancelableAnimationState state = GetAttachment(targetObject, StateProperty);

        state.Storyboard?.SkipToFill(targetObject);
    }
}
