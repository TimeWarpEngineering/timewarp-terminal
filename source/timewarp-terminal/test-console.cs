namespace TimeWarp.Terminal;

#region Purpose
// Simplified test double for IConsole applications without interactive features.
// Captures output to StringWriter for assertion; provides scripted input via StringReader.
#endregion

#region Design
// Lighter alternative to TestTerminal for tests needing only IConsole (line-based I/O).
// Separate from TestTerminal: independent test doubles per interface, not inheritance.
// Read falls back to In when CharacterQueue is empty so constructor input and SetIn share one source.
// Write*/Read*/ReadLine route through Out/Error/In; capture writers are teed so Output/ErrorOutput
// still work after SetOut/SetError.
// IDisposable disposes StringReader/StringWriter plus only constructor-created MemoryStreams
// (Owned* fields) — consumer-assigned Standard*Stream values are never disposed.
// ClearOutput discards captured stdout/stderr. Clear is an alias; TestConsole has no ITerminal.Clear
// screen-clear marker (that lives on TestTerminal.Clear).
// Helper methods (OutputContains, GetOutputLines) reduce boilerplate in test assertions.
#endregion

/// <summary>
/// A testable implementation of <see cref="IConsole"/> that captures all output
/// and provides scripted input for deterministic testing.
/// </summary>
/// <remarks>
/// Use this class in unit tests to verify console output without interacting with the real console.
/// <example>
/// <code>
/// using TestConsole console = new("line1\nline2\n");
/// myService.Execute(console);
///
///
/// await app.RunAsync(["command1"]);
///
/// Assert.Contains("Hello!", console.Output);
/// </code>
/// </example>
/// </remarks>
public sealed class TestConsole : IConsole, IDisposable
{
  private readonly StringReader InputReader;
  private readonly StringWriter OutputWriter;
  private readonly StringWriter ErrorWriter;
  private readonly Queue<char> CharacterQueue;
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
  /// Gets or sets the input encoding for this test console.
  /// </summary>
  /// <value>The encoding used for input. Defaults to <see cref="Encoding.UTF8"/>.</value>
  public Encoding InputEncoding { get; set; } = Encoding.UTF8;

  /// <summary>
  /// Gets or sets the output encoding for this test console.
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
  /// Initializes a new instance of <see cref="TestConsole"/> with optional scripted input.
  /// </summary>
  /// <param name="input">
  /// The input to provide when <see cref="ReadLine"/> is called.
  /// Multiple lines should be separated by newlines.
  /// </param>
  public TestConsole(string input = "")
  {
    InputReader = new StringReader(input);
    OutputWriter = new StringWriter();
    ErrorWriter = new StringWriter();
    CharacterQueue = new Queue<char>();
    In = InputReader;
    Out = OutputWriter;
    Error = ErrorWriter;
    OwnedStandardInputStream = new MemoryStream();
    OwnedStandardOutputStream = new MemoryStream();
    OwnedStandardErrorStream = new MemoryStream();
    StandardInputStream = OwnedStandardInputStream;
    StandardOutputStream = OwnedStandardOutputStream;
    StandardErrorStream = OwnedStandardErrorStream;
  }

  /// <summary>
  /// Gets all standard output written to this console.
  /// </summary>
  public string Output => OutputWriter.ToString();

  /// <summary>
  /// Gets all error output written to this console.
  /// </summary>
  public string ErrorOutput => ErrorWriter.ToString();

  /// <summary>
  /// Gets all output (both standard and error) combined.
  /// </summary>
  public string AllOutput => Output + ErrorOutput;

  /// <inheritdoc />
  public IConsole Write(string message)
  {
    WriteTo(Out, OutputWriter, message, newLine: false);
    return this;
  }

  /// <inheritdoc />
  public IConsole WriteLine(string? message = null)
  {
    WriteTo(Out, OutputWriter, message ?? string.Empty, newLine: true);
    return this;
  }

