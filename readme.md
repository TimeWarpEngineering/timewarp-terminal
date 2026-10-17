# TimeWarp.Terminal

The testable output layer for [Nuru](https://github.com/TimeWarpEngineering/timewarp-nuru) CLIs.

TimeWarp.Terminal writes the text, widgets, and layouts a command prints. It does not run an event loop, and it does not depend on a reactive framework. Tests capture that output with `TestTerminal`, `TestConsole`, and an async-scoped `TestTerminalContext`. Both packages are marked `IsAotCompatible` and run the trim and AOT analyzers.

A Nuru handler takes `ITerminal`, which also implements `IConsole`. Call `UseTerminal` with a `TestTerminal` in a test. When you do not call it, Nuru uses `TimeWarpTerminal.Default`.

## What you get

- **Console-compatible `IConsole` and `ITerminal`.** The static `Terminal` class covers the usual `Console` calls. `TimeWarpTerminal` is the production terminal. `TimeWarpConsole` is the production console.
- **Parallel-safe test capture.** Give each test its own `TestTerminal` or `TestConsole`. `TestTerminalContext.Use` stores that instance in an async-local scope, so parallel tests do not share or mutate the process-global terminal. One `TestTerminal` serves a single test flow. Its members are not thread-safe.
- **Static widgets.** Panels, tables, and rules. OSC 8 hyperlinks. Unicode display width for emoji and wide characters. Widgets render once. They do not redraw in place.
- **Optional flexbox layout.** [`TimeWarp.Terminal.Layout`](https://www.nuget.org/packages/TimeWarp.Terminal.Layout) composes those widgets into rows and columns. It is a separate package, and it is not a dependency of `TimeWarp.Terminal`. Layout output is one-shot scrolling text, not an interactive TUI.
- **A small dependency set.** The only library dependency of `TimeWarp.Terminal` is [`TimeWarp.Builder`](https://www.nuget.org/packages/TimeWarp.Builder).

## When to use something else

TimeWarp.Terminal is the output you assert on after a command finishes. It does not redraw a live region, and it does not host windows, focus, or a widget event loop.

- **[Spectre.Console](https://github.com/spectreconsole/spectre.console)** draws live displays, progress, and status, and it has its own widgets. Use it when a command should update the same lines while work is still running.
- **[Termina](https://github.com/Aaronontheweb/termina)** is a reactive terminal UI with input and routing. It can take the full screen, or keep one inline live region in the primary buffer. Use it when the program is an interactive view.
- **[Terminal.Gui](https://github.com/gui-cs/Terminal.Gui)** is a full-screen terminal GUI with windows and controls. Use it when the program is a TUI application rather than a command that prints a result.

A Nuru command handler can call one of these libraries directly. TimeWarp.Terminal stays the default for commands whose output tests should capture.

## Installation

```bash
dotnet add package TimeWarp.Terminal
```

## Quick Start

```csharp
using TimeWarp.Terminal;

// Use the static Terminal class (Console-compatible API)
Terminal.WriteLine("Hello, World!".Green());
Terminal.WriteLine("Warning!".Yellow().Bold());

// Or get a terminal instance — all Write methods return ITerminal for fluent chaining
ITerminal terminal = TimeWarpTerminal.Default;
terminal
  .WritePanel("Important message", "Notice")
  .WriteRule("Section")
  .WriteTable(t => t
    .AddColumn("Name")
    .AddColumn("Value")
    .AddRow("Status", "OK".Green()))
  .WriteLine("Done!");
```

## Testing a Nuru command

`NuruAppBuilder.UseTerminal` supplies the `ITerminal` that handlers receive. Pass a `TestTerminal` and assert on what it captured:

```csharp
using TimeWarp.Nuru;
using TimeWarp.Terminal;

using TestTerminal terminal = new();

NuruApp app = NuruApp.CreateBuilder()
  .UseTerminal(terminal)
  .Map("greet {name}")
    .WithHandler((string name, ITerminal output) =>
    {
      output.WriteLine($"Hello, {name}");
    })
    .AsCommand()
    .Done()
  .Build();

await app.RunAsync(["greet", "World"]);

terminal.OutputContains("Hello, World").ShouldBeTrue();
```

`OutputContains` is on `TestTerminal` and `TestConsole`. `ShouldBeTrue()` is [Shouldly](https://github.com/shouldly/shouldly). `Assert.Contains("Hello, World", terminal.Output)` checks the same string.

Code that calls the static `Terminal` class, instead of an injected `ITerminal`, uses `TestTerminalContext` in the next section. `TestConsole` is the same idea when the code only needs `IConsole`.

## Static Terminal API

The `Terminal` static class provides a Console-compatible API for easy migration:

```csharp
using static TimeWarp.Terminal.Terminal;

// Direct replacement for Console methods
WriteLine("Hello, World!");
WriteErrorLine("Error occurred!");
string? input = ReadLine();
Clear();

// Properties
int width = WindowWidth;
bool interactive = IsInteractive;
bool colorSupport = SupportsColor;

// Cursor operations
SetCursorPosition(10, 5);
var (left, top) = GetCursorPosition();
```

### Testing with Static Terminal

Use `TestTerminalContext` to redirect the static `Terminal` class in tests:

```csharp
using TestTerminal terminal = new();
using IDisposable scope = TestTerminalContext.Use(terminal);

Terminal.WriteLine("Hello");
terminal.OutputContains("Hello").ShouldBeTrue();
```

The scope is async-context isolated - each test flow sees only its own `TestTerminal`, so parallel tests never interfere with each other, and the global terminal is never mutated. Disposing the scope restores the previous context automatically.

For serial tests you can also swap `Terminal.Instance` directly, but remember to restore it:

```csharp
using TestTerminal testTerminal = new();
Terminal.Instance = testTerminal;

Terminal.WriteLine("test output");
Assert.Contains("test output", testTerminal.Output);

// Restore after test
Terminal.Instance = TimeWarpTerminal.Default;
```

## Interfaces

The sketches below are the members the examples use. The full contracts also include streams, encodings, redirection flags, cursor and window geometry, and cancel-key notification.

### IConsole

Basic console I/O abstraction for testable console applications.

```csharp
public interface IConsole
{
    IConsole Write(string message);
    IConsole WriteLine(string? message = null);
    Task WriteLineAsync(string? message = null);
    IConsole WriteErrorLine(string? message = null);
    Task WriteErrorLineAsync(string? message = null);
    string? ReadLine();
}
```

### ITerminal

Extended terminal interface with cursor control, colors, and hyperlinks.

```csharp
public interface ITerminal : IConsole
{
    new ITerminal Write(string message);
    new ITerminal WriteLine(string? message = null);
    new ITerminal WriteErrorLine(string? message = null);
    ConsoleKeyInfo ReadKey(bool intercept);
    void SetCursorPosition(int left, int top);
    (int Left, int Top) GetCursorPosition();
    int WindowWidth { get; }
    bool IsInteractive { get; }
    bool SupportsColor { get; }
    bool SupportsHyperlinks { get; }
    void Clear();
}
```

## Implementations

| Class | Description |
|-------|-------------|
| `TimeWarpTerminal` | Production `ITerminal` with full terminal capabilities |
| `TimeWarpConsole` | Production `IConsole` wrapping `System.Console` |
| `TestTerminal` | Test implementation with captured output and scripted input |
| `TestConsole` | Simpler test implementation for basic I/O testing |

### Testing Example

```csharp
using TestTerminal terminal = new();

// Queue input for ReadLine
terminal = new TestTerminal("line1\nline2");

// Queue keys for ReadKey
terminal.QueueKey(ConsoleKey.Enter);
terminal.QueueKeys("hello");
terminal.QueueLine("complete line");

// Run code that uses ITerminal
myCommand.Execute(terminal);

// Verify output
Assert.Contains("expected text", terminal.Output);
Assert.Contains("error message", terminal.ErrorOutput);
```

## Widgets

### Panel

Bordered panel with optional header and content.

```csharp
// Simple panel
terminal.WritePanel("This is important information");

// Panel with header
terminal.WritePanel("Content here", "Notice");

// Fluent builder with full options
terminal.WritePanel
( 
  panel => 
    panel
    .Header("Configuration".Cyan().Bold())
    .Content("Setting: value")
    .Border(BorderStyle.Rounded)
    .BorderColor(AnsiColors.Cyan)
    .Padding(2, 1)
    .Width(60)
    .WordWrap(true)
);
```

**Border Styles:** `Rounded`, `Square`, `Doubled`, `Heavy`, `None`

### Table

Formatted table with columns, alignment, and styling.

```csharp
// Simple table
terminal.WriteTable
( 
  t => t
    .AddColumn("Name")
    .AddColumn("Value", Alignment.Right)
    .AddRow("CPU", "45%")
    .AddRow("Memory", "2.1 GB")
);

// Full-featured table
terminal.WriteTable(t => t
    .AddColumn("Package")
    .AddColumn("Downloads", Alignment.Right)
    .AddColumn(new TableColumn("Path") { TruncateMode = TruncateMode.Start })
    .AddRow("GuardClauses", "12M", "/home/user/packages/guard")
    .Border(BorderStyle.Rounded)
    .BorderColor(AnsiColors.Cyan)
    .Expand());  // tables shrink to fit the terminal automatically

terminal.WriteTable(table);
```

**Alignment:** `Left` (default), `Right`, `Center`

**TruncateMode:** `End` (default), `Start`, `Middle`

### Rule

Horizontal rule with optional centered title.

```csharp
// Simple rule
terminal.WriteRule();

// Rule with title
terminal.WriteRule("Section Title");

// Styled rule
terminal.WriteRule("Results".Cyan().Bold());

// Fluent builder
terminal.WriteRule(rule => rule
    .Title("Configuration")
    .Style(LineStyle.Doubled)
    .Color(AnsiColors.Cyan));
```

**Line Styles:** `Thin`, `Doubled`, `Heavy`

## Layout (companion package)

Flexbox-composed layouts for side-by-side panels, status-bar rows, and dashboards. This is a **companion package** — it is not pulled in by `TimeWarp.Terminal`.

```bash
dotnet add package TimeWarp.Terminal.Layout
```

```csharp
using TimeWarp.Terminal;
using TimeWarp.Flexbox;

terminal.WriteLayout(layout => layout
  .Direction(FlexDirection.Row)
  .Gap(2)
  .Item(i => i.Grow(1), panel => panel.Header("Build").Content(buildSummary))
  .Item(i => i.Grow(2), table => table.AddColumns("Test", "Result").AddRow("unit", "pass")));

// Column of rows. statusPanel, versionPanel, and logTable are widgets built earlier.
terminal.WriteLayout(layout => layout
  .Direction(FlexDirection.Column)
  .Row(r => r.Item(statusPanel).Item(versionPanel))
  .Row(r => r.Item(i => i.Grow(1), logTable)));
```

## ANSI Colors

Extension methods for colored and styled console output.

```csharp
// Foreground colors
terminal.WriteLine("Success!".Green());
terminal.WriteLine("Warning!".Yellow());
terminal.WriteLine("Error!".Red());

// Chained styles
terminal.WriteLine("Important".Red().Bold().Underline());

// Background colors
terminal.WriteLine("Highlighted".OnYellow());
terminal.WriteLine("Inverted".Black().OnWhite());
```

### Available Colors

**Standard:** `Black`, `Red`, `Green`, `Yellow`, `Blue`, `Magenta`, `Cyan`, `White`, `Gray`

**Bright:** `BrightRed`, `BrightGreen`, `BrightYellow`, `BrightBlue`, `BrightMagenta`, `BrightCyan`, `BrightWhite`

### Styles

`Bold()`, `Dim()`, `Italic()`, `Underline()`, `Strikethrough()`

### Background Colors

`OnBlack()`, `OnRed()`, `OnGreen()`, `OnYellow()`, `OnBlue()`, `OnMagenta()`, `OnCyan()`, `OnWhite()`

## ConsoleColor Support

Use `ConsoleColor` enum values for color output without needing ANSI escape codes directly. This provides a Console-compatible API for colored output.

```csharp
// Single foreground color
Terminal.WriteLine("Error!", ConsoleColor.Red);
Terminal.WriteLine("Success!", ConsoleColor.Green);
Terminal.WriteLine("Warning!", ConsoleColor.Yellow);

// Foreground and background colors
Terminal.WriteLine("Highlighted", ConsoleColor.Black, ConsoleColor.Yellow);
Terminal.WriteLine("Inverted", ConsoleColor.White, ConsoleColor.Black);

// Error output with color
Terminal.WriteErrorLine("Error: File not found", ConsoleColor.Red);

// Write without newline
Terminal.Write("Loading...", ConsoleColor.Cyan);

// Widgets with colors
Terminal.WriteTable(table => table
    .AddColumn("Name")
    .AddColumn("Value")
    .AddRow("Status", "OK"),
  ConsoleColor.White, ConsoleColor.DarkBlue);

Terminal.WritePanel("Important content", "Notice",
  ConsoleColor.White, ConsoleColor.DarkBlue);
```

### Supported Colors

All `ConsoleColor` values are mapped to their ANSI equivalents:

**Foreground:** `Black`, `Red`, `Green`, `Yellow`, `Blue`, `Magenta`, `Cyan`, `White`, `Gray`, `DarkGray`, `DarkRed`, `DarkGreen`, `DarkYellow`, `DarkBlue`, `DarkMagenta`, `DarkCyan`

**Background:** Maps to corresponding ANSI background codes (`BgBlack`, `BgRed`, etc.)

## Hyperlinks

OSC 8 hyperlinks for supported terminals (Windows Terminal, iTerm2, VS Code, etc.).

```csharp
// Write a clickable link
terminal.WriteLink("https://github.com", "GitHub");
terminal.WriteLinkLine("https://example.com", "Click here");

// String extension method
string link = "Click here".Link("https://example.com");
terminal.WriteLine(link);

// Styled hyperlink
terminal.WriteLine("Visit us".Link("https://example.com").Cyan().Underline());

// Check terminal support
if (terminal.SupportsHyperlinks)
    terminal.WriteLinkLine("https://docs.com", "View docs");
else
    terminal.WriteLine("View docs at https://docs.com");
```

## AnsiStringUtils

Utilities for working with ANSI-styled strings.

```csharp
// Get visible length (excludes ANSI codes)
int length = AnsiStringUtils.GetVisibleLength("Hello".Red()); // 5

// Strip all ANSI codes
string plain = AnsiStringUtils.StripAnsiCodes("\x1b[31mError\x1b[0m"); // "Error"

// Pad accounting for ANSI codes
string padded = AnsiStringUtils.PadRightVisible("Hi".Red(), 10);
string centered = AnsiStringUtils.CenterVisible("Title".Bold(), 40);

// Wrap text preserving ANSI codes
string[] lines = AnsiStringUtils.WrapText(longStyledText, maxWidth: 80);
```
