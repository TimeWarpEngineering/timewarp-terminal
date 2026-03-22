# Add color state APIs to ITerminal

## Description

Add color state properties to `ITerminal` to match `System.Console` capabilities. This enables code to set foreground/background colors as terminal state (not inline ANSI styling).

Parent: #020

## Checklist

### Implementation
- [ ] Add `ForegroundColor` get/set property to `ITerminal` (ConsoleColor)
- [ ] Add `BackgroundColor` get/set property to `ITerminal` (ConsoleColor)
- [ ] Add `ResetColor()` method to `ITerminal`
- [ ] Implement in `TimeWarpTerminal`

### Testing
- [ ] Add `TestTerminal` implementations for all new members
- [ ] Add `ForegroundColor` property to `TestTerminal` (default: ConsoleColor.Gray)
- [ ] Add `BackgroundColor` property to `TestTerminal` (default: ConsoleColor.Black)
- [ ] Write unit tests for `ForegroundColor` get/set
- [ ] Write unit tests for `BackgroundColor` get/set
- [ ] Write unit tests for `ResetColor()` (resets to defaults)

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
