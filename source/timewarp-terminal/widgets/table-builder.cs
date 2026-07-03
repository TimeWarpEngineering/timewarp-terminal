namespace TimeWarp.Terminal;

#region Design
// AddRow uses params string[] because rows are ordered, homogeneous positional data
// that maps 1:1 to already-defined columns. A row builder (.AddRow(r => r.Cell("x")))
// would add ceremony with no benefit — same positional semantics, more typing,
// no new information. Use params for filling slots, builder methods for named configuration.
#endregion

/// <summary>
/// Fluent builder for creating <see cref="Table"/> instances.
/// </summary>
/// <example>
/// <code>
/// var table = new TableBuilder()
///     .AddColumn("Name")
///     .AddColumn("Stars", Alignment.Right)
///     .AddRow("CleanArchitecture", "16.5k")
///     .Border(BorderStyle.Rounded)
///     .Build();
/// </code>
/// </example>
public sealed class TableBuilder : IBuilder<Table>
{
  private readonly Table Table = new();

  /// <summary>
  /// Adds a column with the specified header.
  /// </summary>
  /// <param name="header">The column header text.</param>
  /// <returns>This builder for method chaining.</returns>
  public TableBuilder AddColumn(string header)
  {
    _ = Table.AddColumn(header);
    return this;
  }

  /// <summary>
  /// Adds a column with the specified header and alignment.
  /// </summary>
  /// <param name="header">The column header text.</param>
  /// <param name="alignment">The column alignment.</param>
  /// <returns>This builder for method chaining.</returns>
  public TableBuilder AddColumn(string header, Alignment alignment)
  {
    _ = Table.AddColumn(header, alignment);
    return this;
  }

  /// <summary>
  /// Adds a pre-configured column to the table.
  /// </summary>
  /// <param name="column">The column to add.</param>
  /// <returns>This builder for method chaining.</returns>
  public TableBuilder AddColumn(TableColumn column)
  {
    _ = Table.AddColumn(column);
    return this;
  }

  /// <summary>
  /// Adds multiple columns with the specified headers.
  /// </summary>
  /// <param name="headers">The column header texts.</param>
  /// <returns>This builder for method chaining.</returns>
  public TableBuilder AddColumns(params string[] headers)
  {
    _ = Table.AddColumns(headers);
    return this;
  }

  /// <summary>
  /// Adds multiple pre-configured columns to the table.
  /// </summary>
  /// <param name="columns">The columns to add.</param>
  /// <returns>This builder for method chaining.</returns>
  public TableBuilder AddColumns(params TableColumn[] columns)
  {
    _ = Table.AddColumns(columns);
    return this;
  }

  /// <summary>
  /// Adds a row of data to the table.
  /// </summary>
  /// <param name="cells">The cell values for the row.</param>
  /// <returns>This builder for method chaining.</returns>
  public TableBuilder AddRow(params string[] cells)
  {
    _ = Table.AddRow(cells);
    return this;
  }

  /// <summary>
  /// Sets the border style for the table.
  /// </summary>
  /// <param name="style">The border style to use.</param>
  /// <returns>This builder for method chaining.</returns>
  public TableBuilder Border(BorderStyle style)
  {
    Table.Border = style;
    return this;
  }

  /// <summary>
  /// Sets the border color for the table.
  /// </summary>
  /// <param name="color">The ANSI color code to use.</param>
  /// <returns>This builder for method chaining.</returns>
  public TableBuilder BorderColor(string color)
  {
    Table.BorderColor = color;
    return this;
  }

  /// <summary>
  /// Hides the header row.
  /// </summary>
  /// <returns>This builder for method chaining.</returns>
  public TableBuilder HideHeaders()
  {
    Table.ShowHeaders = false;
    return this;
  }

  /// <summary>
  /// Shows separator lines between data rows.
  /// </summary>
  /// <returns>This builder for method chaining.</returns>
  public TableBuilder ShowRowSeparators()
  {
    Table.ShowRowSeparators = true;
    return this;
  }

  /// <summary>
  /// Expands the table to fill the terminal width.
  /// </summary>
  /// <returns>This builder for method chaining.</returns>
  public TableBuilder Expand()
  {
    Table.Expand = true;
    return this;
  }

  /// <summary>
  /// Builds a <see cref="Table"/> snapshot of the current builder state.
  /// Each call returns an independent table; mutating the builder afterwards
  /// does not affect previously built tables.
  /// </summary>
  /// <returns>The configured table.</returns>
  public Table Build()
  {
    Table snapshot = new()
    {
      Border = Table.Border,
      BorderColor = Table.BorderColor,
      ShowHeaders = Table.ShowHeaders,
      ShowRowSeparators = Table.ShowRowSeparators,
      Expand = Table.Expand
    };

    // Copy the lists so the snapshot is independent of the builder.
    // TableColumn instances are settings objects and may be shared.
    foreach (TableColumn column in Table.Columns)
    {
      _ = snapshot.AddColumn(column);
    }

    foreach (string[] row in Table.Rows)
    {
      _ = snapshot.AddRow(row);
    }

    return snapshot;
  }

  /// <summary>
  /// Builds a <see cref="Table"/> snapshot of the current builder state.
  /// Explicit alternative to <see cref="Build"/>.
  /// </summary>
  /// <returns>The configured table.</returns>
  public Table ToTable() => Build();
}
