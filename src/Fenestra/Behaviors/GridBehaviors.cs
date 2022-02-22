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

namespace BadEcho.Fenestra.Behaviors;

/// <summary>
/// Provides behaviors that target and augment <see cref="Grid"/> controls.
/// </summary>
public static class GridBehaviors
{
    /// <summary>
    /// Identifies the attached property that gets or sets the column definitions of a grid.
    /// </summary>
    public static readonly DependencyProperty ColumnDefinitionsProperty
        = DelegateBehaviorFactory.Create<Grid, SizeDefinitionCollection>(
            AssociateColumnDefinitions,
            DisassociateColumnDefinitions,
            NameOf.ReadDependencyPropertyName(() => ColumnDefinitionsProperty),
            typeof(GridBehaviors));

    /// <summary>
    /// Identifies the attached property that gets or sets the row definitions of a grid.
    /// </summary>
    public static readonly DependencyProperty RowDefinitionsProperty
        = DelegateBehaviorFactory.Create<Grid, SizeDefinitionCollection>(
            AssociateRowDefinitions,
            DisassociateRowDefinitions,
            NameOf.ReadDependencyPropertyName(() => RowDefinitionsProperty),
            typeof(GridBehaviors));

    /// <summary>
    /// Gets the value of the <see cref="ColumnDefinitionsProperty"/> attached property for a given <see cref="Grid"/>.
    /// </summary>
    /// <param name="source">The grid from which the property value is read.</param>
    /// <returns>The column size definitions for <c>source</c>.</returns>
    public static SizeDefinitionCollection GetColumnDefinitions(Grid source)
    {
        Require.NotNull(source, nameof(source));

        return (SizeDefinitionCollection) source.GetValue(ColumnDefinitionsProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="ColumnDefinitionsProperty"/> attached property on a given <see cref="Grid"/>.
    /// </summary>
    /// <param name="source">The grid to which the attached property is written.</param>
    /// <param name="collection">The collection of column size definitions to set.</param>
    public static void SetColumnDefinitions(Grid source, SizeDefinitionCollection collection)
    {
        Require.NotNull(source, nameof(source));

        source.SetValue(ColumnDefinitionsProperty, collection);
    }

    /// <summary>
    /// Gets the value of the <see cref="RowDefinitionsProperty"/> attached property for a given <see cref="Grid"/>.
    /// </summary>
    /// <param name="source">The grid from which the property value is read.</param>
    /// <returns>The row size definitions for <c>source</c>.</returns>
    public static SizeDefinitionCollection GetRowDefinitions(Grid source)
    {
        Require.NotNull(source, nameof(source));

        return (SizeDefinitionCollection) source.GetValue(RowDefinitionsProperty);
    }

    /// <summary>
    /// Sets the value of the <see cref="RowDefinitionsProperty"/> attached property on a given <see cref="Grid"/>.
    /// </summary>
    /// <param name="source">The grid to which the attached property is written.</param>
    /// <param name="collection">The collection of row size definitions to set.</param>
    public static void SetRowDefinitions(Grid source, SizeDefinitionCollection collection)
    {
        Require.NotNull(source, nameof(source));

        source.SetValue(RowDefinitionsProperty, collection);
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