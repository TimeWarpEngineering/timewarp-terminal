namespace TimeWarp.Terminal;

#region Purpose
// Natural width, min-width floor, and height measurement for layout leaves.
#endregion

#region Design
// Min-width floors prevent silent zero-width collapse under FlexShrink. Table/Panel
// internals stay authoritative: we only supply a coarse floor, then Render(assignedWidth).
// Natural widths prefer public widget surface (Columns/Rows, Content, Title) over internals.
#endregion

internal static class WidgetMeasure
{
  public static int GetMinWidth(LayoutLeaf leaf)
  {
    ArgumentNullException.ThrowIfNull(leaf);

    return leaf.Kind switch
    {
      LayoutLeafKind.Text => GetTextMinWidth(leaf.Text),
      LayoutLeafKind.Panel => GetPanelMinWidth(leaf.Panel!),
      LayoutLeafKind.Table => GetTableMinWidth(leaf.Table!),
      LayoutLeafKind.Rule => GetRuleMinWidth(leaf.Rule!),
      _ => 1
    };
  }

  public static int GetNaturalWidth(LayoutLeaf leaf)
  {
    ArgumentNullException.ThrowIfNull(leaf);

    return leaf.Kind switch
    {
      LayoutLeafKind.Text => GetTextNaturalWidth(leaf.Text),
      LayoutLeafKind.Panel => GetPanelNaturalWidth(leaf.Panel!),
      LayoutLeafKind.Table => GetTableNaturalWidth(leaf.Table!),
      LayoutLeafKind.Rule => GetRuleNaturalWidth(leaf.Rule!),
      _ => 1
    };
  }

  public static int ResolveMinWidth(LayoutLeaf leaf, FlexItemOptions flex)
  {
    ArgumentNullException.ThrowIfNull(leaf);
    ArgumentNullException.ThrowIfNull(flex);

    int floor = GetMinWidth(leaf);
    if (flex.MinWidth.HasValue)
    {
      floor = Math.Max(floor, flex.MinWidth.Value);
    }

    return Math.Max(1, floor);
  }

  public static int? ResolveFixedWidth(LayoutLeaf leaf, FlexItemOptions flex)
  {
    ArgumentNullException.ThrowIfNull(leaf);
    ArgumentNullException.ThrowIfNull(flex);

    if (flex.Width.HasValue)
    {
      return flex.Width.Value;
    }

    return leaf.Kind switch
    {
      LayoutLeafKind.Text => null,
      LayoutLeafKind.Panel when leaf.Panel!.Width.HasValue => leaf.Panel.Width.Value,
      LayoutLeafKind.Panel => null,
      LayoutLeafKind.Table => null,
      LayoutLeafKind.Rule when leaf.Rule!.Width.HasValue => leaf.Rule.Width.Value,
      LayoutLeafKind.Rule => null,
      _ => null
    };
  }

  public static string[] RenderLeaf(LayoutLeaf leaf, int width)
  {
    ArgumentNullException.ThrowIfNull(leaf);

    width = Math.Max(1, width);

    return leaf.Kind switch
    {
      LayoutLeafKind.Text => RenderText(leaf.Text, width),
      LayoutLeafKind.Panel => leaf.Panel!.Render(width),
      LayoutLeafKind.Table => leaf.Table!.Render(width),
      LayoutLeafKind.Rule => [leaf.Rule!.Render(width)],
      _ => [string.Empty]
    };
  }

  public static int MeasureHeight(LayoutLeaf leaf, int width, FlexItemOptions flex)
  {
    ArgumentNullException.ThrowIfNull(leaf);
    ArgumentNullException.ThrowIfNull(flex);

    if (flex.Height.HasValue)
    {
      return Math.Max(1, flex.Height.Value);
    }

    string[] lines = RenderLeaf(leaf, width);
    return Math.Max(1, lines.Length);
  }

  private static string[] RenderText(string? text, int width)
  {
    if (string.IsNullOrEmpty(text))
    {
      return [string.Empty];
    }

    List<string> lines = [];
    string[] rawLines = text.Split('\n');
    foreach (string rawLine in rawLines)
    {
      lines.AddRange(AnsiStringUtils.WrapText(rawLine, width));
    }

    return lines.Count == 0 ? [string.Empty] : [.. lines];
  }

