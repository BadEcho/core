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

using Microsoft.Xna.Framework;
using System.Diagnostics.CodeAnalysis;
using BadEcho.Game.Properties;
using Microsoft.Xna.Framework.Input;

namespace BadEcho.Game.UI;

/// <summary>
/// Provides a panel that consists of columns and rows.
/// </summary>
public sealed class Grid : Panel<Grid>, ISelectable
{
    private static readonly GridDimension _DefaultDimension 
        = new(1.0f, GridDimensionUnit.Auto);

    private readonly List<IControl> _visibleChildren = [];
    private readonly List<int> _columnWidths = [];
    private readonly List<int> _rowHeights = [];
    private readonly List<int> _cellsX = [];
    private readonly List<int> _cellsY = [];

    private List<IControl>?[,] _cells 
        = new List<IControl>[0,0];

    private int? _mouseOverColumn;
    private int? _mouseOverRow;
    private int? _selectedColumn;
    private int? _selectedRow;

    private bool _selectionBeingMade;

    /// <summary>
    /// Initializes a new instance of the <see cref="Grid"/> class.
    /// </summary>
    public Grid()
    {
        IsFocusable = true;
    }

    /// <summary>
    /// Occurs when a new cell has been selected.
    /// </summary>
    public event EventHandler<EventArgs<IEnumerable<IControl>>>? SelectionChanged;

    /// <summary>
    /// Gets specified measurements for each column of this grid.
    /// </summary>
    public IList<GridDimension> Columns
    { get; } = new List<GridDimension>();

    /// <summary>
    /// Gets specified measurements for each row of this grid.
    /// </summary>
    public IList<GridDimension> Rows
    { get; } = new List<GridDimension>();

    /// <summary>
    /// Gets or sets the default measurement value used for rows and columns that lack specified measurements during
    /// this control's measure and arrange passes.
    /// </summary>
    public GridDimension? DefaultDimension
    { get; set; }

    /// <inheritdoc/>
    public bool IsSelectable
    { get; set; }

    /// <inheritdoc/>
    public bool IsHoverPersistent
    { get; set; }

    /// <inheritdoc/>
    public IVisual? HoveredItemBackground
    { get; set; }

    /// <inheritdoc/>
    public IVisual? SelectedItemBackground
    { get; set; }

    /// <summary>
    /// Gets a value indicating if the cursor is located over a non-selected cell.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_mouseOverColumn), nameof(_mouseOverRow))]
    private bool IsCellHovered
        => _mouseOverColumn.HasValue && _mouseOverRow.HasValue
            && (_mouseOverColumn != _selectedColumn || _mouseOverRow != _selectedRow);

    /// <summary>
    /// Gets a value indicating if a cell has been selected.
    /// </summary>
    [MemberNotNullWhen(true, nameof(_selectedColumn), nameof(_selectedRow))]
    private bool IsCellSelected
        => _selectedColumn != null && _selectedRow != null;

    /// <summary>
    /// Gets a value indicating if a selection being made is currently invalid.
    /// </summary>
    /// <remarks>
    /// A selection, started via the depressing of the mouse button while the cursor is over an item, is no longer considered valid
    /// if the cursor moves away from said item while the button is still pressed. Releasing the button in this state will result in
    /// no selection.
    /// </remarks>
    private bool IsSelectionInvalid
        => _selectionBeingMade && _selectedColumn != _mouseOverColumn && _selectedRow != _mouseOverRow;

    /// <inheritdoc/>
    public override bool Focus()
    {
        if (!base.Focus())
            return false;

        if (Children.Count != 0)
            UpdateMouseOver(0, 0);

        return true;
    }

    /// <summary>
    /// Gets the coordinates to the cell whose column and row match the specified values.
    /// </summary>
    /// <param name="column">The index of the column that the cell occupies.</param>
    /// <param name="row">The index of the row that the cell occupies.</param>
    /// <returns>
    /// The <see cref="Point"/> value representing the location of the cell found at <c>column</c> and <c>row</c>.
    /// </returns>
    public Point GetCellLocation(int column, int row)
    {
        ValidateDimensions(column, row);

        return new Point(_cellsX[column], _cellsY[row]);
    }

