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
  /// The pressed key is displayed in the console window.
  /// </summary>
  /// <returns>
  /// An object that describes the <see cref="ConsoleKey"/> constant and Unicode character,
  /// if any, that correspond to the pressed console key.
  /// </returns>
  /// <remarks>
  /// Key-by-key input is an interactive-terminal capability, which is why this member lives
  /// on <see cref="ITerminal"/> rather than <see cref="IConsole"/> — it has no meaning for a
  /// redirected, stream-oriented console.
  /// </remarks>
  ConsoleKeyInfo ReadKey();

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
  /// <remarks>
  /// When the console is redirected or unavailable, the default implementation returns 0
  /// from the getter instead of throwing; the setter silently ignores I/O errors and
  /// out-of-range values.
  /// </remarks>
  int CursorLeft { get; set; }

  /// <summary>
  /// Gets or sets the row position of the cursor.
  /// </summary>
  /// <value>The row position, 0-based from top to bottom.</value>
  /// <remarks>
  /// When the console is redirected or unavailable, the default implementation returns 0
  /// from the getter instead of throwing; the setter silently ignores I/O errors and
  /// out-of-range values.
  /// </remarks>
  int CursorTop { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether the cursor is visible.
  /// </summary>
  /// <value><c>true</c> if the cursor is visible; otherwise, <c>false</c>.</value>
  /// <remarks>
  /// Implementations targeting the system console may only support reading this value on Windows;
  /// the default implementation returns <c>true</c> on other platforms and when the console is
  /// redirected or unavailable. The setter is cross-platform and silently does nothing when the
  /// console is redirected or unavailable.
  /// </remarks>
  bool CursorVisible { get; set; }

  /// <summary>
  /// Sets the position of the cursor.
  /// </summary>
  /// <param name="left">The column position of the cursor. Columns are numbered from left to right starting at 0.</param>
  /// <param name="top">The row position of the cursor. Rows are numbered from top to bottom starting at 0.</param>
  /// <remarks>
  /// The default implementation silently ignores out-of-range positions and I/O errors
  /// (for example, when the console is redirected) instead of throwing.
  /// </remarks>
  void SetCursorPosition(int left, int top);

  /// <summary>
  /// Gets the current position of the cursor.
  /// </summary>
  /// <returns>A tuple containing the column (Left) and row (Top) position of the cursor.</returns>
  /// <remarks>
  /// When the console is redirected or unavailable, the default implementation returns
  /// (0, 0) instead of throwing.
  /// </remarks>
  (int Left, int Top) GetCursorPosition();

  /// <summary>
  /// Gets the width of the terminal window in characters.
  /// </summary>
  /// <value>The width of the terminal window measured in columns.</value>
  /// <remarks>
  /// When the console is redirected or unavailable, the default implementation returns 80
  /// instead of throwing.
  /// </remarks>
  int WindowWidth { get; }

  /// <summary>
  /// Gets the height of the terminal window in characters.
  /// </summary>
  /// <value>The height of the terminal window measured in rows.</value>
  /// <remarks>
  /// When the console is redirected or unavailable, the default implementation returns 24
  /// instead of throwing.
  /// </remarks>
  int WindowHeight { get; }

  /// <summary>
  /// Gets the width of the buffer area.
  /// </summary>
  /// <value>The width of the buffer area measured in columns.</value>
  /// <remarks>
  /// On modern terminals the buffer width equals the window width. When the console is
  /// redirected or unavailable, the default implementation returns 80 instead of throwing.
  /// </remarks>
  int BufferWidth { get; }

  /// <summary>
  /// Gets the height of the buffer area (including scrollback where the host reports it).
  /// </summary>
  /// <value>The height of the buffer area measured in rows.</value>
  /// <remarks>
  /// When the console is redirected or unavailable, the default implementation returns 300
  /// instead of throwing.
  /// </remarks>
  int BufferHeight { get; }

  /// <summary>
  /// Gets a value indicating whether the terminal is interactive.
  /// </summary>
  /// <value>
  /// <c>true</c> if the terminal supports interactive input and output;
  /// otherwise, <c>false</c>.
  /// </value>
  /// <remarks>
  /// The default implementation consults both standard input and standard output:
  /// it returns <c>true</c> only when neither stream is redirected. If either
  /// stdin or stdout is redirected (for example, <c>app &lt; file</c> or
  /// <c>app | tee</c>), the terminal is not considered interactive.
  /// Standard error is not consulted.
  /// </remarks>
  bool IsInteractive { get; }

  /// <summary>
  /// Gets a value indicating whether the terminal supports ANSI color codes.
  /// </summary>
  /// <value>
  /// <c>true</c> if the terminal supports color output;
  /// otherwise, <c>false</c>.
  /// </value>
  /// <remarks>
  /// The default implementation returns <c>false</c> when any of the following apply:
  /// standard output is redirected; the NO_COLOR environment variable is set to a
  /// non-empty value (per the no-color.org spec, an empty value does not disable color);
  /// or the TERM environment variable equals "dumb" (ordinal comparison).
  /// </remarks>
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
  /// Detection is a heuristic based on environment variables; the default implementation
  /// returns <c>false</c> when output is redirected or the terminal is not recognized.
  /// </remarks>
  bool SupportsHyperlinks { get; }

  /// <summary>
  /// Gets or sets the foreground color of the console.
  /// </summary>
  /// <value>The foreground color. The default is gray.</value>
  /// <remarks>
  /// When the console is redirected or unavailable, the default implementation returns
  /// <see cref="ConsoleColor.Gray"/> from the getter instead of throwing, and the setter
  /// silently does nothing.
  /// </remarks>
  ConsoleColor ForegroundColor { get; set; }

  /// <summary>
  /// Gets or sets the background color of the console.
  /// </summary>
  /// <value>The background color. The default is black.</value>
  /// <remarks>
  /// When the console is redirected or unavailable, the default implementation returns
  /// <see cref="ConsoleColor.Black"/> from the getter instead of throwing, and the setter
  /// silently does nothing.
  /// </remarks>
  ConsoleColor BackgroundColor { get; set; }

  /// <summary>
  /// Resets the foreground and background console colors to their defaults.
  /// </summary>
  /// <remarks>
  /// When the console is redirected or unavailable, the default implementation
  /// silently does nothing instead of throwing.
  /// </remarks>
  void ResetColor();

  /// <summary>
  /// Clears the console buffer and corresponding console window of display information.
  /// </summary>
  /// <remarks>
  /// When the console is redirected or unavailable, the default implementation
  /// silently does nothing instead of throwing.
  /// </remarks>
  void Clear();

  /// <summary>
  /// Occurs when the Ctrl+C key combination is pressed.
  /// </summary>
  /// <remarks>
  /// This event allows graceful handling of Ctrl+C for interactive applications like REPLs.
  /// </remarks>
  event ConsoleCancelEventHandler? CancelKeyPress;

  /// <summary>
  /// Plays a beep sound through the console speaker.
  /// </summary>
  /// <remarks>
  /// This parameterless overload is cross-platform (it emits the BEL character on
  /// Unix-like systems). When the console is redirected or unavailable, the default
  /// implementation silently does nothing instead of throwing.
  /// </remarks>
  void Beep();

  /// <summary>
  /// Plays a beep sound at the specified frequency and duration through the console speaker.
  /// </summary>
  /// <param name="frequency">
  /// The frequency of the beep, ranging from 37 to 32767 hertz.
  /// </param>
  /// <param name="duration">
  /// The duration of the beep, measured in milliseconds.
  /// </param>
  /// <remarks>
  /// Implementations targeting the system console may only support this overload on Windows;
  /// the default implementation silently does nothing on other platforms.
  /// </remarks>
  void Beep(int frequency, int duration);

  /// <summary>
  /// Gets or sets a value indicating whether the Ctrl+C key combination
  /// is treated as ordinary input or as an interrupt.
  /// </summary>
  /// <value>
  /// <c>true</c> if Ctrl+C is treated as ordinary input; <c>false</c> if it raises
  /// the <see cref="CancelKeyPress"/> event. The default is <c>false</c>.
  /// </value>
  /// <remarks>
  /// When the console is redirected or unavailable, the default implementation returns
  /// <c>false</c> from the getter instead of throwing, and the setter silently does nothing.
  /// </remarks>
  bool TreatControlCAsInput { get; set; }

  /// <summary>
  /// Gets or sets the title to display in the console title bar.
  /// </summary>
  /// <value>The string to display in the title bar of the console.</value>
  /// <remarks>
  /// Implementations targeting the system console may only support reading the title on Windows;
  /// the default implementation returns an empty string on other platforms. The setter is
  /// cross-platform and silently does nothing when the console is redirected or unavailable.
  /// </remarks>
  string Title { get; set; }

  /// <summary>
  /// Gets a value indicating whether a key press is available in the input stream.
  /// </summary>
  /// <value>
  /// <c>true</c> if a key press is available; otherwise, <c>false</c>.
  /// </value>
  /// <remarks>
  /// When standard input is redirected from a file or pipe, or the console is unavailable,
  /// the default implementation returns <c>false</c> instead of throwing.
  /// </remarks>
  bool KeyAvailable { get; }
}
