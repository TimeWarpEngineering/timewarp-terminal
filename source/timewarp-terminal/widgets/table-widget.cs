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
public sealed class Table
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

  private int[] CalculateColumnWidths(int terminalWidth)
  {
    int[] widths = new int[ColumnsList.Count];

    // Calculate natural width for each column (max of header and all cell values)
    for (int i = 0; i < ColumnsList.Count; i++)
    {
      TableColumn column = ColumnsList[i];

      // Start with header width
      int maxWidth = AnsiStringUtils.GetVisibleLength(column.Header);

      // Check all cell values in this column
      foreach (string[] row in RowsList)
      {
        if (i < row.Length)
        {
          int cellWidth = AnsiStringUtils.GetVisibleLength(row[i]);
          maxWidth = Math.Max(maxWidth, cellWidth);
        }
      }

      // Apply max width constraint if set
      if (column.MaxWidth.HasValue)
      {
        maxWidth = Math.Min(maxWidth, column.MaxWidth.Value);
      }

      widths[i] = maxWidth;
    }

    // Calculate overhead: borders + padding + separators
    // │ cell │ cell │ = 2 outer borders + (n-1) inner separators + 2*n padding spaces
    int overhead = Border != BorderStyle.None
      ? 2 + (ColumnsList.Count - 1) + (2 * ColumnsList.Count)
      : (ColumnsList.Count - 1) * 2; // Borderless: just column separators

    // Separate grow columns from fixed columns
    bool hasGrowColumns = ColumnsList.Any(c => c.Grow);

    if (hasGrowColumns)
    {
      // Allocate fixed columns at natural width, distribute remainder to grow columns
      int fixedContentWidth = 0;
      for (int i = 0; i < ColumnsList.Count; i++)
      {
        if (!ColumnsList[i].Grow)
        {
          fixedContentWidth += widths[i];
        }
      }

      // A grow column is never sized below max(4, MinWidth)
      int[] growMinWidths = new int[ColumnsList.Count];
      int totalGrowMin = 0;
      int growCount = 0;
      for (int i = 0; i < ColumnsList.Count; i++)
      {
        if (ColumnsList[i].Grow)
        {
          growMinWidths[i] = Math.Max(4, ColumnsList[i].MinWidth ?? 4);
          totalGrowMin += growMinWidths[i];
          growCount++;
        }
      }

      int availableForGrow = terminalWidth - overhead - fixedContentWidth;

      if (availableForGrow >= totalGrowMin)
      {
        // Give each grow column its minimum, then distribute the surplus evenly
        int surplus = availableForGrow - totalGrowMin;
        int perGrowColumn = surplus / growCount;
        int remainder = surplus % growCount;
        int growIndex = 0;

        for (int i = 0; i < ColumnsList.Count; i++)
        {
          if (ColumnsList[i].Grow)
          {
            widths[i] = growMinWidths[i] + perGrowColumn + (growIndex < remainder ? 1 : 0);
            growIndex++;
          }
        }
      }
      else
      {
        // Not enough space — grow columns hold at their minimum width and the
        // remaining overflow is shrunk out of the fixed columns proportionally
        for (int i = 0; i < ColumnsList.Count; i++)
        {
          if (ColumnsList[i].Grow)
          {
            widths[i] = growMinWidths[i];
          }
        }

        int excessWidth = overhead + fixedContentWidth + totalGrowMin - terminalWidth;

        if (excessWidth > 0)
        {
          int[] minWidths = new int[ColumnsList.Count];
          for (int i = 0; i < ColumnsList.Count; i++)
          {
            minWidths[i] = ColumnsList[i].MinWidth ?? 4;
          }

          // Shrink fixed columns proportionally (grow columns are already at minimum)
          int[] shrinkableAmounts = new int[ColumnsList.Count];
          int totalShrinkable = 0;
          for (int i = 0; i < ColumnsList.Count; i++)
          {
            if (!ColumnsList[i].Grow)
            {
              shrinkableAmounts[i] = Math.Max(0, widths[i] - minWidths[i]);
              totalShrinkable += shrinkableAmounts[i];
            }
          }

          int remainingExcess = Math.Min(excessWidth, totalShrinkable);
          for (int i = 0; i < ColumnsList.Count; i++)
          {
            if (!ColumnsList[i].Grow && shrinkableAmounts[i] > 0)
            {
              int shrinkAmount = (int)Math.Ceiling((double)shrinkableAmounts[i] / totalShrinkable * remainingExcess);
              shrinkAmount = Math.Min(shrinkAmount, shrinkableAmounts[i]);
              widths[i] -= shrinkAmount;
              totalShrinkable -= shrinkableAmounts[i];
              remainingExcess -= shrinkAmount;
            }
          }
        }
      }

      return widths;
    }

    int contentWidth = widths.Sum();
    int totalWidth = overhead + contentWidth;

    // If expandable, distribute remaining width
    if (Expand && Border != BorderStyle.None && totalWidth < terminalWidth)
    {
      int extraWidth = terminalWidth - totalWidth;

      // Distribute extra width among columns that are not capped at MaxWidth.
      // Loop because a round can push a column to its cap, requiring its
      // unused share to be redistributed to the remaining uncapped columns.
      while (extraWidth > 0)
      {
        List<int> expandableIndices = [];
        for (int i = 0; i < ColumnsList.Count; i++)
        {
          int? maxWidth = ColumnsList[i].MaxWidth;
          if (!maxWidth.HasValue || widths[i] < maxWidth.Value)
          {
            expandableIndices.Add(i);
          }
        }

        if (expandableIndices.Count == 0)
        {
          break; // All columns capped at MaxWidth — stop distributing
        }

        int perColumn = extraWidth / expandableIndices.Count;
        int remainder = extraWidth % expandableIndices.Count;

        for (int j = 0; j < expandableIndices.Count; j++)
        {
          int i = expandableIndices[j];
          int share = perColumn + (j < remainder ? 1 : 0);
          int? maxWidth = ColumnsList[i].MaxWidth;
          int addition = maxWidth.HasValue ? Math.Min(share, maxWidth.Value - widths[i]) : share;
          widths[i] += addition;
          extraWidth -= addition;
        }
      }
    }
    // If table exceeds terminal width, shrink columns to fit
    else if (totalWidth > terminalWidth)
    {
      int excessWidth = totalWidth - terminalWidth;
      int availableContentWidth = terminalWidth - overhead;

      if (availableContentWidth > 0)
      {
        // Get minimum widths for each column (default 4 for ellipsis)
        int[] minWidths = new int[ColumnsList.Count];
        for (int i = 0; i < ColumnsList.Count; i++)
        {
          minWidths[i] = ColumnsList[i].MinWidth ?? 4;
        }

        // Calculate how much each column can shrink (width above minimum)
        // Grow columns should NOT shrink - they keep their natural width
        int[] shrinkableAmounts = new int[ColumnsList.Count];
        int totalShrinkable = 0;
        for (int i = 0; i < ColumnsList.Count; i++)
        {
          if (!ColumnsList[i].Grow)
          {
            shrinkableAmounts[i] = Math.Max(0, widths[i] - minWidths[i]);
            totalShrinkable += shrinkableAmounts[i];
          }
        }

        if (totalShrinkable > 0)
        {
          // Shrink proportionally based on shrinkable amount (wider columns shrink more)
          int remainingExcess = Math.Min(excessWidth, totalShrinkable);

          for (int i = 0; i < ColumnsList.Count; i++)
          {
            if (shrinkableAmounts[i] > 0)
            {
              // Calculate this column's share of the shrinkage
              int shrinkAmount = (int)Math.Ceiling((double)shrinkableAmounts[i] / totalShrinkable * remainingExcess);
              shrinkAmount = Math.Min(shrinkAmount, shrinkableAmounts[i]); // Don't shrink below min
              widths[i] -= shrinkAmount;

              // Update totals for remaining columns
              totalShrinkable -= shrinkableAmounts[i];
              remainingExcess -= shrinkAmount;
            }
          }
        }
      }
    }

    return widths;
  }

  private string[] RenderWithoutBorder(int[] columnWidths)
  {
    List<string> lines = [];

    // Render header row if enabled
    if (ShowHeaders)
    {
      lines.Add(RenderDataRow([.. ColumnsList.Select(c => c.Header)], columnWidths, isHeader: true));
    }

    // Render data rows
    foreach (string[] row in RowsList)
    {
      lines.Add(RenderDataRow(row, columnWidths, isHeader: false));
    }

    return [.. lines];
  }

  private string[] RenderWithBorder(int[] columnWidths)
  {
    List<string> lines = [];

    char horizontal = BoxChars.GetHorizontal(Border);
    char vertical = BoxChars.GetVertical(Border);
    char topLeft = BoxChars.GetTopLeft(Border);
    char topRight = BoxChars.GetTopRight(Border);
    char bottomLeft = BoxChars.GetBottomLeft(Border);
    char bottomRight = BoxChars.GetBottomRight(Border);
    char topT = BoxChars.GetTopT(Border);
    char bottomT = BoxChars.GetBottomT(Border);
    char leftT = BoxChars.GetLeftT(Border);
    char rightT = BoxChars.GetRightT(Border);
    char cross = BoxChars.GetCross(Border);

    // Top border
    lines.Add(RenderHorizontalBorder(columnWidths, horizontal, topLeft, topRight, topT));

    // Header row if enabled
    if (ShowHeaders)
    {
      lines.Add(RenderCellRow([.. ColumnsList.Select(c => c.Header)], columnWidths, vertical, isHeader: true));

      // Header separator
      lines.Add(RenderHorizontalBorder(columnWidths, horizontal, leftT, rightT, cross));
    }

    // Data rows
    for (int i = 0; i < RowsList.Count; i++)
    {
      lines.Add(RenderCellRow(RowsList[i], columnWidths, vertical, isHeader: false));

      // Row separator (except after last row)
      if (ShowRowSeparators && i < RowsList.Count - 1)
      {
        lines.Add(RenderHorizontalBorder(columnWidths, horizontal, leftT, rightT, cross));
      }
    }

    // Bottom border
    lines.Add(RenderHorizontalBorder(columnWidths, horizontal, bottomLeft, bottomRight, bottomT));

    return [.. lines];
  }

  private string RenderHorizontalBorder(int[] columnWidths, char horizontal, char left, char right, char junction)
  {
    string colorStart = !string.IsNullOrEmpty(BorderColor) ? BorderColor : "";
    string colorEnd = !string.IsNullOrEmpty(BorderColor) ? AnsiColors.Reset : "";

    StringBuilder sb = new();
    _ = sb.Append(colorStart);
    _ = sb.Append(left);

    for (int i = 0; i < columnWidths.Length; i++)
    {
      if (columnWidths[i] == 0)
      {
        continue;
      }

      // Each cell has padding (1 space on each side) plus content width
      _ = sb.Append(horizontal, columnWidths[i] + 2);

      if (i < columnWidths.Length - 1)
      {
        // Only add junction if the next non-zero column exists
        bool hasNextNonZero = false;
        for (int j = i + 1; j < columnWidths.Length; j++)
        {
          if (columnWidths[j] > 0) { hasNextNonZero = true; break; }
        }

        if (hasNextNonZero)
        {
          _ = sb.Append(junction);
        }
      }
    }

    _ = sb.Append(right);
    _ = sb.Append(colorEnd);

    return sb.ToString();
  }

  private string RenderCellRow(string[] cells, int[] columnWidths, char vertical, bool isHeader)
  {
    string colorStart = !string.IsNullOrEmpty(BorderColor) ? BorderColor : "";
    string colorEnd = !string.IsNullOrEmpty(BorderColor) ? AnsiColors.Reset : "";

    StringBuilder sb = new();
    _ = sb.Append(colorStart);
    _ = sb.Append(vertical);
    _ = sb.Append(colorEnd);

    for (int i = 0; i < columnWidths.Length; i++)
    {
      if (columnWidths[i] == 0)
      {
        continue;
      }

      string cellValue = i < cells.Length ? cells[i] ?? "" : "";
      TableColumn column = ColumnsList[i];

      // Apply header color if this is a header and the column has a header color
      if (isHeader && !string.IsNullOrEmpty(column.HeaderColor))
      {
        cellValue = column.HeaderColor + cellValue + AnsiColors.Reset;
      }

      // Truncate if necessary (preserving ANSI codes as much as possible)
      int visibleLength = AnsiStringUtils.GetVisibleLength(cellValue);
      if (visibleLength > columnWidths[i])
      {
        cellValue = TruncateWithEllipsis(cellValue, columnWidths[i], column.TruncateMode);
      }

      // Apply alignment
      string alignedCell = AlignCell(cellValue, columnWidths[i], column.Alignment);

      _ = sb.Append(' '); // Left padding
      _ = sb.Append(alignedCell);
      _ = sb.Append(' '); // Right padding
      _ = sb.Append(colorStart);
      _ = sb.Append(vertical);
      _ = sb.Append(colorEnd);
    }

    return sb.ToString();
  }

  private string RenderDataRow(string[] cells, int[] columnWidths, bool isHeader)
  {
    StringBuilder sb = new();

    for (int i = 0; i < columnWidths.Length; i++)
    {
      string cellValue = i < cells.Length ? cells[i] ?? "" : "";
      TableColumn column = ColumnsList[i];

      // Apply header color if this is a header and the column has a header color
      if (isHeader && !string.IsNullOrEmpty(column.HeaderColor))
      {
        cellValue = column.HeaderColor + cellValue + AnsiColors.Reset;
      }

      // Truncate if necessary
      int visibleLength = AnsiStringUtils.GetVisibleLength(cellValue);
      if (visibleLength > columnWidths[i])
      {
        cellValue = TruncateWithEllipsis(cellValue, columnWidths[i], column.TruncateMode);
      }

      // Apply alignment
      string alignedCell = AlignCell(cellValue, columnWidths[i], column.Alignment);

      if (i > 0)
      {
        _ = sb.Append("  "); // Column separator for borderless tables
      }

      _ = sb.Append(alignedCell);
    }

    return sb.ToString();
  }

  private static string AlignCell(string content, int width, Alignment alignment)
  {
    return alignment switch
    {
      Alignment.Left => AnsiStringUtils.PadRightVisible(content, width),
      Alignment.Right => AnsiStringUtils.PadLeftVisible(content, width),
      Alignment.Center => AnsiStringUtils.CenterVisible(content, width),
      _ => AnsiStringUtils.PadRightVisible(content, width)
    };
  }

  private static string TruncateWithEllipsis(string text, int maxWidth, TruncateMode mode)
  {
    if (maxWidth <= 3)
    {
      return new string('.', maxWidth);
    }

    if (AnsiStringUtils.GetVisibleLength(text) <= maxWidth)
    {
      return text; // No truncation needed
    }

    // ANSI codes in the kept visible span stay in place; the ellipsis is always plain:
    // TakeVisibleFromStart closes any style active at the cut, and TakeVisibleFromEnd replays
    // the style established by the discarded prefix so the kept tail keeps its color.
    return mode switch
    {
      TruncateMode.Start => "..." + AnsiStringUtils.TakeVisibleFromEnd(text, maxWidth - 3),
      TruncateMode.Middle => TruncateMiddle(text, maxWidth),
      TruncateMode.End => AnsiStringUtils.TakeVisibleFromStart(text, maxWidth - 3) + "...",
      _ => AnsiStringUtils.TakeVisibleFromStart(text, maxWidth - 3) + "..."
    };
  }

  private static string TruncateMiddle(string text, int maxWidth)
  {
    int availableWidth = maxWidth - 3;
    int startWidth = (availableWidth + 1) / 2;
    int endWidth = availableWidth - startWidth;

    return AnsiStringUtils.TakeVisibleFromStart(text, startWidth) + "..." + AnsiStringUtils.TakeVisibleFromEnd(text, endWidth);
  }
}
