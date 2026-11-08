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
// Color support respects the NO_COLOR spec (non-empty value disables color) and TERM=dumb.
// Parameterless Beep swallows IOException when the console is unavailable but does not gate on
// stdout redirection: Windows BCL still beeps via Kernel32.Beep when output is redirected.
// Cursor, window, and buffer members live in timewarp-terminal.geometry.cs (same type).
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
public sealed partial class TimeWarpTerminal : ITerminal
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

  /// <inheritdoc />
  public ConsoleKeyInfo ReadKey(bool intercept)
    => Console.ReadKey(intercept);

  /// <inheritdoc />
  public bool IsInteractive
    => !Console.IsInputRedirected && !Console.IsOutputRedirected;

  /// <inheritdoc />
  public bool SupportsColor
    => !Console.IsOutputRedirected
      && Environment.GetEnvironmentVariable("NO_COLOR") is not { Length: > 0 }
      && !string.Equals(Environment.GetEnvironmentVariable("TERM"), "dumb", StringComparison.Ordinal);

  /// <inheritdoc />
  public bool SupportsHyperlinks => DetectHyperlinkSupport();

  /// <summary>
  /// Detects whether the terminal supports OSC 8 hyperlinks based on environment variables.
  /// </summary>
  private static bool DetectHyperlinkSupport()
  {
    // No hyperlinks if output is redirected
    if (Console.IsOutputRedirected)
    {
      return false;
    }

    // Windows Terminal
    if (Environment.GetEnvironmentVariable("WT_SESSION") is not null)
    {
      return true;
    }

    // VS Code integrated terminal
    if (Environment.GetEnvironmentVariable("TERM_PROGRAM") == "vscode")
    {
      return true;
    }

    // iTerm2
    if (Environment.GetEnvironmentVariable("TERM_PROGRAM") == "iTerm.app")
    {
      return true;
    }

    // Konsole
    if (Environment.GetEnvironmentVariable("KONSOLE_VERSION") is not null)
    {
      return true;
    }

    // GNOME Terminal (VTE 0.50+ / version 5000+)
    string? vteVersion = Environment.GetEnvironmentVariable("VTE_VERSION");
    if (vteVersion is not null && int.TryParse(vteVersion, out int version) && version >= 5000)
    {
      return true;
    }

    // Hyper terminal
    if (Environment.GetEnvironmentVariable("TERM_PROGRAM") == "Hyper")
    {
      return true;
    }

    // Default: assume no support for unknown terminals
    return false;
  }

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

  /// <inheritdoc />
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

  /// <inheritdoc />
  public void Beep()
  {
    // Console.Beep() is cross-platform (emits BEL on Unix; Unix BCL already no-ops when stdout
    // is redirected). Windows still calls Kernel32.Beep when stdout is redirected; match that
    // fallback and only swallow IOException when the console is unavailable.
    try
    {
      Console.Beep();
    }
    catch (IOException)
    {
      // Silently ignore if the console is unavailable
    }
  }

  /// <inheritdoc />
  public void Beep(int frequency, int duration)
  {
    if (OperatingSystem.IsWindows())
    {
      Console.Beep(frequency, duration);
    }
  }

  /// <inheritdoc />
  public bool TreatControlCAsInput
  {
    get
    {
      try
      {
        return Console.TreatControlCAsInput;
      }
      catch (IOException)
      {
        return false;
      }
      catch (InvalidOperationException)
      {
        // Thrown when no console is attached (e.g., redirected input)
        return false;
      }
    }
    set
    {
      try
      {
        Console.TreatControlCAsInput = value;
      }
      catch (IOException)
      {
      }
      catch (InvalidOperationException)
      {
        // Silently ignore when no console is attached
      }
    }
  }

  /// <inheritdoc />
  public string Title
  {
    get
    {
      if (OperatingSystem.IsWindows())
      {
        try
        {
          return Console.Title;
        }
        catch (IOException)
        {
          return string.Empty;
        }
      }

      return string.Empty;
    }
    set
    {
      // Console.Title's setter is supported on all platforms
      // (only the getter is Windows-only)
      try
      {
        Console.Title = value;
      }
      catch (IOException)
      {
        // Silently ignore if console is redirected
      }
    }
  }

  /// <inheritdoc />
  public bool KeyAvailable
  {
    get
    {
      try
      {
        return Console.KeyAvailable;
      }
      catch (IOException)
      {
        return false;
      }
      catch (InvalidOperationException)
      {
        // Console.KeyAvailable throws InvalidOperationException when
        // standard input is redirected from a file or pipe
        return false;
      }
    }
  }
}
