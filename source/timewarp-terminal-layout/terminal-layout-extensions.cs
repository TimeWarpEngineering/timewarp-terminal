namespace TimeWarp.Terminal.Layout;

#region Purpose
// ITerminal extension methods for writing flexbox layouts.
#endregion

/// <summary>
/// Extension methods for writing <see cref="Layout"/> trees to an <see cref="ITerminal"/>.
/// </summary>
public static class TerminalLayoutExtensions
{
  /// <summary>
  /// Builds and writes a layout configured via <paramref name="configure"/> using the terminal width.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="configure">Configures the layout builder.</param>
  /// <returns>The terminal instance for fluent chaining.</returns>
  public static ITerminal WriteLayout(this ITerminal terminal, Action<LayoutBuilder> configure)
  {
    ArgumentNullException.ThrowIfNull(terminal);
    ArgumentNullException.ThrowIfNull(configure);

    LayoutBuilder builder = new();
    configure(builder);
    Layout layout = builder.Build();
    WriteLines(terminal, layout.Render(terminal.WindowWidth));
    return terminal;
  }

  /// <summary>
  /// Builds and writes a layout with optional foreground/background colors when the terminal supports color.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="configure">Configures the layout builder.</param>
  /// <param name="foregroundColor">Optional foreground color.</param>
  /// <param name="backgroundColor">Optional background color.</param>
  /// <returns>The terminal instance for fluent chaining.</returns>
  public static ITerminal WriteLayout(
    this ITerminal terminal,
    Action<LayoutBuilder> configure,
    ConsoleColor? foregroundColor,
    ConsoleColor? backgroundColor = null)
  {
    ArgumentNullException.ThrowIfNull(terminal);
    ArgumentNullException.ThrowIfNull(configure);

    LayoutBuilder builder = new();
    configure(builder);
    Layout layout = builder.Build();
    WriteLinesWithColor(terminal, layout.Render(terminal.WindowWidth), foregroundColor, backgroundColor);
    return terminal;
  }

  private static void WriteLines(ITerminal terminal, string[] lines)
  {
    foreach (string line in lines)
    {
      _ = terminal.WriteLine(line);
    }
  }

  private static void WriteLinesWithColor(
    ITerminal terminal,
    string[] lines,
    ConsoleColor? foregroundColor,
    ConsoleColor? backgroundColor)
  {
    bool useColor = (foregroundColor.HasValue || backgroundColor.HasValue) && terminal.SupportsColor;
    string colorPrefix = useColor
      ? (foregroundColor.HasValue ? AnsiColors.GetForeground(foregroundColor.Value) : "") +
        (backgroundColor.HasValue ? AnsiColors.GetBackground(backgroundColor.Value) : "")
      : "";

    foreach (string line in lines)
    {
      if (useColor)
      {
        // Re-apply the color prefix after any embedded SGR reset so requested colors are not cancelled mid-line.
        string coloredLine = colorPrefix +
                             line.Replace(AnsiColors.Reset, AnsiColors.Reset + colorPrefix, StringComparison.Ordinal) +
                             AnsiColors.Reset;
        _ = terminal.WriteLine(coloredLine);
      }
      else
      {
        _ = terminal.WriteLine(line);
      }
    }
  }
}
