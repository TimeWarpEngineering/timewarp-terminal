namespace TimeWarp.Terminal;

#region Purpose
// Static facade providing Console-compatible API while maintaining testability.
// Routes all calls to configurable Instance property, enabling test doubles.
#endregion

#region Design
// Static API mimics System.Console for easy migration from existing code.
// Instance property allows swapping implementation for testing without DI container.
// Dedicated format overloads for 1-3 args avoid array allocation (params variant for 4+).
// Format overloads use FormatProvider ?? CurrentCulture (resolved per call), matching
// System.Console's culture behavior while allowing an invariant override for determinism.
// FormatProvider is an async-local override while TestTerminalContext is active, else
// process-global, so parallel Use scopes that assign it do not race.
// Color methods use AnsiColors to wrap messages with ANSI escape sequences, applied only
// when Instance.SupportsColor is true (NO_COLOR / redirected output degrade to plain text).
// A null message writes plain (no color prefix/reset), matching the non-colored overloads.
// Error-colored writers also require !IsErrorRedirected so library SGR is not written into
// a redirected stderr when stdout is still a TTY.
// CancelKeyPress handlers live on the facade; a single forwarder is attached to the Instance
// resolved at first subscribe. Process-global Instance assignment rebinds the forwarder to the
// assigned field, not TestTerminalContext.Current; async-local Use swaps do not move it.
// Colored widget output re-applies the color prefix after every embedded SGR reset (from
// BorderColor or styled cells) so the requested colors survive styled segments in a line.
// CA1054 suppressed for WriteLink/WriteLinkLine: OSC 8 hyperlinks use raw URL strings by design.
// Write, widget, and console members live in terminal-static.write.cs,
// terminal-static.widgets.cs, and terminal-static.console.cs so Terminal stays one
// public type and each file stays within the 500-line cap.
#endregion

using System.Globalization;

/// <summary>
/// Static facade providing a Console-compatible API for terminal operations.
/// Routes all calls to the configurable <see cref="Instance"/>.
/// </summary>
/// <remarks>
/// This class provides a convenient static API similar to <see cref="System.Console"/>
/// while maintaining testability through the configurable <see cref="Instance"/> property.
/// <para>
/// Usage patterns:
/// <list type="bullet">
///   <item><description>Production code uses the static methods directly</description></item>
///   <item><description>Tests can replace <see cref="Instance"/> with a test implementation</description></item>
/// </list>
/// </para>
/// <example>
/// Production usage:
/// <code>
/// Terminal.WriteLine("Hello, World!");
/// var input = Terminal.ReadLine();
/// </code>
/// </example>
/// <example>
/// Test setup:
/// <code>
/// Terminal.Instance = new TestTerminal();
/// // ... run tests ...
/// Terminal.Instance = TimeWarpTerminal.Default; // restore
/// </code>
/// </example>
/// </remarks>
public static partial class Terminal
{
  private static readonly AsyncLocal<FormatProviderBox?> FormatProviderOverride = new();
  private static IFormatProvider? ProcessFormatProvider { get; set; }
  private static readonly Lock CancelKeyPressSync = new();
  private static readonly ConsoleCancelEventHandler CancelKeyPressForwarder = OnCancelKeyPress;
  private static ConsoleCancelEventHandler? CancelKeyPressHandlers;
  private static ITerminal? CancelKeyPressBoundInstance;

  private sealed class FormatProviderBox
  {
    public required IFormatProvider? Provider { get; init; }
  }

  /// <summary>
  /// Gets or sets the terminal instance used by all static methods.
  /// Defaults to <see cref="TimeWarpTerminal.Default"/> for production use.
  /// </summary>
  /// <value>The current terminal implementation.</value>
  /// <exception cref="ArgumentNullException">Thrown when attempting to set a null value.</exception>
  /// <remarks>
  /// The getter resolves the async-local <c>TestTerminalContext.Current</c> first, falling
  /// back to the process-global instance assigned via the setter. This makes
  /// <c>TestTerminalContext.SetCurrent</c>/<c>Use</c> safe for parallel test execution —
  /// each async flow sees its own terminal — while direct assignment
  /// (<c>Terminal.Instance = new TestTerminal()</c>) keeps working for serial tests.
  /// </remarks>
  public static ITerminal Instance
  {
    get => TestTerminalContext.Current ?? field;
    set
    {
      field = value ?? throw new ArgumentNullException(nameof(value));
      SyncCancelKeyPressForwarder(field);
    }
  } = TimeWarpTerminal.Default;

  /// <summary>
  /// Gets or sets the format provider used by the format overloads of
  /// <see cref="Write(string, object?)"/>, <see cref="WriteLine(string, object?)"/>,
  /// and <see cref="WriteErrorLine(string, object?)"/>.
  /// </summary>
  /// <value>
  /// The provider to format with, or <c>null</c> to use <see cref="CultureInfo.CurrentCulture"/>
  /// as resolved at the time of each call. Defaults to <c>null</c>, matching
  /// <see cref="System.Console"/>'s current-culture formatting behavior.
  /// </value>
  /// <remarks>
  /// Set to <see cref="CultureInfo.InvariantCulture"/> for deterministic output in tests or logs.
  /// When a <c>TestTerminalContext</c> is active, the setter writes an async-local override so
  /// parallel <c>Use</c> scopes do not race; otherwise the value is process-global. The getter
  /// prefers the async-local override and falls back to the process-global value.
  /// </remarks>
  public static IFormatProvider? FormatProvider
  {
    get => FormatProviderOverride.Value is FormatProviderBox box
      ? box.Provider
      : ProcessFormatProvider;
    set
    {
      if (TestTerminalContext.HasValue)
      {
        FormatProviderOverride.Value = new FormatProviderBox { Provider = value };
      }
      else
      {
        ProcessFormatProvider = value;
        FormatProviderOverride.Value = null;
      }
    }
  }

  internal static void CaptureFormatProviderOverride(out bool hasOverride, out IFormatProvider? provider)
  {
    if (FormatProviderOverride.Value is FormatProviderBox box)
    {
      hasOverride = true;
      provider = box.Provider;
      return;
    }

    hasOverride = false;
    provider = null;
  }

  internal static void RestoreFormatProviderOverride(bool hasOverride, IFormatProvider? provider)
  {
    FormatProviderOverride.Value = hasOverride
      ? new FormatProviderBox { Provider = provider }
      : null;
  }

  private static IFormatProvider ActiveFormatProvider => FormatProvider ?? CultureInfo.CurrentCulture;

  // Output Methods

  /// <summary>
  /// Writes the specified string value to the standard output stream.
  /// </summary>
  /// <param name="message">The value to write. If null, an empty string is written.</param>
  public static void Write(string? message) => Instance.Write(message ?? string.Empty);

  /// <summary>
  /// Writes the specified string value, followed by the current line terminator,
  /// to the standard output stream.
  /// </summary>
  /// <param name="message">The value to write. If null, only the line terminator is written.</param>
  public static void WriteLine(string? message = null) => Instance.WriteLine(message);

}
