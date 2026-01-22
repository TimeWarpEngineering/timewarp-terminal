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

// Or get a terminal instance
ITerminal terminal = TimeWarpTerminal.Default;
terminal.WritePanel("Important message", "Notice");
terminal.WriteRule("Section");
terminal.WriteTable(t => t
    .AddColumn("Name")
    .AddColumn("Value")
    .AddRow("Status", "OK".Green()));
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

```csharp
// Replace Instance for testing
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
    void Write(string message);
    void WriteLine(string? message = null);
    Task WriteLineAsync(string? message = null);
    void WriteErrorLine(string? message = null);
    Task WriteErrorLineAsync(string? message = null);
    string? ReadLine();
}
```

### ITerminal

Extended terminal interface with cursor control, colors, and hyperlinks.

```csharp
public interface ITerminal : IConsole
{
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
| `NuruConsole` | Production `IConsole` wrapping `System.Console` |
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
terminal.WritePanel(panel => panel
    .Header("Configuration".Cyan().Bold())
    .Content("Setting: value")
    .Border(BorderStyle.Rounded)
    .BorderColor(AnsiColors.Cyan)
    .Padding(2, 1)
    .Width(60)
    .WordWrap(true));
```

**Border Styles:** `Rounded`, `Square`, `Doubled`, `Heavy`, `None`

### Table

Formatted table with columns, alignment, and styling.

```csharp
// Simple table
terminal.WriteTable(t => t
    .AddColumn("Name")
    .AddColumn("Value", Alignment.Right)
    .AddRow("CPU", "45%")
    .AddRow("Memory", "2.1 GB"));

// Full-featured table
Table table = new Table()
    .AddColumn("Package")
    .AddColumn("Downloads", Alignment.Right)
    .AddColumn(new TableColumn("Path") { TruncateMode = TruncateMode.Start })
    .AddRow("GuardClauses", "12M", "/home/user/packages/guard");

table.Border = BorderStyle.Rounded;
table.BorderColor = AnsiColors.Cyan;
table.Expand = true;  // Fill terminal width
table.Shrink = true;  // Shrink to fit (default)
table.ShowHeaders = true;
table.ShowRowSeparators = false;

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
