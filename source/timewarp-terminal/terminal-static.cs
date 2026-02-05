namespace TimeWarp.Terminal;

using System.Globalization;

/// <summary>
/// Static facade providing a Console-compatible API for terminal operations.
/// Routes all calls to the configurable <see cref="Instance"/>.
/// </summary>
/// <remarks>
/// This class provides a convenient static API similar to <see cref="System.Console"/>
/// while maintaining testability through the configurable <see cref="Instance"/> property.
/// <para>
/// Usage patterns:
/// <list type="bullet">
///   <item><description>Production code uses the static methods directly</description></item>
///   <item><description>Tests can replace <see cref="Instance"/> with a test implementation</description></item>
/// </list>
/// </para>
/// <example>
/// Production usage:
/// <code>
/// Terminal.WriteLine("Hello, World!");
/// var input = Terminal.ReadLine();
/// </code>
/// </example>
/// <example>
/// Test setup:
/// <code>
/// Terminal.Instance = new TestTerminal();
/// // ... run tests ...
/// Terminal.Instance = TimeWarpTerminal.Default; // restore
/// </code>
/// </example>
/// </remarks>
public static class Terminal
{
  private static ITerminal s_Instance = TimeWarpTerminal.Default;

  /// <summary>
  /// Gets or sets the terminal instance used by all static methods.
  /// Defaults to <see cref="TimeWarpTerminal.Default"/> for production use.
  /// </summary>
  /// <value>The current terminal implementation.</value>
  /// <exception cref="ArgumentNullException">Thrown when attempting to set a null value.</exception>
  /// <remarks>
  /// Replace this instance with a test implementation (such as <c>TestTerminal</c>)
  /// to capture output and simulate input during unit tests.
  /// </remarks>
  public static ITerminal Instance
  {
    get => s_Instance;
    set => s_Instance = value ?? throw new ArgumentNullException(nameof(value));
  }

  // Output Methods

  /// <summary>
  /// Writes the specified string value to the standard output stream.
  /// </summary>
  /// <param name="message">The value to write. If null, an empty string is written.</param>
  public static void Write(string? message) => Instance.Write(message ?? string.Empty);

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  public static void WriteLine(string? message = null) => Instance.WriteLine(message);

  /// <summary>
  /// Writes the specified string value to the standard output stream with the specified foreground color.
  /// </summary>
  /// <param name="message">The value to write. If null, an empty string is written.</param>
  /// <param name="foregroundColor">The foreground color to apply.</param>
  /// <example>
  /// <code>
  /// Terminal.Write("Error occurred!", ConsoleColor.Red);
  /// Terminal.Write("Success!", ConsoleColor.Green);
  /// </code>
  /// </example>
  public static void Write(string? message, ConsoleColor foregroundColor)
  {
    string coloredMessage = AnsiColors.GetForeground(foregroundColor) + (message ?? string.Empty) + AnsiColors.Reset;
    Instance.Write(coloredMessage);
  }

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream with the specified foreground color.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <param name="foregroundColor">The foreground color to apply.</param>
  /// <example>
  /// <code>
  /// Terminal.WriteLine("Error occurred!", ConsoleColor.Red);
  /// Terminal.WriteLine("Success!", ConsoleColor.Green);
  /// </code>
  /// </example>
  public static void WriteLine(string? message, ConsoleColor foregroundColor)
  {
    string coloredMessage = AnsiColors.GetForeground(foregroundColor) + (message ?? string.Empty) + AnsiColors.Reset;
    Instance.WriteLine(coloredMessage);
  }

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream with the specified foreground and background colors.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <param name="foregroundColor">The foreground color to apply.</param>
  /// <param name="backgroundColor">The background color to apply.</param>
  /// <example>
  /// <code>
  /// Terminal.WriteLine("Highlighted text", ConsoleColor.Black, ConsoleColor.Yellow);
  /// </code>
  /// </example>
  public static void WriteLine(string? message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
  {
    string coloredMessage = AnsiColors.GetForeground(foregroundColor) +
                            AnsiColors.GetBackground(backgroundColor) +
                            (message ?? string.Empty) +
                            AnsiColors.Reset;
    Instance.WriteLine(coloredMessage);
  }

  /// <summary>
  /// Asynchronously writes the specified string value, followed by the current line terminator,
  /// to the standard output stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <returns>A task that represents the asynchronous write operation.</returns>
  public static Task WriteLineAsync(string? message = null) => Instance.WriteLineAsync(message);

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  public static void WriteErrorLine(string? message = null) => Instance.WriteErrorLine(message);

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream with the specified foreground color.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <param name="foregroundColor">The foreground color to apply.</param>
  /// <example>
  /// <code>
  /// Terminal.WriteErrorLine("Error: File not found", ConsoleColor.Red);
  /// </code>
  /// </example>
  public static void WriteErrorLine(string? message, ConsoleColor foregroundColor)
  {
    string coloredMessage = AnsiColors.GetForeground(foregroundColor) + (message ?? string.Empty) + AnsiColors.Reset;
    Instance.WriteErrorLine(coloredMessage);
  }

  /// <summary>
  /// Asynchronously writes the specified string value, followed by the current line terminator,
  /// to the standard error stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <returns>A task that represents the asynchronous write operation.</returns>
  public static Task WriteErrorLineAsync(string? message = null) => Instance.WriteErrorLineAsync(message);

  // Format Overloads

  /// <summary>
  /// Writes the specified string value to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The object to format.</param>
  public static void Write(string format, object? arg0)
    => Instance.Write(string.Format(CultureInfo.InvariantCulture, format, arg0));

  /// <summary>
  /// Writes the specified string value to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  public static void Write(string format, object? arg0, object? arg1)
    => Instance.Write(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1));

