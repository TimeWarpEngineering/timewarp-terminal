namespace TimeWarp.Terminal;

#region Purpose
// Table widget: renders formatted columnar data with headers, alignment, borders, and width management.
#endregion

#region Design
// Cell truncation preserves ANSI escape codes for all TruncateMode values: codes within the kept
// visible span stay in their original positions, and for Start/Middle the style established by the
// discarded prefix is replayed before the kept tail (AnsiStringUtils.TakeVisibleFromEnd), so a
// color opened before the cut still styles the kept text. The ellipsis is always PLAIN: any style
// active at a cut boundary is closed with a reset (AnsiStringUtils.TakeVisibleFromStart) so
// styling never bleeds into the ellipsis, padding, or borders. Cuts are grapheme-aware and never
// split a wide (two-column) grapheme; a straddling grapheme is dropped and AlignCell pads the
// shortfall.
// Width measurement and row rendering live in table-widget.layout.cs (same type).
#endregion

/// <summary>
/// Represents a table widget for rendering formatted columnar data with headers, alignment, and borders.
/// </summary>
/// <example>
/// <code>
/// // Simple table via builder
/// terminal.WriteTable(t => t
///     .AddColumn("Name")
///     .AddColumn("Stars", Alignment.Right)
///     .AddColumn("Description")
///     .AddRow("CleanArchitecture", "16.5k", "Clean Architecture template")
///     .AddRow("GuardClauses", "3.2k", "Guard clause library"));
/// </code>
/// </example>
public sealed partial class Table
{
  internal Table() { }

  private readonly List<TableColumn> ColumnsList = [];
  private readonly List<string[]> RowsList = [];

  /// <summary>
  /// Gets the columns defined for this table.
  /// </summary>
  public IReadOnlyList<TableColumn> Columns => ColumnsList;

  /// <summary>
  /// Gets the data rows in this table.
  /// </summary>
  public IReadOnlyList<string[]> Rows => RowsList;

  /// <summary>
  /// Gets or sets the border style for the table.
  /// Defaults to <see cref="BorderStyle.Square"/>.
  /// </summary>
  public BorderStyle Border { get; set; } = BorderStyle.Square;

  /// <summary>
  /// Gets or sets a value indicating whether to display the header row.
  /// Defaults to <c>true</c>.
  /// </summary>
  public bool ShowHeaders { get; set; } = true;

  /// <summary>
  /// Gets or sets a value indicating whether to display separator lines between data rows.
  /// Defaults to <c>false</c>.
  /// </summary>
  public bool ShowRowSeparators { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the table should expand to fill the terminal width.
  /// Defaults to <c>false</c>.
  /// </summary>
  /// <remarks>
  /// Has no effect when <see cref="Border"/> is <see cref="BorderStyle.None"/>; borderless tables always render at their natural width.
  /// </remarks>
  public bool Expand { get; set; }

  /// <summary>
  /// Gets or sets the ANSI color code for the border.
  /// If null, uses the terminal default color.
  /// </summary>
  public string? BorderColor { get; set; }

  /// <summary>
  /// Adds a column with the specified header and default left alignment.
  /// </summary>
  /// <param name="header">The column header text.</param>
  /// <returns>This table for method chaining.</returns>
  internal Table AddColumn(string header)
  {
    ColumnsList.Add(new TableColumn(header));
    return this;
  }

  /// <summary>
  /// Adds a column with the specified header and alignment.
  /// </summary>
  /// <param name="header">The column header text.</param>
  /// <param name="alignment">The column alignment.</param>
  /// <returns>This table for method chaining.</returns>
  internal Table AddColumn(string header, Alignment alignment)
  {
    ColumnsList.Add(new TableColumn(header, alignment));
    return this;
  }

  /// <summary>
  /// Adds a pre-configured column to the table.
  /// </summary>
  /// <param name="column">The column to add.</param>
  /// <returns>This table for method chaining.</returns>
  internal Table AddColumn(TableColumn column)
  {
    ArgumentNullException.ThrowIfNull(column);
    ColumnsList.Add(column);
    return this;
  }

  /// <summary>
  /// Adds multiple columns with the specified headers.
  /// </summary>
  /// <param name="headers">The column header texts.</param>
  /// <returns>This table for method chaining.</returns>
  internal Table AddColumns(params string[] headers)
  {
    ArgumentNullException.ThrowIfNull(headers);

    foreach (string header in headers)
    {
      ColumnsList.Add(new TableColumn(header));
    }

    return this;
  }

  /// <summary>
  /// Adds multiple pre-configured columns to the table.
  /// </summary>
  /// <param name="columns">The columns to add.</param>
  /// <returns>This table for method chaining.</returns>
  internal Table AddColumns(params TableColumn[] columns)
  {
    ArgumentNullException.ThrowIfNull(columns);
    ColumnsList.AddRange(columns);
    return this;
  }

  /// <summary>
  /// Adds a row of data to the table.
  /// </summary>
  /// <param name="cells">The cell values for the row. Can include ANSI color codes.</param>
  /// <returns>This table for method chaining.</returns>
  /// <remarks>
  /// If fewer cells are provided than columns, remaining cells will be empty.
  /// If more cells are provided than columns, extra cells are ignored.
  /// </remarks>
  internal Table AddRow(params string[] cells)
  {
    ArgumentNullException.ThrowIfNull(cells);
    RowsList.Add(cells);
    return this;
  }

  /// <summary>
  /// Renders the table to an array of strings (one per line).
  /// </summary>
  /// <param name="terminalWidth">The terminal width to use for expandable tables.</param>
  /// <returns>The rendered table lines.</returns>
  public string[] Render(int terminalWidth = 80)
  {
    if (ColumnsList.Count == 0)
    {
      return [];
    }

    int[] columnWidths = CalculateColumnWidths(terminalWidth);

    if (Border == BorderStyle.None)
    {
      return RenderWithoutBorder(columnWidths);
    }

    return RenderWithBorder(columnWidths);
  }

}
