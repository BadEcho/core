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

using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using BadEcho.Presentation.Properties;
using BadEcho.Collections;
using BadEcho.Extensions;

namespace BadEcho.Presentation;

/// <summary>
/// Provides a base collection able to participate in a logical tree's inheritance context by attaching to a target dependency
/// object.
/// </summary>
/// <typeparam name="TTarget">The type of <see cref="DependencyObject"/> this collection can attach to.</typeparam>
/// <typeparam name="TAttachableComponent">
/// The type of <see cref="IAttachableComponent{TTarget}"/> objects in the collection.
/// </typeparam>
public abstract class AttachableComponentCollection<TTarget, TAttachableComponent> 
    : FreezableCollection<TAttachableComponent>, IAttachableComponent<TTarget> where TTarget : DependencyObject
                                                                               where TAttachableComponent : AttachableComponent<TTarget>
{
    private TTarget? _targetObject;

    /// <summary>
    /// Initializes a new instance of the <see cref="AttachableComponentCollection{TTarget,TAttachableComponent}"/> class.
    /// </summary>
    protected AttachableComponentCollection()
    {
        INotifyCollectionChanged collectionChanged = this;
            
        collectionChanged.CollectionChanged +=
            (_, e) => HandleCollectionChanged(new EmptiableNotifyCollectionChangedEventArgs(e));
    }

    /// <inheritdoc/>
    public void Attach(TTarget targetObject)
    {
        if (TargetObject.Equals<TTarget>(targetObject))
            return;

        if (TargetObject != null)
            throw new InvalidOperationException(Strings.AttachableCannotTargetMultipleObjects);

        WritePreamble();
        _targetObject = targetObject;
        WritePostscript();

        AttachItems();
    }

    /// <inheritdoc/>
    public void Detach(TTarget targetObject)
    {
        if (TargetObject == null || !TargetObject.Equals<TTarget>(targetObject))
            return;

        DetachItems();

        WritePreamble();
        _targetObject = null;
        WritePostscript();
    }

    /// <summary>
    /// Gets the target dependency object this component is attached to while assuring this <see cref="Freezable"/>
    /// is being accessed appropriately.
    /// </summary>
    private TTarget? TargetObject
    {
        get
        {
            ReadPreamble();

            return _targetObject;
        }
    }

    private void AttachItems(IList? items = null)
    {
        if (TargetObject == null)
            return;

        items ??= this;

        foreach (TAttachableComponent item in items)
        {
            item.Attach(TargetObject);
        }
    }

    private void DetachItems(IList? items = null)
    {
        if (TargetObject == null)
            return;

        items ??= this;

        foreach (TAttachableComponent item in items)
        {
            item.Detach(TargetObject);
        }
    }

    private void HandleCollectionChanged(EmptiableNotifyCollectionChangedEventArgs e)
    {
        if (TargetObject == null)
            return;

        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                AttachItems(e.NewItems);
                break;

            case NotifyCollectionChangedAction.Remove:
                DetachItems(e.OldItems);
                break;

            case NotifyCollectionChangedAction.Replace:
                DetachItems(e.OldItems);
                AttachItems(e.NewItems);
                break;

            case NotifyCollectionChangedAction.Reset:
                DetachItems();
                break;
        }
    }
}