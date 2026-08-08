# Static Console-Like API Design for TimeWarp.Terminal

## Executive Summary

This analysis examines options for providing a static API that mimics `System.Console` to enable easy migration from `Console.WriteLinexxx()` to `Terminal.WriteLinexxx()` calls. The current codebase uses an instance-based approach with a singleton `TimeWarpTerminal.Default` property. We recommend implementing a `Terminal` static facade class that routes to a configurable ambient context, enabling both simple migration (`using static TimeWarp.Terminal.Terminal;`) and testability via ambient context override.

---

## Scope

- Analyze current `TimeWarp.Terminal` architecture and API patterns
- Examine `IConsole` and `ITerminal` interfaces and implementations
- Evaluate static API design options for Console compatibility
- Provide implementation recommendations with code examples
- Consider testability, AOT compatibility, and migration scenarios

---

## Methodology

- Explored source files: `iterminal.cs`, `iconsole.cs`, `timewarp-terminal.cs`
- Reviewed extension method patterns in widgets directory
- Analyzed singleton patterns and existing static access patterns
- Referenced .NET 10 Console API additions (ReadOnlySpan support)
- Evaluated test context patterns (`TestTerminalContext`)

---

## Current Architecture Analysis

### Core Interfaces

| Interface | Purpose | Key Methods |
|-----------|---------|-------------|
| `IConsole` | Basic I/O abstraction | `Write`, `WriteLine`, `WriteErrorLine`, `ReadLine`, async variants |
| `ITerminal` | Interactive terminal | Extends IConsole + `ReadKey`, `SetCursorPosition`, `Clear`, properties |

### Current Production Implementation

**`TimeWarpTerminal`** (timewarp-terminal.cs:12-156):
- Sealed class implementing `ITerminal`
- Static singleton property: `public static TimeWarpTerminal Default { get; } = new();`
- Instance methods delegate to `System.Console`
- Graceful error handling for redirected I/O

### Current Usage Pattern

```csharp
// Current approach - requires instance reference
ITerminal terminal = TimeWarpTerminal.Default;
terminal.WriteLine("Hello, World!");
terminal.WriteTable(tableBuilder);

// Widgets via extension methods
terminal.WriteTable(t => t.AddColumns("A", "B").AddRow("1", "2"));
```

### Extension Method Pattern

Widgets use extension methods on `ITerminal`:
- `TerminalTableExtensions.WriteTable()`
- `TerminalPanelExtensions.WritePanel()`
- `TerminalRuleExtensions.WriteRule()`

---

## Design Options for Static API

### Option 1: Simple Static Facade Class

A static class that mirrors `System.Console` signatures, routing to `TimeWarpTerminal.Default`.

**Pros:**
- Simple, familiar pattern
- Direct migration path from `Console.WriteLine()` to `Terminal.WriteLine()`
- No breaking changes to existing API

**Cons:**
- Hard to test - always routes to production implementation
- No dependency injection support
- Global state concerns

### Option 2: Ambient Context with Static Accessors

Extends `TestTerminalContext` with a `Terminal` property that can be overridden for testing.

**Pros:**
- Testable via ambient context
- Still provides simple static access for production code
- Existing pattern already in codebase (`TestTerminalContext`)

**Cons:**
- More complex implementation
- Thread-safety considerations with `AsyncLocal`
- May be surprising to some developers

### Option 3: Hybrid Approach (Recommended)

A `Terminal` static class with:
- Default behavior routing to `TimeWarpTerminal.Default`
- A `Func<ITerminal>` factory that can be replaced for testing
- Same method signatures as `System.Console`

**Pros:**
- Simple for production use
- Testable via factory injection
- Clear separation of concerns
- Matches patterns from other libraries (e.g., `Microsoft.Extensions.Logging`)

**Cons:**
- Slightly more complex than simple facade
- Requires documentation of test pattern

---

## Recommended Implementation

### Design Principles

1. **API Parity with Console**: Match `System.Console` method signatures where applicable
2. **Source Generator for Convenience**: Generate overloads matching Console's format support
3. **Testability**: Allow factory/instance override for testing
4. **AOT Compatibility**: No reflection-heavy patterns
5. **Minimal Breaking Changes**: Existing API remains functional

### Proposed API Surface

