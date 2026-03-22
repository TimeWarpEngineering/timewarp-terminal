namespace TimeWarp.Terminal;

#region Purpose
// Test double implementation of ITerminal capturing output and scripting input.
// Enables unit testing of REPL applications with simulated user interactions.
#endregion

#region Design
// Captures output via StringWriter instances for both stdout and stderr.
// Queues individual ConsoleKeyInfo for ReadKey — simulates real keystroke-by-keystroke REPL input.
// Clear() writes "[CLEAR]" marker so tests can verify it was called without losing captured output.
// IsInteractive defaults to false; SupportsColor defaults to true for color verification.
// IDisposable cleans up StringReader and StringWriter resources.
#endregion

/// <summary>
/// A testable implementation of <see cref="ITerminal"/> that captures all output
/// and provides scripted input including key sequences for REPL testing.
/// </summary>
/// <remarks>
/// Use this class to test interactive terminal features like:
/// <list type="bullet">
///   <item><description>Arrow key navigation</description></item>
///   <item><description>Tab completion</description></item>
///   <item><description>Command history</description></item>
///   <item><description>Line editing</description></item>
/// </list>
/// <example>
/// <code>
/// using TestTerminal terminal = new();
/// terminal.QueueKeys("hello");
/// terminal.QueueKey(ConsoleKey.Tab);  // Trigger completion
/// terminal.QueueKey(ConsoleKey.Enter);
/// terminal.QueueLine("exit");
///
/// await app.RunReplAsync(terminal);
///
/// Assert.Contains("hello", terminal.Output);
/// </code>
/// </example>
/// </remarks>
public sealed class TestTerminal : ITerminal, IDisposable
{
  private readonly StringReader InputReader;
  private readonly StringWriter OutputWriter;
  private readonly StringWriter ErrorWriter;
  private readonly Queue<ConsoleKeyInfo> KeyQueue;
  private int CursorLeftField;
  private int CursorTopField;
  private int CursorSizeField = 100;
  private bool Disposed;
  private TextReader InReader;
  private TextWriter OutWriter;
  private TextWriter ErrorWriterField;

  /// <summary>
  /// Gets or sets the mock standard input stream.
  /// </summary>
  public Stream StandardInputStream { get; set; }

  /// <summary>
  /// Gets or sets the mock standard output stream.
  /// </summary>
  public Stream StandardOutputStream { get; set; }

  /// <summary>
  /// Gets or sets the mock standard error stream.
  /// </summary>
  public Stream StandardErrorStream { get; set; }

  /// <summary>
  /// Gets or sets the input encoding for this test terminal.
  /// </summary>
  /// <value>The encoding used for input. Defaults to <see cref="Encoding.UTF8"/>.</value>
  public Encoding InputEncoding { get; set; } = Encoding.UTF8;

  /// <summary>
  /// Gets or sets the output encoding for this test terminal.
  /// </summary>
  /// <value>The encoding used for output. Defaults to <see cref="Encoding.UTF8"/>.</value>
  public Encoding OutputEncoding { get; set; } = Encoding.UTF8;

