namespace TimeWarp.Terminal.Layout;

#region Purpose
// Two-pass Yoga layout: allocate integer widths, then heights, collect leaf boxes.
#endregion

#region Design
// Yoga measure funcs on grow items disturb pixel-grid rounding (unequal Grow ratios
// stop tiling). Pass 1 sets FlexBasis/fixed widths with no measure funcs; pass 2 pins
// those widths and content heights. One Config (PointScaleFactor=1) per tree. Positions
// from Yoga are parent-relative and are accumulated to root-relative LayoutBox values.
#endregion

internal sealed class LaidOutLeaf
{
  public required LayoutLeaf Leaf { get; init; }
  public required LayoutBox Box { get; init; }
  public required string[] Lines { get; init; }
}

internal static class LayoutEngine
{
  public static IReadOnlyList<LaidOutLeaf> Calculate(LayoutContainer root, int width)
  {
    ArgumentNullException.ThrowIfNull(root);
    width = Math.Max(1, width);

    Config config = new() { PointScaleFactor = 1.0f };

    List<LayoutLeaf> leafOrder = [];
    CollectLeaves(root, leafOrder);

    Dictionary<LayoutLeaf, Node> pass1Nodes = [];
    Node pass1Root = BuildPass1(root, config, pass1Nodes, isRoot: true);
    pass1Root.Style.SetDimension(Dimension.Width, StyleSizeLength.Points(width));
    pass1Root.CalculateLayout(width, float.NaN, Direction.LTR);

    Dictionary<LayoutLeaf, int> widths = [];
    Dictionary<LayoutLeaf, string[]> rendered = [];
    Dictionary<LayoutLeaf, int> heights = [];

    foreach (LayoutLeaf leaf in leafOrder)
    {
      Node node = pass1Nodes[leaf];
      int leafWidth = ToCell(node.Layout.GetDimension(Dimension.Width));
      leafWidth = Math.Max(1, leafWidth);
      widths[leaf] = leafWidth;

      // Flex options were applied on the child wrapper; height override lives on the leaf's flex via BuildPass2.
      string[] lines = WidgetMeasure.RenderLeaf(leaf, leafWidth);
      rendered[leaf] = lines;
    }

    // Re-walk to apply Height overrides from flex options
    ApplyHeightOverrides(root, widths, rendered, heights);

    Dictionary<LayoutLeaf, Node> pass2Nodes = [];
    Node pass2Root = BuildPass2(root, config, widths, heights, pass2Nodes, isRoot: true);
    pass2Root.Style.SetDimension(Dimension.Width, StyleSizeLength.Points(width));
    pass2Root.CalculateLayout(width, float.NaN, Direction.LTR);

    List<LaidOutLeaf> result = [];
    CollectAbsoluteLeaves(pass2Root, pass2Nodes, rendered, heights, 0, 0, result);
    return result;
  }

  public static IReadOnlyList<LayoutBox> CalculateBoxes(LayoutContainer root, int width)
  {
    IReadOnlyList<LaidOutLeaf> leaves = Calculate(root, width);
    List<LayoutBox> boxes = new(leaves.Count);
    foreach (LaidOutLeaf leaf in leaves)
    {
      boxes.Add(leaf.Box);
    }

    return boxes;
  }

  private static void ApplyHeightOverrides(
    LayoutContainer container,
    Dictionary<LayoutLeaf, int> widths,
    Dictionary<LayoutLeaf, string[]> rendered,
    Dictionary<LayoutLeaf, int> heights)
  {
    foreach (LayoutChild child in container.Children)
    {
      if (child.Leaf is not null)
      {
        int width = widths[child.Leaf];
        if (child.Flex.Height.HasValue)
        {
          heights[child.Leaf] = Math.Max(1, child.Flex.Height.Value);
        }
        else
        {
          heights[child.Leaf] = Math.Max(1, rendered[child.Leaf].Length);
        }

        // Keep rendered lines in sync for fixed height (canvas pads/clips).
        _ = width;
      }
      else if (child.Nested is not null)
      {
        ApplyHeightOverrides(child.Nested, widths, rendered, heights);
      }
    }
  }

