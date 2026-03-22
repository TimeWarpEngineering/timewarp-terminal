# Add rich input APIs to IConsole

## Description

Add character-level input methods to `IConsole` to match `System.Console` capabilities. This enables reading single characters and key presses without requiring a full line.

Parent: #020

## Checklist

### Implementation
- [ ] Add `Read()` → `int` method to `IConsole` (reads single character, returns -1 on EOF)
- [ ] Add `ReadKey()` overload without parameter to `IConsole` (defaults to intercept: false)
- [ ] Implement in `TimeWarpConsole`
- [ ] Implement in `TimeWarpTerminal`

### Testing
- [ ] Add `TestConsole` implementations for all new members
- [ ] Add `TestTerminal` implementations for all new members
- [ ] Add character queue support to `TestConsole` for `Read()`
- [ ] Write unit tests for `Read()` returning single character
- [ ] Write unit tests for `Read()` returning -1 on EOF
- [ ] Write unit tests for `ReadKey()` without parameter (intercept: false)
- [ ] Write unit tests for `ReadKey()` with intercept: true (existing)

## Session

- Created: ses_2f2ab32c3ffeoD0gwPTVU0agTi (2026-03-22)

## Notes

### Files to modify
- `iconsole.cs` - add interface members
- `timewarp-console.cs` - implement in TimeWarpConsole
- `timewarp-terminal.cs` - implement in TimeWarpTerminal
- `test-console.cs` - add test implementations
- `test-terminal.cs` - add test implementations (may already have ReadKey support)

### Design considerations
- `Read()` returns `int` to allow -1 for EOF (same as `Console.Read()`)
- `ReadKey()` without parameter should default to `intercept: false` (display the key)
- `TestConsole` already has `ReadLine()` - need to add character-level input support

### Reference
- https://learn.microsoft.com/en-us/dotnet/api/system.console.read
- https://learn.microsoft.com/en-us/dotnet/api/system.console.readkey

### Coding Standards
Follow the `/csharp` skill for all implementation work.