  /// <summary>
  /// Gets or sets a value indicating whether input is redirected.
  /// </summary>
  /// <value><c>true</c> if input is redirected; otherwise, <c>false</c>. Defaults to <c>false</c>.</value>
  public bool IsInputRedirected { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether output is redirected.
  /// </summary>
  /// <value><c>true</c> if output is redirected; otherwise, <c>false</c>. Defaults to <c>false</c>.</value>
  public bool IsOutputRedirected { get; set; }

  /// <summary>
  /// Gets or sets a value indicating whether error output is redirected.
  /// </summary>
  /// <value><c>true</c> if error output is redirected; otherwise, <c>false</c>. Defaults to <c>false</c>.</value>
  public bool IsErrorRedirected { get; set; }

  /// <inheritdoc />
  public Stream OpenStandardInput()
    => StandardInputStream;

  /// <inheritdoc />
  public Stream OpenStandardOutput()
    => StandardOutputStream;

  /// <inheritdoc />
  public Stream OpenStandardError()
    => StandardErrorStream;

  /// <inheritdoc />
  public TextReader In => InReader;

  /// <inheritdoc />
  public TextWriter Out => OutWriter;

  /// <inheritdoc />
  public TextWriter Error => ErrorWriterField;

  /// <inheritdoc />
  public void SetIn(TextReader reader)
    => InReader = reader;

  /// <inheritdoc />
  public void SetOut(TextWriter writer)
    => OutWriter = writer;

  /// <inheritdoc />
  public void SetError(TextWriter writer)
    => ErrorWriterField = writer;

  /// <summary>
  /// Initializes a new instance of <see cref="TestTerminal"/> with optional scripted line input.
  /// </summary>
  /// <param name="input">
  /// The input to provide when <see cref="ReadLine"/> is called.
  /// Multiple lines should be separated by newlines.
  /// </param>
  public TestTerminal(string input = "")
  {
    InputReader = new StringReader(input);
    OutputWriter = new StringWriter();
    ErrorWriter = new StringWriter();
    KeyQueue = new Queue<ConsoleKeyInfo>();
    WindowWidth = 80;
    IsInteractive = false; // Testing is non-interactive by default
    SupportsColor = true;
    InReader = InputReader;
    OutWriter = OutputWriter;
    ErrorWriterField = ErrorWriter;
    StandardInputStream = new MemoryStream();
    StandardOutputStream = new MemoryStream();
    StandardErrorStream = new MemoryStream();

    // Suppress unused field warnings - these fields will be used when REPL is updated to use ITerminal
    _ = CursorLeftField;
    _ = CursorTopField;
  }

  /// <inheritdoc />
  public int CursorLeft
  {
    get => CursorLeftField;
    set => CursorLeftField = value;
  }

  /// <inheritdoc />
  public int CursorTop
  {
    get => CursorTopField;
    set => CursorTopField = value;
  }

  /// <inheritdoc />
  public bool CursorVisible { get; set; } = true;

  /// <inheritdoc />
  public int CursorSize
  {
    get => CursorSizeField;
    set
    {
      if (value < 1 || value > 100)
        throw new ArgumentOutOfRangeException(nameof(value), value, "CursorSize must be between 1 and 100.");
      CursorSizeField = value;
    }
  }

  /// <summary>
  /// Gets all standard output written to this terminal.
  /// </summary>
  public string Output => OutputWriter.ToString();

  /// <summary>
  /// Gets all error output written to this terminal.
  /// </summary>
  public string ErrorOutput => ErrorWriter.ToString();

  /// <summary>
  /// Gets all output (both standard and error) combined.
  /// </summary>
  public string AllOutput => Output + ErrorOutput;
  /// <inheritdoc />
  public ITerminal Write(string message)
  {
    OutputWriter.Write(message);
    return this;
  }

  /// <inheritdoc />
  IConsole IConsole.Write(string message) => Write(message);

  /// <inheritdoc />
  public ITerminal WriteLine(string? message = null)
  {
    OutputWriter.WriteLine(message ?? string.Empty);
    return this;
  }

  /// <inheritdoc />
  IConsole IConsole.WriteLine(string? message) => WriteLine(message);

  /// <inheritdoc />
  public async Task WriteLineAsync(string? message = null)
    => await OutputWriter.WriteLineAsync(message ?? string.Empty).ConfigureAwait(false);

  /// <inheritdoc />
  public ITerminal WriteErrorLine(string? message = null)
  {
    ErrorWriter.WriteLine(message ?? string.Empty);
    return this;
  }

  /// <inheritdoc />
  IConsole IConsole.WriteErrorLine(string? message) => WriteErrorLine(message);

  /// <inheritdoc />
  public async Task WriteErrorLineAsync(string? message = null)
    => await ErrorWriter.WriteLineAsync(message ?? string.Empty).ConfigureAwait(false);

  /// <inheritdoc />
  public string? ReadLine()
    => InputReader.ReadLine();

  /// <inheritdoc />
  public int Read()
  {
    if (KeyQueue.Count > 0)
    {
      ConsoleKeyInfo keyInfo = KeyQueue.Dequeue();
      return keyInfo.KeyChar;
    }

    return -1;
  }

  /// <inheritdoc />
  public ConsoleKeyInfo ReadKey()
    => ReadKey(false);

  /// <inheritdoc />
  public ConsoleKeyInfo ReadKey(bool intercept)
  {
    if (KeyQueue.Count > 0)
      return KeyQueue.Dequeue();

    // If no keys queued, try to read from input as a line
    string? line = InputReader.ReadLine();
    if (line is null)
    {
      // EOF - simulate Ctrl+D
      return new ConsoleKeyInfo('\u0004', ConsoleKey.D, false, false, true);
    }

    // Convert line characters to keys and queue them
    foreach (char c in line)
    {
      ConsoleKey key = CharToConsoleKey(c);
      KeyQueue.Enqueue(new ConsoleKeyInfo(c, key, false, false, false));
    }

    // Add Enter at end of line
    KeyQueue.Enqueue(new ConsoleKeyInfo('\r', ConsoleKey.Enter, false, false, false));

    return KeyQueue.Count > 0
      ? KeyQueue.Dequeue()
      : new ConsoleKeyInfo('\0', ConsoleKey.NoName, false, false, false);
  }

  /// <inheritdoc />
  public void SetCursorPosition(int left, int top)
  {
    CursorLeftField = left;
    CursorTopField = top;
  }

  /// <inheritdoc />
  public (int Left, int Top) GetCursorPosition()
    => (CursorLeftField, CursorTopField);

  /// <inheritdoc />
  public int WindowWidth { get; set; }

  /// <inheritdoc />
  public int WindowHeight { get; set; } = 24;

  /// <inheritdoc />
  public int WindowLeft { get; set; }

  /// <inheritdoc />
  public int WindowTop { get; set; }

  /// <inheritdoc />
  public int BufferWidth { get; set; } = 80;

  /// <inheritdoc />
  public int BufferHeight { get; set; } = 300;

  /// <inheritdoc />
  public int LargestWindowWidth { get; set; } = 120;

  /// <inheritdoc />
  public int LargestWindowHeight { get; set; } = 40;

  /// <summary>
  /// Gets the number of times <see cref="MoveBufferArea"/> has been called.
  /// </summary>
  public int MoveBufferAreaCallCount { get; private set; }

  /// <inheritdoc />
  public void SetWindowSize(int width, int height)
  {
    WindowWidth = width;
    WindowHeight = height;
  }

  /// <inheritdoc />
  public void SetWindowPosition(int left, int top)
  {
    WindowLeft = left;
    WindowTop = top;
  }

  /// <inheritdoc />
  public void SetBufferSize(int width, int height)
  {
    BufferWidth = width;
    BufferHeight = height;
  }

  /// <inheritdoc />
  public void MoveBufferArea
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
  )
  {
    MoveBufferAreaCallCount++;
  }

