namespace TimeWarp.Terminal;

#region Purpose
// Integer cell rectangle for a laid-out leaf item.
#endregion

/// <summary>
/// An integer character-cell bounding box for a layout leaf.
/// </summary>
/// <param name="X">Left edge in cells, relative to the layout root.</param>
/// <param name="Y">Top edge in cells, relative to the layout root.</param>
/// <param name="Width">Width in cells.</param>
/// <param name="Height">Height in cells.</param>
public readonly record struct LayoutBox(int X, int Y, int Width, int Height);
