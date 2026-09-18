namespace TimeWarp.Terminal.Layout;

#region Purpose
// Fluent flex item options applied to a layout leaf or nested container.
#endregion

/// <summary>
/// Fluent builder for flex item sizing options (grow, shrink, basis, min/fixed size).
/// </summary>
public sealed class FlexItemBuilder
{
  private float GrowValue;
  private float ShrinkValue = 1f;
  private int? BasisValue;
  private int? WidthValue;
  private int? HeightValue;
  private int? MinWidthValue;

  /// <summary>
  /// Sets the flex-grow factor.
  /// </summary>
  /// <param name="grow">Non-negative grow factor.</param>
  /// <returns>This builder for chaining.</returns>
  public FlexItemBuilder Grow(float grow)
  {
    GrowValue = grow;
    return this;
  }

  /// <summary>
  /// Sets the flex-shrink factor. Defaults to 1 so items can collapse to their min-width floor.
  /// </summary>
  /// <param name="shrink">Non-negative shrink factor.</param>
  /// <returns>This builder for chaining.</returns>
  public FlexItemBuilder Shrink(float shrink)
  {
    ShrinkValue = shrink;
    return this;
  }

  /// <summary>
  /// Sets the flex basis in character cells.
  /// </summary>
  /// <param name="cells">Basis width in cells.</param>
  /// <returns>This builder for chaining.</returns>
  public FlexItemBuilder Basis(int cells)
  {
    BasisValue = cells;
    return this;
  }

  /// <summary>
  /// Sets a fixed width in character cells.
  /// </summary>
  /// <param name="cells">Fixed width in cells.</param>
  /// <returns>This builder for chaining.</returns>
  public FlexItemBuilder Width(int cells)
  {
    WidthValue = cells;
    return this;
  }

  /// <summary>
  /// Sets a fixed height in character cells.
  /// </summary>
  /// <param name="cells">Fixed height in cells.</param>
  /// <returns>This builder for chaining.</returns>
  public FlexItemBuilder Height(int cells)
  {
    HeightValue = cells;
    return this;
  }

  /// <summary>
  /// Sets a minimum width in character cells (combined with the content floor).
  /// </summary>
  /// <param name="cells">Minimum width in cells.</param>
  /// <returns>This builder for chaining.</returns>
  public FlexItemBuilder MinWidth(int cells)
  {
    MinWidthValue = cells;
    return this;
  }

  internal FlexItemOptions ToOptions() => new()
  {
    Grow = GrowValue,
    Shrink = ShrinkValue,
    Basis = BasisValue,
    Width = WidthValue,
    Height = HeightValue,
    MinWidth = MinWidthValue
  };
}
