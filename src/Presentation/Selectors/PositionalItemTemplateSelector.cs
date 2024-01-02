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

namespace BadEcho.Presentation.Selectors;

/// <summary>
/// Provides a data template selector which returns styles based on the items position in a collection.
/// </summary>
public sealed class PositionalItemTemplateSelector : DataTemplateSelector
{
    /// <summary>
    /// Gets or sets the data template returned by the selector for items occupying any position other than at the
    /// end of the collection.
    /// </summary>
    public DataTemplate? Template
    { get; set; }

    /// <summary>
    /// Gets or sets the data template returned by the selector for the item occupying a position at the end of the
    /// collection. 
    /// </summary>
    public DataTemplate? EndingTemplate
    { get; set; }

    /// <inheritdoc/>
    public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
    {
        ItemsControl control = ItemsControl.ItemsControlFromItemContainer(container);
        int index = control.ItemContainerGenerator.IndexFromContainer(container);

        return (index == control.Items.Count - 1 ? EndingTemplate : Template)
            ?? base.SelectTemplate(item, container);
    }
}