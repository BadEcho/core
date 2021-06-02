//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2021 Bad Echo LLC. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using BadEcho.Odin;

namespace BadEcho.Fenestra.Behaviors
{
    /// <summary>
    /// Provides behaviors that target and augment <see cref="Grid"/> controls.
    /// </summary>
    public static class GridBehaviors
    {
        /// <summary>
        /// Identifies the attached property that gets or sets the column definitions of a grid.
        /// </summary>
        public static readonly DependencyProperty ColumnDefinitionsProperty
            = CreateBehavior<Grid, SizeDefinitionCollection>(
                ColumnDefinitionsAction,
                NameOf.ReadDependencyPropertyName(() => ColumnDefinitionsProperty));

        /// <summary>
        /// Gets the column size definitions for the provided <see cref="Grid"/> instance.
        /// </summary>
        /// <param name="source">The <see cref="Grid"/> to get the column definitions for.</param>
        /// <returns>The column size definitions for <c>source</c>.</returns>
        public static SizeDefinitionCollection GetColumnDefinitions(Grid source)
        {
            Require.NotNull(source, nameof(source));

            return (SizeDefinitionCollection) source.GetValue(ColumnDefinitionsProperty);
        }

        /// <summary>
        /// Sets the column size definitions for the provided <see cref="Grid"/> instance.
        /// </summary>
        /// <param name="source">The <see cref="Grid"/> to set the column size definitions on.</param>
        /// <param name="collection">The collection of column size definitions to set.</param>
        public static void SetColumnDefinitions(Grid source, SizeDefinitionCollection collection)
        {
            Require.NotNull(source, nameof(source));

            source.SetValue(ColumnDefinitionsProperty, collection);
        }

        private static DependencyProperty CreateBehavior<TTarget, TParameter>(Action<TTarget, TParameter> action,
                                                                              string propertyName) where TTarget : DependencyObject
        {
            var behavior = new DelegateBehavior<TTarget, TParameter>(action);

            return
                DependencyProperty.RegisterAttached(propertyName,
                                                    typeof(TParameter),
                                                    typeof(GridBehaviors),
                                                    behavior.DefaultMetadata);
        }

        private static void ColumnDefinitionsAction(Grid target, SizeDefinitionCollection? collection)
        {
            target.ColumnDefinitions.Clear();

            if (collection == null)
                return;

            IEnumerable<ColumnDefinition> copiedDefinitions
                = collection.Select(d => new ColumnDefinition {SharedSizeGroup = d.SharedSizeGroup, Width = d.Size});

            foreach (var copiedDefinition in copiedDefinitions)
            {
                target.ColumnDefinitions.Add(copiedDefinition);
            }
        }
    }
}
