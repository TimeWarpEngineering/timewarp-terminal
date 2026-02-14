---
name: terminal
description: TimeWarp.Terminal library - console abstractions (IConsole, ITerminal), widgets (panels, tables, rules), ANSI colors, hyperlinks, and Unicode width handling for testable C# console apps
---

# TimeWarp.Terminal

Console abstractions and widgets for testable C# applications.

**Repository:** https://github.com/TimeWarpEngineering/timewarp-terminal
**Package:** `TimeWarp.Terminal`

For detailed API documentation, fetch the README from the repository.

## When to Use What

| Need | Use |
|------|-----|
| Basic testable I/O | `IConsole` |
| Rich output (colors, widgets, hyperlinks) | `ITerminal` |
| Quick utility script, easy Console migration | `Terminal` static class |
| Unit testing | `TestTerminal` or `TestConsole` |
| Measure terminal display width | `UnicodeWidth` |
| Strip/measure ANSI strings | `AnsiStringUtils` |

## Installation

```bash
dotnet add package TimeWarp.Terminal
```

## Core Pattern: Dependency Injection

```csharp
// Production
services.AddSingleton<ITerminal>(TimeWarpTerminal.Default);

// Testing
services.AddSingleton<ITerminal>(new TestTerminal());
```

## Quick Examples

### Basic Output

```csharp
using TimeWarp.Terminal;

// Static API (Console replacement)
Terminal.WriteLine("Hello".Green());
Terminal.WriteLine("Warning".Yellow().Bold());

// Instance API (testable) — all Write methods return ITerminal for fluent chaining
ITerminal terminal = TimeWarpTerminal.Default;
terminal
  .WriteLine("Hello".Cyan())
  .WriteRule("Section")
  .WriteLine("World".Green());
```

### Widgets

All widgets use the builder pattern. Constructors for `Table`, `Panel`, and `Rule` are internal — always use the builder or `Action<XxxBuilder>` overloads.

```csharp
// Panel — simple
terminal.WritePanel("Content here");

// Panel — with header parameter
terminal.WritePanel("Content here", "Title");

// Panel with builder
terminal.WritePanel(panel => panel
  .Header("Configuration")
  .Content("Setting: value")
  .Border(BorderStyle.Rounded)
  .Padding(2, 1));

// Table
terminal.WriteTable(table => table
  .AddColumn("Name")
  .AddColumn("Status", Alignment.Right)
  .AddRow("API", "Online".Green())
  .AddRow("DB", "Offline".Red())
  .Border(BorderStyle.Rounded));

// Standalone builder (when you need the Table object)
Table table = new TableBuilder()
  .AddColumn("Name")
  .AddRow("foo")
  .Border(BorderStyle.Rounded)
  .Build();
terminal.WriteTable(table);

// Rule (section divider)
terminal.WriteRule("Section Title".Cyan());

// Rule with style
terminal.WriteRule("Configuration", style: LineStyle.Doubled);

// Rule with builder
terminal.WriteRule(rule => rule
  .Title("Configuration")
  .Style(LineStyle.Doubled)
  .Color(AnsiColors.Cyan));
```

### Hyperlinks

```csharp
// String extension — creates OSC 8 hyperlink
string link = "Click here".Link("https://example.com");

// With styling
string styledLink = "Docs".Link("https://docs.example.com").Blue().Underline();

// Terminal extension methods (gracefully degrade if unsupported)
terminal.WriteLink("https://example.com", "Click here");    // no newline
terminal.WriteLinkLine("https://example.com", "Visit site"); // with newline

// Static API
Terminal.WriteLink("https://example.com", "Click here");

// Check support
if (terminal.SupportsHyperlinks)
  terminal.WriteLinkLine("https://example.com", "Link");
else
  terminal.WriteLine("https://example.com");

// Low-level
string osc8 = AnsiHyperlinks.CreateLink("text", "https://example.com");
```

### Colors and Styles

```csharp
// Colors: Black, Red, Green, Yellow, Blue, Magenta, Cyan, White, Gray
// Bright variants: BrightRed, BrightGreen, etc.
// Styles: Bold(), Dim(), Italic(), Underline(), Strikethrough()
// Background: OnRed(), OnGreen(), OnYellow(), etc.

terminal.WriteLine("Success".Green().Bold());
terminal.WriteLine("Error".Red().OnWhite());

// ConsoleColor overloads on static API and WritePanel
Terminal.WriteLine("colored", ConsoleColor.Green);
Terminal.WriteLine("colored", ConsoleColor.Red, ConsoleColor.White);
```

