namespace TimeWarp.Terminal;

#region Purpose
// Test double implementation of ITerminal capturing output and scripting input.
// Enables unit testing of REPL applications with simulated user interactions.
#endregion

#region Design
// Captures output via StringWriter instances for both stdout and stderr.
// Queues individual ConsoleKeyInfo for ReadKey — simulates real keystroke-by-keystroke REPL input.
// Read, ReadKey, and KeyAvailable fall back to unread In input when the key queue is empty,
// matching System.Console semantics where Read/ReadLine/ReadKey share one input source (constructor
// input and SetIn).
// Write*/ReadLine route through Out/Error/In; capture writers are teed so Output/ErrorOutput still
// work after SetOut/SetError.
// QueueKey honors modifiers for letters: shift produces an uppercase KeyChar and ctrl produces the
// control character (char)(key - ConsoleKey.A + 1); QueueKeys and constructor-input ReadKey synthesis
// set the shift flag for uppercase letters.
// Clear() writes "[CLEAR]" marker so tests can verify it was called without losing captured output.
// ClearOutput discards captured stdout/stderr (same helper as TestConsole.ClearOutput).
// Key-queue helpers and the capture tee live in test-terminal.input.cs (same type).
// IsInteractive defaults to false; SupportsColor defaults to true for color verification.
// IDisposable cleans up StringReader and StringWriter resources, plus only the constructor-created
// MemoryStreams (tracked in Owned* fields) — consumer-assigned Standard*Stream values are never disposed.
// SimulateCancelKeyPress throws InvalidOperationException instead of silently no-oping if the
// non-public ConsoleCancelEventArgs constructor cannot be found or invoked (BCL internals changed).
// Not thread-safe by design (unlike System.Console); single-threaded test flows only.
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
/// <para>
/// Unlike <see cref="System.Console"/>, whose members are documented as thread-safe, this test
/// double is intended for single-threaded test flows: the key queue and captured-output writers
/// are unsynchronized, and members are not thread-safe. Tests driving a multi-threaded CLI must
/// synchronize access to the <see cref="TestTerminal"/> externally.
/// </para>
/// </remarks>
public sealed partial class TestTerminal : ITerminal, IDisposable
{
  private readonly StringReader InputReader;
  private readonly StringWriter OutputWriter;
  private readonly StringWriter ErrorWriter;
  private readonly Queue<ConsoleKeyInfo> KeyQueue;
  private readonly MemoryStream OwnedStandardInputStream;
  private readonly MemoryStream OwnedStandardOutputStream;
  private readonly MemoryStream OwnedStandardErrorStream;
  private bool Disposed;

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
  public TextReader In { get; private set; }

  /// <inheritdoc />
  public TextWriter Out { get; private set; }

  /// <inheritdoc />
  public TextWriter Error { get; private set; }

  /// <inheritdoc />
  public void SetIn(TextReader reader)
    => In = reader ?? throw new ArgumentNullException(nameof(reader));

  /// <inheritdoc />
  public void SetOut(TextWriter writer)
    => Out = writer ?? throw new ArgumentNullException(nameof(writer));

  /// <inheritdoc />
  public void SetError(TextWriter writer)
    => Error = writer ?? throw new ArgumentNullException(nameof(writer));

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
    In = InputReader;
    Out = OutputWriter;
    Error = ErrorWriter;
    OwnedStandardInputStream = new MemoryStream();
    OwnedStandardOutputStream = new MemoryStream();
    OwnedStandardErrorStream = new MemoryStream();
    StandardInputStream = OwnedStandardInputStream;
    StandardOutputStream = OwnedStandardOutputStream;
    StandardErrorStream = OwnedStandardErrorStream;

