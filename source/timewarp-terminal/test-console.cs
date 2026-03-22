namespace TimeWarp.Terminal;

#region Purpose
// Simplified test double for IConsole applications without interactive features.
// Captures output to StringWriter for assertion; provides scripted input via StringReader.
#endregion

#region Design
// Lighter alternative to TestTerminal for tests needing only IConsole (line-based I/O).
// Separate from TestTerminal: independent test doubles per interface, not inheritance.
// IDisposable cleans up StringReader and StringWriter resources.
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
  private bool Disposed;

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
    OutputWriter.Write(message);
    return this;
  }

  /// <inheritdoc />
  public IConsole WriteLine(string? message = null)
  {
    OutputWriter.WriteLine(message ?? string.Empty);
    return this;
  }

  /// <inheritdoc />
  public async Task WriteLineAsync(string? message = null)
    => await OutputWriter.WriteLineAsync(message ?? string.Empty).ConfigureAwait(false);

  /// <inheritdoc />
  public IConsole WriteErrorLine(string? message = null)
  {
    ErrorWriter.WriteLine(message ?? string.Empty);
    return this;
  }

  /// <inheritdoc />
  public async Task WriteErrorLineAsync(string? message = null)
    => await ErrorWriter.WriteLineAsync(message ?? string.Empty).ConfigureAwait(false);

  /// <inheritdoc />
  public string? ReadLine()
    => InputReader.ReadLine();

  /// <inheritdoc />
  public int Read()
  {
    if (CharacterQueue.Count > 0)
      return CharacterQueue.Dequeue();

    return -1;
  }

  /// <inheritdoc />
  public ConsoleKeyInfo ReadKey()
    => throw new NotSupportedException("TestConsole does not support key-by-key input. Use TestTerminal for interactive key input.");

  /// <summary>
  /// Queues characters for <see cref="Read"/> to return.
  /// </summary>
  /// <param name="characters">The characters to queue.</param>
  public void QueueCharacters(string characters)
  {
    ArgumentNullException.ThrowIfNull(characters);
    foreach (char c in characters)
      CharacterQueue.Enqueue(c);
  }

  /// <summary>
  /// Gets the number of characters currently in the queue.
  /// </summary>
  public int CharactersInQueue => CharacterQueue.Count;

  /// <summary>
  /// Clears all captured output.
  /// </summary>
  public void Clear()
  {
    OutputWriter.GetStringBuilder().Clear();
    ErrorWriter.GetStringBuilder().Clear();
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
      return;

    InputReader.Dispose();
    OutputWriter.Dispose();
    ErrorWriter.Dispose();
    Disposed = true;
  }
}
