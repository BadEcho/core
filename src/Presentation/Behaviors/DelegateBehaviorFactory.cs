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

namespace BadEcho.Presentation.Behaviors;

/// <summary>
/// Provides a factory class for registering dependency properties as attachable delegate behaviors.
/// </summary>
public static class DelegateBehaviorFactory
{
    /// <summary>
    /// Creates a <see cref="DependencyProperty"/> instance with an attached behavior that will execute the provided
    /// method upon parameter association.
    /// </summary>
    /// <typeparam name="TTarget">The type of <see cref="DependencyObject"/> the behavior will attach to.</typeparam>
    /// <typeparam name="TParameter">The type of parameter accepted by the method executed by the behavior.</typeparam>
    /// <param name="associationAction">
    /// The method to execute upon parameter information becoming associated with the behavior.
    /// </param>
    /// <param name="propertyName">The name of the <see cref="DependencyProperty"/>.</param>
    /// <param name="ownerType">The type of object that will own this property.</param>
    /// <returns>
    /// A <see cref="DependencyProperty"/> instance that will execute the <c>associationAction</c> method upon parameter
    /// information association with the object it's attached to.
    /// </returns>
    public static DependencyProperty Create<TTarget, TParameter>(Action<TTarget, TParameter> associationAction,
                                                                 string propertyName,
                                                                 Type ownerType) where TTarget : DependencyObject
    {
        var behavior = new DelegateBehavior<TTarget, TParameter>(associationAction);

        return Register<TParameter>(propertyName, ownerType, behavior.DefaultMetadata);
    }

    /// <summary>
    /// Creates a <see cref="DependencyProperty"/> instance with an attached behavior that will execute the two provided
    /// methods upon parameter association and disassociation, respectively.
    /// </summary>
    /// <typeparam name="TTarget">The type of <see cref="DependencyObject"/> the behavior will attach to.</typeparam>
    /// <typeparam name="TParameter">The type of parameter accepted by the method executed by the behavior.</typeparam>
    /// <param name="associationAction">
    /// The method to execute upon parameter information becoming associated with the behavior.
    /// </param>
    /// <param name="disassociationAction">
    /// The method to execute upon parameter information becoming disassociated with the behavior.
    /// </param>
    /// <param name="propertyName">The name of the <see cref="DependencyProperty"/>.</param>
    /// <param name="ownerType">The type of object that will own this property.</param>
    /// <returns>
    /// A <see cref="DependencyProperty"/> instance that will execute the <c>associationAction</c> and
    /// <c>disassociationAction</c> methods upon parameter information association and disassociation, respectively, with
    /// the object it's attached to.
    /// </returns>
    public static DependencyProperty Create<TTarget, TParameter>(Action<TTarget, TParameter> associationAction,
                                                                 Action<TTarget, TParameter> disassociationAction,
                                                                 string propertyName,
                                                                 Type ownerType) where TTarget : DependencyObject
    {
        var behavior = new DelegateBehavior<TTarget, TParameter>(associationAction, disassociationAction);

        return Register<TParameter>(propertyName, ownerType, behavior.DefaultMetadata);
    }

    private static DependencyProperty Register<TParameter>(string propertyName, Type ownerType, PropertyMetadata propertyMetadata)
    {
        return
            DependencyProperty.RegisterAttached(propertyName,
                                                typeof(TParameter),
                                                ownerType,
                                                propertyMetadata);
    }
}