  private static int GetTextMinWidth(string? text)
  {
    if (string.IsNullOrEmpty(text))
    {
      return 1;
    }

    int maxGrapheme = 1;
    string plain = AnsiStringUtils.StripAnsiCodes(text);
    foreach (Rune rune in plain.EnumerateRunes())
    {
      maxGrapheme = Math.Max(maxGrapheme, UnicodeWidth.GetRuneWidth(rune));
    }

    return Math.Max(1, maxGrapheme);
  }

  private static int GetTextNaturalWidth(string? text)
  {
    if (string.IsNullOrEmpty(text))
    {
      return 1;
    }

    int max = 1;
    foreach (string line in text.Split('\n'))
    {
      max = Math.Max(max, AnsiStringUtils.GetVisibleLength(line));
    }

    return Math.Max(1, max);
  }

  private static int GetPanelMinWidth(Panel panel)
  {
    return panel.Border == BorderStyle.None ? 1 : 4;
  }

  private static int GetPanelNaturalWidth(Panel panel)
  {
    if (panel.Width.HasValue)
    {
      return Math.Max(GetPanelMinWidth(panel), panel.Width.Value);
    }

    int contentMax = 0;
    if (!string.IsNullOrEmpty(panel.Header))
    {
      contentMax = Math.Max(contentMax, AnsiStringUtils.GetVisibleLength(panel.Header));
    }

    if (!string.IsNullOrEmpty(panel.Content))
    {
      foreach (string line in panel.Content.Split('\n'))
      {
        contentMax = Math.Max(contentMax, AnsiStringUtils.GetVisibleLength(line));
      }
    }

    if (panel.Border == BorderStyle.None)
    {
      return Math.Max(1, contentMax);
    }

    int natural = contentMax + 2 + (2 * panel.PaddingHorizontal);
    natural = Math.Max(4, natural);

    // Panel omits the header from the top border when width < headerVisible + 6.
    if (!string.IsNullOrEmpty(panel.Header))
    {
      natural = Math.Max(natural, AnsiStringUtils.GetVisibleLength(panel.Header) + 6);
    }

    return natural;
  }

  private static int GetTableMinWidth(Table table)
  {
    return table.Border == BorderStyle.None ? 1 : 4;
  }

  private static int GetTableNaturalWidth(Table table)
  {
    if (table.Columns.Count == 0)
    {
      return GetTableMinWidth(table);
    }

    int columnCount = table.Columns.Count;
    int sum = 0;
    for (int i = 0; i < columnCount; i++)
    {
      TableColumn column = table.Columns[i];
      int maxWidth = AnsiStringUtils.GetVisibleLength(column.Header);
      foreach (string[] row in table.Rows)
      {
        if (i < row.Length)
        {
          maxWidth = Math.Max(maxWidth, AnsiStringUtils.GetVisibleLength(row[i]));
        }
      }

      if (column.MaxWidth.HasValue)
      {
        maxWidth = Math.Min(maxWidth, column.MaxWidth.Value);
      }

      sum += maxWidth;
    }

    int overhead = table.Border != BorderStyle.None
      ? 2 + (columnCount - 1) + (2 * columnCount)
      : (columnCount - 1) * 2;

    return Math.Max(GetTableMinWidth(table), overhead + sum);
  }

  private static int GetRuleMinWidth(Rule rule)
  {
    if (string.IsNullOrEmpty(rule.Title))
    {
      return 1;
    }

    return Math.Max(1, AnsiStringUtils.GetVisibleLength(rule.Title) + 4);
  }

  private static int GetRuleNaturalWidth(Rule rule)
  {
    if (rule.Width.HasValue)
    {
      return Math.Max(GetRuleMinWidth(rule), rule.Width.Value);
    }

    // Untitled rules prefer to grow; natural width is the min-width floor.
    return GetRuleMinWidth(rule);
  }
}
