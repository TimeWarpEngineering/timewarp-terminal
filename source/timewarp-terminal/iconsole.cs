namespace TimeWarp.Terminal;

#region Purpose
// Minimal abstraction for basic console I/O operations.
// Enables testability by abstracting System.Console for dependency injection.
#endregion

#region Design
// Return types are IConsole for fluent chaining across implementations.
// Async methods return Task (not IConsole) since they cannot participate in fluent chains.
// XML documentation emphasizes testability scenarios over implementation details.
#endregion

/// <summary>
/// Abstraction for basic console I/O operations.
/// Provides a testable interface for console input and output.
/// </summary>
/// <remarks>
/// Implement this interface for custom console environments such as:
/// <list type="bullet">
///   <item><description>Unit testing with captured output</description></item>
///   <item><description>Web-based terminals</description></item>
///   <item><description>GUI application integration</description></item>
///   <item><description>Remote console access</description></item>
/// </list>
/// </remarks>
public interface IConsole
{
  /// <summary>
  /// Writes the specified string value to the standard output stream.
  /// </summary>
  /// <param name="message">The value to write.</param>
  /// <returns>This console instance for fluent chaining.</returns>
  IConsole Write(string message);

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <returns>This console instance for fluent chaining.</returns>
  IConsole WriteLine(string? message = null);

  /// <summary>
  /// Asynchronously writes the specified string value, followed by the current line terminator,
  /// to the standard output stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <returns>A task that represents the asynchronous write operation.</returns>
  Task WriteLineAsync(string? message = null);

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard error stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <returns>This console instance for fluent chaining.</returns>
  IConsole WriteErrorLine(string? message = null);

  /// <summary>
  /// Asynchronously writes the specified string value, followed by the current line terminator,
  /// to the standard error stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  /// <returns>A task that represents the asynchronous write operation.</returns>
  Task WriteErrorLineAsync(string? message = null);

  /// <summary>
  /// Reads the next line of characters from the standard input stream.
  /// </summary>
  /// <returns>
  /// The next line of characters from the input stream, or null if no more lines are available.
  /// </returns>
  string? ReadLine();

  /// <summary>
  /// Reads the next character from the standard input stream.
  /// </summary>
  /// <returns>
  /// The next character from the input stream, or -1 if no more characters are available.
  /// </returns>
  int Read();

  /// <summary>
  /// Obtains the next character or function key pressed by the user.
  /// The pressed key is displayed in the console window.
  /// </summary>
  /// <returns>
  /// An object that describes the <see cref="ConsoleKey"/> constant and Unicode character,
  /// if any, that correspond to the pressed console key.
  /// </returns>
  ConsoleKeyInfo ReadKey();

  /// <summary>
  /// Gets or sets the encoding the console uses to read input.
  /// </summary>
  /// <value>The encoding used to read console input.</value>
  Encoding InputEncoding { get; set; }

  /// <summary>
  /// Gets or sets the encoding the console uses to write output.
  /// </summary>
  /// <value>The encoding used to write console output.</value>
  Encoding OutputEncoding { get; set; }

  /// <summary>
  /// Gets a value indicating whether the input stream has been redirected from the standard input stream.
  /// </summary>
  /// <value><c>true</c> if input is redirected; otherwise, <c>false</c>.</value>
  bool IsInputRedirected { get; }

  /// <summary>
  /// Gets a value indicating whether the output stream has been redirected from the standard output stream.
  /// </summary>
  /// <value><c>true</c> if output is redirected; otherwise, <c>false</c>.</value>
  bool IsOutputRedirected { get; }

  /// <summary>
  /// Gets a value indicating whether the error stream has been redirected from the standard error stream.
  /// </summary>
  /// <value><c>true</c> if error output is redirected; otherwise, <c>false</c>.</value>
  bool IsErrorRedirected { get; }

  /// <summary>
  /// Acquires the standard input stream.
  /// </summary>
  /// <returns>The standard input stream.</returns>
  Stream OpenStandardInput();

  /// <summary>
  /// Acquires the standard output stream.
  /// </summary>
  /// <returns>The standard output stream.</returns>
  Stream OpenStandardOutput();

  /// <summary>
  /// Acquires the standard error output stream.
  /// </summary>
  /// <returns>The standard error output stream.</returns>
  Stream OpenStandardError();

  /// <summary>
  /// Gets the standard input reader.
  /// </summary>
  /// <value>A <see cref="TextReader"/> that represents the standard input stream.</value>
  TextReader In { get; }

  /// <summary>
  /// Gets the standard output writer.
  /// </summary>
  /// <value>A <see cref="TextWriter"/> that represents the standard output stream.</value>
  TextWriter Out { get; }

  /// <summary>
  /// Gets the standard error writer.
  /// </summary>
  /// <value>A <see cref="TextWriter"/> that represents the standard error output stream.</value>
  TextWriter Error { get; }

  /// <summary>
  /// Sets the <see cref="In"/> property to the specified <see cref="TextReader"/>.
  /// </summary>
  /// <param name="reader">A <see cref="TextReader"/> that represents the new standard input stream.</param>
  void SetIn(TextReader reader);

  /// <summary>
  /// Sets the <see cref="Out"/> property to the specified <see cref="TextWriter"/>.
  /// </summary>
  /// <param name="writer">A <see cref="TextWriter"/> that represents the new standard output stream.</param>
  void SetOut(TextWriter writer);

  /// <summary>
  /// Sets the <see cref="Error"/> property to the specified <see cref="TextWriter"/>.
  /// </summary>
  /// <param name="writer">A <see cref="TextWriter"/> that represents the new standard error output stream.</param>
  void SetError(TextWriter writer);
}
