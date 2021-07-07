//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Windows;
using System.Windows.Controls;

namespace BadEcho.Fenestra
{
    /// <summary>
    /// Provides a data template selector that honors data templates for data contexts targeting particular views and defined an assembly's
    /// <c>ViewContexts.xaml</c> file.
    /// </summary>
    public class ViewContextTemplateSelector : DataTemplateSelector
    {
        /// <inheritdoc/>
        public override DataTemplate? SelectTemplate(object? item, DependencyObject container)
        {
            if (null == item || container is not FrameworkElement containerElement)
                return base.SelectTemplate(item, container);

            var contextKey = new DataTemplateKey(item.GetType());

            return containerElement.TryFindResource(contextKey) as DataTemplate ?? base.SelectTemplate(item, container);
        }
    }
}
