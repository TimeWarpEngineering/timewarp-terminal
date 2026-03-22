# Add color state APIs to ITerminal

## Description

Add color state properties to `ITerminal` to match `System.Console` capabilities. This enables code to set foreground/background colors as terminal state (not inline ANSI styling).

Parent: #020

## Checklist

### Implementation
- [x] Add `ForegroundColor` get/set property to `ITerminal` (ConsoleColor)
- [x] Add `BackgroundColor` get/set property to `ITerminal` (ConsoleColor)
- [x] Add `ResetColor()` method to `ITerminal`
- [x] Implement in `TimeWarpTerminal`

### Testing
- [x] Add `TestTerminal` implementations for all new members
- [x] Add `ForegroundColor` property to `TestTerminal` (default: ConsoleColor.Gray)
- [x] Add `BackgroundColor` property to `TestTerminal` (default: ConsoleColor.Black)
- [x] Write unit tests for `ForegroundColor` get/set
- [x] Write unit tests for `BackgroundColor` get/set
- [x] Write unit tests for `ResetColor()` (resets to defaults)

## Session

- Created: ses_2f2ab32c3ffeoD0gwPTVU0agTi (2026-03-22)

## Notes

### Files to modify
- `iterminal.cs` - add interface members
- `timewarp-terminal.cs` - implement in TimeWarpTerminal
- `test-terminal.cs` - add test implementations

### Design considerations
- Current ANSI extension methods (`.Red()`, `.Green()`, etc.) are for **inline styling** - wrapping text with ANSI codes
- These new properties are for **terminal state** - changing the active color for all subsequent output
- `ResetColor()` should reset both foreground and background to defaults
- `TimeWarpTerminal` implementation should use `AnsiColors` to generate ANSI codes for the ConsoleColor values

### Reference
- https://learn.microsoft.com/en-us/dotnet/api/system.console.foregroundcolor
- https://learn.microsoft.com/en-us/dotnet/api/system.console.backgroundcolor
- https://learn.microsoft.com/en-us/dotnet/api/system.console.resetcolor

### Coding Standards
Follow the `/csharp` skill for all implementation work.

## Results

### What was implemented
- Added 3 color state members to `ITerminal`:
  - `ConsoleColor ForegroundColor { get; set; }` - foreground color
  - `ConsoleColor BackgroundColor { get; set; }` - background color
  - `void ResetColor()` - reset to default colors

- Implemented in `TimeWarpTerminal`:
  - ForegroundColor/BackgroundColor wrap Console properties with IOException handling
  - ResetColor() calls Console.ResetColor()
  
- Implemented in `TestTerminal`:
  - ForegroundColor property (default: ConsoleColor.Gray)
  - BackgroundColor property (default: ConsoleColor.Black)
  - ResetColor() method - resets to defaults (Gray/Black)

### Files changed
- `source/timewarp-terminal/iterminal.cs` - added interface members
- `source/timewarp-terminal/timewarp-terminal.cs` - added implementation
- `source/timewarp-terminal/test-terminal.cs` - added test implementation
- `tests/terminal-color-state-01-basic.cs` - new test file (7 tests)

### Test results
- All 7 new tests pass
- All existing tests pass
- Build succeeds with 0 warnings

### Design decisions
- These are terminal state properties (change color for all subsequent output)
- Distinct from existing ANSI extension methods (`.Red()`, `.Green()`) which are for inline styling
- TestTerminal defaults match Console defaults (Gray foreground, Black background)
