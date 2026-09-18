namespace TimeWarp.Terminal.Layout;

#region Purpose
// Immutable layout tree that renders to terminal lines or exposes leaf boxes for tests.
#endregion

/// <summary>
/// A flexbox layout of terminal widgets and text ready to render at a given width.
/// </summary>
public sealed class Layout
{
  private readonly LayoutContainer Root;

  internal Layout(LayoutContainer root)
  {
    Root = root ?? throw new ArgumentNullException(nameof(root));
  }

  /// <summary>
  /// Renders the layout to an array of lines sized to <paramref name="width"/> cells.
  /// </summary>
  /// <param name="width">Container width in character cells.</param>
  /// <returns>Composed output lines.</returns>
  public string[] Render(int width)
  {
    IReadOnlyList<LaidOutLeaf> leaves = LayoutEngine.Calculate(Root, width);
    return LayoutCanvas.Compose(leaves, Math.Max(1, width));
  }

  /// <summary>
  /// Calculates integer leaf boxes in tree order for the given container width.
  /// </summary>
  /// <param name="width">Container width in character cells.</param>
  /// <returns>Leaf boxes with root-relative coordinates.</returns>
  public IReadOnlyList<LayoutBox> CalculateBoxes(int width)
  {
    return LayoutEngine.CalculateBoxes(Root, width);
  }
}
