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

namespace BadEcho.Presentation.Behaviors;

/// <summary>
/// Provides an action that, when executed, will prevent any and all subsequent actions from being executed based on the state
/// of bound predicate property.
/// </summary>
public sealed class ConditionAction : BehaviorAction<DependencyObject>
{
    /// <summary>
    /// Identifies the <see cref="IsEnabled"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty IsEnabledProperty
        = DependencyProperty.Register(nameof(IsEnabled),
                                      typeof(bool),
                                      typeof(ConditionAction));

    /// <summary>
    /// Gets or sets a value indicating if the action should allow execution of subsequent actions.
    /// </summary>
    public bool IsEnabled
    {
        get => (bool) GetValue(IsEnabledProperty);
        set => SetValue(IsEnabledProperty, value);
    }

    /// <inheritdoc/>
    public override bool Execute() 
        => IsEnabled;

    /// <inheritdoc/>
    protected override Freezable CreateInstanceCore() 
        => new ConditionAction();
}