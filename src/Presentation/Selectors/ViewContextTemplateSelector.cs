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
using System.Windows.Controls;
using BadEcho.Extensions;

namespace BadEcho.Presentation.Selectors;

/// <summary>
/// Provides a data template selector that honors data templates for data contexts targeting particular views, defined in an
/// assembly's <c>ViewContexts.xaml</c> file.
/// </summary>
public class ViewContextTemplateSelector : DataTemplateSelector
{
    /// <inheritdoc/>
    /// <remarks>
    /// <para>
    /// We first try to locate a data template linked to the specific object's type, but if one does not exist, we
    /// fallback to the first template found associated with the most derived of interfaces for the object type.
    /// This is to avoid including templates targeting interfaces that act as a base to other interfaces, which themselves
    /// may have distinct concrete implementations.
    /// </para>
    /// <para>
    /// Interfaces are useful for the times where we may want to support view model types that exist in some sort of
    /// object hierarchy that uses generic type parameters, something which is not supported by the majority of XAML parsers.
    /// </para>
    /// </remarks>
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        if (null == item || container is not FrameworkElement containerElement)
            return base.SelectTemplate(item, container);

        Type itemType = item.GetType();
        IEnumerable<Type> interfaces = itemType.GetInterfaces().ToList();

        var template = TryFindResource(itemType) ?? interfaces.Except(interfaces.SelectMany(i => i.GetInterfaces()))
                                                              .Select(TryFindResource)
                                                              .WhereNotNull()
                                                              .FirstOrDefault();

        return template ?? base.SelectTemplate(item, container);

        DataTemplate? TryFindResource(Type type)
        {
            var contextKey = new DataTemplateKey(type);

            return containerElement.TryFindResource(contextKey) as DataTemplate;
        }
    }
}