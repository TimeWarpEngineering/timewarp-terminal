namespace TimeWarp.Terminal;

#region Purpose
// Colored and composite-format writes on the Terminal facade.
#endregion

#region Design
// Same public type as terminal-static.cs. Color writes honor SupportsColor and, for stderr, !IsErrorRedirected.
#endregion

public static partial class Terminal
{
  /// <summary>
  /// Writes the specified string value to the standard output stream with the specified foreground color.
  /// </summary>
  /// <param name="message">The value to write. If null, an empty string is written without color codes.</param>
  /// <param name="foregroundColor">The foreground color to apply.</param>
  /// <example>
  /// <code>
  /// Terminal.Write("Error occurred!", ConsoleColor.Red);
  /// Terminal.Write("Success!", ConsoleColor.Green);
  /// </code>
  /// </example>
  /// <remarks>
  /// The color is applied only when the current instance's <see cref="ITerminal.SupportsColor"/>
  /// is <c>true</c>; otherwise the text is written plain (honoring NO_COLOR and redirected output).
  /// </remarks>
  public static void Write(string? message, ConsoleColor foregroundColor)
  {
    ITerminal instance = Instance;
    if (message is null || !instance.SupportsColor)
    {
      _ = instance.Write(message ?? string.Empty);
      return;
    }

    string coloredMessage = AnsiColors.GetForeground(foregroundColor) + message + AnsiColors.Reset;
    _ = instance.Write(coloredMessage);
  }

