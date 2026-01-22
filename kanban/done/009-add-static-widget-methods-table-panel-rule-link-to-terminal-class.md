# Add static widget methods (Table, Panel, Rule, Link) to Terminal class

## Description

Add static methods to the `Terminal` class that provide direct access to widget functionality (Table, Panel, Rule, Link) without requiring an instance. This enables users to use widgets with the same simple syntax as basic output methods.

## Checklist

- [ ] Add `WriteTable(Action<TableBuilder> configure)` static method
- [ ] Add `WriteTable(Table table)` static method
- [ ] Add `WritePanel(Action<PanelBuilder> configure)` static method
- [ ] Add `WritePanel(string content, string? header = null)` static method
- [ ] Add `WriteRule(string? title = null)` static method
- [ ] Add `WriteRule(Action<RuleBuilder> configure)` static method
- [ ] Add `WriteLink(string url, string text)` static method
- [ ] Add XML documentation for all widget methods
- [ ] Write unit tests for widget static methods

## Notes

## Implementation Plan

### Insertion Point
**File:** `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/Cramer-2025-12-22-dev/source/timewarp-terminal/terminal-static.cs`
**Location:** After line 206 (end of Format Overloads section), before line 208 (Input Methods comment)

### Methods to Add (7 total):

1. **WriteTable(Action<TableBuilder> configure)** - Fluent builder pattern
2. **WriteTable(Table table)** - Pre-configured table
3. **WritePanel(Action<PanelBuilder> configure)** - Fluent builder pattern
4. **WritePanel(string content, string? header = null)** - Simple content + optional header
5. **WriteRule(string? title = null)** - Simple rule with optional title
6. **WriteRule(Action<RuleBuilder> configure)** - Fluent builder pattern
7. **WriteLink(string url, string text)** - OSC 8 hyperlink

### Implementation Pattern Examples:

**Table:**
```csharp
public static void WriteTable(Action<TableBuilder> configure)
{
  TableBuilder builder = new();
  configure(builder);
  Table table = builder.Build();
  string[] lines = table.Render(WindowWidth);
  foreach (string line in lines)
    Instance.WriteLine(line);
}
```

**Panel:**
```csharp
public static void WritePanel(string content, string? header = null)
{
  Panel panel = new() { Content = content, Header = header };
  string[] lines = panel.Render(WindowWidth);
  foreach (string line in lines)
    Instance.WriteLine(line);
}
```

**Rule:**
```csharp
public static void WriteRule(string? title = null)
{
  Rule rule = new() { Title = title };
  string rendered = rule.Render(WindowWidth);
  Instance.WriteLine(rendered);
}
```

**Link:**
```csharp
public static void WriteLink(string url, string text)
{
  string link = AnsiHyperlinks.CreateLink(text, url);
  Instance.Write(link);
}
```

### New Test File
**File:** `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/Cramer-2025-12-22-dev/tests/terminal-static-05-widgets.cs`
- 14 test cases covering all widget methods
- Tests for null argument validation
- Uses TestTerminal for output capture

### Verification Steps
1. Compile: `dotnet build source/timewarp-terminal/timewarp-terminal.csproj`
2. Run tests: `./tests/terminal-static-05-widgets.cs`
3. Verify all tests pass

### Summary
- 7 new widget static methods
- 14 new test cases
- Full XML documentation for all methods
- Consistent with existing codebase patterns

## Results

### What Was Implemented
Added 7 static widget methods to the `Terminal` class:

1. **Table Methods:**
   - `WriteTable(Action<TableBuilder> configure)` - Fluent builder pattern
   - `WriteTable(Table table)` - Pre-configured Table instance

2. **Panel Methods:**
   - `WritePanel(Action<PanelBuilder> configure)` - Fluent builder pattern
   - `WritePanel(string content, string? header = null)` - Simple content + optional header

3. **Rule Methods:**
   - `WriteRule(string? title = null)` - Simple rule with optional title
   - `WriteRule(Action<RuleBuilder> configure)` - Fluent builder pattern

4. **Link Method:**
   - `WriteLink(string url, string text)` - OSC 8 clickable hyperlink

### Files Changed
1. `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/Cramer-2025-12-22-dev/source/timewarp-terminal/terminal-static.cs`
   - Added 7 static widget methods
   - All methods include XML documentation with examples
   - Added `#pragma warning disable CA1054` for hyperlink method (matching existing pattern in ansi-hyperlink-extensions.cs)

2. `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/Cramer-2025-12-22-dev/tests/terminal-static-05-widgets.cs` (new file)
   - 15 comprehensive unit tests covering all widget methods
   - Tests for null argument validation

### Key Decisions
- Used `ArgumentNullException.ThrowIfNull()` for parameter validation, consistent with existing extension methods
- Simplified `WritePanel(string, string?)` signature without BorderStyle parameter for cleaner API
- Simplified `WriteRule(string?)` signature without LineStyle parameter for cleaner API
- Added `#pragma warning disable CA1054` for WriteLink to allow string URLs (matches existing hyperlink extensions pattern)
- All methods use `WindowWidth` from Instance for responsive rendering

### Test Outcomes
All 15 tests pass successfully:
- 2 Table tests (builder and pre-configured)
- 3 Panel tests (builder, content+header, content-only)
- 3 Rule tests (with title, without title, builder)
- 1 Link test (OSC 8 hyperlink output)
- 6 Null argument validation tests

### Verification
- Build: ✓ Succeeded with 0 warnings, 0 errors
- Tests: ✓ All 15 tests pass

### Usage Examples
```csharp
using static TimeWarp.Terminal.Terminal;

// Table with builder
WriteTable(t => t
    .AddColumns("Name", "Stars")
    .AddRow("CleanArchitecture", "16.5k")
    .AddRow("GuardClauses", "3.2k"));

// Panel with content
WritePanel("Configuration loaded successfully", "Settings");

// Rule separator
WriteRule("Section Title");

// Hyperlink
WriteLink("https://example.com", "Click here");
```
