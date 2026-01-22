# Add color support overloads and ANSI integration to Terminal static class

## Description

Add color parameter overloads and ensure seamless integration with the existing ANSI extension methods (C# 14 extension blocks). This enables users to write colored output using both explicit parameters and the fluent extension method style.

## Checklist

- [ ] Add `WriteLine(string message, ConsoleColor foregroundColor)` overload
- [ ] Add `WriteLine(string message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)` overload
- [ ] Add `Write(string message, ConsoleColor foregroundColor)` overload
- [ ] Add `WriteErrorLine(string message, ConsoleColor foregroundColor)` overload
- [ ] Add `WriteTable` color parameter support
- [ ] Add `WritePanel` color parameter support
- [ ] Ensure ANSI extension methods work with `Terminal.Instance.Write()`
- [ ] Add XML documentation for color overloads
- [ ] Write unit tests for color support
- [ ] Update README with color usage examples

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