```csharp
using static TimeWarp.Terminal.Terminal;

public static partial class Terminal
{
  // === Output Methods ===

  // Basic Write/WriteLine
  public static void Write(string? message);
  public static void WriteLine(string? message = null);
  public static void WriteLine(string format, arg0);
  public static void WriteLine(string format, arg0, arg1);
  public static void WriteLine(string format, arg0, arg1, arg2);
  public static void WriteLine(string format, params object?[] args);

  // Async variants
  public static Task WriteLineAsync(string? message = null);

  // Error output
  public static void WriteErrorLine(string? message = null);
  public static Task WriteErrorLineAsync(string? message = null);

  // Formatted output
  public static void Write(string format, arg0);
  public static void Write(string format, arg0, arg1);
  public static void Write(string format, arg0, arg1, arg2);
  public static void Write(string format, params object?[] args);

  // ANSI-styled output
  public static void WriteLine(string message, ConsoleColor foregroundColor);
  public static void WriteLine(string message, ConsoleColor foregroundColor, ConsoleColor backgroundColor);

  // === Input Methods ===

  public static string? ReadLine();
  public static ConsoleKeyInfo ReadKey(bool intercept = false);

  // === Terminal Properties ===

  public static int WindowWidth { get; }
  public static bool IsInteractive { get; }
  public static bool SupportsColor { get; }
  public static bool SupportsHyperlinks { get; }

  // === Terminal Operations ===

  public static void Clear();
  public static void SetCursorPosition(int left, int top);
  public static (int Left, int Top) GetCursorPosition();

  // === Widgets (Static Access) ===

  public static void WriteTable(Action<TableBuilder> configure);
  public static void WriteTable(Table table);
  public static void WritePanel(Action<PanelBuilder> configure);
  public static void WritePanel(string content, string? header = null);
  public static void WriteRule(string? title = null, LineStyle style = LineStyle.Single);
  public static void WriteLink(string url, string text);

  // === Configuration for Testing ===

  public static ITerminal Instance { get; set; }
}
```

### Implementation Sketch

```csharp
namespace TimeWarp.Terminal;

/// <summary>
/// Static facade providing Console-compatible API for terminal output.
/// Mirrors <see cref="System.Console"/> for easy migration from Console to Terminal.
/// </summary>
/// <example>
/// <code>
/// using static TimeWarp.Terminal.Terminal;
///
/// // Migration from Console.WriteLine
/// WriteLine("Hello, World!");
///
/// // Format strings work like Console
/// WriteLine("User {0} logged in at {1}", userName, time);
///
/// // ANSI colors
/// WriteLine("Error!".Red());
/// WriteLine("Success!".Green());
/// </code>
/// </example>
public static partial class Terminal
{
  private static readonly TimeWarpTerminal DefaultInstance = new();
  private static ITerminal _instance = DefaultInstance;

  /// <summary>
  /// Gets or sets the terminal instance used by static methods.
  /// Set to a <see cref="TestTerminal"/> for unit testing.
  /// </summary>
  public static ITerminal Instance
  {
    get => _instance;
    set => _instance ??= DefaultInstance;
  }

  // === Output Methods ===

  public static void Write(string? message) => Instance.Write(message ?? string.Empty);

  public static void WriteLine(string? message = null) => Instance.WriteLine(message);

  // Generated format overloads via source generator
  public static void WriteLine(string format, object? arg0)
    => Instance.WriteLine(string.Format(CultureInfo.InvariantCulture, format, arg0));

  // ... etc for remaining overloads

  // === Async Variants ===

  public static Task WriteLineAsync(string? message = null) => Instance.WriteLineAsync(message);

  // === Error Output ===

  public static void WriteErrorLine(string? message = null) => Instance.WriteErrorLine(message);

  public static Task WriteErrorLineAsync(string? message = null)
    => Instance.WriteErrorLineAsync(message);

  // === Input Methods ===

  public static string? ReadLine() => Instance.ReadLine();

  public static ConsoleKeyInfo ReadKey(bool intercept = false) => Instance.ReadKey(intercept);

  // === Terminal Properties ===

  public static int WindowWidth => Instance.WindowWidth;

  public static bool IsInteractive => Instance.IsInteractive;

  public static bool SupportsColor => Instance.SupportsColor;

  public static bool SupportsHyperlinks => Instance.SupportsHyperlinks;

  // === Terminal Operations ===

  public static void Clear() => Instance.Clear();

  public static void SetCursorPosition(int left, int top) => Instance.SetCursorPosition(left, top);

  public static (int Left, int Top) GetCursorPosition() => Instance.GetCursorPosition();

  // === Widgets (Static Access) ===

  public static void WriteTable(Action<TableBuilder> configure)
  {
    ArgumentNullException.ThrowIfNull(configure);
    Instance.WriteTable(configure);
  }

  public static void WriteTable(Table table) => Instance.WriteTable(table);

  public static void WritePanel(Action<PanelBuilder> configure)
  {
    ArgumentNullException.ThrowIfNull(configure);
    Instance.WritePanel(configure);
  }

  public static void WritePanel(string content, string? header = null)
    => Instance.WritePanel(p => p.Header(header).Content(content));

  public static void WriteRule(string? title = null, LineStyle style = LineStyle.Single)
    => Instance.WriteRule(r => r.Title(title).Style(style));

  public static void WriteLink(string url, string text)
    => Instance.WriteLink(url, text);
}
```