### Unicode Width

Calculates terminal display width accounting for emoji, CJK, fullwidth forms, and zero-width characters. Uses exact Emoji_Presentation=Yes code points from Unicode 16.0.

```csharp
UnicodeWidth.GetTextWidth("Hello");      // 5 (ASCII)
UnicodeWidth.GetTextWidth("📍");          // 2 (emoji)
UnicodeWidth.GetTextWidth("漢字");        // 4 (CJK)
UnicodeWidth.GetTextWidth("📍 Location"); // 11 (mixed)
UnicodeWidth.GetTextWidth("🇺🇸");         // 2 (flag - multi-codepoint cluster)
UnicodeWidth.GetTextWidth("☁️");          // 2 (text char + VS16)

UnicodeWidth.GetRuneWidth(new Rune('A'));    // 1
UnicodeWidth.GetRuneWidth(new Rune('漢'));   // 2
UnicodeWidth.GetRuneWidth(new Rune(0x200D)); // 0 (ZWJ)
```

### Testing

```csharp
// Create test terminal with scripted input
using TestTerminal terminal = new("yes\n");

// Run code under test
MyCommand command = new(terminal);
command.Execute();

// Verify output
Assert.Contains("expected text", terminal.Output);
Assert.Contains("error message", terminal.ErrorOutput);
```

### Static Terminal Testing

```csharp
using TestTerminal testTerminal = new();
Terminal.Instance = testTerminal;
try
{
  Terminal.WriteLine("test");
  Assert.Contains("test", testTerminal.Output);
}
finally
{
  Terminal.Instance = TimeWarpTerminal.Default;
}
```

## Key Interfaces

**IConsole:** `Write` -> `IConsole`, `WriteLine` -> `IConsole`, `WriteLineAsync` -> `Task`, `WriteErrorLine` -> `IConsole`, `WriteErrorLineAsync` -> `Task`, `ReadLine`

**ITerminal extends IConsole:** Overrides `Write` -> `ITerminal`, `WriteLine` -> `ITerminal`, `WriteErrorLine` -> `ITerminal`. Adds `ReadKey`, `SetCursorPosition`, `GetCursorPosition`, `WindowWidth`, `IsInteractive`, `SupportsColor`, `SupportsHyperlinks`, `Clear`

All Write methods return the interface for fluent chaining:
```csharp
terminal
  .WriteLine("Build Output")
  .WriteRule("Results")
  .WriteTable(t => t
    .AddColumn("Test")
    .AddColumn("Status")
    .AddRow("Unit", "PASSED".Green()))
  .WriteRule()
  .WriteLine("Done!");
```

## Widget Builder API

Widget constructors are internal. Always use builders.

**PanelBuilder:** `.Header()`, `.Content(string?)`, `.Border()`, `.BorderColor()`, `.Padding()`, `.PaddingHorizontal()`, `.PaddingVertical()`, `.Width()`, `.WordWrap()`, `.Build()`

**TableBuilder:** `.AddColumn(name)`, `.AddColumn(name, alignment)`, `.AddColumn(TableColumn)`, `.AddColumns(params)`, `.AddRow(params)`, `.Border()`, `.BorderColor()`, `.HideHeaders()`, `.ShowRowSeparators()`, `.Expand()`, `.Shrink()`, `.Build()`

**RuleBuilder:** `.Title()`, `.Style()`, `.Color()`, `.Width()`, `.Build()`

### WritePanel Overloads

```csharp
// Simple content
terminal.WritePanel("Content");
terminal.WritePanel("Content", BorderStyle.Rounded);

// With header parameter
terminal.WritePanel("Content", "Header");
terminal.WritePanel("Content", "Header", BorderStyle.Doubled);

// With colors
terminal.WritePanel("Content", "Header", BorderStyle.Rounded, ConsoleColor.Green, ConsoleColor.Black);

// Builder pattern
terminal.WritePanel(p => p.Header("Title").Content("Body").Border(BorderStyle.Rounded));

// Pre-built panel
terminal.WritePanel(panel);
```

