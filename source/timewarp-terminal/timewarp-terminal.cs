namespace TimeWarp.Terminal;

#region Purpose
// Production implementation of ITerminal wrapping System.Console.
// Provides singleton access via Default property for applications requiring interactive features.
#endregion

#region Design
// Singleton pattern with public static Default property for convenient access.
// Explicit interface implementations (IConsole.Write) hide base interface methods from public API.
// IOExceptions silently swallowed because redirected output is a valid runtime scenario.
// Hyperlink detection uses environment variable heuristics for major terminal emulators.
// Color support respects NO_COLOR environment variable (de facto standard).
#endregion

/// <summary>
/// Default implementation of <see cref="ITerminal"/> that wraps <see cref="System.Console"/>
/// with full interactive terminal capabilities.
/// </summary>
/// <remarks>
/// This class provides the production terminal implementation for console applications
/// requiring interactive features such as REPL, tab completion, and arrow key navigation.
/// For testing scenarios, use <see cref="TestTerminal"/> or create a custom implementation.
/// </remarks>
public sealed class TimeWarpTerminal : ITerminal
{
  /// <summary>
  /// Gets the default singleton instance of <see cref="TimeWarpTerminal"/>.
  /// </summary>
  public static TimeWarpTerminal Default { get; } = new();

  /// <inheritdoc />
  public ITerminal Write(string message)
  {
    Console.Write(message);
    return this;
  }

  /// <inheritdoc />
  IConsole IConsole.Write(string message) => Write(message);

  /// <inheritdoc />
  public ITerminal WriteLine(string? message = null)
  {
    Console.WriteLine(message ?? string.Empty);
    return this;
  }

  /// <inheritdoc />
  IConsole IConsole.WriteLine(string? message) => WriteLine(message);

  /// <inheritdoc />
  public Task WriteLineAsync(string? message = null)
    => Console.Out.WriteLineAsync(message);

  /// <inheritdoc />
  public ITerminal WriteErrorLine(string? message = null)
  {
    Console.Error.WriteLine(message ?? string.Empty);
    return this;
  }

  /// <inheritdoc />
  IConsole IConsole.WriteErrorLine(string? message) => WriteErrorLine(message);

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
  public ConsoleKeyInfo ReadKey(bool intercept)
    => Console.ReadKey(intercept);

  /// <inheritdoc />
  public int CursorLeft
  {
    get
    {
      try
      {
        return Console.CursorLeft;
      }
      catch (IOException)
      {
        return 0;
      }
    }
    set
    {
      try
      {
        Console.CursorLeft = value;
      }
      catch (ArgumentOutOfRangeException)
      {
      }
      catch (IOException)
      {
      }
    }
  }

  /// <inheritdoc />
  public int CursorTop
  {
    get
    {
      try
      {
        return Console.CursorTop;
      }
      catch (IOException)
      {
        return 0;
      }
    }
    set
    {
      try
      {
        Console.CursorTop = value;
      }
      catch (ArgumentOutOfRangeException)
      {
      }
      catch (IOException)
      {
      }
    }
  }

  /// <inheritdoc />
  public bool CursorVisible
  {
    get
    {
      if (!OperatingSystem.IsWindows())
        return true;

      try
      {
        return Console.CursorVisible;
      }
      catch (IOException)
      {
        return true;
      }
    }
    set
    {
      if (!OperatingSystem.IsWindows())
        return;

      try
      {
        Console.CursorVisible = value;
      }
      catch (IOException)
      {
      }
    }
  }

  /// <inheritdoc />
  public int CursorSize
  {
    get
    {
      if (!OperatingSystem.IsWindows())
        return 100;

      try
      {
        return Console.CursorSize;
      }
      catch (IOException)
      {
        return 100;
      }
    }
    set
    {
      if (!OperatingSystem.IsWindows())
        return;

      try
      {
        Console.CursorSize = value;
      }
      catch (ArgumentOutOfRangeException)
      {
      }
      catch (IOException)
      {
      }
    }
  }

