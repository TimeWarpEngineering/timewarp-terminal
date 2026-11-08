namespace TimeWarp.Terminal;

#region Purpose
// Console stream, cursor, color, and control members on the Terminal facade.
#endregion

#region Design
// Same public type as terminal-static.cs. CancelKeyPress forwarding stays with the event it serves.
#endregion

public static partial class Terminal
{
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

  // Window/Buffer Geometry Properties (ITerminal)

  /// <summary>
  /// Gets the height of the terminal window in characters.
  /// </summary>
  /// <value>The height of the terminal window measured in rows.</value>
  public static int WindowHeight => Instance.WindowHeight;

  /// <summary>
  /// Gets the width of the buffer area.
  /// </summary>
  /// <value>The width of the buffer area measured in columns.</value>
  public static int BufferWidth => Instance.BufferWidth;

  /// <summary>
  /// Gets the height of the buffer area.
  /// </summary>
  /// <value>The height of the buffer area measured in rows.</value>
  public static int BufferHeight => Instance.BufferHeight;

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
  /// Occurs when the Ctrl+C key combination is pressed.
  /// </summary>
  /// <remarks>
  /// This event allows graceful handling of Ctrl+C for interactive applications like REPLs.
  /// Handlers are stored on the facade and forwarded through a single handler on
  /// <see cref="Instance"/>. Add and remove do not require the same instance to be current,
  /// so a subscription made inside a test context can be removed after that context ends.
  /// The forwarder follows process-global <see cref="Instance"/> assignment; async-local
  /// <c>TestTerminalContext</c> swaps do not move it.
  /// </remarks>
  public static event ConsoleCancelEventHandler? CancelKeyPress
  {
    add
    {
      lock (CancelKeyPressSync)
      {
        CancelKeyPressHandlers += value;
        if (CancelKeyPressBoundInstance is null)
        {
          BindCancelKeyPressForwarder(Instance);
        }
      }
    }
    remove
    {
      lock (CancelKeyPressSync)
      {
        CancelKeyPressHandlers -= value;
        if (CancelKeyPressHandlers is null)
        {
          UnbindCancelKeyPressForwarder();
        }
      }
    }
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

  private static void OnCancelKeyPress(object? sender, ConsoleCancelEventArgs args)
  {
    ConsoleCancelEventHandler? handler;
    lock (CancelKeyPressSync)
    {
      handler = CancelKeyPressHandlers;
    }

    handler?.Invoke(sender, args);
  }

  private static void SyncCancelKeyPressForwarder(ITerminal processGlobalTerminal)
  {
    lock (CancelKeyPressSync)
    {
      if (CancelKeyPressHandlers is null)
      {
        return;
      }

      BindCancelKeyPressForwarder(processGlobalTerminal);
    }
  }

  private static void BindCancelKeyPressForwarder(ITerminal terminal)
  {
    if (ReferenceEquals(CancelKeyPressBoundInstance, terminal))
    {
      return;
    }

    UnbindCancelKeyPressForwarder();
    terminal.CancelKeyPress += CancelKeyPressForwarder;
    CancelKeyPressBoundInstance = terminal;
  }

  private static void UnbindCancelKeyPressForwarder()
  {
    if (CancelKeyPressBoundInstance is null)
    {
      return;
    }

    CancelKeyPressBoundInstance.CancelKeyPress -= CancelKeyPressForwarder;
    CancelKeyPressBoundInstance = null;
  }

}
