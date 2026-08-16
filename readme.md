# TimeWarp.Terminal

Terminal abstractions and widgets for console applications - IConsole, ITerminal, panels, tables, rules, and ANSI color support.

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
