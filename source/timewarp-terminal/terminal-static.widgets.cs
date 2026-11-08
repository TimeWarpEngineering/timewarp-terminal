namespace TimeWarp.Terminal;

#region Purpose
// Table, panel, rule, and hyperlink writes on the Terminal facade.
#endregion

#region Design
// Same public type as terminal-static.cs. Widget color is re-applied after embedded SGR resets.
#endregion

public static partial class Terminal
{
  // Widget Methods

  /// <summary>
  /// Writes a table configured via a builder action to the terminal.
  /// </summary>
  /// <param name="configure">An action to configure the table using a <see cref="TableBuilder"/>.</param>
  /// <example>
  /// <code>
  /// Terminal.WriteTable(table => table
  ///     .AddColumns("Package", "Downloads", "Version")
  ///     .AddRow("Ardalis.GuardClauses", "12M", "5.0.0")
  ///     .AddRow("Ardalis.Result", "8M", "10.0.0"));
  /// </code>
  /// </example>
  public static void WriteTable(Action<TableBuilder> configure)
  {
    ArgumentNullException.ThrowIfNull(configure);

    TableBuilder builder = new();
    configure(builder);

    Table table = builder.Build();
    string[] lines = table.Render(WindowWidth);
    foreach (string line in lines)
    {
      _ = Instance.WriteLine(line);
    }
  }

  /// <summary>
  /// Writes a pre-configured <see cref="Table"/> to the terminal.
  /// </summary>
  /// <param name="table">The table to write.</param>
  /// <example>
  /// <code>
  /// var table = new TableBuilder()
  ///     .AddColumn("Name")
  ///     .AddColumn("Value")
  ///     .AddRow("Foo", "123")
  ///     .Build();
  /// Terminal.WriteTable(table);
  /// </code>
  /// </example>
  public static void WriteTable(Table table)
  {
    ArgumentNullException.ThrowIfNull(table);

    string[] lines = table.Render(WindowWidth);
    foreach (string line in lines)
    {
      _ = Instance.WriteLine(line);
    }
  }