  private static void CollectLeaves(LayoutContainer container, List<LayoutLeaf> leaves)
  {
    foreach (LayoutChild child in container.Children)
    {
      if (child.Leaf is not null)
      {
        leaves.Add(child.Leaf);
      }
      else if (child.Nested is not null)
      {
        CollectLeaves(child.Nested, leaves);
      }
    }
  }

  private static Node BuildPass1(
    LayoutContainer container,
    Config config,
    Dictionary<LayoutLeaf, Node> leafNodes,
    bool isRoot)
  {
    Node node = new(config);
    ApplyContainerStyle(node, container);

    int index = 0;
    foreach (LayoutChild child in container.Children)
    {
      Node childNode;
      if (child.Leaf is not null)
      {
        childNode = new(config);
        ApplyPass1LeafStyle(childNode, child.Leaf, child.Flex);
        leafNodes[child.Leaf] = childNode;
      }
      else if (child.Nested is not null)
      {
        childNode = BuildPass1(child.Nested, config, leafNodes, isRoot: false);
        ApplyItemFlex(childNode, child.Flex, minWidth: 0, fixedWidth: null, naturalWidth: null, preferGrowBasis: child.Flex.Grow > 0f);
      }
      else
      {
        continue;
      }

      node.InsertChild(childNode, index);
      index++;
    }

    _ = isRoot;
    return node;
  }

  private static Node BuildPass2(
    LayoutContainer container,
    Config config,
    Dictionary<LayoutLeaf, int> widths,
    Dictionary<LayoutLeaf, int> heights,
    Dictionary<LayoutLeaf, Node> leafNodes,
    bool isRoot)
  {
    Node node = new(config);
    ApplyContainerStyle(node, container);

    int index = 0;
    foreach (LayoutChild child in container.Children)
    {
      Node childNode;
      if (child.Leaf is not null)
      {
        childNode = new(config);
        int leafWidth = widths[child.Leaf];
        int leafHeight = heights[child.Leaf];
        childNode.Style.SetDimension(Dimension.Width, StyleSizeLength.Points(leafWidth));
        childNode.Style.SetDimension(Dimension.Height, StyleSizeLength.Points(leafHeight));
        // Preserve grow/shrink so wrap and justify stay consistent with pass 1.
        childNode.Style.FlexGrow = child.Flex.Grow;
        childNode.Style.FlexShrink = child.Flex.Shrink;
        leafNodes[child.Leaf] = childNode;
      }
      else if (child.Nested is not null)
      {
        childNode = BuildPass2(child.Nested, config, widths, heights, leafNodes, isRoot: false);
        childNode.Style.FlexGrow = child.Flex.Grow;
        childNode.Style.FlexShrink = child.Flex.Shrink;
      }
      else
      {
        continue;
      }

      node.InsertChild(childNode, index);
      index++;
    }

    _ = isRoot;
    return node;
  }

  private static void ApplyContainerStyle(Node node, LayoutContainer container)
  {
    node.Style.FlexDirection = container.Direction;
    node.Style.FlexWrap = container.Wrap;
    node.Style.AlignItems = container.AlignItems;
    node.Style.JustifyContent = container.JustifyContent;
    if (container.Gap > 0)
    {
      node.Style.SetGap(Gutter.All, StyleLength.Points(container.Gap));
    }
  }

  private static void ApplyPass1LeafStyle(Node node, LayoutLeaf leaf, FlexItemOptions flex)
  {
    int minWidth = WidgetMeasure.ResolveMinWidth(leaf, flex);
    int? fixedWidth = WidgetMeasure.ResolveFixedWidth(leaf, flex);
    int naturalWidth = WidgetMeasure.GetNaturalWidth(leaf);
    ApplyItemFlex(node, flex, minWidth, fixedWidth, naturalWidth, preferGrowBasis: flex.Grow > 0f);
  }

