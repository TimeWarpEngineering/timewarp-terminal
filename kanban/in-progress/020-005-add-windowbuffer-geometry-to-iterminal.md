# Add window/buffer geometry to ITerminal

## Description

Add window and buffer geometry properties/methods to `ITerminal` to match `System.Console` capabilities. This enables code to query and control terminal dimensions.

Parent: #020

## Checklist

### Implementation
- [ ] Add `WindowHeight` property to `ITerminal`
- [ ] Add `WindowLeft` property to `ITerminal`
- [ ] Add `WindowTop` property to `ITerminal`
- [ ] Add `BufferWidth` property to `ITerminal`
- [ ] Add `BufferHeight` property to `ITerminal`
- [ ] Add `SetWindowSize(int width, int height)` method to `ITerminal`
- [ ] Add `SetWindowPosition(int left, int top)` method to `ITerminal`
- [ ] Add `SetBufferSize(int width, int height)` method to `ITerminal`
- [ ] Add `MoveBufferArea(...)` method to `ITerminal`
- [ ] Add `LargestWindowWidth` property to `ITerminal`
- [ ] Add `LargestWindowHeight` property to `ITerminal`
- [ ] Implement in `TimeWarpTerminal`

### Testing
- [ ] Add `TestTerminal` implementations for all new members
- [ ] Add window/buffer geometry properties to `TestTerminal` with sensible defaults
- [ ] Write unit tests for `WindowHeight`/`WindowWidth` (already have WindowWidth)
- [ ] Write unit tests for `WindowLeft`/`WindowTop`
- [ ] Write unit tests for `BufferWidth`/`BufferHeight`
- [ ] Write unit tests for `SetWindowSize()`
- [ ] Write unit tests for `SetWindowPosition()`
- [ ] Write unit tests for `SetBufferSize()`
- [ ] Write unit tests for `MoveBufferArea()`
- [ ] Write unit tests for `LargestWindowWidth`/`LargestWindowHeight`

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