    /// <summary>
    /// Gets the size of the cell whose column and row match the specified values.
    /// </summary>
    /// <param name="column">The index of the column that the cell occupies.</param>
    /// <param name="row">The index of the row that the cell occupies.</param>
    /// <returns>
    /// The <see cref="Size"/> value representing the size of the cell found at <c>column</c> and <c>row</c>.
    /// </returns>
    public Size GetCellSize(int column, int row)
    {
        ValidateDimensions(column, row);

        return new Size(_columnWidths[column], _rowHeights[row]);
    }

    /// <summary>
    /// Cancels the selection of any previously selected item in the grid.
    /// </summary>
    public void Unselect()
    {
        UpdateMouseOver(null, null);
        UpdateSelection(null, null);
    } 

    /// <inheritdoc/>
    protected override Size MeasureCore(Size availableSize)
    {
        _visibleChildren.Clear();
        _columnWidths.Clear();
        _rowHeights.Clear();

        if (Children.Count == 0)
            return Size.Empty;

        _visibleChildren.AddRange(Children.Where(c => c.IsVisible));

        int columns = _visibleChildren.Max(c => c.Column) + 1;
        int rows = _visibleChildren.Max(c => c.Row) + 1;

        if (Columns.Count > columns)
            columns = Columns.Count;

        if (Rows.Count > rows)
            rows = Rows.Count;

        _columnWidths.AddRange(Enumerable.Repeat(0, columns));
        _rowHeights.AddRange(Enumerable.Repeat(0, rows));

        if (_cells.GetLength(0) < columns || _cells.GetLength(1) < rows)
            _cells = new List<IControl>?[rows, columns];

        foreach (IControl child in _visibleChildren)
        {
            List<IControl> cell = _cells[child.Row, child.Column] ??= [];

            cell.Add(child);
        }

        // We conduct two measure passes: the first accounting for provided discrete dimensional values, and the second
        // accounting for the measurements proportional to the sizes calculated in the first pass.
        MeasureDiscreteDimensions(availableSize);
        MeasureProportionalDimensions();

        int desiredWidth = _columnWidths.Sum();
        int desiredHeight = _rowHeights.Sum();

        return new Size(desiredWidth, desiredHeight);
    }

    /// <inheritdoc/>
    protected override void ArrangeCore()
    {
        base.ArrangeCore();

        ArrangeDimension(ContentBounds.Width, _columnWidths, GetColumn);
        ArrangeDimension(ContentBounds.Height, _rowHeights, GetRow);

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

        foreach (IControl child in _visibleChildren)
        {
            ArrangeCell(child);
        }
    }

    /// <inheritdoc/>
    protected override void DrawCore(ConfiguredSpriteBatch spriteBatch)
    {
        if (HoveredItemBackground != null && IsCellHovered)
        {
            int backgroundX = _cellsX[_mouseOverColumn.Value];
            int backgroundY = _cellsY[_mouseOverRow.Value];

            HoveredItemBackground.Draw(spriteBatch,
                                       new Rectangle(backgroundX,
                                                     backgroundY,
                                                     _columnWidths[_mouseOverColumn.Value],
                                                     _rowHeights[_mouseOverRow.Value]));
        }

        if (SelectedItemBackground != null && IsCellSelected)
        {
            if (!IsSelectionInvalid)
            {
                int backgroundX = _cellsX[_selectedColumn.Value];
                int backgroundY = _cellsY[_selectedRow.Value];

                SelectedItemBackground.Draw(spriteBatch,
                                            new Rectangle(backgroundX,
                                                          backgroundY,
                                                          _columnWidths[_selectedColumn.Value],
                                                          _rowHeights[_selectedRow.Value]));
            }
        }

        base.DrawCore(spriteBatch);
    }

    /// <inheritdoc/>
    protected override void OnMouseMove()
    {
        base.OnMouseMove();

        if (InputHandler == null)
            throw new InvalidOperationException(Strings.NoInputHandler);

        int mouseX = InputHandler.MousePosition.X;
        int mouseY = InputHandler.MousePosition.Y;

        int? column = _cellsX.Select((x, i) => new { X = x, Index = i })
                             .FirstOrDefault(cell => mouseX >= cell.X && mouseX < cell.X + _columnWidths[cell.Index])
                             ?.Index;

        int ? row = _cellsY.Select((y, i) => new { Y = y, Index = i })
                           .FirstOrDefault(cell => mouseY >= cell.Y && mouseY < cell.Y + _rowHeights[cell.Index])
                           ?.Index;
        
        UpdateMouseOver(column, row);
    }