### TableColumn Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Header` | `string` | `""` | Column header text (supports ANSI colors) |
| `Alignment` | `Alignment` | `Left` | Horizontal alignment (`Left`, `Right`, `Center`) |
| `MinWidth` | `int?` | `4` | Column won't shrink below this width |
| `MaxWidth` | `int?` | `null` | Content exceeding this is truncated with ellipsis |
| `TruncateMode` | `TruncateMode` | `End` | Where to place ellipsis when truncating |
| `HeaderColor` | `string?` | `null` | ANSI color code for header text |

### TruncateMode (Ellipsis Placement)

| Mode | Result | Use case |
|------|--------|----------|
| `TruncateMode.End` | `"long text..."` | Default — shows beginning |
| `TruncateMode.Start` | `"...long text"` | File paths — shows the meaningful end |
| `TruncateMode.Middle` | `"long...text"` | Shows both start and end |

#### Example: All three modes side-by-side

```csharp
string longText = "This-is-a-very-long-text-that-will-be-truncated-differently";

terminal.WriteTable(t => t
  .AddColumn(new TableColumn("Mode") { MaxWidth = 8 })
  .AddColumn(new TableColumn("End (default)") { MaxWidth = 25, TruncateMode = TruncateMode.End })
  .AddColumn(new TableColumn("Start") { MaxWidth = 25, TruncateMode = TruncateMode.Start })
  .AddColumn(new TableColumn("Middle") { MaxWidth = 25, TruncateMode = TruncateMode.Middle })
  .AddRow("Result", longText, longText, longText)
  .Border(BorderStyle.Rounded));
```

#### Example: File paths with TruncateMode.Start

```csharp
terminal.WriteTable(t => t
  .AddColumn("Repository")
  .AddColumn(new TableColumn("Worktree Path") { TruncateMode = TruncateMode.Start })
  .AddColumn("Branch")
  .AddRow("timewarp-nuru",
    "/home/user/worktrees/github.com/TimeWarpEngineering/timewarp-nuru/feature-branch-name",
    "feature-xyz")
  .Border(BorderStyle.Rounded));
// Path column shows: "...timewarp-nuru/feature-branch-name"
```

#### Example: Column with MinWidth constraint

```csharp
terminal.WriteTable(t => t
  .AddColumn("ID")
  .AddColumn(new TableColumn("Description") { MinWidth = 20 })
  .AddRow("1", "This is a long description that would normally be truncated heavily"));
```

### Table Shrink/Expand Behavior

- **Shrink** (default: `true`): Proportionally reduces column widths to fit terminal. Wider columns shrink more aggressively. Respects `MinWidth` per column.
- **Expand**: Distributes extra terminal width evenly across columns.
- Disable shrinking with builder `.Shrink(false)` to allow horizontal overflow.

**BorderStyle:** `Rounded`, `Square`, `Doubled`, `Heavy`, `None`

**Alignment:** `Left`, `Right`, `Center`

## AnsiStringUtils

Static utility class for ANSI-aware string operations. Handles CSI sequences and OSC 8 hyperlinks.

```csharp
AnsiStringUtils.StripAnsiCodes("\x1b[32mGreen\x1b[0m");     // "Green"
AnsiStringUtils.GetVisibleLength("\x1b[32mGreen\x1b[0m");   // 5
AnsiStringUtils.GetVisibleLength("📍 Location");             // 11 (uses UnicodeWidth)
AnsiStringUtils.PadRightVisible("\x1b[32mHi\x1b[0m", 10);   // pads to 10 visible chars
AnsiStringUtils.PadLeftVisible("\x1b[32mHi\x1b[0m", 10);    // left-pads to 10
AnsiStringUtils.CenterVisible("\x1b[32mHi\x1b[0m", 10);     // centers to 10
AnsiStringUtils.WrapText("long text...", 40);                 // word-wrap, grapheme-aware
```

## Common Pitfalls

1. **Don't use `new Table()`, `new Panel()`, `new Rule()`** - Constructors are internal. Use builders or `Action<XxxBuilder>` extension methods.
2. **Don't mix Console and Terminal** - Pick one, stick with it
3. **Always restore Terminal.Instance in tests** - Use try/finally or using pattern
4. **Check SupportsColor/SupportsHyperlinks** before using those features in production
5. **Use `UnicodeWidth.GetTextWidth()` not `.Length`** for terminal column calculations — `.Length` counts UTF-16 code units, not display columns