  /// <inheritdoc />
  public async Task WriteLineAsync(string? message = null)
    => await WriteLineToAsync(Out, OutputWriter, message ?? string.Empty).ConfigureAwait(false);

  /// <inheritdoc />
  public IConsole WriteErrorLine(string? message = null)
  {
    WriteTo(Error, ErrorWriter, message ?? string.Empty, newLine: true);
    return this;
  }

  /// <inheritdoc />
  public async Task WriteErrorLineAsync(string? message = null)
    => await WriteLineToAsync(Error, ErrorWriter, message ?? string.Empty).ConfigureAwait(false);

  /// <inheritdoc />
  public string? ReadLine()
    => In.ReadLine();

  /// <inheritdoc />
  public int Read()
    => CharacterQueue.Count > 0 ? CharacterQueue.Dequeue() : In.Read();

  /// <summary>
  /// Queues characters for <see cref="Read"/> to return.
  /// </summary>
  /// <param name="characters">The characters to queue.</param>
  public void QueueCharacters(string characters)
  {
    ArgumentNullException.ThrowIfNull(characters);
    foreach (char c in characters)
    {
      CharacterQueue.Enqueue(c);
    }
  }

  /// <summary>
  /// Gets the number of characters currently in the queue.
  /// </summary>
  public int CharactersInQueue => CharacterQueue.Count;

  /// <summary>
  /// Clears all captured output.
  /// </summary>
  /// <remarks>
  /// Discards captured stdout and stderr so subsequent assertions observe only writes after
  /// this call. Prefer <see cref="ClearOutput"/> when switching between <see cref="TestConsole"/>
  /// and <see cref="TestTerminal"/> — on <see cref="TestTerminal"/>, <c>Clear</c> is the
  /// <see cref="ITerminal.Clear"/> screen-clear marker and <c>ClearOutput</c> discards capture.
  /// <see cref="TestConsole"/> has no screen-clear operation; <see cref="Clear"/> is an alias
  /// of <see cref="ClearOutput"/>.
  /// </remarks>
  public void Clear()
    => ClearOutput();

  /// <summary>
  /// Clears all captured output.
  /// </summary>
  /// <remarks>
  /// Discards captured stdout and stderr. Same helper as <see cref="TestTerminal.ClearOutput"/>.
  /// Unlike <see cref="TestTerminal.Clear"/>, this does not append a <c>[CLEAR]</c> marker.
  /// </remarks>
  public void ClearOutput()
  {
    _ = OutputWriter.GetStringBuilder().Clear();
    _ = ErrorWriter.GetStringBuilder().Clear();
  }

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
  /// Gets the error lines as an array.
  /// </summary>
  /// <returns>An array of error lines.</returns>
  public string[] GetErrorLines()
    => ErrorOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

  /// <summary>
  /// Disposes the resources used by this instance.
  /// </summary>
  public void Dispose()
  {
    if (Disposed)
    {
      return;
    }

    InputReader.Dispose();
    OutputWriter.Dispose();
    ErrorWriter.Dispose();

    // Dispose only the streams this instance created in its constructor.
    // Consumer-assigned Standard*Stream replacements are owned by the consumer and must not be disposed here.
    OwnedStandardInputStream.Dispose();
    OwnedStandardOutputStream.Dispose();
    OwnedStandardErrorStream.Dispose();
    Disposed = true;
  }

  private static void WriteTo(TextWriter destination, StringWriter capture, string value, bool newLine)
  {
    if (newLine)
    {
      destination.WriteLine(value);
    }
    else
    {
      destination.Write(value);
    }

    if (ReferenceEquals(destination, capture))
    {
      return;
    }

    if (newLine)
    {
      capture.WriteLine(value);
    }
    else
    {
      capture.Write(value);
    }
  }

  private static async Task WriteLineToAsync(TextWriter destination, StringWriter capture, string value)
  {
    await destination.WriteLineAsync(value).ConfigureAwait(false);
    if (!ReferenceEquals(destination, capture))
    {
      await capture.WriteLineAsync(value).ConfigureAwait(false);
    }
  }
}
