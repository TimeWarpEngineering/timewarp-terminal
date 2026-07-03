namespace TimeWarp.Terminal;

/// <summary>
/// Extension methods for writing <see cref="Panel"/> widgets to an <see cref="ITerminal"/>.
/// </summary>
/// <example>
/// <code>
/// // Simple panel with content
/// terminal.WritePanel("This is important information");
///
/// // Panel with header
/// terminal.WritePanel("Content here", header: "Notice");
///
/// // Fluent builder
/// terminal.WritePanel(panel => panel
///     .Header("Configuration")
///     .Content("Setting: value")
///     .Border(BorderStyle.Rounded)
///     .Padding(2, 1));
/// </code>
/// </example>
public static class TerminalPanelExtensions
{
  /// <summary>
  /// Writes a simple panel with content to the terminal.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="content">The content to display inside the panel.</param>
  /// <param name="border">The border style to use. Defaults to <see cref="BorderStyle.Rounded"/>.</param>
  /// <returns>The terminal instance for fluent chaining.</returns>
  public static ITerminal WritePanel(this ITerminal terminal, string content, BorderStyle border = BorderStyle.Rounded)
  {
    ArgumentNullException.ThrowIfNull(terminal);

    Panel panel = new() { Content = content, Border = border };
    WritePanelInternal(terminal, panel);
    return terminal;
  }

  /// <summary>
  /// Writes a simple panel with content to the terminal with optional colors.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="content">The content to display inside the panel.</param>
  /// <param name="border">The border style to use. Defaults to <see cref="BorderStyle.Rounded"/>.</param>
  /// <param name="foregroundColor">The foreground color to apply to panel content. Defaults to <c>null</c>.</param>
  /// <param name="backgroundColor">The background color to apply to panel content. Defaults to <c>null</c>.</param>
  /// <returns>The terminal instance for fluent chaining.</returns>
  public static ITerminal WritePanel(this ITerminal terminal, string content, BorderStyle border, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
  {
    ArgumentNullException.ThrowIfNull(terminal);

    Panel panel = new() { Content = content, Border = border };
    WriteLinesWithColor(terminal, panel.Render(terminal.WindowWidth), foregroundColor, backgroundColor);
    return terminal;
  }

  /// <summary>
  /// Writes a panel with a header and content to the terminal.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="content">The content to display inside the panel.</param>
  /// <param name="header">The header to display in the top border.</param>
  /// <param name="border">The border style to use. Defaults to <see cref="BorderStyle.Rounded"/>.</param>
  /// <returns>The terminal instance for fluent chaining.</returns>
  public static ITerminal WritePanel(this ITerminal terminal, string content, string header, BorderStyle border = BorderStyle.Rounded)
  {
    ArgumentNullException.ThrowIfNull(terminal);

    Panel panel = new() { Content = content, Header = header, Border = border };
    WritePanelInternal(terminal, panel);
    return terminal;
  }

  /// <summary>
  /// Writes a panel with a header and content to the terminal with optional colors.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="content">The content to display inside the panel.</param>
  /// <param name="header">The header to display in the top border.</param>
  /// <param name="border">The border style to use. Defaults to <see cref="BorderStyle.Rounded"/>.</param>
  /// <param name="foregroundColor">The foreground color to apply to panel content. Defaults to <c>null</c>.</param>
  /// <param name="backgroundColor">The background color to apply to panel content. Defaults to <c>null</c>.</param>
  /// <returns>The terminal instance for fluent chaining.</returns>
  public static ITerminal WritePanel(this ITerminal terminal, string content, string header, BorderStyle border, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
  {
    ArgumentNullException.ThrowIfNull(terminal);

    Panel panel = new() { Content = content, Header = header, Border = border };
    WriteLinesWithColor(terminal, panel.Render(terminal.WindowWidth), foregroundColor, backgroundColor);
    return terminal;
  }

  /// <summary>
  /// Writes a panel configured via a builder action to the terminal.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="configure">An action to configure the panel using a <see cref="PanelBuilder"/>.</param>
  /// <returns>The terminal instance for fluent chaining.</returns>
  /// <example>
  /// <code>
  /// terminal.WritePanel(panel => panel
  ///     .Header("Configuration")
  ///     .Content("Setting: value")
  ///     .Border(BorderStyle.Rounded)
  ///     .Padding(2, 1));
  /// </code>
  /// </example>
  public static ITerminal WritePanel(this ITerminal terminal, Action<PanelBuilder> configure)
  {
    ArgumentNullException.ThrowIfNull(terminal);
    ArgumentNullException.ThrowIfNull(configure);

    PanelBuilder builder = new();
    configure(builder);

    Panel panel = builder.Build();
    WritePanelInternal(terminal, panel);
    return terminal;
  }

  /// <summary>
  /// Writes a panel configured via a builder action to the terminal with optional colors.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="configure">An action to configure the panel using a <see cref="PanelBuilder"/>.</param>
  /// <param name="foregroundColor">The foreground color to apply to panel content. Defaults to <c>null</c>.</param>
  /// <param name="backgroundColor">The background color to apply to panel content. Defaults to <c>null</c>.</param>
  /// <returns>The terminal instance for fluent chaining.</returns>
  public static ITerminal WritePanel(this ITerminal terminal, Action<PanelBuilder> configure, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
  {
    ArgumentNullException.ThrowIfNull(terminal);
    ArgumentNullException.ThrowIfNull(configure);

    PanelBuilder builder = new();
    configure(builder);

    Panel panel = builder.Build();
    WriteLinesWithColor(terminal, panel.Render(terminal.WindowWidth), foregroundColor, backgroundColor);
    return terminal;
  }

  /// <summary>
  /// Writes a pre-configured <see cref="Panel"/> to the terminal.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="panel">The panel to write.</param>
  /// <returns>The terminal instance for fluent chaining.</returns>
  public static ITerminal WritePanel(this ITerminal terminal, Panel panel)
  {
    ArgumentNullException.ThrowIfNull(terminal);
    ArgumentNullException.ThrowIfNull(panel);

    WritePanelInternal(terminal, panel);
    return terminal;
  }

  /// <summary>
  /// Writes a pre-configured <see cref="Panel"/> to the terminal with optional colors.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="panel">The panel to write.</param>
  /// <param name="foregroundColor">The foreground color to apply to panel content. Defaults to <c>null</c>.</param>
  /// <param name="backgroundColor">The background color to apply to panel content. Defaults to <c>null</c>.</param>
  /// <returns>The terminal instance for fluent chaining.</returns>
  public static ITerminal WritePanel(this ITerminal terminal, Panel panel, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
  {
    ArgumentNullException.ThrowIfNull(terminal);
    ArgumentNullException.ThrowIfNull(panel);

    WriteLinesWithColor(terminal, panel.Render(terminal.WindowWidth), foregroundColor, backgroundColor);
    return terminal;
  }

  private static void WritePanelInternal(ITerminal terminal, Panel panel)
  {
    string[] lines = panel.Render(terminal.WindowWidth);
    foreach (string line in lines)
    {
      _ = terminal.WriteLine(line);
    }
  }

  private static void WriteLinesWithColor(ITerminal terminal, string[] lines, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor)
  {
    bool useColor = (foregroundColor.HasValue || backgroundColor.HasValue) && terminal.SupportsColor;
    foreach (string line in lines)
    {
      if (useColor)
      {
        string coloredLine = (foregroundColor.HasValue ? AnsiColors.GetForeground(foregroundColor.Value) : "") +
                             (backgroundColor.HasValue ? AnsiColors.GetBackground(backgroundColor.Value) : "") +
                             line +
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
