# Add cursor properties to ITerminal

## Description

Add cursor properties to `ITerminal` to match `System.Console` capabilities. Currently only have method pair `SetCursorPosition()`/`GetCursorPosition()`. Add direct property access and visibility/size control.

Parent: #020

## Checklist

### Implementation
- [ ] Add `CursorLeft` get/set property to `ITerminal`
- [ ] Add `CursorTop` get/set property to `ITerminal`
- [ ] Add `CursorVisible` get/set property to `ITerminal`
- [ ] Add `CursorSize` get/set property to `ITerminal` (1-100 percentage)
- [ ] Implement in `TimeWarpTerminal`

### Testing
- [ ] Add `TestTerminal` implementations for all new members
- [ ] Add cursor position tracking to `TestTerminal` (currently has fields but not properties)
- [ ] Add `CursorVisible` property to `TestTerminal` (default: true)
- [ ] Add `CursorSize` property to `TestTerminal` (default: 100)
- [ ] Write unit tests for `CursorLeft` get/set
- [ ] Write unit tests for `CursorTop` get/set
- [ ] Write unit tests for `CursorVisible` get/set
- [ ] Write unit tests for `CursorSize` get/set (validate 1-100 range)

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
