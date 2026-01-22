namespace TimeWarp.Terminal;

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
  /// Asynchronously writes the specified string value, followed by the current line terminator,
  /// to the standard error stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <returns>A task that represents the asynchronous write operation.</returns>
  public static Task WriteErrorLineAsync(string? message = null) => Instance.WriteErrorLineAsync(message);

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