  /// <inheritdoc />
  public bool IsInteractive { get; set; }

  /// <inheritdoc />
  public bool SupportsColor { get; set; }

  /// <inheritdoc />
  public bool SupportsHyperlinks { get; set; }

  /// <inheritdoc />
  public ConsoleColor ForegroundColor { get; set; } = ConsoleColor.Gray;

  /// <inheritdoc />
  public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

  /// <inheritdoc />
  public void ResetColor()
  {
    ForegroundColor = ConsoleColor.Gray;
    BackgroundColor = ConsoleColor.Black;
  }

  private ConsoleCancelEventHandler? CancelKeyPressHandler;

  /// <inheritdoc />
  public event ConsoleCancelEventHandler? CancelKeyPress
  {
    add => CancelKeyPressHandler += value;
    remove => CancelKeyPressHandler -= value;
  }

  /// <summary>
  /// Simulates a Ctrl+C key press for testing.
  /// </summary>
  /// <param name="specialKey">The special key type (default: CtrlC).</param>
  /// <remarks>
  /// Uses reflection to create ConsoleCancelEventArgs since it has no public constructor.
  /// </remarks>
  public void SimulateCancelKeyPress(ConsoleSpecialKey specialKey = ConsoleSpecialKey.ControlC)
  {
    if (CancelKeyPressHandler is null)
      return;

    // ConsoleCancelEventArgs has no public constructor, so we use reflection
    System.Reflection.ConstructorInfo? constructor = typeof(ConsoleCancelEventArgs)
      .GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, [typeof(ConsoleSpecialKey)]);