    /// <inheritdoc/>
    protected override void OnMouseLeave()
    {
        base.OnMouseLeave();

        if (!IsHoverPersistent)
            UpdateMouseOver(null, null);
    }

    /// <inheritdoc/>
    protected override void OnMouseDown(MouseButton pressedButton)
    {
        base.OnMouseDown(pressedButton);
        
        if (pressedButton == MouseButton.Left)
        {
            _selectionBeingMade = true;
            SelectHoveredItem(false);
        }
    }

    /// <inheritdoc/>
    protected override void OnMouseUp(MouseButton releasedButton)
    {
        base.OnMouseUp(releasedButton);

        if (releasedButton == MouseButton.Left)
        {
            if (IsSelectionInvalid)
            {
                UpdateSelection(null, null);
            }
            else if (_selectionBeingMade && IsCellSelected)
            {
                IEnumerable<IControl>? selectedControls = _cells[_selectedRow.Value, _selectedColumn.Value];

                if (selectedControls != null)
                    SelectionChanged?.Invoke(this, new EventArgs<IEnumerable<IControl>>(selectedControls));
            }

            _selectionBeingMade = false;
        }
    }

    /// <inheritdoc/>
    protected override void OnKeyDown(Keys pressedKey)
    {
        base.OnKeyDown(pressedKey);

        if (!IsSelectable || Children.Count == 0)
            return;

        switch (pressedKey)
        {
            case Keys.Enter:
            case Keys.Space:
                if (_mouseOverColumn != null && _mouseOverRow != null)
                    SelectHoveredItem(true);
                break;

            case Keys.Left:
                _mouseOverRow ??= 0;
                _mouseOverColumn = WrapToGrid((_mouseOverColumn ?? 0) - 1, _cellsX.Count);
                break;

            case Keys.Right:
                _mouseOverRow ??= 0;
                _mouseOverColumn = WrapToGrid((_mouseOverColumn ?? -1) + 1, _cellsX.Count);
                break;

            case Keys.Up:
                _mouseOverColumn ??= 0;
                _mouseOverRow = WrapToGrid((_mouseOverRow ?? 0) - 1, _cellsY.Count);
                break;

            case Keys.Down:
                _mouseOverColumn ??= 0;
                _mouseOverRow = WrapToGrid((_mouseOverRow ?? -1) + 1, _cellsY.Count);
                break;
        }
    }
    
    private static int WrapToGrid(int index, int dimensionalMax)
    {
        int remainder = index % dimensionalMax;

        return remainder < 0 ? dimensionalMax - 1 : remainder;
    }

    private static void ArrangeDimension(int availableSpace, IList<int> measurements, Func<int, GridDimension> dimensionSelector)
    {
        float totalProportions = 0.0f;
        var proportionalMeasurements = new List<(int Index, GridDimension Dimension)>();

        for (int i = 0; i < measurements.Count; i++)
        {
            GridDimension dimension = dimensionSelector(i);

            if (dimension.Unit is GridDimensionUnit.Auto or GridDimensionUnit.Absolute)
                availableSpace -= measurements[i];
            else
            {
                totalProportions += dimension.Value;
                proportionalMeasurements.Add((i, dimension));
            }
        }

        foreach (var (index, dimension) in proportionalMeasurements)
        {
            if (dimension.Unit == GridDimensionUnit.Proportional)
            {
                measurements[index] = (int)(dimension.Value * availableSpace / totalProportions);
            }
        }
    }