  /// <summary>
  /// Writes a table configured via a builder action to the terminal with optional colors.
  /// </summary>
  /// <param name="configure">An action to configure the table using a <see cref="TableBuilder"/>.</param>
  /// <param name="foregroundColor">The foreground color to apply to table content. Defaults to <c>null</c> (no color).</param>
  /// <param name="backgroundColor">The background color to apply to table content. Defaults to <c>null</c> (no color).</param>
  /// <example>
  /// <code>
  /// Terminal.WriteTable(table => table
  ///     .AddColumns("Package", "Version")
  ///     .AddRow("TimeWarp", "1.0.0"),
  ///     ConsoleColor.White, ConsoleColor.DarkBlue);
  /// </code>
  /// </example>
  public static void WriteTable(Action<TableBuilder> configure, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
  {
    ArgumentNullException.ThrowIfNull(configure);

    TableBuilder builder = new();
    configure(builder);

    Table table = builder.Build();
    WriteTable(table, foregroundColor, backgroundColor);
  }

  /// <summary>
  /// Writes a pre-configured <see cref="Table"/> to the terminal with optional colors.
  /// </summary>
  /// <param name="table">The table to write.</param>
  /// <param name="foregroundColor">The foreground color to apply to table content. Defaults to <c>null</c> (no color).</param>
  /// <param name="backgroundColor">The background color to apply to table content. Defaults to <c>null</c> (no color).</param>
  /// <example>
  /// <code>
  /// var table = new TableBuilder()
  ///     .AddColumn("Name")
  ///     .AddColumn("Value")
  ///     .AddRow("Foo", "123")
  ///     .Build();
  /// Terminal.WriteTable(table, ConsoleColor.White, ConsoleColor.DarkBlue);
  /// </code>
  /// </example>
  /// <remarks>
  /// The color is applied only when the current instance's <see cref="ITerminal.SupportsColor"/>
  /// is <c>true</c>; otherwise the text is written plain (honoring NO_COLOR and redirected output).
  /// The color prefix is re-applied after any embedded SGR reset (from border colors or styled
  /// cells) so the requested colors are not cancelled mid-line.
  /// </remarks>
  public static void WriteTable(Table table, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
  {
    ArgumentNullException.ThrowIfNull(table);

    bool useColor = (foregroundColor.HasValue || backgroundColor.HasValue) && Instance.SupportsColor;
    string colorPrefix = useColor
      ? (foregroundColor.HasValue ? AnsiColors.GetForeground(foregroundColor.Value) : "") +
        (backgroundColor.HasValue ? AnsiColors.GetBackground(backgroundColor.Value) : "")
      : "";
    string[] lines = table.Render(WindowWidth);
    foreach (string line in lines)
    {
      if (useColor)
      {
        string coloredLine = colorPrefix +
                             line.Replace(AnsiColors.Reset, AnsiColors.Reset + colorPrefix, StringComparison.Ordinal) +
                             AnsiColors.Reset;
        _ = Instance.WriteLine(coloredLine);
      }
      else
      {
        _ = Instance.WriteLine(line);
      }
    }
  }

  /// <summary>
  /// Writes a panel configured via a builder action to the terminal.
  /// </summary>
  /// <param name="configure">An action to configure the panel using a <see cref="PanelBuilder"/>.</param>
  /// <example>
  /// <code>
  /// Terminal.WritePanel(panel => panel
  ///     .Header("Configuration")
  ///     .Content("Setting: value")
  ///     .Border(BorderStyle.Rounded));
  /// </code>
  /// </example>
  public static void WritePanel(Action<PanelBuilder> configure)
  {
    ArgumentNullException.ThrowIfNull(configure);

    PanelBuilder builder = new();
    configure(builder);

    Panel panel = builder.Build();
    string[] lines = panel.Render(WindowWidth);
    foreach (string line in lines)
    {
      _ = Instance.WriteLine(line);
    }
  }

  /// <summary>
  /// Writes a panel configured via a builder action to the terminal with optional colors.
  /// </summary>
  /// <param name="configure">An action to configure the panel using a <see cref="PanelBuilder"/>.</param>
  /// <param name="foregroundColor">The foreground color to apply to panel content. Defaults to <c>null</c> (no color).</param>
  /// <param name="backgroundColor">The background color to apply to panel content. Defaults to <c>null</c> (no color).</param>
  /// <example>
  /// <code>
  /// Terminal.WritePanel(panel => panel
  ///     .Header("Configuration")
  ///     .Content("Setting: value"),
  ///     ConsoleColor.White, ConsoleColor.DarkBlue);
  /// </code>
  /// </example>
  public static void WritePanel(Action<PanelBuilder> configure, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
  {
    ArgumentNullException.ThrowIfNull(configure);

    PanelBuilder builder = new();
    configure(builder);

    Panel panel = builder.Build();
    WritePanel(panel, foregroundColor, backgroundColor);
  }

  /// <summary>
  /// Writes a panel with content and optional header to the terminal with optional colors.
  /// </summary>
  /// <param name="content">The content to display inside the panel.</param>
  /// <param name="header">The header to display in the top border. Defaults to <c>null</c>.</param>
  /// <param name="foregroundColor">The foreground color to apply to panel content. Defaults to <c>null</c> (no color).</param>
  /// <param name="backgroundColor">The background color to apply to panel content. Defaults to <c>null</c> (no color).</param>
  /// <example>
  /// <code>
  /// Terminal.WritePanel("This is important information");
  /// Terminal.WritePanel("Content here", "Notice");
  /// Terminal.WritePanel("This is important information", header: "Notice",
  ///     foregroundColor: ConsoleColor.White, backgroundColor: ConsoleColor.DarkBlue);
  /// </code>
  /// </example>
  public static void WritePanel(string content, string? header = null, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
  {
    Panel panel = new() { Content = content, Header = header };
    WritePanel(panel, foregroundColor, backgroundColor);
  }

  /// <summary>
  /// Writes a pre-configured <see cref="Panel"/> to the terminal with optional colors.
  /// </summary>
  /// <param name="panel">The panel to write.</param>
  /// <param name="foregroundColor">The foreground color to apply to panel content. Defaults to <c>null</c> (no color).</param>
  /// <param name="backgroundColor">The background color to apply to panel content. Defaults to <c>null</c> (no color).</param>
  /// <example>
  /// <code>
  /// var panel = new PanelBuilder()
  ///     .Content("Content here")
  ///     .Header("Notice")
  ///     .Build();
  /// Terminal.WritePanel(panel, ConsoleColor.White, ConsoleColor.DarkBlue);
  /// </code>
  /// </example>
  /// <remarks>
  /// The color is applied only when the current instance's <see cref="ITerminal.SupportsColor"/>
  /// is <c>true</c>; otherwise the text is written plain (honoring NO_COLOR and redirected output).
  /// The color prefix is re-applied after any embedded SGR reset (from border colors or styled
  /// content) so the requested colors are not cancelled mid-line.
  /// </remarks>
  public static void WritePanel(Panel panel, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
  {
    ArgumentNullException.ThrowIfNull(panel);

    bool useColor = (foregroundColor.HasValue || backgroundColor.HasValue) && Instance.SupportsColor;
    string colorPrefix = useColor
      ? (foregroundColor.HasValue ? AnsiColors.GetForeground(foregroundColor.Value) : "") +
        (backgroundColor.HasValue ? AnsiColors.GetBackground(backgroundColor.Value) : "")
      : "";
    string[] lines = panel.Render(WindowWidth);
    foreach (string line in lines)
    {
      if (useColor)
      {
        string coloredLine = colorPrefix +
                             line.Replace(AnsiColors.Reset, AnsiColors.Reset + colorPrefix, StringComparison.Ordinal) +
                             AnsiColors.Reset;
        _ = Instance.WriteLine(coloredLine);
      }
      else
      {
        _ = Instance.WriteLine(line);
      }
    }
  }

  /// <summary>
  /// Writes a horizontal rule to the terminal.
  /// </summary>
  /// <param name="title">The title to display centered in the rule. Can include ANSI styling. Defaults to <c>null</c>.</param>
  /// <example>
  /// <code>
  /// Terminal.WriteRule();
  /// Terminal.WriteRule("Section Title");
  /// </code>
  /// </example>
  public static void WriteRule(string? title = null)
  {
    Rule rule = new() { Title = title };
    string rendered = rule.Render(WindowWidth);
    _ = Instance.WriteLine(rendered);
  }

  /// <summary>
  /// Writes a horizontal rule configured via a builder action to the terminal.
  /// </summary>
  /// <param name="configure">An action to configure the rule using a <see cref="RuleBuilder"/>.</param>
  /// <example>
  /// <code>
  /// Terminal.WriteRule(rule => rule
  ///     .Title("Configuration")
  ///     .Style(LineStyle.Doubled)
  ///     .Color(AnsiColors.Cyan));
  /// </code>
  /// </example>
  public static void WriteRule(Action<RuleBuilder> configure)
  {
    ArgumentNullException.ThrowIfNull(configure);

    RuleBuilder builder = new();
    configure(builder);

    Rule rule = builder.Build();
    string rendered = rule.Render(WindowWidth);
    _ = Instance.WriteLine(rendered);
  }

  /// <summary>
  /// Writes a clickable hyperlink to the terminal using OSC 8 sequences.
  /// </summary>
  /// <param name="url">The URL to link to.</param>
  /// <param name="text">The text to display (clickable in supported terminals).</param>
  /// <remarks>
  /// If the terminal does not support hyperlinks (<see cref="ITerminal.SupportsHyperlinks"/>
  /// is false), only the display text is written without the hyperlink escape sequences,
  /// consistent with the <c>ITerminal.WriteLink</c> extension.
  /// </remarks>
  /// <example>
  /// <code>
  /// Terminal.WriteLink("https://github.com", "GitHub Repository");
  /// </code>
  /// </example>
  // CA1054: OSC 8 hyperlinks use raw URL strings by design for ergonomic API
#pragma warning disable CA1054
  public static void WriteLink(string url, string text)
#pragma warning restore CA1054
  {
    ArgumentNullException.ThrowIfNull(url);
    ArgumentNullException.ThrowIfNull(text);

    ITerminal instance = Instance;
    if (!instance.SupportsHyperlinks)
    {
      _ = instance.Write(text);
      return;
    }

    string link = AnsiHyperlinks.CreateLink(url, text);
    _ = instance.Write(link);
  }

  /// <summary>
  /// Writes a clickable hyperlink to the terminal using OSC 8 sequences,
  /// followed by the current line terminator.
  /// </summary>
  /// <param name="url">The URL to link to.</param>
  /// <param name="text">The text to display (clickable in supported terminals).</param>
  /// <remarks>
  /// If the terminal does not support hyperlinks (<see cref="ITerminal.SupportsHyperlinks"/>
  /// is false), only the display text is written (followed by the line terminator) without
  /// the hyperlink escape sequences, consistent with <see cref="WriteLink"/>.
  /// </remarks>
  /// <example>
  /// <code>
  /// Terminal.WriteLinkLine("https://github.com", "GitHub Repository");
  /// </code>
  /// </example>
  // CA1054: OSC 8 hyperlinks use raw URL strings by design for ergonomic API
#pragma warning disable CA1054
  public static void WriteLinkLine(string url, string text)
#pragma warning restore CA1054
  {
    ArgumentNullException.ThrowIfNull(url);
    ArgumentNullException.ThrowIfNull(text);

    ITerminal instance = Instance;
    if (!instance.SupportsHyperlinks)
    {
      _ = instance.WriteLine(text);
      return;
    }

    string link = AnsiHyperlinks.CreateLink(url, text);
    _ = instance.WriteLine(link);
  }

}
