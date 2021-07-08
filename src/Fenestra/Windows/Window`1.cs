//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Windows;
using BadEcho.Fenestra.Properties;

namespace BadEcho.Fenestra.Windows
{
    /// <summary>
    /// Provides a basic window for the Fenestra framework that additionally offers support for a specific type of data
    /// context.
    /// </summary>
    /// <typeparam name="T">The type of data context used by the window.</typeparam>
    public class Window<T> : Window
    {
        static Window() =>
            DataContextProperty.OverrideMetadata(typeof(Window<T>), new FrameworkPropertyMetadata(OnDataContextChanged));

        /// <summary>
        /// Gets or sets the typed data context for an element when it participated in data binding.
        /// </summary>
        public new T DataContext
        {
            get => (T) GetValue(DataContextProperty);
            set => SetValue(DataContextProperty, value);
        }

        private static void OnDataContextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue == null)
                return;
            
            if (e.NewValue is not T)
                throw new InvalidOperationException(Strings.IncompatibleDataContextType);
        }
    }
}