    private void ArrangeCell(IControl child)
    {
        int cellWidth = _columnWidths[child.Column];
        int cellHeight = _rowHeights[child.Row];

        var effectiveChildArea = new Rectangle(_cellsX[child.Column],
                                               _cellsY[child.Row],
                                               cellWidth,
                                               cellHeight);

        // Ensure the cell's size is clamped to its allotted area.
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

    private void MeasureDiscreteDimensions(Size availableSize)
    {   // For our first measure pass, we make some initial discrete cell measurements using either a provided absolute
        // dimensional value or the size properties of the cell's content, based on said cell's dimensional configuration.
        for (int row = 0; row < _rowHeights.Count; row++)
        {
            GridDimension rowDimension = GetRow(row);

            if (rowDimension.Unit == GridDimensionUnit.Absolute)
                _rowHeights[row] = (int) rowDimension.Value;

            for (int column = 0; column < _columnWidths.Count; column++)
            {
                GridDimension columnDimension = GetColumn(column);

                if (columnDimension.Unit == GridDimensionUnit.Absolute)
                    _columnWidths[column] = (int) columnDimension.Value;

                List<IControl> cellChildren = _cells[row, column] ??= [];

                // If both the row and column are using absolute values for their measurements, we're done measuring this column.
                // The desired sizes of cell controls take a back seat to absolute value dimensional definitions.
                if (rowDimension.Unit == GridDimensionUnit.Absolute && columnDimension.Unit == GridDimensionUnit.Absolute)
                    continue;

                foreach (IControl cellChild in cellChildren)
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
    }

    private void MeasureProportionalDimensions()
    {   // After the initial measure pass, we'll need to do another one in order to apply specified multiplier
        // values to the desired sizes of any proportionally dimensioned cell controls.
        var proportionalColumns
            = _columnWidths.Select((width, index) => new { Width = width, Index = index, Dimension = GetColumn(index) })
                           .Where(c => c.Dimension.Unit == GridDimensionUnit.Proportional)
                           .ToList();

        if (proportionalColumns.Count > 0)
        {
            int maxColumnWidth = proportionalColumns.Max(c => c.Width);

            foreach (var proportionalColumn in proportionalColumns)
            {
                _columnWidths[proportionalColumn.Index]
                    = (int)(maxColumnWidth * proportionalColumn.Dimension.Value);
            }
        }

        var proportionalRows
            = _rowHeights.Select((height, index) => new { Height = height, Index = index, Dimension = GetRow(index) })
                         .Where(r => r.Dimension.Unit == GridDimensionUnit.Proportional)
                         .ToList();

        if (proportionalRows.Count > 0)
        {
            int maxRowHeight = proportionalRows.Max(r => r.Height);

            foreach (var proportionalRow in proportionalRows)
            {
                _rowHeights[proportionalRow.Index]
                    = (int)(maxRowHeight * proportionalRow.Dimension.Value);
            }
        }
    }

    private void SelectHoveredItem(bool notify)
    {
        int? previousSelectedColumn = _selectedColumn;
        int? previousSelectedRow = _selectedRow;

        UpdateSelection(_mouseOverColumn, _mouseOverRow);

        if (IsCellSelected && (previousSelectedColumn != _selectedColumn || previousSelectedRow != _selectedRow))
        {
            IEnumerable<IControl>? selectedControls = _cells[_selectedRow.Value, _selectedColumn.Value];

            if (notify && selectedControls != null)
                SelectionChanged?.Invoke(this, new EventArgs<IEnumerable<IControl>>(selectedControls));
        }
    }

    private GridDimension GetRow(int row) 
        => row >= Rows.Count ? DefaultDimension ?? _DefaultDimension : Rows[row];

    private GridDimension GetColumn(int column)
        => column >= Columns.Count ? DefaultDimension ?? _DefaultDimension : Columns[column];

    private void ValidateDimensions(int column, int row)
    {
        if (column < 0 || column >= _cellsX.Count)
            throw new ArgumentOutOfRangeException(nameof(column), Strings.GridColumnOutOfRange);
        
        if (row < 0 || row >= _cellsY.Count)
            throw new ArgumentOutOfRangeException(nameof(row), Strings.GridRowOutOfRange);
    }
    
    private void UpdateMouseOver(int? column, int? row)
    {
        _mouseOverColumn = column;
        _mouseOverRow = row;
    }

    private void UpdateSelection(int? column, int? row)
    {
        _selectedColumn = column;
        _selectedRow = row;
    }
}
