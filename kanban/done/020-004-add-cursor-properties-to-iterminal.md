# Add cursor properties to ITerminal

## Description

Add cursor properties to `ITerminal` to match `System.Console` capabilities. Currently only have method pair `SetCursorPosition()`/`GetCursorPosition()`. Add direct property access and visibility/size control.

Parent: #020

## Checklist

### Implementation
- [x] Add `CursorLeft` get/set property to `ITerminal`
- [x] Add `CursorTop` get/set property to `ITerminal`
- [x] Add `CursorVisible` get/set property to `ITerminal`
- [x] Add `CursorSize` get/set property to `ITerminal` (1-100 percentage)
- [x] Implement in `TimeWarpTerminal`

### Testing
- [x] Add `TestTerminal` implementations for all new members
- [x] Add cursor position tracking to `TestTerminal` (currently has fields but not properties)
- [x] Add `CursorVisible` property to `TestTerminal` (default: true)
- [x] Add `CursorSize` property to `TestTerminal` (default: 100)
- [x] Write unit tests for `CursorLeft` get/set
- [x] Write unit tests for `CursorTop` get/set
- [x] Write unit tests for `CursorVisible` get/set
- [x] Write unit tests for `CursorSize` get/set (validate 1-100 range)

## Session

- Created: ses_2f2ab32c3ffeoD0gwPTVU0agTi (2026-03-22)

## Notes

### Files to modify
- `iterminal.cs` - add interface members
- `timewarp-terminal.cs` - implement in TimeWarpTerminal
- `test-terminal.cs` - add test implementations

### Design considerations
- `CursorLeft`/`CursorTop` properties are more idiomatic than the existing method pair
- Consider keeping `SetCursorPosition()`/`GetCursorPosition()` for backward compatibility
- `CursorSize` is 1-100 percentage (size of cursor, 1=small line, 100=full block)
- `TestTerminal` already has `CursorLeft`/`CursorTop` fields - convert to properties

### Reference
- https://learn.microsoft.com/en-us/dotnet/api/system.console.cursorleft
- https://learn.microsoft.com/en-us/dotnet/api/system.console.cursorvisible
- https://learn.microsoft.com/en-us/dotnet/api/system.console.cursorsize

### Coding Standards
Follow the `/csharp` skill for all implementation work.

## Results

### What was implemented
- Added 4 cursor properties to `ITerminal`:
  - `int CursorLeft { get; set; }` - cursor column position
  - `int CursorTop { get; set; }` - cursor row position
  - `bool CursorVisible { get; set; }` - whether cursor is visible
  - `int CursorSize { get; set; }` - cursor size as percentage (1-100)

- Implemented in `TimeWarpTerminal`:
  - CursorLeft/CursorTop wrap Console properties with IOException handling
  - CursorVisible/CursorSize have Windows platform guards (throw PlatformNotSupportedException on non-Windows)
  
- Implemented in `TestTerminal`:
  - Converted existing CursorLeft/CursorTop fields to properties
  - Added CursorVisible property (default: true)
  - Added CursorSize property (default: 100, validates 1-100 range)
  - Backward compatibility: SetCursorPosition()/GetCursorPosition() still work

### Files changed
- `source/timewarp-terminal/iterminal.cs` - added interface properties
- `source/timewarp-terminal/timewarp-terminal.cs` - added implementations with platform guards
- `source/timewarp-terminal/test-terminal.cs` - converted fields to properties, added new properties
- `tests/terminal-cursor-properties.cs` - new test file (13 tests)

### Test results
- All 13 new tests pass
- All existing tests pass
- Build succeeds with 0 warnings

### Design decisions
- Kept SetCursorPosition()/GetCursorPosition() for backward compatibility
- CursorVisible/CursorSize throw PlatformNotSupportedException on non-Windows (matches Console behavior)
- TestTerminal validates CursorSize range (1-100)
