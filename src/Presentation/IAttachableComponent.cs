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

namespace BadEcho.Presentation;

/// <summary>
/// Defines a component able to provide animation support as well as participate in a logical tree's inheritance context
/// by attaching to a target dependency object.
/// </summary>
/// <typeparam name="T">The type of <see cref="DependencyObject"/> this component can attach to.</typeparam>
public interface IAttachableComponent<in T> : IAnimatable
    where T : DependencyObject
{
    /// <summary>
    /// Attaches this component to the provided target dependency object.
    /// </summary>
    /// <param name="targetObject">The dependency object to attach to.</param>
    void Attach(T targetObject);

    /// <summary>
    /// Detaches from the provided dependency object if this component is currently attached to it.
    /// </summary>
    void Detach(T targetObject);
}