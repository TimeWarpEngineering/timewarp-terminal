namespace TimeWarp.Terminal;

#region Purpose
// Internal layout tree: containers hold children; leaves wrap widgets or text.
#endregion

internal sealed class FlexItemOptions
{
  public float Grow { get; init; }
  public float Shrink { get; init; } = 1f;
  public int? Basis { get; init; }
  public int? Width { get; init; }
  public int? Height { get; init; }
  public int? MinWidth { get; init; }
}

internal enum LayoutLeafKind
{
  Text,
  Panel,
  Table,
  Rule
}

internal sealed class LayoutContainer
{
  public FlexDirection Direction { get; init; } = FlexDirection.Column;
  public int Gap { get; init; }
  public TimeWarp.Flexbox.Wrap Wrap { get; init; } = TimeWarp.Flexbox.Wrap.NoWrap;
  public Align AlignItems { get; init; } = Align.Stretch;
  public Justify JustifyContent { get; init; } = Justify.FlexStart;
  public required IReadOnlyList<LayoutChild> Children { get; init; }
}

internal sealed class LayoutChild
{
  public required FlexItemOptions Flex { get; init; }
  public LayoutLeaf? Leaf { get; init; }
  public LayoutContainer? Nested { get; init; }
}

internal sealed class LayoutLeaf
{
  public required LayoutLeafKind Kind { get; init; }
  public string? Text { get; init; }
  public Panel? Panel { get; init; }
  public Table? Table { get; init; }
  public Rule? Rule { get; init; }
}
