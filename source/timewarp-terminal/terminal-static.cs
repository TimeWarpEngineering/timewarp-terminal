namespace TimeWarp.Terminal;

#region Purpose
// Static facade providing Console-compatible API while maintaining testability.
// Routes all calls to configurable Instance property, enabling test doubles.
#endregion

#region Design
// Static API mimics System.Console for easy migration from existing code.
// Instance property allows swapping implementation for testing without DI container.
// Dedicated format overloads for 1-3 args avoid array allocation (params variant for 4+).
// Color methods use AnsiColors to wrap messages with ANSI escape sequences.
// CA1054 suppressed for WriteLink: OSC 8 hyperlinks use raw URL strings by design.
#endregion

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
    get;
    set => field = value ?? throw new ArgumentNullException(nameof(value));
  } = TimeWarpTerminal.Default;

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
    _ = Instance.Write(coloredMessage);
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
    _ = Instance.WriteLine(coloredMessage);
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
    _ = Instance.WriteLine(coloredMessage);
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
    _ = Instance.WriteErrorLine(coloredMessage);
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
    _ = Instance.Write(link);
  }

  // Stream Access Methods (IConsole)

  /// <summary>
  /// Acquires the standard input stream.
  /// </summary>
  /// <returns>The standard input stream.</returns>
  public static Stream OpenStandardInput() => Instance.OpenStandardInput();

  /// <summary>
  /// Acquires the standard output stream.
  /// </summary>
  /// <returns>The standard output stream.</returns>
  public static Stream OpenStandardOutput() => Instance.OpenStandardOutput();

  /// <summary>
  /// Acquires the standard error output stream.
  /// </summary>
  /// <returns>The standard error output stream.</returns>
  public static Stream OpenStandardError() => Instance.OpenStandardError();

  /// <summary>
  /// Gets the standard input reader.
  /// </summary>
  /// <value>A <see cref="TextReader"/> that represents the standard input stream.</value>
  public static TextReader In => Instance.In;

  /// <summary>
  /// Gets the standard output writer.
  /// </summary>
  /// <value>A <see cref="TextWriter"/> that represents the standard output stream.</value>
  public static TextWriter Out => Instance.Out;

  /// <summary>
  /// Gets the standard error writer.
  /// </summary>
  /// <value>A <see cref="TextWriter"/> that represents the standard error output stream.</value>
  public static TextWriter Error => Instance.Error;

  /// <summary>
  /// Sets the <see cref="In"/> property to the specified <see cref="TextReader"/>.
  /// </summary>
  /// <param name="reader">A <see cref="TextReader"/> that represents the new standard input stream.</param>
  public static void SetIn(TextReader reader)
  {
    ArgumentNullException.ThrowIfNull(reader);
    Instance.SetIn(reader);
  }

  /// <summary>
  /// Sets the <see cref="Out"/> property to the specified <see cref="TextWriter"/>.
  /// </summary>
  /// <param name="writer">A <see cref="TextWriter"/> that represents the new standard output stream.</param>
  public static void SetOut(TextWriter writer)
  {
    ArgumentNullException.ThrowIfNull(writer);
    Instance.SetOut(writer);
  }

  /// <summary>
  /// Sets the <see cref="Error"/> property to the specified <see cref="TextWriter"/>.
  /// </summary>
  /// <param name="writer">A <see cref="TextWriter"/> that represents the new standard error output stream.</param>
  public static void SetError(TextWriter writer)
  {
    ArgumentNullException.ThrowIfNull(writer);
    Instance.SetError(writer);
  }

  // Encoding Properties (IConsole)

  /// <summary>
  /// Gets or sets the encoding the console uses to read input.
  /// </summary>
  /// <value>The encoding used to read console input.</value>
  public static Encoding InputEncoding
  {
    get => Instance.InputEncoding;
    set => Instance.InputEncoding = value;
  }

  /// <summary>
  /// Gets or sets the encoding the console uses to write output.
  /// </summary>
  /// <value>The encoding used to write console output.</value>
  public static Encoding OutputEncoding
  {
    get => Instance.OutputEncoding;
    set => Instance.OutputEncoding = value;
  }

  // Redirection Properties (IConsole)

  /// <summary>
  /// Gets a value indicating whether the input stream has been redirected from the standard input stream.
  /// </summary>
  /// <value><c>true</c> if input is redirected; otherwise, <c>false</c>.</value>
  public static bool IsInputRedirected => Instance.IsInputRedirected;

  /// <summary>
  /// Gets a value indicating whether the output stream has been redirected from the standard output stream.
  /// </summary>
  /// <value><c>true</c> if output is redirected; otherwise, <c>false</c>.</value>
  public static bool IsOutputRedirected => Instance.IsOutputRedirected;

  /// <summary>
  /// Gets a value indicating whether the error stream has been redirected from the standard error stream.
  /// </summary>
  /// <value><c>true</c> if error output is redirected; otherwise, <c>false</c>.</value>
  public static bool IsErrorRedirected => Instance.IsErrorRedirected;

  // Input Methods

  /// <summary>
  /// Reads the next line of characters from the standard input stream.
  /// </summary>
  /// <returns>
  /// The next line of characters from the input stream, or null if no more lines are available.
  /// </returns>
  public static string? ReadLine() => Instance.ReadLine();

  /// <summary>
  /// Reads the next character from the standard input stream.
  /// </summary>
  /// <returns>
  /// The next character from the input stream, or -1 if no more characters are available.
  /// </returns>
  public static int Read() => Instance.Read();

  /// <summary>
  /// Obtains the next character or function key pressed by the user.
  /// The pressed key is displayed in the console window.
  /// </summary>
  /// <returns>
  /// An object that describes the <see cref="ConsoleKey"/> constant and Unicode character,
  /// if any, that correspond to the pressed console key.
  /// </returns>
  public static ConsoleKeyInfo ReadKey() => Instance.ReadKey();

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
  public static ConsoleKeyInfo ReadKey(bool intercept) => Instance.ReadKey(intercept);

  // Cursor Properties (ITerminal)

  /// <summary>
  /// Gets or sets the column position of the cursor.
  /// </summary>
  /// <value>The column position, 0-based from left to right.</value>
  public static int CursorLeft
  {
    get => Instance.CursorLeft;
    set => Instance.CursorLeft = value;
  }

  /// <summary>
  /// Gets or sets the row position of the cursor.
  /// </summary>
  /// <value>The row position, 0-based from top to bottom.</value>
  public static int CursorTop
  {
    get => Instance.CursorTop;
    set => Instance.CursorTop = value;
  }

  /// <summary>
  /// Gets or sets a value indicating whether the cursor is visible.
  /// </summary>
  /// <value><c>true</c> if the cursor is visible; otherwise, <c>false</c>.</value>
  public static bool CursorVisible
  {
    get => Instance.CursorVisible;
    set => Instance.CursorVisible = value;
  }

  /// <summary>
  /// Gets or sets the height of the cursor within a character cell.
  /// </summary>
  /// <value>The cursor size as a percentage from 1 to 100.</value>
  public static int CursorSize
  {
    get => Instance.CursorSize;
    set => Instance.CursorSize = value;
  }

  // Window/Buffer Geometry Properties (ITerminal)

  /// <summary>
  /// Gets or sets the height of the terminal window in characters.
  /// </summary>
  /// <value>The height of the terminal window measured in rows.</value>
  public static int WindowHeight
  {
    get => Instance.WindowHeight;
    set => Instance.WindowHeight = value;
  }

  /// <summary>
  /// Gets or sets the left position of the console window area.
  /// </summary>
  /// <value>The leftmost position of the console window.</value>
  public static int WindowLeft
  {
    get => Instance.WindowLeft;
    set => Instance.WindowLeft = value;
  }

  /// <summary>
  /// Gets or sets the top position of the console window area.
  /// </summary>
  /// <value>The topmost position of the console window.</value>
  public static int WindowTop
  {
    get => Instance.WindowTop;
    set => Instance.WindowTop = value;
  }

  /// <summary>
  /// Gets or sets the width of the buffer area.
  /// </summary>
  /// <value>The width of the buffer area measured in columns.</value>
  public static int BufferWidth
  {
    get => Instance.BufferWidth;
    set => Instance.BufferWidth = value;
  }

  /// <summary>
  /// Gets or sets the height of the buffer area.
  /// </summary>
  /// <value>The height of the buffer area measured in rows.</value>
  public static int BufferHeight
  {
    get => Instance.BufferHeight;
    set => Instance.BufferHeight = value;
  }

  /// <summary>
  /// Gets the largest possible number of console window columns.
  /// </summary>
  /// <value>The maximum width of the console window measured in columns.</value>
  public static int LargestWindowWidth => Instance.LargestWindowWidth;

  /// <summary>
  /// Gets the largest possible number of console window rows.
  /// </summary>
  /// <value>The maximum height of the console window measured in rows.</value>
  public static int LargestWindowHeight => Instance.LargestWindowHeight;

  // Window/Buffer Geometry Methods (ITerminal)

  /// <summary>
  /// Sets the dimensions of the console window to the specified values.
  /// </summary>
  /// <param name="width">The width of the console window measured in columns.</param>
  /// <param name="height">The height of the console window measured in rows.</param>
  public static void SetWindowSize(int width, int height) => Instance.SetWindowSize(width, height);

  /// <summary>
  /// Sets the position of the console window relative to the screen buffer.
  /// </summary>
  /// <param name="left">The column position of the upper left corner of the console window.</param>
  /// <param name="top">The row position of the upper left corner of the console window.</param>
  public static void SetWindowPosition(int left, int top) => Instance.SetWindowPosition(left, top);

  /// <summary>
  /// Sets the height and width of the screen buffer area to the specified values.
  /// </summary>
  /// <param name="width">The width of the buffer area measured in columns.</param>
  /// <param name="height">The height of the buffer area measured in rows.</param>
  public static void SetBufferSize(int width, int height) => Instance.SetBufferSize(width, height);

  /// <summary>
  /// Moves a specified source screen buffer area to a specified destination screen buffer area.
  /// </summary>
  /// <param name="sourceLeft">The leftmost column of the source area.</param>
  /// <param name="sourceTop">The topmost row of the source area.</param>
  /// <param name="sourceWidth">The number of columns in the source area.</param>
  /// <param name="sourceHeight">The number of rows in the source area.</param>
  /// <param name="targetLeft">The leftmost column of the destination area.</param>
  /// <param name="targetTop">The topmost row of the destination area.</param>
  /// <param name="sourceChar">The character used to fill the source area.</param>
  /// <param name="sourceForeColor">The foreground color used to fill the source area.</param>
  /// <param name="sourceBackColor">The background color used to fill the source area.</param>
  public static void MoveBufferArea
  (
    int sourceLeft,
    int sourceTop,
    int sourceWidth,
    int sourceHeight,
    int targetLeft,
    int targetTop,
    char sourceChar,
    ConsoleColor sourceForeColor,
    ConsoleColor sourceBackColor
  ) => Instance.MoveBufferArea(sourceLeft, sourceTop, sourceWidth, sourceHeight, targetLeft, targetTop, sourceChar, sourceForeColor, sourceBackColor);

  // Color State Properties (ITerminal)

  /// <summary>
  /// Gets or sets the foreground color of the console.
  /// </summary>
  /// <value>The foreground color. The default is gray.</value>
  public static ConsoleColor ForegroundColor
  {
    get => Instance.ForegroundColor;
    set => Instance.ForegroundColor = value;
  }

  /// <summary>
  /// Gets or sets the background color of the console.
  /// </summary>
  /// <value>The background color. The default is black.</value>
  public static ConsoleColor BackgroundColor
  {
    get => Instance.BackgroundColor;
    set => Instance.BackgroundColor = value;
  }

  /// <summary>
  /// Resets the foreground and background console colors to their defaults.
  /// </summary>
  public static void ResetColor() => Instance.ResetColor();

  // Control/Utility Properties (ITerminal)

  /// <summary>
  /// Gets or sets a value indicating whether the Ctrl+C key combination
  /// is treated as ordinary input or as an interrupt.
  /// </summary>
  /// <value>
  /// <c>true</c> if Ctrl+C is treated as ordinary input; <c>false</c> if it raises
  /// the <see cref="ITerminal.CancelKeyPress"/> event. The default is <c>false</c>.
  /// </value>
  public static bool TreatControlCAsInput
  {
    get => Instance.TreatControlCAsInput;
    set => Instance.TreatControlCAsInput = value;
  }

  /// <summary>
  /// Gets or sets the title to display in the console title bar.
  /// </summary>
  /// <value>The string to display in the title bar of the console.</value>
  public static string Title
  {
    get => Instance.Title;
    set => Instance.Title = value;
  }

  /// <summary>
  /// Gets a value indicating whether a key press is available in the input stream.
  /// </summary>
  /// <value>
  /// <c>true</c> if a key press is available; otherwise, <c>false</c>.
  /// </value>
  public static bool KeyAvailable => Instance.KeyAvailable;

  // Control/Utility Methods (ITerminal)

  /// <summary>
  /// Plays a beep sound through the console speaker.
  /// </summary>
  public static void Beep() => Instance.Beep();

  /// <summary>
  /// Plays a beep sound at the specified frequency and duration through the console speaker.
  /// </summary>
  /// <param name="frequency">
  /// The frequency of the beep, ranging from 37 to 32767 hertz.
  /// </param>
  /// <param name="duration">
  /// The duration of the beep, measured in milliseconds.
  /// </param>
  public static void Beep(int frequency, int duration) => Instance.Beep(frequency, duration);

  // Terminal Properties

  /// <summary>
  /// Gets or sets the width of the terminal window in characters.
  /// </summary>
  /// <value>The width of the terminal window measured in columns.</value>
  public static int WindowWidth
  {
    get => Instance.WindowWidth;
    set => Instance.WindowWidth = value;
  }

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