    // Suppress unused field warnings - these fields will be used when REPL is updated to use ITerminal
    _ = CursorLeft;
    _ = CursorTop;
  }

  /// <inheritdoc />
  public int CursorLeft { get; set; }

  /// <inheritdoc />
  public int CursorTop { get; set; }

  /// <inheritdoc />
  public bool CursorVisible { get; set; } = true;

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
    WriteTo(Out, OutputWriter, message, newLine: false);
    return this;
  }

  /// <inheritdoc />
  IConsole IConsole.Write(string message) => Write(message);

  /// <inheritdoc />
  public ITerminal WriteLine(string? message = null)
  {
    WriteTo(Out, OutputWriter, message ?? string.Empty, newLine: true);
    return this;
  }

  /// <inheritdoc />
  IConsole IConsole.WriteLine(string? message) => WriteLine(message);

  /// <inheritdoc />
  public async Task WriteLineAsync(string? message = null)
    => await WriteLineToAsync(Out, OutputWriter, message ?? string.Empty).ConfigureAwait(false);

  /// <inheritdoc />
  public ITerminal WriteErrorLine(string? message = null)
  {
    WriteTo(Error, ErrorWriter, message ?? string.Empty, newLine: true);
    return this;
  }

  /// <inheritdoc />
  IConsole IConsole.WriteErrorLine(string? message) => WriteErrorLine(message);

  /// <inheritdoc />
  public async Task WriteErrorLineAsync(string? message = null)
    => await WriteLineToAsync(Error, ErrorWriter, message ?? string.Empty).ConfigureAwait(false);

  /// <inheritdoc />
  public string? ReadLine()
    => In.ReadLine();

  /// <inheritdoc />
  public int Read()
  {
    if (KeyQueue.Count > 0)
    {
      ConsoleKeyInfo keyInfo = KeyQueue.Dequeue();
      return keyInfo.KeyChar;
    }

    return In.Read();
  }

  /// <inheritdoc />
  /// <remarks>
  /// When both the key queue and the constructor-supplied input are exhausted, this method does not
  /// block or throw: it returns a Ctrl+D (EOF) <see cref="ConsoleKeyInfo"/> — KeyChar <c>'\u0004'</c>,
  /// <see cref="ConsoleKey.D"/> with the Control modifier — on every subsequent call. Loops driving a
  /// REPL should treat <c>'\u0004'</c> as end-of-input to terminate.
  /// </remarks>
  public ConsoleKeyInfo ReadKey()
    => ReadKey(false);

  /// <inheritdoc />
  /// <remarks>
  /// When both the key queue and the constructor-supplied input are exhausted, this method does not
  /// block or throw: it returns a Ctrl+D (EOF) <see cref="ConsoleKeyInfo"/> — KeyChar <c>'\u0004'</c>,
  /// <see cref="ConsoleKey.D"/> with the Control modifier — on every subsequent call. Loops driving a
  /// REPL should treat <c>'\u0004'</c> as end-of-input to terminate.
  /// </remarks>
  public ConsoleKeyInfo ReadKey(bool intercept)
  {
    if (KeyQueue.Count > 0)
    {
      return KeyQueue.Dequeue();
    }

    // If no keys queued, try to read from input as a line
    string? line = In.ReadLine();
    if (line is null)
    {
      // EOF - simulate Ctrl+D
      return new ConsoleKeyInfo('\u0004', ConsoleKey.D, false, false, true);
    }

    // Convert line characters to keys and queue them
    foreach (char c in line)
    {
      ConsoleKey key = CharToConsoleKey(c);
      bool shift = char.IsAsciiLetterUpper(c);
      KeyQueue.Enqueue(new ConsoleKeyInfo(c, key, shift, false, false));
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
    CursorLeft = left;
    CursorTop = top;
  }

  /// <inheritdoc />
  public (int Left, int Top) GetCursorPosition()
    => (CursorLeft, CursorTop);

  /// <summary>
  /// Gets or sets the width of the terminal window. Settable so tests can control layout.
  /// </summary>
  public int WindowWidth { get; set; }

  /// <summary>
  /// Gets or sets the height of the terminal window. Settable so tests can control layout.
  /// </summary>
  public int WindowHeight { get; set; } = 24;

  /// <summary>
  /// Gets or sets the buffer width. Settable so tests can control layout.
  /// </summary>
  public int BufferWidth { get; set; } = 80;

  /// <summary>
  /// Gets or sets the buffer height. Settable so tests can control layout.
  /// </summary>
  public int BufferHeight { get; set; } = 300;

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
  /// <exception cref="InvalidOperationException">
  /// The non-public <see cref="ConsoleCancelEventArgs"/> constructor could not be found or invoked,
  /// meaning the BCL internals have changed and this helper cannot construct the event args.
  /// </exception>
  public void SimulateCancelKeyPress(ConsoleSpecialKey specialKey = ConsoleSpecialKey.ControlC)
  {
    if (CancelKeyPressHandler is null)
    {
      return;
    }

    // ConsoleCancelEventArgs has no public constructor, so we use reflection
    System.Reflection.ConstructorInfo? constructor = typeof(ConsoleCancelEventArgs)
      .GetConstructor(System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance, [typeof(ConsoleSpecialKey)]);

    if (constructor is null)
    {
      throw new InvalidOperationException
      (
        "The internal ConsoleCancelEventArgs(ConsoleSpecialKey) constructor was not found. " +
        "The BCL internals have changed and SimulateCancelKeyPress cannot construct ConsoleCancelEventArgs."
      );
    }

    ConsoleCancelEventArgs args;
    try
    {
      args = (ConsoleCancelEventArgs)constructor.Invoke([specialKey]);
    }
    catch (Exception exception) when (exception is System.Reflection.TargetInvocationException or MemberAccessException)
    {
      throw new InvalidOperationException
      (
        "Invoking the internal ConsoleCancelEventArgs(ConsoleSpecialKey) constructor failed. " +
        "The BCL internals have changed and SimulateCancelKeyPress cannot construct ConsoleCancelEventArgs.",
        exception
      );
    }

    CancelKeyPressHandler.Invoke(this, args);
  }

  /// <inheritdoc />
  /// <remarks>
  /// Does not erase captured output: output history is preserved so tests can assert on everything
  /// written before the clear. Instead, the marker line <c>[CLEAR]</c> is appended to the captured
  /// output to record that <see cref="Clear"/> was called. Use <see cref="ClearOutput"/> to actually
  /// discard captured output. <see cref="TestConsole"/> has no <see cref="ITerminal.Clear"/>;
  /// its <c>Clear</c>/<see cref="TestConsole.ClearOutput"/> helpers discard capture and do not
  /// write a marker.
  /// </remarks>
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
  public bool KeyAvailable => KeyQueue.Count > 0 || In.Peek() != -1;

}
