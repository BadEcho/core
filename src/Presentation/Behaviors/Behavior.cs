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

using System.Windows;
using System.Windows.Media.Animation;
using BadEcho.Presentation.Properties;
using BadEcho.Extensions;

namespace BadEcho.Presentation.Behaviors;

/// <summary>
/// Provides an infrastructure for attached properties that, when attached to a target dependency object, directly influence the
/// state and functioning of said object.
/// </summary>
/// <typeparam name="TTarget">The type of <see cref="DependencyObject"/> this behavior attaches to.</typeparam>
/// <typeparam name="TProperty">The type of value accepted by this behavior as an attached property.</typeparam>
public abstract class Behavior<TTarget,TProperty> : Animatable, IAttachableComponent<TTarget>
    where TTarget: DependencyObject
{
    private readonly Dictionary<TTarget, TProperty?> _targetMap = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Behavior{TTarget,TProperty}"/> class.
    /// </summary>
    protected Behavior()
        => DefaultMetadata = new PropertyMetadata(default(TProperty), OnAttachChanged);

    /// <summary>
    /// Gets the default property metadata to use when registering this behavior as an attached property.
    /// </summary>
    public PropertyMetadata DefaultMetadata
    { get; }

    /// <inheritdoc/>
    public void Attach(TTarget targetObject)
    {
        VerifyAccess();

        if (TargetMap.ContainsKey(targetObject))
            throw new InvalidOperationException(Strings.BehaviorAlreadyAttachedToTarget);

        WritePreamble();
        TargetMap.Add(targetObject, default);
        WritePostscript();
    }

    /// <inheritdoc/>
    public void Detach(TTarget targetObject)
    {
        VerifyAccess();

        if (!TargetMap.ContainsKey(targetObject))
            return;

        TProperty? associatedValue = TargetMap[targetObject];

        if (associatedValue != null)
            OnValueDisassociated(targetObject, associatedValue);

        WritePreamble();
        TargetMap.Remove(targetObject);
        WritePostscript();
    }

    /// <summary>
    /// Gets a mapping between target dependency objects and associated values while assuring this <see cref="Freezable"/>
    /// is being accessed appropriately.
    /// </summary>
    private IDictionary<TTarget, TProperty?> TargetMap
    {
        get
        {
            ReadPreamble();

            return _targetMap;
        }
    }

    /// <summary>
    /// Called when this behavior is being associated with a new local value being set on a <see cref="DependencyObject"/> instance
    /// this behavior is attached to.
    /// </summary>
    /// <param name="targetObject">A dependency object this behavior is attached to.</param>
    /// <param name="newValue">The new local value of the attached property.</param>
    protected abstract void OnValueAssociated(TTarget targetObject, TProperty newValue);

    /// <summary>
    /// Called when this behavior is being disassociated from a local value previously set on a <see cref="DependencyObject"/>
    /// instance this behavior is attached to.
    /// </summary>
    /// <param name="targetObject">A dependency object this behavior is attached to.</param>
    /// <param name="oldValue">The previous local value for the attached property.</param>
    protected abstract void OnValueDisassociated(TTarget targetObject, TProperty oldValue);

    private void OnAttachChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
    {
        if (sender is not TTarget targetObject)
            throw new InvalidOperationException(Strings.BehaviorUnsupportedTargetObject.InvariantFormat(typeof(TTarget)));

        if (e.NewValue == e.OldValue)
            return;

        if (e.OldValue is TProperty oldValue)
            OnValueDisassociated(targetObject, oldValue);
            
        var newValue = (TProperty?)e.NewValue;

        if (newValue == null)
        {
            Detach(targetObject);
            return;
        }
            
        Attach(targetObject);
            
        OnValueAssociated(targetObject, newValue);
    }
}