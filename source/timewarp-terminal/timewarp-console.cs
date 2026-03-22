namespace TimeWarp.Terminal;

/// <summary>
/// Default implementation of <see cref="IConsole"/> that wraps <see cref="System.Console"/>.
/// </summary>
/// <remarks>
/// This class provides the production console implementation for console applications.
/// For testing scenarios, use <see cref="TestConsole"/> or create a custom implementation.
/// </remarks>
public sealed class TimeWarpConsole : IConsole
{
  /// <summary>
  /// Gets the default singleton instance of <see cref="TimeWarpConsole"/>.
  /// </summary>
  public static TimeWarpConsole Default { get; } = new();

  /// <inheritdoc />
  public IConsole Write(string message)
  {
    Console.Write(message);
    return this;
  }

  /// <inheritdoc />
  public IConsole WriteLine(string? message = null)
  {
    Console.WriteLine(message ?? string.Empty);
    return this;
  }

  /// <inheritdoc />
  public Task WriteLineAsync(string? message = null)
    => Console.Out.WriteLineAsync(message);

  /// <inheritdoc />
  public IConsole WriteErrorLine(string? message = null)
  {
    Console.Error.WriteLine(message ?? string.Empty);
    return this;
  }

  /// <inheritdoc />
  public Task WriteErrorLineAsync(string? message = null)
    => Console.Error.WriteLineAsync(message);

  /// <inheritdoc />
  public string? ReadLine()
    => Console.ReadLine();

  /// <inheritdoc />
  public int Read()
    => Console.Read();

  /// <inheritdoc />
  public ConsoleKeyInfo ReadKey()
    => Console.ReadKey(false);

  /// <inheritdoc />
  public Encoding InputEncoding
  {
    get => Console.InputEncoding;
    set => Console.InputEncoding = value;
  }

  /// <inheritdoc />
  public Encoding OutputEncoding
  {
    get => Console.OutputEncoding;
    set => Console.OutputEncoding = value;
  }

  /// <inheritdoc />
  public bool IsInputRedirected => Console.IsInputRedirected;

  /// <inheritdoc />
  public bool IsOutputRedirected => Console.IsOutputRedirected;

  /// <inheritdoc />
  public bool IsErrorRedirected => Console.IsErrorRedirected;

  /// <inheritdoc />
  public Stream OpenStandardInput()
    => Console.OpenStandardInput();

  /// <inheritdoc />
  public Stream OpenStandardOutput()
    => Console.OpenStandardOutput();

  /// <inheritdoc />
  public Stream OpenStandardError()
    => Console.OpenStandardError();

  /// <inheritdoc />
  public TextReader In => Console.In;

  /// <inheritdoc />
  public TextWriter Out => Console.Out;

  /// <inheritdoc />
  public TextWriter Error => Console.Error;

  /// <inheritdoc />
  public void SetIn(TextReader reader)
    => Console.SetIn(reader);

  /// <inheritdoc />
  public void SetOut(TextWriter writer)
    => Console.SetOut(writer);

  /// <inheritdoc />
  public void SetError(TextWriter writer)
    => Console.SetError(writer);
}
