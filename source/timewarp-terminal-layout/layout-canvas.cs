namespace TimeWarp.Terminal;

#region Purpose
// Composite laid-out leaf lines onto a full-width character-cell canvas.
#endregion

#region Design
// Never slice ANSI by cell index. Each output row gathers covering leaves sorted by X,
// PadRightVisible to the leaf width, and fills X-gaps with spaces. Overlapping boxes
// throw (tiling contract). Short widgets pad; tall widgets clip to the leaf box height.
#endregion

internal static class LayoutCanvas
{
  public static string[] Compose(IReadOnlyList<LaidOutLeaf> leaves, int width)
  {
    ArgumentNullException.ThrowIfNull(leaves);
    width = Math.Max(1, width);

    if (leaves.Count == 0)
    {
      return [];
    }

    int height = 0;
    foreach (LaidOutLeaf leaf in leaves)
    {
      height = Math.Max(height, leaf.Box.Y + leaf.Box.Height);
    }

    if (height <= 0)
    {
      return [];
    }

    string[] lines = new string[height];
    for (int y = 0; y < height; y++)
    {
      lines[y] = ComposeRow(leaves, y, width);
    }

    return lines;
  }

  private static string ComposeRow(IReadOnlyList<LaidOutLeaf> leaves, int y, int width)
  {
    List<LaidOutLeaf> covering = [];
    foreach (LaidOutLeaf leaf in leaves)
    {
      if (y >= leaf.Box.Y && y < leaf.Box.Y + leaf.Box.Height)
      {
        covering.Add(leaf);
      }
    }

    covering.Sort(static (left, right) => left.Box.X.CompareTo(right.Box.X));

    StringBuilder builder = new();
    int cursor = 0;

    foreach (LaidOutLeaf leaf in covering)
    {
      if (leaf.Box.X > cursor)
      {
        _ = builder.Append(' ', leaf.Box.X - cursor);
        cursor = leaf.Box.X;
      }
      else if (leaf.Box.X < cursor)
      {
        throw new InvalidOperationException(
          $"Layout leaf at X={leaf.Box.X} overlaps previous content ending at cursor={cursor}.");
      }

      int lineIndex = y - leaf.Box.Y;
      string content = lineIndex < leaf.Lines.Length
        ? leaf.Lines[lineIndex]
        : string.Empty;

      string padded = AnsiStringUtils.PadRightVisible(content, leaf.Box.Width);
      _ = builder.Append(padded);
      cursor += leaf.Box.Width;
    }

    if (cursor < width)
    {
      _ = builder.Append(' ', width - cursor);
    }

    return builder.ToString();
  }
}
