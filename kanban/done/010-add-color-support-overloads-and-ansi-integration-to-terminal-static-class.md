# Add color support overloads and ANSI integration to Terminal static class

## Description

Add color parameter overloads and ensure seamless integration with the existing ANSI extension methods (C# 14 extension blocks). This enables users to write colored output using both explicit parameters and the fluent extension method style.

## Checklist

- [x] Add `WriteLine(string message, ConsoleColor foregroundColor)` overload
- [x] Add `WriteLine(string message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)` overload
- [x] Add `Write(string message, ConsoleColor foregroundColor)` overload
- [x] Add `WriteErrorLine(string message, ConsoleColor foregroundColor)` overload
- [x] Add `WriteTable` color parameter support
- [x] Add `WritePanel` color parameter support
- [x] Ensure ANSI extension methods work with `Terminal.Instance.Write()`
- [x] Add XML documentation for color overloads
- [x] Write unit tests for color support
- [x] Update README with color usage examples

## Notes

### Implementation Plan

#### 1. ConsoleColor to ANSI Mapping (`ansi-colors.cs`)
Add `GetForeground(ConsoleColor)` and `GetBackground(ConsoleColor)` extension methods to convert ConsoleColor enum values to ANSI escape codes.

#### 2. Terminal Static Class Overloads (`terminal-static.cs`)

**Color Overloads (4 methods):**
- `Write(string? message, ConsoleColor foregroundColor)` - Write with foreground color
- `WriteLine(string? message, ConsoleColor foregroundColor)` - WriteLine with foreground color
- `WriteLine(string? message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)` - WriteLine with fg/bg colors
- `WriteErrorLine(string? message, ConsoleColor foregroundColor)` - WriteErrorLine with foreground color

**Widget Color Support (5 methods):**
- `WriteTable(Action<TableBuilder>, foregroundColor, backgroundColor)` - Table with colors
- `WriteTable(Table, foregroundColor, backgroundColor)` - Pre-built table with colors
- `WritePanel(Action<PanelBuilder>, foregroundColor, backgroundColor)` - Panel with colors
- `WritePanel(string content, string? header, foregroundColor, backgroundColor)` - Simple panel with colors
- `WritePanel(Panel, foregroundColor, backgroundColor)` - Pre-built panel with colors

#### 3. Widget Extension Updates
Add color parameters to `terminal-table-extensions.cs` and `terminal-panel-extensions.cs`.

#### 4. Unit Tests (`tests/terminal-static-06-color.cs`)
8 test cases covering all color overloads and widget color support.

#### 5. README Updates
Add ConsoleColor usage examples section.

#### Implementation Pattern
```csharp
public static void WriteLine(string? message, ConsoleColor foregroundColor)
{
  string coloredMessage = AnsiColors.GetForeground(foregroundColor) + (message ?? string.Empty) + AnsiColors.Reset;
  Instance.WriteLine(coloredMessage);
}
```

#### Files Modified
1. `ansi-colors.cs` - Add GetForeground/GetBackground methods
2. `terminal-static.cs` - Add ConsoleColor overloads
3. `terminal-table-extensions.cs` - Add color parameters
4. `terminal-panel-extensions.cs` - Add color parameters
5. `tests/terminal-static-06-color.cs` - New test file
6. `README.md` - Add usage examples

#### Summary
- 9 new color methods in Terminal static class
- 2 new helper methods in AnsiColors
- Color parameters for WriteTable (2 overloads) and WritePanel (3 overloads)
- 8 unit tests
- Full ConsoleColor enum support (16 colors)

---

### Integration with ANSI extension blocks (C# 14):
```csharp
using static TimeWarp.Terminal.Terminal;

// Fluent ANSI style (existing extension methods)
WriteLine("Error!".Red().Bold());
WriteLine("Success!".Green());

// Explicit color parameters
WriteLine("Error!", ConsoleColor.Red);
WriteLine("Warning!", ConsoleColor.Yellow, ConsoleColor.Black);
```

## Results

### What Was Implemented

Added comprehensive ConsoleColor support to the Terminal static class with the following components:

#### 1. ConsoleColor to ANSI Mapping (`ansi-colors.cs`)
- `GetForeground(ConsoleColor color)` - Converts ConsoleColor to ANSI foreground escape code
- `GetBackground(ConsoleColor color)` - Converts ConsoleColor to ANSI background escape code
- Supports all 16 ConsoleColor enum values

#### 2. Terminal Static Class Overloads (`terminal-static.cs`)
**Color Output Methods (4 new methods):**
- `Write(string? message, ConsoleColor foregroundColor)` - Write with foreground color
- `WriteLine(string? message, ConsoleColor foregroundColor)` - WriteLine with foreground color
- `WriteLine(string? message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)` - WriteLine with fg/bg colors
- `WriteErrorLine(string? message, ConsoleColor foregroundColor)` - WriteErrorLine with foreground color

**Widget Color Support (5 new methods):**
- `WriteTable(Action<TableBuilder>, ConsoleColor?, ConsoleColor?)` - Table with colors via builder
- `WriteTable(Table, ConsoleColor?, ConsoleColor?)` - Pre-built table with colors
- `WritePanel(Action<PanelBuilder>, ConsoleColor?, ConsoleColor?)` - Panel with colors via builder
- `WritePanel(string content, string? header, ConsoleColor?, ConsoleColor?)` - Simple panel with colors
- `WritePanel(Panel, ConsoleColor?, ConsoleColor?)` - Pre-built panel with colors

#### 3. Widget Extension Updates
**Table Extensions (`terminal-table-extensions.cs`):**
- Added 3 methods supporting color parameters
- Added private `WriteLinesWithColor` helper method

**Panel Extensions (`terminal-panel-extensions.cs`):**
- Added 5 methods supporting color parameters
- Added private `WriteLinesWithColor` helper method

#### 4. Unit Tests (`tests/terminal-static-06-color.cs`)
- 8 comprehensive test cases covering all color overloads
- Tests for basic color output, widget colors, and edge cases
- All tests pass successfully

#### 5. Documentation (`README.md`)
- Added "ConsoleColor Support" section with usage examples
- Includes code examples for all color scenarios
- Lists supported colors for foreground and background

### Files Changed

1. **`source/timewarp-terminal/ansi-colors.cs`** - Added GetForeground/GetBackground methods
2. **`source/timewarp-terminal/terminal-static.cs`** - Added 9 color overloads
3. **`source/timewarp-terminal/widgets/terminal-table-extensions.cs`** - Added 3 color methods
4. **`source/timewarp-terminal/widgets/terminal-panel-extensions.cs`** - Added 5 color methods
5. **`tests/terminal-static-06-color.cs`** - New test file with 8 tests
6. **`README.md`** - Added ConsoleColor usage documentation

### Key Implementation Details

- Uses `AnsiColors.Reset` after all colored output to prevent color bleeding
- Delegates colored widget methods to specialized overloads to avoid code duplication
- Null message handling with colors produces color codes + reset (no text)
- All ConsoleColor enum values mapped to appropriate ANSI codes
- Extension methods for widgets maintain consistency with Terminal static API

### Usage Examples

```csharp
using static TimeWarp.Terminal.Terminal;

// Basic colored output
WriteLine("Error!", ConsoleColor.Red);
WriteLine("Success!", ConsoleColor.Green);
WriteLine("Warning!", ConsoleColor.Yellow);

// Colored background
WriteLine("Highlighted", ConsoleColor.Black, ConsoleColor.Yellow);

// Error output
WriteErrorLine("Error: File not found", ConsoleColor.Red);

// Widgets with colors
WriteTable(t => t.AddColumn("Name").AddRow("Value"),
  ConsoleColor.White, ConsoleColor.DarkBlue);

WritePanel("Content", "Header",
  ConsoleColor.White, ConsoleColor.DarkBlue);
```

### Test Outcomes

- **Build:** ✓ Succeeded with 0 warnings, 0 errors
- **Tests:** ✓ All 8 color support tests pass
- **Documentation:** ✓ README updated with comprehensive examples

### Verification Commands

```bash
# Build
dotnet build source/timewarp-terminal/timewarp-terminal.csproj

# Run color tests
dotnet run --project tests/terminal-static-06-color.cs

# Run all tests
dotnet run tests/terminal-static-*.cs
```

### Summary

**Total Methods Added:** 19 new methods across 5 files
- 2 ConsoleColor mapping methods
- 9 Terminal static class overloads
- 8 widget extension methods
- 8 unit tests
- 40 lines of documentation

This implementation provides seamless integration between ConsoleColor enum values and ANSI escape codes, enabling users to use familiar Console-style color parameters while maintaining full compatibility with the existing ANSI extension method patterns.
