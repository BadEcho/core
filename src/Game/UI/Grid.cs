//-----------------------------------------------------------------------
// <copyright>
//      Created by Matt Weber <matt@badecho.com>
//      Copyright @ 2023 Bad Echo LLC. All rights reserved.
//
//      Bad Echo Technologies are licensed under the
//      GNU Affero General Public License v3.0.
//
//      See accompanying file LICENSE.md or a copy at:
//      https://www.gnu.org/licenses/agpl-3.0.html
// </copyright>
//-----------------------------------------------------------------------

using BadEcho.Extensions;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a panel that consists of columns and rows.
/// </summary>
public sealed class Grid : Panel
{
    private static readonly GridDimension _DefaultDimension 
        = new(1.0f, GridDimensionUnit.Proportional);

    private readonly List<Control> _visibleChildren = new();
    private readonly List<int> _columnWidths = new();
    private readonly List<int> _rowHeights = new();

    private List<Control>?[,] _cells 
        = new List<Control>[0,0];

    /// <summary>
    /// Gets specified measurements for each column of this <see cref="Grid"/>.
    /// </summary>
    public IList<GridDimension> Columns
    { get; } = new List<GridDimension>();

    /// <summary>
    /// Gets specified measurements for each row of this <see cref="Grid"/>.
    /// </summary>
    public IList<GridDimension> Rows
    { get; } = new List<GridDimension>();

    /// <inheritdoc/>
    protected override Size MeasureCore(Size availableSize)
    {
        _visibleChildren.Clear();
        _visibleChildren.AddRange(Children.Where(c => c.IsVisible));

        int columns = _visibleChildren.Max(c => c.Column);
        int rows = _visibleChildren.Max(c => c.Row);

        if (Columns.Count > columns)
            columns = Columns.Count;

        if (Rows.Count > rows)
            rows = Rows.Count;

        _columnWidths.Clear();

        _columnWidths.AddRange(Enumerable.Repeat(0, columns));
        _rowHeights.AddRange(Enumerable.Repeat(0, rows));

        if (_cells.GetLength(0) < columns || _cells.GetLength(1) < rows)
            _cells = new List<Control>?[rows, columns];

        foreach (Control child in _visibleChildren)
        {
            List<Control> cell = _cells[child.Row, child.Column] ??= new List<Control>();

            cell.Add(child);
        }

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GridDimension rowDimension = GetRow(row);
                GridDimension columnDimension = GetColumn(column);

                if (rowDimension.Unit == GridDimensionUnit.Absolute)
                    _rowHeights[row] = (int) rowDimension.Value;

                if (columnDimension.Unit == GridDimensionUnit.Absolute)
                    _columnWidths[column] = (int) columnDimension.Value;

                List<Control> cellChildren = _cells[row, column] ??= new List<Control>();

                foreach (Control cellChild in cellChildren)
                {
                    Size desiredSize = Size.Empty;

                    if (rowDimension.Unit != GridDimensionUnit.Absolute || columnDimension.Unit != GridDimensionUnit.Absolute)
                    {
                        cellChild.Measure(availableSize);

                        desiredSize = cellChild.DesiredSize;
                    }

                    if (desiredSize.Width > _columnWidths[column] && columnDimension.Unit != GridDimensionUnit.Absolute)
                        _columnWidths[column] = desiredSize.Width;

                    if (desiredSize.Height > _rowHeights[row] && rowDimension.Unit != GridDimensionUnit.Absolute)
                        _rowHeights[row] = desiredSize.Height;
                }
            }
        }

        var proportionalColumns
            = _columnWidths.Select((width, index) => new { Width = width, Index = index, Dimension = GetColumn(index) })
                           .Where(c => c.Dimension.Unit == GridDimensionUnit.Proportional)
                           .ToList();

        int maxColumnWidth = proportionalColumns.Max(c => c.Width);

        foreach (var proportionalColumn in proportionalColumns)
        {
            _columnWidths[proportionalColumn.Index] 
                = (int) (maxColumnWidth * proportionalColumn.Dimension.Value);
        }

        var proportionalRows
            = _rowHeights.Select((height, index) => new { Height = height, Index = index, Dimension = GetRow(index) })
                         .Where(r => r.Dimension.Unit == GridDimensionUnit.Proportional)
                         .ToList();

        int maxRowHeight = proportionalRows.Max(r => r.Height);

        foreach (var proportionalRow in proportionalRows)
        {
            _rowHeights[proportionalRow.Index]
                = (int) (maxRowHeight * proportionalRow.Dimension.Value);
        }

        int desiredWidth = _columnWidths.Sum();
        int desiredHeight = _rowHeights.Sum();

        return new Size(desiredWidth, desiredHeight);
    }

    /// <inheritdoc/>
    protected override void ArrangeCore()
    {
        base.ArrangeCore();

        float availableWidth = ContentBounds.Width;
        float totalProportions = 0.0f;

        for (int column = 0; column < _columnWidths.Count; column++)
        {
            GridDimension dimension = GetColumn(column);

            if (dimension.Unit is GridDimensionUnit.Auto or GridDimensionUnit.Absolute)
                availableWidth -= _columnWidths[column];
            else
                totalProportions += dimension.Value;
        }

        if (!totalProportions.ApproximatelyEquals(0.0f))
        {
            float allocatedProportions = 0.0f;

            for (int column = 0; column < _columnWidths.Count; column++)
            {
                GridDimension dimension = GetColumn(column);

                if (dimension.Unit == GridDimensionUnit.Proportional)
                {
                    _columnWidths[column] = (int) (dimension.Value * availableWidth / totalProportions);
                    allocatedProportions += _columnWidths[column];
                }
            }

            availableWidth -= allocatedProportions;
        }

        // Do the same with rows...
    }

    private GridDimension GetRow(int row) 
        => row >= Rows.Count ? _DefaultDimension : Rows[row];

    private GridDimension GetColumn(int column)
        => column >= Columns.Count ? _DefaultDimension : Columns[column];
}
