namespace TimeWarp.Terminal;

/// <summary>
/// Provides an ambient context for <see cref="TestTerminal"/> that enables zero-configuration testing
/// of CLI applications. While a context is active, <see cref="TimeWarp.Terminal.Terminal.Instance"/>
/// resolves to it without the process-global instance ever being mutated.
/// </summary>
/// <remarks>
/// <para>
/// This class uses <see cref="AsyncLocal{T}"/> to provide a test terminal that flows with the
/// async execution context, and <c>Terminal.Instance</c>'s getter consults it first. Because
/// <see cref="SetCurrent"/> and <see cref="Use"/> never touch the process-global instance,
/// each test gets its own isolated terminal even when running tests in parallel.
/// </para>
/// <para>
/// Use <see cref="SetCurrent"/> and <see cref="ClearCurrent"/> for explicit lifecycle control,
/// or <see cref="Use"/> for a scoped pattern that restores automatically.
/// </para>
/// <para>
/// Resolution order for <c>Terminal.Instance</c>:
/// <list type="number">
///   <item><description><see cref="Current"/> (if set for the current async flow)</description></item>
///   <item><description>the process-global instance (assignable directly; defaults to
///   <see cref="TimeWarpTerminal.Default"/>)</description></item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// Scoped test pattern:
/// <code>
/// public static async Task Should_display_greeting()
/// {
///     using TestTerminal terminal = new();
///     using IDisposable scope = TestTerminalContext.Use(terminal);
///
///     // TimeWarp.Terminal.Terminal.Instance now resolves to terminal in this async flow
///     Terminal.WriteLine("Hello");  // Routes to test terminal
///
///     await Program.Main(["greet", "World"]);
///
///     terminal.OutputContains("Hello, World!").ShouldBeTrue();
/// }
/// </code>
/// </example>
public static class TestTerminalContext
{
  private static readonly AsyncLocal<TestTerminal?> Context = new();
  private static readonly AsyncLocal<Stack<ContextSnapshot>?> SnapshotStack = new();

  private sealed class ContextSnapshot
  {
    public required TestTerminal? PreviousContext { get; init; }
    public required IFormatProvider? PreviousFormatProvider { get; init; }
  }

  /// <summary>
  /// Gets the current <see cref="TestTerminal"/> for the async execution context.
  /// </summary>
  /// <value>
  /// The <see cref="TestTerminal"/> for the current context, or <c>null</c> if not set.
  /// </value>
  public static TestTerminal? Current => Context.Value;

  /// <summary>
  /// Gets a value indicating whether a <see cref="TestTerminal"/> is set for the current context.
  /// </summary>
  public static bool HasValue => Context.Value is not null;

  /// <summary>
  /// Sets the current <see cref="TestTerminal"/> for the async execution context.
  /// <see cref="TimeWarp.Terminal.Terminal.Instance"/> resolves to it while the context is
  /// active; the process-global instance is never mutated.
  /// </summary>
  /// <param name="terminal">The terminal to set as current.</param>
  public static void SetCurrent(TestTerminal terminal)
  {
    ArgumentNullException.ThrowIfNull(terminal);

    Stack<ContextSnapshot> stack = GetSnapshotStack();
    stack.Push
    (
      new ContextSnapshot
      {
        PreviousContext = Context.Value,
        PreviousFormatProvider = TimeWarp.Terminal.Terminal.FormatProvider
      }
    );

    Context.Value = terminal;
  }

  /// <summary>
  /// Clears the current context, restoring the previous context and
  /// <see cref="TimeWarp.Terminal.Terminal.FormatProvider"/>. The process-global
  /// <see cref="TimeWarp.Terminal.Terminal.Instance"/> is never touched — once the context
  /// is cleared, resolution simply falls back to it.
  /// </summary>
  public static void ClearCurrent()
  {
    Stack<ContextSnapshot>? stack = SnapshotStack.Value;
    if (stack is null || stack.Count == 0)
    {
      Context.Value = null;
      return;
    }

    ContextSnapshot snapshot = stack.Pop();
    Context.Value = snapshot.PreviousContext;
    TimeWarp.Terminal.Terminal.FormatProvider = snapshot.PreviousFormatProvider;

    if (stack.Count == 0)
    {
      SnapshotStack.Value = null;
    }
  }

  /// <summary>
  /// Creates a scoped test terminal context that is restored on dispose.
  /// </summary>
  /// <param name="terminal">The terminal to set for the scope.</param>
  /// <returns>An <see cref="IDisposable"/> scope that restores the previous context.</returns>
  public static IDisposable Use(TestTerminal terminal)
  {
    SetCurrent(terminal);
    return new Scope();
  }

  /// <summary>
  /// Gets the <see cref="TestTerminal"/> for the current context, or throws if not set.
  /// </summary>
  /// <returns>The current <see cref="TestTerminal"/>.</returns>
  /// <exception cref="InvalidOperationException">Thrown when no test terminal is set.</exception>
  public static TestTerminal Terminal
    => Context.Value ?? throw new InvalidOperationException("No TestTerminal set in current context. Call TestTerminalContext.SetCurrent or TestTerminalContext.Use first.");

  private static Stack<ContextSnapshot> GetSnapshotStack()
  {
    Stack<ContextSnapshot>? stack = SnapshotStack.Value;
    if (stack is null)
    {
      stack = new Stack<ContextSnapshot>();
      SnapshotStack.Value = stack;
    }

    return stack;
  }

  private sealed class Scope : IDisposable
  {
    private bool Disposed;

    public void Dispose()
    {
      if (Disposed)
      {
        return;
      }

      ClearCurrent();
      Disposed = true;
    }
  }

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
