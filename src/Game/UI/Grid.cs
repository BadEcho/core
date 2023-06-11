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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics.CodeAnalysis;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a panel that consists of columns and rows.
/// </summary>
public sealed class Grid : Panel
{
    private static readonly GridDimension _DefaultDimension 
        = new(1.0f, GridDimensionUnit.Auto);

    private readonly List<Control> _visibleChildren = new();
    private readonly List<int> _columnWidths = new();
    private readonly List<int> _rowHeights = new();
    private readonly List<int> _cellsX = new();
    private readonly List<int> _cellsY = new();

    private List<Control>?[,] _cells 
        = new List<Control>[0,0];

    private int? _mouseOverColumn;
    private int? _mouseOverRow;
    private int? _selectedColumn;
    private int? _selectedRow;

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

    /// <summary>
    /// Gets or sets the background visual of a cell when the mouse is hovering over it.
    /// </summary>
    public IVisual? MouseOverCellBackground
    { get; set; }

    /// <summary>
    /// Gets or sets the background visual of a cell when it has been selected.
    /// </summary>
    public IVisual? SelectedCellBackground
    { get; set; }

    /// <summary>
    /// Gets or sets a value indicating if the cells of this <see cref="Grid"/> can be selected by the user.
    /// </summary>
    public bool IsSelectable
    { get; set; }

    /// <summary>
    /// Gets a value indicating if the mouse pointer is located over a non-selected cell.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_mouseOverColumn), nameof(_mouseOverRow))]
    public bool IsMouseOverCell
        => _mouseOverColumn.HasValue && _mouseOverRow.HasValue
            && (_mouseOverColumn != _selectedColumn || _mouseOverRow != _selectedRow);

    /// <summary>
    /// Gets a value indicating if a cell has been selected.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_selectedColumn), nameof(_selectedRow))]
    public bool IsCellSelected
        => _selectedColumn != null && _selectedRow != null;