  /// <inheritdoc />
  public void SetCursorPosition(int left, int top)
  {
    try
    {
      Console.SetCursorPosition(left, top);
    }
    catch (ArgumentOutOfRangeException)
    {
      // Silently ignore invalid cursor positions
    }
    catch (IOException)
    {
      // Silently ignore I/O errors (e.g., redirected output)
    }
  }

  /// <inheritdoc />
  public (int Left, int Top) GetCursorPosition()
  {
    try
    {
      return (Console.CursorLeft, Console.CursorTop);
    }
    catch (IOException)
    {
      // Return default if console is redirected
      return (0, 0);
    }
  }

  /// <inheritdoc />
  public int WindowWidth
  {
    get
    {
      try
      {
        return Console.WindowWidth;
      }
      catch (IOException)
      {
        // Return default width if console is redirected
        return 80;
      }
    }
  }

  /// <inheritdoc />
  public bool IsInteractive
    => !Console.IsInputRedirected;

  /// <inheritdoc />
  public bool SupportsColor
    => !Console.IsOutputRedirected && Environment.GetEnvironmentVariable("NO_COLOR") is null;

  /// <inheritdoc />
  public bool SupportsHyperlinks => DetectHyperlinkSupport();

  /// <summary>
  /// Detects whether the terminal supports OSC 8 hyperlinks based on environment variables.
  /// </summary>
  private static bool DetectHyperlinkSupport()
  {
    // No hyperlinks if output is redirected
    if (Console.IsOutputRedirected)
      return false;

    // Windows Terminal
    if (Environment.GetEnvironmentVariable("WT_SESSION") is not null)
      return true;

    // VS Code integrated terminal
    if (Environment.GetEnvironmentVariable("TERM_PROGRAM") == "vscode")
      return true;

    // iTerm2
    if (Environment.GetEnvironmentVariable("TERM_PROGRAM") == "iTerm.app")
      return true;

    // Konsole
    if (Environment.GetEnvironmentVariable("KONSOLE_VERSION") is not null)
      return true;

    // GNOME Terminal (VTE 0.50+ / version 5000+)
    string? vteVersion = Environment.GetEnvironmentVariable("VTE_VERSION");
    if (vteVersion is not null && int.TryParse(vteVersion, out int version) && version >= 5000)
      return true;

    // Hyper terminal
    if (Environment.GetEnvironmentVariable("TERM_PROGRAM") == "Hyper")
      return true;

    // Default: assume no support for unknown terminals
    return false;
  }

  /// <inheritdoc />
  /// <inheritdoc />
  public event ConsoleCancelEventHandler? CancelKeyPress
  {
    add => Console.CancelKeyPress += value;
    remove => Console.CancelKeyPress -= value;
  }

  /// <inheritdoc />
  public ConsoleColor ForegroundColor
  {
    get
    {
      try
      {
        return Console.ForegroundColor;
      }
      catch (IOException)
      {
        return ConsoleColor.Gray;
      }
    }
    set
    {
      try
      {
        Console.ForegroundColor = value;
      }
      catch (IOException)
      {
      }
    }
  }

  /// <inheritdoc />
  public ConsoleColor BackgroundColor
  {
    get
    {
      try
      {
        return Console.BackgroundColor;
      }
      catch (IOException)
      {
        return ConsoleColor.Black;
      }
    }
    set
    {
      try
      {
        Console.BackgroundColor = value;
      }
      catch (IOException)
      {
      }
    }
  }

  /// <inheritdoc />
  public void ResetColor()
  {
    try
    {
      Console.ResetColor();
    }
    catch (IOException)
    {
    }
  }

  public void Clear()
  {
    try
    {
      Console.Clear();
    }
    catch (IOException)
    {
      // Silently ignore if console is redirected
    }
  }
}
