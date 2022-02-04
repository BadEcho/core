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
using System.Windows.Controls;

namespace BadEcho.Fenestra.Selectors;

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
    public override DataTemplate? SelectTemplate(object item, DependencyObject container)
    {
        ItemsControl control = ItemsControl.ItemsControlFromItemContainer(container);
        int index = control.ItemContainerGenerator.IndexFromContainer(container);

        return (index == control.Items.Count - 1 ? EndingTemplate : Template)
            ?? base.SelectTemplate(item, container);
    }
}