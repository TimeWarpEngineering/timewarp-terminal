# Add rich input APIs to IConsole

## Description

Add character-level input methods to `IConsole` to match `System.Console` capabilities. This enables reading single characters and key presses without requiring a full line.

Parent: #020

## Checklist

### Implementation
- [x] Add `Read()` → `int` method to `IConsole` (reads single character, returns -1 on EOF)
- [x] Add `ReadKey()` overload without parameter to `IConsole` (defaults to intercept: false)
- [x] Implement in `TimeWarpConsole`
- [x] Implement in `TimeWarpTerminal`

### Testing
- [x] Add `TestConsole` implementations for all new members
- [x] Add `TestTerminal` implementations for all new members
- [x] Add character queue support to `TestConsole` for `Read()`
- [x] Write unit tests for `Read()` returning single character
- [x] Write unit tests for `Read()` returning -1 on EOF
- [x] Write unit tests for `ReadKey()` without parameter (intercept: false)
- [x] Write unit tests for `ReadKey()` with intercept: true (existing)

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

## Results

### What was implemented
- Added `int Read()` method to `IConsole` - reads single character, returns -1 on EOF
- Added `ConsoleKeyInfo ReadKey()` overload to `IConsole` - defaults to intercept: false
- Implemented in `TimeWarpConsole` - wraps Console.Read() and Console.ReadKey(false)
- Implemented in `TimeWarpTerminal` - wraps Console.Read() and Console.ReadKey(false)
- Implemented in `TestConsole` with character queue:
  - `QueueCharacters(string)` method to queue characters for Read()
  - `CharactersInQueue` property
  - `Read()` returns next char from queue, or -1 if empty
  - `ReadKey()` throws NotSupportedException (use TestTerminal for key input)
- Implemented in `TestTerminal`:
  - `Read()` uses existing key queue
  - `ReadKey()` overload without parameter calls ReadKey(false)

### Files changed
- `source/timewarp-terminal/iconsole.cs` - added interface members
- `source/timewarp-terminal/timewarp-console.cs` - implemented Read/ReadKey
- `source/timewarp-terminal/timewarp-terminal.cs` - implemented Read/ReadKey
- `source/timewarp-terminal/test-console.cs` - added character queue and implementations
- `source/timewarp-terminal/test-terminal.cs` - added Read/ReadKey implementations
- `tests/rich-input-01-basic.cs` - new test file (15 tests)

### Test results
- All 15 new tests pass
- All existing tests pass
- Build succeeds with 0 warnings