  /// <summary>
  /// Writes the specified string value to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  /// <param name="arg2">The third object to format.</param>
  public static void Write(string format, object? arg0, object? arg1, object? arg2)
    => Instance.Write(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1, arg2));

  /// <summary>
  /// Writes the specified string value to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="args">An array of objects to format.</param>
  public static void Write(string format, params object?[] args)
    => Instance.Write(string.Format(CultureInfo.InvariantCulture, format, args));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The object to format.</param>
  public static void WriteLine(string format, object? arg0)
    => Instance.WriteLine(string.Format(CultureInfo.InvariantCulture, format, arg0));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  public static void WriteLine(string format, object? arg0, object? arg1)
    => Instance.WriteLine(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  /// <param name="arg2">The third object to format.</param>
  public static void WriteLine(string format, object? arg0, object? arg1, object? arg2)
    => Instance.WriteLine(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1, arg2));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="args">An array of objects to format.</param>
  public static void WriteLine(string format, params object?[] args)
    => Instance.WriteLine(string.Format(CultureInfo.InvariantCulture, format, args));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The object to format.</param>
  public static void WriteErrorLine(string format, object? arg0)
    => Instance.WriteErrorLine(string.Format(CultureInfo.InvariantCulture, format, arg0));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  public static void WriteErrorLine(string format, object? arg0, object? arg1)
    => Instance.WriteErrorLine(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  /// <param name="arg2">The third object to format.</param>
  public static void WriteErrorLine(string format, object? arg0, object? arg1, object? arg2)
    => Instance.WriteErrorLine(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1, arg2));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="args">An array of objects to format.</param>
  public static void WriteErrorLine(string format, params object?[] args)
    => Instance.WriteErrorLine(string.Format(CultureInfo.InvariantCulture, format, args));

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
      Instance.WriteLine(line);
  }

  /// <summary>
  /// Writes a pre-configured <see cref="Table"/> to the terminal.
  /// </summary>
  /// <param name="table">The table to write.</param>
  /// <example>
  /// <code>
  /// var table = new Table()
  ///     .AddColumn("Name")
  ///     .AddColumn("Value")
  ///     .AddRow("Foo", "123");
  /// Terminal.WriteTable(table);
  /// </code>
  /// </example>
  public static void WriteTable(Table table)
  {
    ArgumentNullException.ThrowIfNull(table);

    string[] lines = table.Render(WindowWidth);
    foreach (string line in lines)
      Instance.WriteLine(line);
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
  /// var table = new Table()
  ///     .AddColumn("Name")
  ///     .AddColumn("Value")
  ///     .AddRow("Foo", "123");
  /// Terminal.WriteTable(table, ConsoleColor.White, ConsoleColor.DarkBlue);
  /// </code>
  /// </example>
  public static void WriteTable(Table table, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
  {
    ArgumentNullException.ThrowIfNull(table);

    string[] lines = table.Render(WindowWidth);
    foreach (string line in lines)
    {
      if (foregroundColor.HasValue || backgroundColor.HasValue)
      {
        string coloredLine = (foregroundColor.HasValue ? AnsiColors.GetForeground(foregroundColor.Value) : "") +
                             (backgroundColor.HasValue ? AnsiColors.GetBackground(backgroundColor.Value) : "") +
                             line +
                             AnsiColors.Reset;
        Instance.WriteLine(coloredLine);
      }
      else
      {
        Instance.WriteLine(line);
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
      Instance.WriteLine(line);
  }

  /// <summary>
  /// Writes a panel with content and optional header to the terminal.
  /// </summary>
  /// <param name="content">The content to display inside the panel.</param>
  /// <param name="header">The header to display in the top border. Defaults to <c>null</c>.</param>
  /// <example>
  /// <code>
  /// Terminal.WritePanel("This is important information");
  /// Terminal.WritePanel("Content here", header: "Notice");
  /// </code>
  /// </example>
  public static void WritePanel(string content, string? header = null)
  {
    Panel panel = new() { Content = content, Header = header };
    string[] lines = panel.Render(WindowWidth);
    foreach (string line in lines)
      Instance.WriteLine(line);
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
  /// var panel = new Panel() { Content = "Content here", Header = "Notice" };
  /// Terminal.WritePanel(panel, ConsoleColor.White, ConsoleColor.DarkBlue);
  /// </code>
  /// </example>
  public static void WritePanel(Panel panel, ConsoleColor? foregroundColor = null, ConsoleColor? backgroundColor = null)
  {
    ArgumentNullException.ThrowIfNull(panel);

    string[] lines = panel.Render(WindowWidth);
    foreach (string line in lines)
    {
      if (foregroundColor.HasValue || backgroundColor.HasValue)
      {
        string coloredLine = (foregroundColor.HasValue ? AnsiColors.GetForeground(foregroundColor.Value) : "") +
                             (backgroundColor.HasValue ? AnsiColors.GetBackground(backgroundColor.Value) : "") +
                             line +
                             AnsiColors.Reset;
        Instance.WriteLine(coloredLine);
      }
      else
      {
        Instance.WriteLine(line);
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
    Instance.WriteLine(rendered);
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
    Instance.WriteLine(rendered);
  }

  /// <summary>
  /// Writes a clickable hyperlink to the terminal using OSC 8 sequences.
  /// </summary>
  /// <param name="url">The URL to link to.</param>
  /// <param name="text">The text to display (clickable in supported terminals).</param>
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

    string link = AnsiHyperlinks.CreateLink(text, url);
    Instance.Write(link);
  }

  // Input Methods

  /// <summary>
  /// Reads the next line of characters from the standard input stream.
  /// </summary>
  /// <returns>
  /// The next line of characters from the input stream, or null if no more lines are available.
  /// </returns>
  public static string? ReadLine() => Instance.ReadLine();

  /// <summary>
  /// Obtains the next character or function key pressed by the user.
  /// </summary>
  /// <param name="intercept">
  /// If true, the pressed key is not displayed in the console window. Default is false.
  /// </param>
  /// <returns>
  /// An object that describes the <see cref="ConsoleKey"/> constant and Unicode character,
  /// if any, that correspond to the pressed console key.
  /// </returns>
  public static ConsoleKeyInfo ReadKey(bool intercept = false) => Instance.ReadKey(intercept);

  // Terminal Properties

  /// <summary>
  /// Gets the width of the terminal window in characters.
  /// </summary>
  /// <value>The width of the terminal window measured in columns.</value>
  public static int WindowWidth => Instance.WindowWidth;

  /// <summary>
  /// Gets a value indicating whether the terminal is interactive.
  /// </summary>
  /// <value>
  /// <c>true</c> if the terminal supports interactive input (not redirected);
  /// otherwise, <c>false</c>.
  /// </value>
  public static bool IsInteractive => Instance.IsInteractive;

  /// <summary>
  /// Gets a value indicating whether the terminal supports ANSI color codes.
  /// </summary>
  /// <value>
  /// <c>true</c> if the terminal supports color output;
  /// otherwise, <c>false</c>.
  /// </value>
  public static bool SupportsColor => Instance.SupportsColor;

  /// <summary>
  /// Gets a value indicating whether the terminal supports OSC 8 hyperlinks.
  /// </summary>
  /// <value>
  /// <c>true</c> if the terminal supports clickable hyperlinks;
  /// otherwise, <c>false</c>.
  /// </value>
  /// <remarks>
  /// Supported terminals include Windows Terminal, iTerm2, VS Code terminal,
  /// Hyper, Konsole, and GNOME Terminal 3.26+.
  /// </remarks>
  public static bool SupportsHyperlinks => Instance.SupportsHyperlinks;

  // Terminal Operations

  /// <summary>
  /// Clears the console buffer and corresponding console window of display information.
  /// </summary>
  public static void Clear() => Instance.Clear();

  /// <summary>
  /// Sets the position of the cursor.
  /// </summary>
  /// <param name="left">The column position of the cursor. Columns are numbered from left to right starting at 0.</param>
  /// <param name="top">The row position of the cursor. Rows are numbered from top to bottom starting at 0.</param>
  public static void SetCursorPosition(int left, int top) => Instance.SetCursorPosition(left, top);

  /// <summary>
  /// Gets the current position of the cursor.
  /// </summary>
  /// <returns>A tuple containing the column (Left) and row (Top) position of the cursor.</returns>
  public static (int Left, int Top) GetCursorPosition() => Instance.GetCursorPosition();
}
