namespace TimeWarp.Terminal;

#region Purpose
// Extends IConsole with interactive terminal capabilities for REPL and rich CLI applications.
// Provides cursor control, key-by-key input, and terminal capability detection.
#endregion

#region Design
// Uses 'new' modifier on sync Write methods to change return type from IConsole to ITerminal.
// This enables fluent chaining while maintaining backward compatibility with IConsole consumers.
// Async methods inherited from IConsole (returning Task) are not re-declared.
// Capability properties (SupportsColor, SupportsHyperlinks) allow feature detection at runtime.
#endregion

/// <summary>
/// Abstraction for interactive terminal operations.
/// Extends <see cref="IConsole"/> with capabilities needed for REPL and interactive CLI features.
/// </summary>
/// <remarks>
/// Implement this interface for interactive terminal environments requiring:
/// <list type="bullet">
///   <item><description>Key-by-key input handling (arrow keys, tab completion)</description></item>
///   <item><description>Cursor positioning for line editing</description></item>
///   <item><description>Terminal capability detection</description></item>
///   <item><description>Screen clearing</description></item>
/// </list>
/// </remarks>
public interface ITerminal : IConsole
{
  /// <summary>
  /// Writes the specified string value to the standard output stream.
  /// </summary>
  /// <param name="message">The value to write.</param>
  /// <returns>This terminal instance for fluent chaining.</returns>
  new ITerminal Write(string message);

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <returns>This terminal instance for fluent chaining.</returns>
  new ITerminal WriteLine(string? message = null);

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <returns>This terminal instance for fluent chaining.</returns>
  new ITerminal WriteErrorLine(string? message = null);

  /// <summary>
  /// Obtains the next character or function key pressed by the user.
  /// </summary>
  /// <param name="intercept">
  /// If true, the pressed key is not displayed in the console window.
  /// </param>
  /// <returns>
  /// An object that describes the <see cref="ConsoleKey"/> constant and Unicode character,
  /// if any, that correspond to the pressed console key.
  /// </returns>
  ConsoleKeyInfo ReadKey(bool intercept);

  /// <summary>
  /// Gets or sets the column position of the cursor.
  /// </summary>
  /// <value>The column position, 0-based from left to right.</value>
  int CursorLeft { get; set; }

  /// <summary>
  /// Gets or sets the row position of the cursor.
  /// </summary>
  /// <value>The row position, 0-based from top to bottom.</value>
  int CursorTop { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the cursor is visible.
  /// </summary>
  /// <value><c>true</c> if the cursor is visible; otherwise, <c>false</c>.</value>
  bool CursorVisible { get; set; }

  /// <summary>
  /// Gets or sets the height of the cursor within a character cell.
  /// </summary>
  /// <value>The cursor size as a percentage from 1 to 100.</value>
  /// <remarks>
  /// A value of 1 indicates a horizontal line at the bottom of the cell.
  /// A value of 100 indicates a full block cursor.
  /// </remarks>
  int CursorSize { get; set; }

  /// <summary>
  /// Sets the position of the cursor.
  /// </summary>
  /// <param name="left">The column position of the cursor. Columns are numbered from left to right starting at 0.</param>
  /// <param name="top">The row position of the cursor. Rows are numbered from top to bottom starting at 0.</param>
  void SetCursorPosition(int left, int top);

  /// <summary>
  /// Gets the current position of the cursor.
  /// </summary>
  /// <returns>A tuple containing the column (Left) and row (Top) position of the cursor.</returns>
  (int Left, int Top) GetCursorPosition();

  /// <summary>
  /// Gets the width of the terminal window in characters.
  /// </summary>
  /// <value>The width of the terminal window measured in columns.</value>
  int WindowWidth { get; }

  /// <summary>
  /// Gets a value indicating whether the terminal is interactive.
  /// </summary>
  /// <value>
  /// <c>true</c> if the terminal supports interactive input (not redirected);
  /// otherwise, <c>false</c>.
  /// </value>
  bool IsInteractive { get; }

  /// <summary>
  /// Gets a value indicating whether the terminal supports ANSI color codes.
  /// </summary>
  /// <value>
  /// <c>true</c> if the terminal supports color output;
  /// otherwise, <c>false</c>.
  /// </value>
  bool SupportsColor { get; }

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
  bool SupportsHyperlinks { get; }

  /// <summary>
  /// Gets or sets the foreground color of the console.
  /// </summary>
  /// <value>The foreground color. The default is gray.</value>
  ConsoleColor ForegroundColor { get; set; }

  /// <summary>
  /// Gets or sets the background color of the console.
  /// </summary>
  /// <value>The background color. The default is black.</value>
  ConsoleColor BackgroundColor { get; set; }

  /// <summary>
  /// Resets the foreground and background console colors to their defaults.
  /// </summary>
  void ResetColor();

  /// <summary>
  /// Clears the console buffer and corresponding console window of display information.
  /// </summary>
  void Clear();

  /// <summary>
  /// Occurs when the Ctrl+C key combination is pressed.
  /// </summary>
  /// <remarks>
  /// This event allows graceful handling of Ctrl+C for interactive applications like REPLs.
  /// </remarks>
  event ConsoleCancelEventHandler? CancelKeyPress;
}
