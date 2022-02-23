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
using BadEcho.Presentation.Properties;
using BadEcho.Extensions;

namespace BadEcho.Presentation;

/// <summary>
/// Provides a base component able to provide animation support as well as participate in a logical tree's inheritance context
/// by attaching to a target dependency object.
/// </summary>
/// <typeparam name="T">The type of <see cref="DependencyObject"/> this component can attach to.</typeparam>
public abstract class AttachableComponent<T> : Animatable, IAttachableComponent<T>
    where T : DependencyObject
{
    private T? _targetObject;

    /// <summary>
    /// Gets the target dependency object this component is attached to while assuring this <see cref="Freezable"/>
    /// is being accessed appropriately.
    /// </summary>
    protected T? TargetObject
    {
        get
        {
            ReadPreamble();

            return _targetObject;
        }
    }

    /// <inheritdoc/>
    public void Attach(T targetObject)
    {
        if (TargetObject.Equals<T>(targetObject))
            return;

        if (TargetObject != null)
            throw new InvalidOperationException(Strings.AttachableCannotTargetMultipleObjects);

        WritePreamble();
        _targetObject = targetObject;
        WritePostscript();
    }

    /// <inheritdoc/>
    public void Detach(T targetObject)
    {
        if (TargetObject == null || !TargetObject.Equals<T>(targetObject))
            return;
            
        WritePreamble();
        _targetObject = null;
        WritePostscript();
    }
}