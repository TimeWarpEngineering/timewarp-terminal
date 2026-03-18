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
/// Use <see cref="SetCurrent"/> and <see cref="ClearCurrent"/> for explicit lifecycle control,
/// or <see cref="Use"/> for a scoped pattern that restores automatically.
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
/// Scoped test pattern with automatic TimeWarp.Terminal.Terminal.Instance synchronization:
/// <code>
/// public static async Task Should_display_greeting()
/// {
///     using TestTerminal terminal = new();
///     using IDisposable scope = TestTerminalContext.Use(terminal);
///     
///     // TimeWarp.Terminal.Terminal.Instance is now set to terminal
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
    public required ITerminal PreviousInstance { get; init; }
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
  /// Sets the current <see cref="TestTerminal"/> and synchronizes <see cref="TimeWarp.Terminal.Terminal.Instance"/>.
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
        PreviousInstance = TimeWarp.Terminal.Terminal.Instance
      }
    );

    Context.Value = terminal;
    TimeWarp.Terminal.Terminal.Instance = terminal;
  }

  /// <summary>
  /// Clears the current context and restores the previous <see cref="TimeWarp.Terminal.Terminal.Instance"/>.
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
    TimeWarp.Terminal.Terminal.Instance = snapshot.PreviousInstance;

    if (stack.Count == 0)
      SnapshotStack.Value = null;
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
        return;

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
