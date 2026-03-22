# Mirror new APIs on Terminal static class

## Description

Mirror all new `IConsole` and `ITerminal` members on the `Terminal` static class. This ensures the static facade provides complete Console replacement.

Parent: #020

Depends on: #020-001 through #020-007 (all other child tasks must be complete)

## Checklist

### Implementation
- [x] Mirror stream access APIs from IConsole on `Terminal`
- [x] Mirror encoding APIs from IConsole on `Terminal`
- [x] Mirror redirection state APIs from IConsole on `Terminal`
- [x] Mirror rich input APIs from IConsole on `Terminal`
- [x] Mirror cursor properties from ITerminal on `Terminal`
- [x] Mirror window/buffer geometry from ITerminal on `Terminal`
- [x] Mirror color state APIs from ITerminal on `Terminal`
- [x] Mirror control/utility APIs from ITerminal on `Terminal`

### Testing
- [x] Write unit tests for static stream access methods
- [x] Write unit tests for static encoding properties
- [x] Write unit tests for static redirection properties
- [x] Write unit tests for static input methods
- [x] Write unit tests for static cursor properties
- [x] Write unit tests for static window/buffer methods
- [x] Write unit tests for static color state methods
- [x] Write unit tests for static control/utility methods

## Session

- Created: ses_2f2ab32c3ffeoD0gwPTVU0agTi (2026-03-22)

## Notes

### Files to modify
- `terminal-static.cs` - add all static members

### Design considerations
- All static members route to `Instance` (the configured `ITerminal`)
- Properties should be simple pass-through: `public static int WindowHeight => Instance.WindowHeight;`
- Methods should be simple pass-through: `public static void Beep() => Instance.Beep();`
- This task should be done LAST after all interface changes are complete

### Reference
- See existing `Terminal` static class for pattern

### Coding Standards
Follow the `/csharp` skill for all implementation work.

## Results

### What was implemented
Added 50+ static members to the `Terminal` class that route to `Instance`:

**Stream access (9 members):**
- `OpenStandardInput()`, `OpenStandardOutput()`, `OpenStandardError()`
- `In`, `Out`, `Error` properties
- `SetIn()`, `SetOut()`, `SetError()` methods

**Encoding/redirection (5 members):**
- `InputEncoding`, `OutputEncoding` properties
- `IsInputRedirected`, `IsOutputRedirected`, `IsErrorRedirected` properties

**Rich input (2 members):**
- `Read()` method
- `ReadKey()` method (parameterless)

**Cursor properties (4 members):**
- `CursorLeft`, `CursorTop`, `CursorVisible`, `CursorSize` properties

**Window/buffer geometry (11 members):**
- `WindowHeight`, `WindowLeft`, `WindowTop`, `BufferWidth`, `BufferHeight` properties
- `LargestWindowWidth`, `LargestWindowHeight` properties
- `SetWindowSize()`, `SetWindowPosition()`, `SetBufferSize()`, `MoveBufferArea()` methods

**Color state (3 members):**
- `ForegroundColor`, `BackgroundColor` properties
- `ResetColor()` method

**Control/utility (5 members):**
- `Beep()`, `Beep(int, int)` methods
- `TreatControlCAsInput`, `Title`, `KeyAvailable` properties

### Files changed
- `source/timewarp-terminal/terminal-static.cs` - added 50+ static members
- `tests/terminal-static-08-new-apis.cs` - new test file (56 tests)

### Test results
- All 56 new tests pass
- All existing tests pass
- Build succeeds with 0 warnings

### Design decisions
- All static members are simple pass-through to `Instance`
- Properties use expression-bodied members
- Methods use expression-bodied statements
- Follows existing pattern in `Terminal` static class