  /// <summary>
  /// Writes the specified string value to the standard output stream with the specified
  /// foreground and background colors.
  /// </summary>
  /// <param name="message">The value to write. If null, an empty string is written without color codes.</param>
  /// <param name="foregroundColor">The foreground color to apply.</param>
  /// <param name="backgroundColor">The background color to apply.</param>
  /// <example>
  /// <code>
  /// Terminal.Write("Highlighted text", ConsoleColor.Black, ConsoleColor.Yellow);
  /// </code>
  /// </example>
  /// <remarks>
  /// The color is applied only when the current instance's <see cref="ITerminal.SupportsColor"/>
  /// is <c>true</c>; otherwise the text is written plain (honoring NO_COLOR and redirected output).
  /// </remarks>
  public static void Write(string? message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
  {
    ITerminal instance = Instance;
    if (message is null || !instance.SupportsColor)
    {
      _ = instance.Write(message ?? string.Empty);
      return;
    }

    string coloredMessage = AnsiColors.GetForeground(foregroundColor) +
                            AnsiColors.GetBackground(backgroundColor) +
                            message +
                            AnsiColors.Reset;
    _ = instance.Write(coloredMessage);
  }

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream with the specified foreground color.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written (no color codes).</param>
  /// <param name="foregroundColor">The foreground color to apply.</param>
  /// <example>
  /// <code>
  /// Terminal.WriteLine("Error occurred!", ConsoleColor.Red);
  /// Terminal.WriteLine("Success!", ConsoleColor.Green);
  /// </code>
  /// </example>
  /// <remarks>
  /// The color is applied only when the current instance's <see cref="ITerminal.SupportsColor"/>
  /// is <c>true</c>; otherwise the text is written plain (honoring NO_COLOR and redirected output).
  /// </remarks>
  public static void WriteLine(string? message, ConsoleColor foregroundColor)
  {
    ITerminal instance = Instance;
    if (message is null || !instance.SupportsColor)
    {
      _ = instance.WriteLine(message);
      return;
    }

    string coloredMessage = AnsiColors.GetForeground(foregroundColor) + message + AnsiColors.Reset;
    _ = instance.WriteLine(coloredMessage);
  }

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream with the specified foreground and background colors.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written (no color codes).</param>
  /// <param name="foregroundColor">The foreground color to apply.</param>
  /// <param name="backgroundColor">The background color to apply.</param>
  /// <example>
  /// <code>
  /// Terminal.WriteLine("Highlighted text", ConsoleColor.Black, ConsoleColor.Yellow);
  /// </code>
  /// </example>
  /// <remarks>
  /// The color is applied only when the current instance's <see cref="ITerminal.SupportsColor"/>
  /// is <c>true</c>; otherwise the text is written plain (honoring NO_COLOR and redirected output).
  /// </remarks>
  public static void WriteLine(string? message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
  {
    ITerminal instance = Instance;
    if (message is null || !instance.SupportsColor)
    {
      _ = instance.WriteLine(message);
      return;
    }

    string coloredMessage = AnsiColors.GetForeground(foregroundColor) +
                            AnsiColors.GetBackground(backgroundColor) +
                            message +
                            AnsiColors.Reset;
    _ = instance.WriteLine(coloredMessage);
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
  /// <param name="message">The value to write. If null, only the line terminator is written (no color codes).</param>
  /// <param name="foregroundColor">The foreground color to apply.</param>
  /// <example>
  /// <code>
  /// Terminal.WriteErrorLine("Error: File not found", ConsoleColor.Red);
  /// </code>
  /// </example>
  /// <remarks>
  /// The color is applied only when the current instance's <see cref="ITerminal.SupportsColor"/>
  /// is <c>true</c> and standard error is not redirected; otherwise the text is written plain
  /// (honoring NO_COLOR, redirected stdout, and redirected stderr).
  /// </remarks>
  public static void WriteErrorLine(string? message, ConsoleColor foregroundColor)
  {
    ITerminal instance = Instance;
    if (message is null || !instance.SupportsColor || instance.IsErrorRedirected)
    {
      _ = instance.WriteErrorLine(message);
      return;
    }

    string coloredMessage = AnsiColors.GetForeground(foregroundColor) + message + AnsiColors.Reset;
    _ = instance.WriteErrorLine(coloredMessage);
  }

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream with the specified foreground and background colors.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written (no color codes).</param>
  /// <param name="foregroundColor">The foreground color to apply.</param>
  /// <param name="backgroundColor">The background color to apply.</param>
  /// <example>
  /// <code>
  /// Terminal.WriteErrorLine("Fatal error", ConsoleColor.White, ConsoleColor.Red);
  /// </code>
  /// </example>
  /// <remarks>
  /// The color is applied only when the current instance's <see cref="ITerminal.SupportsColor"/>
  /// is <c>true</c> and standard error is not redirected; otherwise the text is written plain
  /// (honoring NO_COLOR, redirected stdout, and redirected stderr).
  /// </remarks>
  public static void WriteErrorLine(string? message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
  {
    ITerminal instance = Instance;
    if (message is null || !instance.SupportsColor || instance.IsErrorRedirected)
    {
      _ = instance.WriteErrorLine(message);
      return;
    }

    string coloredMessage = AnsiColors.GetForeground(foregroundColor) +
                            AnsiColors.GetBackground(backgroundColor) +
                            message +
                            AnsiColors.Reset;
    _ = instance.WriteErrorLine(coloredMessage);
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
    => Instance.Write(string.Format(ActiveFormatProvider, format, arg0));

  /// <summary>
  /// Writes the specified string value to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  public static void Write(string format, object? arg0, object? arg1)
    => Instance.Write(string.Format(ActiveFormatProvider, format, arg0, arg1));

  /// <summary>
  /// Writes the specified string value to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  /// <param name="arg2">The third object to format.</param>
  public static void Write(string format, object? arg0, object? arg1, object? arg2)
    => Instance.Write(string.Format(ActiveFormatProvider, format, arg0, arg1, arg2));

  /// <summary>
  /// Writes the specified string value to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="args">An array of objects to format.</param>
  public static void Write(string format, params object?[] args)
    => Instance.Write(string.Format(ActiveFormatProvider, format, args));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The object to format.</param>
  public static void WriteLine(string format, object? arg0)
    => Instance.WriteLine(string.Format(ActiveFormatProvider, format, arg0));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  public static void WriteLine(string format, object? arg0, object? arg1)
    => Instance.WriteLine(string.Format(ActiveFormatProvider, format, arg0, arg1));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  /// <param name="arg2">The third object to format.</param>
  public static void WriteLine(string format, object? arg0, object? arg1, object? arg2)
    => Instance.WriteLine(string.Format(ActiveFormatProvider, format, arg0, arg1, arg2));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="args">An array of objects to format.</param>
  public static void WriteLine(string format, params object?[] args)
    => Instance.WriteLine(string.Format(ActiveFormatProvider, format, args));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The object to format.</param>
  public static void WriteErrorLine(string format, object? arg0)
    => Instance.WriteErrorLine(string.Format(ActiveFormatProvider, format, arg0));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  public static void WriteErrorLine(string format, object? arg0, object? arg1)
    => Instance.WriteErrorLine(string.Format(ActiveFormatProvider, format, arg0, arg1));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="arg0">The first object to format.</param>
  /// <param name="arg1">The second object to format.</param>
  /// <param name="arg2">The third object to format.</param>
  public static void WriteErrorLine(string format, object? arg0, object? arg1, object? arg2)
    => Instance.WriteErrorLine(string.Format(ActiveFormatProvider, format, arg0, arg1, arg2));

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream, using the specified format information.
  /// </summary>
  /// <param name="format">A composite format string.</param>
  /// <param name="args">An array of objects to format.</param>
  public static void WriteErrorLine(string format, params object?[] args)
    => Instance.WriteErrorLine(string.Format(ActiveFormatProvider, format, args));

}
