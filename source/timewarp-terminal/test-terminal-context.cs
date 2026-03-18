namespace TimeWarp.Terminal;

/// <summary>
/// Provides an ambient context for <see cref="TestTerminal"/> that enables zero-configuration testing
/// of CLI applications with automatic <see cref="TimeWarp.Terminal.Terminal.Instance"/> synchronization.
/// </summary>
/// <remarks>
/// <para>
/// This class uses <see cref="AsyncLocal{T}"/> to provide a test terminal that flows with the
/// async execution context. This means each test gets its own isolated terminal even when
/// running tests in parallel.
/// </para>
/// <para>
/// When <see cref="Current"/> is set, it automatically updates <see cref="TimeWarp.Terminal.Terminal.Instance"/>
/// and restores the previous instance when cleared.
/// </para>
/// <para>
/// Resolution order when determining which terminal to use:
/// <list type="number">
///   <item><description><see cref="Current"/> (if set)</description></item>
///   <item><description><c>ITerminal</c> from DI (if registered)</description></item>
///   <item><description><see cref="TimeWarpTerminal.Default"/> (fallback)</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Simple test pattern with automatic TimeWarp.Terminal.Terminal.Instance synchronization:
/// <code>
/// public static async Task Should_display_greeting()
/// {
///     using TestTerminal terminal = new();
///     TestTerminalContext.Current = terminal;
///     
///     // TimeWarp.Terminal.Terminal.Instance is now set to terminal
///     Terminal.WriteLine("Hello");  // Routes to test terminal
///     
///     await Program.Main(["greet", "World"]);
///     
///     terminal.OutputContains("Hello, World!").ShouldBeTrue();
///     
///     // On dispose, TestTerminal clears context and restores previous TimeWarp.Terminal.Terminal.Instance
/// }
/// </code>
/// </example>
public static class TestTerminalContext
{
  private static readonly AsyncLocal<TestTerminal?> Context = new();
  private static readonly AsyncLocal<ITerminal?> PreviousInstance = new();

  /// <summary>
  /// Gets or sets the current <see cref="TestTerminal"/> for the async execution context.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Setting this property to a non-null value causes:
  /// <list type="bullet">
  ///   <item><description>The current <see cref="TimeWarp.Terminal.Terminal.Instance"/> to be saved</description></item>
  ///   <item><description><see cref="TimeWarp.Terminal.Terminal.Instance"/> to be set to the provided terminal</description></item>
  /// </list>
  /// </para>
  /// <para>
  /// Setting this property to <c>null</c> causes:
  /// <list type="bullet">
  ///   <item><description><see cref="TimeWarp.Terminal.Terminal.Instance"/> to be restored to its previous value</description></item>
  /// </list>
  /// </para>
  /// <para>
  /// The value is scoped to the current async execution context, so parallel tests
  /// each have their own isolated value.
  /// </para>
  /// </remarks>
  /// <value>
  /// The <see cref="TestTerminal"/> for the current context, or <c>null</c> if not set.
  /// </value>
  public static TestTerminal? Current
  {
    get => Context.Value;
    set
    {
      if (value is not null)
      {
        // Save current TimeWarp.Terminal.Terminal.Instance before replacing
        PreviousInstance.Value = TimeWarp.Terminal.Terminal.Instance;
        Context.Value = value;
        TimeWarp.Terminal.Terminal.Instance = value;
      }
      else if (Context.Value is not null)
      {
        // Restore previous TimeWarp.Terminal.Terminal.Instance
        Context.Value = null;
        if (PreviousInstance.Value is not null)
        {
          TimeWarp.Terminal.Terminal.Instance = PreviousInstance.Value;
          PreviousInstance.Value = null;
        }
      }
    }
  }

  /// <summary>
  /// Gets a value indicating whether a <see cref="TestTerminal"/> is set for the current context.
  /// </summary>
  public static bool HasValue => Context.Value is not null;

  /// <summary>
  /// Gets the <see cref="TestTerminal"/> for the current context, or throws if not set.
  /// </summary>
  /// <returns>The current <see cref="TestTerminal"/>.</returns>
  /// <exception cref="InvalidOperationException">Thrown when no test terminal is set.</exception>
  public static TestTerminal Terminal
    => Context.Value ?? throw new InvalidOperationException("No TestTerminal set in current context. Set TestTerminalContext.Current first.");

  /// <summary>
  /// Resolves a terminal using the standard resolution order:
  /// TestTerminalContext.Current → provided terminal → fallback.
  /// </summary>
  /// <param name="terminal">The terminal to use if no context is set.</param>
  /// <param name="fallback">The fallback terminal if both context and terminal are null.</param>
  /// <returns>The resolved terminal.</returns>
  public static ITerminal Resolve(ITerminal? terminal, ITerminal fallback)
    => Current ?? terminal ?? fallback;

  /// <summary>
  /// Resolves a terminal using the standard resolution order with TimeWarpTerminal.Default as fallback:
  /// TestTerminalContext.Current → provided terminal → TimeWarpTerminal.Default.
  /// </summary>
  /// <param name="terminal">The terminal to use if no context is set.</param>
  /// <returns>The resolved terminal.</returns>
  public static ITerminal Resolve(ITerminal? terminal)
    => Current ?? terminal ?? TimeWarpTerminal.Default;
}
