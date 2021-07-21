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
                AssociateColumnDefinitions,
                DisassociateColumnDefinitions,
                NameOf.ReadDependencyPropertyName(() => ColumnDefinitionsProperty));

        /// <summary>
        /// Identifies the attached property that gets or sets the row definitions of a grid.
        /// </summary>
        public static readonly DependencyProperty RowDefinitionsProperty
            = CreateBehavior<Grid, SizeDefinitionCollection>(
                AssociateRowDefinitions,
                DisassociateRowDefinitions,
                NameOf.ReadDependencyPropertyName(() => RowDefinitionsProperty));

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

        /// <summary>
        /// Gets the row size definitions for the provided <see cref="Grid"/> instance.
        /// </summary>
        /// <param name="source">The <see cref="Grid"/> to get the row definitions for.</param>
        /// <returns>The row size definitions for <c>source</c>.</returns>
        public static SizeDefinitionCollection GetRowDefinitions(Grid source)
        {
            Require.NotNull(source, nameof(source));

            return (SizeDefinitionCollection) source.GetValue(RowDefinitionsProperty);
        }

        /// <summary>
        /// Sets the row size definitions for the provided <see cref="Grid"/> instance.
        /// </summary>
        /// <param name="source">The <see cref="Grid"/> to set the row definitions for.</param>
        /// <param name="collection">The collection of row size definitions to set.</param>
        public static void SetRowDefinitions(Grid source, SizeDefinitionCollection collection)
        {
            Require.NotNull(source, nameof(source));

            source.SetValue(RowDefinitionsProperty, collection);
        }

        private static DependencyProperty CreateBehavior<TTarget, TParameter>(Action<TTarget, TParameter> associationAction,
                                                                              Action<TTarget, TParameter> disassociationAction,
                                                                              string propertyName) where TTarget : DependencyObject
        {
            var behavior = new DelegateBehavior<TTarget, TParameter>(associationAction, disassociationAction);

            return
                DependencyProperty.RegisterAttached(propertyName,
                                                    typeof(TParameter),
                                                    typeof(GridBehaviors),
                                                    behavior.DefaultMetadata);
        }

        private static void AssociateColumnDefinitions(Grid target, SizeDefinitionCollection collection)
        {
            target.ColumnDefinitions.Clear();

            IEnumerable<ColumnDefinition> copiedDefinitions
                = collection.Select(d => new ColumnDefinition {SharedSizeGroup = d.SharedSizeGroup, Width = d.Size});

            foreach (var copiedDefinition in copiedDefinitions)
            {
                target.ColumnDefinitions.Add(copiedDefinition);
            }
        }

        private static void DisassociateColumnDefinitions(Grid target, SizeDefinitionCollection? collection) 
            => target.ColumnDefinitions.Clear();

        private static void AssociateRowDefinitions(Grid target, SizeDefinitionCollection collection)
        {
            target.RowDefinitions.Clear();

            IEnumerable<RowDefinition> copiedDefinitions
                = collection.Select(d => new RowDefinition {SharedSizeGroup = d.SharedSizeGroup, Height = d.Size});

            foreach (var copiedDefinition in copiedDefinitions)
            {
                target.RowDefinitions.Add(copiedDefinition);
            }
        }
        
        private static void DisassociateRowDefinitions(Grid target, SizeDefinitionCollection? collection) 
            => target.RowDefinitions.Clear();
    }
}