    /// <inheritdoc/>
    protected override Size MeasureCore(Size availableSize)
    {
        _visibleChildren.Clear();
        _visibleChildren.AddRange(Children.Where(c => c.IsVisible));

        int columns = _visibleChildren.Max(c => c.Column) + 1;
        int rows = _visibleChildren.Max(c => c.Row) + 1;

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

        if (proportionalColumns.Any())
        {
            int maxColumnWidth = proportionalColumns.Max(c => c.Width);

            foreach (var proportionalColumn in proportionalColumns)
            {
                _columnWidths[proportionalColumn.Index]
                    = (int) (maxColumnWidth * proportionalColumn.Dimension.Value);
            }
        }

        var proportionalRows
            = _rowHeights.Select((height, index) => new { Height = height, Index = index, Dimension = GetRow(index) })
                         .Where(r => r.Dimension.Unit == GridDimensionUnit.Proportional)
                         .ToList();

        if (proportionalRows.Any())
        {
            int maxRowHeight = proportionalRows.Max(r => r.Height);

            foreach (var proportionalRow in proportionalRows)
            {
                _rowHeights[proportionalRow.Index]
                    = (int) (maxRowHeight * proportionalRow.Dimension.Value);
            }
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

        for (int column = 0; column < _columnWidths.Count; column++)
        {
            GridDimension dimension = GetColumn(column);

            if (dimension.Unit == GridDimensionUnit.Fill)
            {
                _columnWidths[column] = (int) availableWidth;
                break;
            }
        }

        // Do the same with rows...
        float availableHeight = ContentBounds.Height;
        totalProportions = 0.0f;

        for (int row = 0; row < _rowHeights.Count; row++)
        {
            GridDimension dimension = GetRow(row);

            if (dimension.Unit is GridDimensionUnit.Auto or GridDimensionUnit.Absolute)
                availableHeight -= _rowHeights[row];
            else
                totalProportions += dimension.Value;
        }

        if (!totalProportions.ApproximatelyEquals(0.0f))
        {
            float allocatedProportions = 0.0f;

            for (int row = 0; row < _rowHeights.Count; row++)
            {
                GridDimension dimension = GetRow(row);

                if (dimension.Unit == GridDimensionUnit.Proportional)
                {
                    _rowHeights[row] = (int) (dimension.Value * availableHeight / totalProportions);
                    allocatedProportions += _rowHeights[row];
                }
            }

            availableHeight -= allocatedProportions;
        }

        for (int row = 0; row < _rowHeights.Count; row++)
        {
            GridDimension dimension = GetRow(row);

            if (dimension.Unit == GridDimensionUnit.Fill)
            {
                _rowHeights[row] = (int) availableHeight;
                break;
            }
        }

        _cellsX.Clear();

        int nextChildPosition = 0;

        foreach (int columnWidth in _columnWidths)
        {
            _cellsX.Add(nextChildPosition + ContentBounds.X);
            nextChildPosition += columnWidth;
        }

        _cellsY.Clear();

        nextChildPosition = 0;

        foreach (int rowHeight in _rowHeights)
        {
            _cellsY.Add(nextChildPosition + ContentBounds.Y);
            nextChildPosition += rowHeight;
        }

        foreach (Control child in _visibleChildren)
        {
            int cellWidth = _columnWidths[child.Column];
            int cellHeight = _rowHeights[child.Row];

            var effectiveChildArea = new Rectangle(_cellsX[child.Column],
                                                   _cellsY[child.Row],
                                                   cellWidth,
                                                   cellHeight);

            if (effectiveChildArea.Right > ContentBounds.Right)
                effectiveChildArea.Width = ContentBounds.Right - effectiveChildArea.X;

            if (effectiveChildArea.Width < 0)
                effectiveChildArea.Width = 0;

            if (effectiveChildArea.Bottom > ContentBounds.Bottom)
                effectiveChildArea.Height = ContentBounds.Bottom - effectiveChildArea.Y;

            if (effectiveChildArea.Height < 0)
                effectiveChildArea.Height = 0;

            child.Arrange(effectiveChildArea);
        }
    }

    /// <inheritdoc/>
    protected override void DrawCore(SpriteBatch spriteBatch)
    {
        if (IsSelectable && IsMouseOver)
        {
            UpdateSelection();
        }
        else if (!IsMouseOver)
        {
            _mouseOverColumn = _mouseOverRow = null;
        }

        if (MouseOverCellBackground != null && IsMouseOverCell)
        {
            int backgroundX = _cellsX[_mouseOverColumn.Value];
            int backgroundY = _cellsY[_mouseOverRow.Value];

            MouseOverCellBackground.Draw(spriteBatch,
                                         new Rectangle(backgroundX,
                                                       backgroundY,
                                                       _columnWidths[_mouseOverColumn.Value],
                                                       _rowHeights[_mouseOverRow.Value]));
        }

        if (SelectedCellBackground != null && IsCellSelected)
        {
            int backgroundX = _cellsX[_selectedColumn.Value];
            int backgroundY = _cellsY[_selectedRow.Value];

            SelectedCellBackground.Draw(spriteBatch,
                                        new Rectangle(backgroundX,
                                                      backgroundY,
                                                      _columnWidths[_selectedColumn.Value],
                                                      _rowHeights[_selectedRow.Value]));
        }

        base.DrawCore(spriteBatch);
    }

    private void UpdateSelection()
    {
        MouseState mouseState = Mouse.GetState();
        int mouseX = mouseState.Position.X;
        int mouseY = mouseState.Position.Y;

        _mouseOverColumn = _cellsX.Select((x, i) => new { X = x, Index = i })
                                  .FirstOrDefault(cell => mouseX >= cell.X && mouseX < cell.X + _columnWidths[cell.Index])
                                  ?.Index;

        _mouseOverRow = _cellsY.Select((y, i) => new { Y = y, Index = i })
                               .FirstOrDefault(cell => mouseY >= cell.Y && mouseY < cell.Y + _rowHeights[cell.Index])
                               ?.Index;

        if (mouseState.LeftButton == ButtonState.Pressed)
        {
            _selectedColumn = _mouseOverColumn;
            _selectedRow = _mouseOverRow;
        }
    }

    private GridDimension GetRow(int row) 
        => row >= Rows.Count ? _DefaultDimension : Rows[row];

    private GridDimension GetColumn(int column)
        => column >= Columns.Count ? _DefaultDimension : Columns[column];
}