### Source Generator for Format Overloads

A source generator can generate the format method overloads to avoid boilerplate:

```csharp
// Generated source would produce:
public static void WriteLine(string format, object? arg0)
  => Instance.WriteLine(string.Format(CultureInfo.InvariantCulture, format, arg0));

public static void WriteLine(string format, object? arg0, object? arg1)
  => Instance.WriteLine(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1));

public static void WriteLine(string format, object? arg0, object? arg1, object? arg2)
  => Instance.WriteLine(string.Format(CultureInfo.InvariantCulture, format, arg0, arg1, arg2));

public static void WriteLine(string format, params object?[] args)
  => Instance.WriteLine(string.Format(CultureInfo.InvariantCulture, format, args));
```

### Testability Pattern

```csharp
// In test setup
[SetUp]
public void Setup()
{
  var testTerminal = new TestTerminal();
  Terminal.Instance = testTerminal;
}

[Test]
public void WriteLine_OutputsMessage()
{
  Terminal.WriteLine("Hello");
  Assert.That(Terminal.Instance.Output, Does.Contain("Hello"));
}
```

---

## Migration Scenarios

### Scenario 1: Simple Search and Replace

**Before:**
```csharp
Console.WriteLine("Hello, World!");
Console.WriteLine("User {0} logged in", userName);
```

**After:**
```csharp
Terminal.WriteLine("Hello, World!");
Terminal.WriteLine("User {0} logged in", userName);
```

### Scenario 2: Static Import

**Before:**
```csharp
using static System.Console;

WriteLine("Hello, World!");
```

**After:**
```csharp
using static TimeWarp.Terminal.Terminal;

WriteLine("Hello, World!");
```

### Scenario 3: Gradual Migration with Compatibility

```csharp
// Old code still works
Console.WriteLine("Legacy");

// New code uses Terminal
Terminal.WriteLine("New feature");
```

---

## ANSI Color Integration

The static API should integrate seamlessly with the existing ANSI extension methods:

```csharp
using static TimeWarp.Terminal.Terminal;

// Combined with C# 14 extension blocks
WriteLine("Error!".Red().Bold());
WriteLine("Success!".Green());
WriteLine("Warning!".Yellow().OnRed());

// Or explicit color parameters
WriteLine("Error!", ConsoleColor.Red);
WriteLine("Info", ConsoleColor.Cyan, ConsoleColor.DarkGray);
```

---

## Breaking Changes Assessment

| Change | Impact | Mitigation |
|--------|--------|------------|
| Add new `Terminal` static class | None | New type, doesn't affect existing code |
| `Instance` property | None | New property, doesn't affect existing code |
| Existing `TimeWarpTerminal.Default` | None | Remains unchanged |
| Existing interface API | None | Remains unchanged |

**No breaking changes** - this is purely additive.

---

## Recommendations

### Priority 1: Core Static Facade

1. Create `Terminal` static class with Console-compatible methods
2. Implement `Instance` property with `TimeWarpTerminal.Default` as default
3. Add basic Write/WriteLine/WriteErrorLine methods
4. Add async variants

### Priority 2: Format Overloads

1. Implement source generator for format method overloads
2. Generate Write/WriteLine overloads matching Console signatures
3. Ensure `IFormattable` and `CultureInfo.InvariantCulture` usage

### Priority 3: Widget Integration

1. Add static methods for Table, Panel, Rule widgets
2. Match extension method signatures for consistency
3. Document widget usage patterns

### Priority 4: Enhanced Color Support

1. Add color parameter overloads
2. Integrate with existing ANSI extension methods
3. Consider `ConsoleColor` parameter support

### Priority 5: Testing Documentation

1. Document `Terminal.Instance` override pattern
2. Provide example test setups
3. Update `TestTerminalContext` to integrate with static API

---

## References

- `IConsole` interface: source/timewarp-terminal/iconsole.cs
- `ITerminal` interface: source/timewarp-terminal/iterminal.cs
- `TimeWarpTerminal` implementation: source/timewarp-terminal/timewarp-terminal.cs
- Extension method pattern: source/timewarp-terminal/widgets/terminal-table-extensions.cs
- .NET 10 Console API: https://learn.microsoft.com/en-us/dotnet/api/system.console
- Test context pattern: `TestTerminalContext`