    if (constructor is not null)
    {
      ConsoleCancelEventArgs args = (ConsoleCancelEventArgs)constructor.Invoke([specialKey]);
      CancelKeyPressHandler.Invoke(this, args);
    }
  }

  /// <inheritdoc />

  public void Clear()
    => OutputWriter.WriteLine("[CLEAR]");

  /// <summary>
  /// Gets the number of times <see cref="Beep()"/> has been called.
  /// </summary>
  public int BeepCount { get; private set; }

  /// <summary>
  /// Gets the frequency of the last <see cref="Beep(int, int)"/> call.
  /// </summary>
  public int LastBeepFrequency { get; private set; }

  /// <summary>
  /// Gets the duration of the last <see cref="Beep(int, int)"/> call.
  /// </summary>
  public int LastBeepDuration { get; private set; }

  /// <inheritdoc />
  public void Beep()
    => BeepCount++;

  /// <inheritdoc />
  public void Beep(int frequency, int duration)
  {
    BeepCount++;
    LastBeepFrequency = frequency;
    LastBeepDuration = duration;
  }

  /// <inheritdoc />
  public bool TreatControlCAsInput { get; set; }

  /// <inheritdoc />
  public string Title { get; set; } = string.Empty;

  /// <inheritdoc />
  public bool KeyAvailable => KeyQueue.Count > 0;

  // ========== Test Helper Methods ==========

  /// <summary>
  /// Queues a single key press for the next <see cref="ReadKey"/> call.
  /// </summary>
  /// <param name="key">The console key to queue.</param>
  /// <param name="shift">Whether Shift is pressed.</param>
  /// <param name="alt">Whether Alt is pressed.</param>
  /// <param name="ctrl">Whether Ctrl is pressed.</param>
  public void QueueKey(ConsoleKey key, bool shift = false, bool alt = false, bool ctrl = false)
  {
    char keyChar = key switch
    {
      ConsoleKey.Tab => '\t',
      ConsoleKey.Enter => '\r',
      ConsoleKey.Escape => '\u001b',
      ConsoleKey.Backspace => '\b',
      ConsoleKey.Spacebar => ' ',
      >= ConsoleKey.A and <= ConsoleKey.Z => (char)('a' + (key - ConsoleKey.A)),
      >= ConsoleKey.D0 and <= ConsoleKey.D9 => (char)('0' + (key - ConsoleKey.D0)),
      _ => '\0'
    };

    KeyQueue.Enqueue(new ConsoleKeyInfo(keyChar, key, shift, alt, ctrl));
  }

  /// <summary>
  /// Queues a <see cref="ConsoleKeyInfo"/> directly.
  /// </summary>
  /// <param name="keyInfo">The key info to queue.</param>
  public void QueueKeyInfo(ConsoleKeyInfo keyInfo)
    => KeyQueue.Enqueue(keyInfo);

  /// <summary>
  /// Queues a string of characters as individual key presses.
  /// </summary>
  /// <param name="text">The text to queue as key presses.</param>
  public void QueueKeys(string text)
  {
    ArgumentNullException.ThrowIfNull(text);
    foreach (char c in text)
    {
      ConsoleKey key = CharToConsoleKey(c);
      KeyQueue.Enqueue(new ConsoleKeyInfo(c, key, false, false, false));
    }
  }

  /// <summary>
  /// Queues a string followed by Enter.
  /// </summary>
  /// <param name="text">The text to queue as a complete line.</param>
  public void QueueLine(string text)
  {
    QueueKeys(text);
    QueueKey(ConsoleKey.Enter);
  }

  /// <summary>
  /// Queues an arrow key press.
  /// </summary>
  /// <param name="direction">The arrow direction (UpArrow, DownArrow, LeftArrow, RightArrow).</param>
  public void QueueArrow(ConsoleKey direction)
  {
    if (direction is ConsoleKey.UpArrow or ConsoleKey.DownArrow
        or ConsoleKey.LeftArrow or ConsoleKey.RightArrow)
    {
      QueueKey(direction);
    }
  }

  /// <summary>
  /// Clears all captured output.
  /// </summary>
  public void ClearOutput()
  {
    OutputWriter.GetStringBuilder().Clear();
    ErrorWriter.GetStringBuilder().Clear();
  }

  /// <summary>
  /// Clears all queued keys.
  /// </summary>
  public void ClearKeys()
    => KeyQueue.Clear();

  /// <summary>
  /// Checks if the standard output contains the specified text.
  /// </summary>
  /// <param name="text">The text to search for.</param>
  /// <returns><c>true</c> if the output contains the text; otherwise, <c>false</c>.</returns>
  public bool OutputContains(string text)
    => Output.Contains(text, StringComparison.Ordinal);

  /// <summary>
  /// Checks if the error output contains the specified text.
  /// </summary>
  /// <param name="text">The text to search for.</param>
  /// <returns><c>true</c> if the error output contains the text; otherwise, <c>false</c>.</returns>
  public bool ErrorContains(string text)
    => ErrorOutput.Contains(text, StringComparison.Ordinal);

  /// <summary>
  /// Gets the output lines as an array.
  /// </summary>
  /// <returns>An array of output lines.</returns>
  public string[] GetOutputLines()
    => Output.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

  /// <summary>
  /// Gets the number of keys currently in the queue.
  /// </summary>
  public int KeysInQueue => KeyQueue.Count;

  /// <summary>
  /// Disposes the resources used by this instance.
  /// </summary>
  /// <summary>
  /// Disposes the resources used by this instance and clears the test context if this is the current terminal.
  /// </summary>
  public void Dispose()
  {
    if (Disposed)
      return;

    // Clear context if this is the current terminal (restores previous Terminal.Instance)
    if (TestTerminalContext.Current == this)
    {
      TestTerminalContext.ClearCurrent();
    }

    InputReader.Dispose();
    OutputWriter.Dispose();
    ErrorWriter.Dispose();
    StandardInputStream.Dispose();
    StandardOutputStream.Dispose();
    StandardErrorStream.Dispose();
    Disposed = true;
  }

  private static ConsoleKey CharToConsoleKey(char c) => c switch
  {
    >= 'a' and <= 'z' => ConsoleKey.A + (c - 'a'),
    >= 'A' and <= 'Z' => ConsoleKey.A + (c - 'A'),
    >= '0' and <= '9' => ConsoleKey.D0 + (c - '0'),
    ' ' => ConsoleKey.Spacebar,
    '\t' => ConsoleKey.Tab,
    '\r' or '\n' => ConsoleKey.Enter,
    '\b' => ConsoleKey.Backspace,
    '-' => ConsoleKey.OemMinus,
    '=' => ConsoleKey.OemPlus,
    '/' => ConsoleKey.Oem2,
    '.' => ConsoleKey.OemPeriod,
    ',' => ConsoleKey.OemComma,
    _ => ConsoleKey.NoName
  };
}
