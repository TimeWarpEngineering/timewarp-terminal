# Add window/buffer geometry to ITerminal

## Description

Add window and buffer geometry properties/methods to `ITerminal` to match `System.Console` capabilities. This enables code to query and control terminal dimensions.

Parent: #020

## Checklist

### Implementation
- [x] Add `WindowHeight` property to `ITerminal`
- [x] Add `WindowLeft` property to `ITerminal`
- [x] Add `WindowTop` property to `ITerminal`
- [x] Add `BufferWidth` property to `ITerminal`
- [x] Add `BufferHeight` property to `ITerminal`
- [x] Add `SetWindowSize(int width, int height)` method to `ITerminal`
- [x] Add `SetWindowPosition(int left, int top)` method to `ITerminal`
- [x] Add `SetBufferSize(int width, int height)` method to `ITerminal`
- [x] Add `MoveBufferArea(...)` method to `ITerminal`
- [x] Add `LargestWindowWidth` property to `ITerminal`
- [x] Add `LargestWindowHeight` property to `ITerminal`
- [x] Implement in `TimeWarpTerminal`

### Testing
- [x] Add `TestTerminal` implementations for all new members
- [x] Add window/buffer geometry properties to `TestTerminal` with sensible defaults
- [x] Write unit tests for `WindowHeight`/`WindowWidth` (already have WindowWidth)
- [x] Write unit tests for `WindowLeft`/`WindowTop`
- [x] Write unit tests for `BufferWidth`/`BufferHeight`
- [x] Write unit tests for `SetWindowSize()`
- [x] Write unit tests for `SetWindowPosition()`
- [x] Write unit tests for `SetBufferSize()`
- [x] Write unit tests for `MoveBufferArea()`
- [x] Write unit tests for `LargestWindowWidth`/`LargestWindowHeight`

## Session

- Created: ses_2f2ab32c3ffeoD0gwPTVU0agTi (2026-03-22)

## Notes

### Files to modify
- `iterminal.cs` - add interface members
- `timewarp-terminal.cs` - implement in TimeWarpTerminal
- `test-terminal.cs` - add test implementations

### Design considerations
- `WindowWidth` already exists on `ITerminal` - add `WindowHeight`
- Test implementations need sensible defaults (e.g., 80x24 for window, 80x300 for buffer)
- `MoveBufferArea` has complex signature - check Console.MoveBufferArea for parameters
- Some properties may throw `IOException` on redirected output - handle gracefully

### Reference
- https://learn.microsoft.com/en-us/dotnet/api/system.console.windowheight
- https://learn.microsoft.com/en-us/dotnet/api/system.console.setwindowsize
- https://learn.microsoft.com/en-us/dotnet/api/system.console.movebufferarea

### Coding Standards
Follow the `/csharp` skill for all implementation work.

## Results

### What was implemented
Added 11 window/buffer geometry members to `ITerminal`:
- `int WindowHeight { get; set; }` - window height
- `int WindowLeft { get; set; }` - window left position
- `int WindowTop { get; set; }` - window top position
- `int BufferWidth { get; set; }` - buffer width
- `int BufferHeight { get; set; }` - buffer height
- `void SetWindowSize(int width, int height)` - set window size
- `void SetWindowPosition(int left, int top)` - set window position
- `void SetBufferSize(int width, int height)` - set buffer size
- `void MoveBufferArea(...)` - move buffer area (9 parameters)
- `int LargestWindowWidth { get; }` - largest possible window width
- `int LargestWindowHeight { get; }` - largest possible window height

### Files changed
- `source/timewarp-terminal/iterminal.cs` - added interface members
- `source/timewarp-terminal/timewarp-terminal.cs` - implemented with OperatingSystem.IsWindows() guards
- `source/timewarp-terminal/test-terminal.cs` - added test implementations
- `tests/terminal-window-buffer-geometry.cs` - new test file (21 tests)

### Test results
- All 21 new tests pass
- All existing tests pass
- Build succeeds with 0 warnings

### Design decisions
- WindowLeft, WindowTop, SetWindowPosition, SetBufferSize, MoveBufferArea are Windows-only (throw PlatformNotSupportedException on other platforms)
- TestTerminal uses sensible defaults: WindowHeight=24, BufferWidth=80, BufferHeight=300, LargestWindowWidth=120, LargestWindowHeight=40
- MoveBufferAreaCallCount property added to TestTerminal to track calls