  private static void ApplyItemFlex(
    Node node,
    FlexItemOptions flex,
    int minWidth,
    int? fixedWidth,
    int? naturalWidth,
    bool preferGrowBasis)
  {
    node.Style.FlexGrow = flex.Grow;
    node.Style.FlexShrink = flex.Shrink;

    if (minWidth > 0)
    {
      node.Style.SetMinDimension(Dimension.Width, StyleSizeLength.Points(minWidth));
    }

    if (fixedWidth.HasValue)
    {
      node.Style.SetDimension(Dimension.Width, StyleSizeLength.Points(Math.Max(minWidth, fixedWidth.Value)));
      return;
    }

    if (flex.Basis.HasValue)
    {
      node.Style.FlexBasis = StyleSizeLength.Points(Math.Max(minWidth, flex.Basis.Value));
      return;
    }

    if (preferGrowBasis)
    {
      node.Style.FlexBasis = StyleSizeLength.Points(0);
      return;
    }

    if (naturalWidth.HasValue)
    {
      node.Style.FlexBasis = StyleSizeLength.Points(Math.Max(minWidth, naturalWidth.Value));
    }
  }

  private static void CollectAbsoluteLeaves(
    Node node,
    Dictionary<LayoutLeaf, Node> leafNodes,
    Dictionary<LayoutLeaf, string[]> rendered,
    Dictionary<LayoutLeaf, int> heights,
    int parentAbsX,
    int parentAbsY,
    List<LaidOutLeaf> result)
  {
    // Invert leafNodes for lookup by Node reference
    Dictionary<Node, LayoutLeaf> nodeToLeaf = [];
    foreach (KeyValuePair<LayoutLeaf, Node> pair in leafNodes)
    {
      nodeToLeaf[pair.Value] = pair.Key;
    }

    CollectAbsoluteLeavesCore(node, nodeToLeaf, rendered, heights, parentAbsX, parentAbsY, result);
  }

  private static void CollectAbsoluteLeavesCore(
    Node node,
    Dictionary<Node, LayoutLeaf> nodeToLeaf,
    Dictionary<LayoutLeaf, string[]> rendered,
    Dictionary<LayoutLeaf, int> heights,
    int absX,
    int absY,
    List<LaidOutLeaf> result)
  {
    if (nodeToLeaf.TryGetValue(node, out LayoutLeaf? leaf))
    {
      int width = ToCell(node.Layout.GetDimension(Dimension.Width));
      int height = heights[leaf];
      // Prefer Yoga height when present (should match); still use measured height for canvas.
      int yogaHeight = ToCell(node.Layout.GetDimension(Dimension.Height));
      if (yogaHeight > 0)
      {
        height = yogaHeight;
      }

      result.Add(new LaidOutLeaf
      {
        Leaf = leaf,
        Box = new LayoutBox(absX, absY, Math.Max(1, width), Math.Max(1, height)),
        Lines = rendered[leaf]
      });
      return;
    }

    int childCount = node.GetChildCount();
    for (int i = 0; i < childCount; i++)
    {
      Node child = node.GetChild(i);
      int childX = absX + ToCell(child.Layout.GetPosition(PhysicalEdge.Left));
      int childY = absY + ToCell(child.Layout.GetPosition(PhysicalEdge.Top));
      CollectAbsoluteLeavesCore(child, nodeToLeaf, rendered, heights, childX, childY, result);
    }
  }

  private static int ToCell(float value)
  {
    if (float.IsNaN(value) || float.IsInfinity(value))
    {
      throw new InvalidOperationException($"Layout value '{value}' is not a finite cell coordinate.");
    }

    double rounded = Math.Round(value, MidpointRounding.AwayFromZero);
    if (Math.Abs(value - rounded) > 0.001)
    {
      throw new InvalidOperationException($"Layout value '{value}' is not within 0.001 of an integer cell.");
    }

    return checked((int)rounded);
  }
}
