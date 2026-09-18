namespace TimeWarp.Terminal.Layout;

#region Purpose
// Static facade mirroring WriteLayout onto Terminal.Instance.
#endregion

/// <summary>
/// Static helpers that write layouts through <see cref="TimeWarp.Terminal.Terminal.Instance"/>.
/// </summary>
public static class TerminalLayout
{
  /// <summary>
  /// Builds and writes a layout using the current <see cref="TimeWarp.Terminal.Terminal.Instance"/>.
  /// </summary>
  /// <param name="configure">Configures the layout builder.</param>
  public static void WriteLayout(Action<LayoutBuilder> configure)
    => TimeWarp.Terminal.Terminal.Instance.WriteLayout(configure);

  /// <summary>
  /// Builds and writes a layout with optional colors using the current terminal instance.
  /// </summary>
  /// <param name="configure">Configures the layout builder.</param>
  /// <param name="foregroundColor">Optional foreground color.</param>
  /// <param name="backgroundColor">Optional background color.</param>
  public static void WriteLayout(
    Action<LayoutBuilder> configure,
    ConsoleColor? foregroundColor,
    ConsoleColor? backgroundColor = null)
    => TimeWarp.Terminal.Terminal.Instance.WriteLayout(configure, foregroundColor, backgroundColor);
}
