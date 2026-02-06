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
}
