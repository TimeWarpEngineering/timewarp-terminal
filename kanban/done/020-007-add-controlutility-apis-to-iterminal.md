# Add control/utility APIs to ITerminal

## Description

Add control and utility methods/properties to `ITerminal` to match `System.Console` capabilities. This enables beeps, title control, and Ctrl+C handling configuration.

Parent: #020

## Checklist

### Implementation
- [x] Add `Beep()` method to `ITerminal`
- [x] Add `Beep(int frequency, int duration)` overload to `ITerminal`
- [x] Add `TreatControlCAsInput` get/set property to `ITerminal`
- [x] Add `Title` get/set property to `ITerminal`
- [x] Add `KeyAvailable` property to `ITerminal`
- [x] Implement in `TimeWarpTerminal`

### Testing
- [x] Add `TestTerminal` implementations for all new members
- [x] Add `BeepCount` property to `TestTerminal` to track beep calls
- [x] Add `TreatControlCAsInput` property to `TestTerminal` (default: false)
- [x] Add `Title` property to `TestTerminal` (default: empty string)
- [x] Add `KeyAvailable` property to `TestTerminal` (based on key queue)
- [x] Write unit tests for `Beep()` (verify call count)
- [x] Write unit tests for `Beep(int, int)` (verify parameters captured)
- [x] Write unit tests for `TreatControlCAsInput` get/set
- [x] Write unit tests for `Title` get/set
- [x] Write unit tests for `KeyAvailable`

## Session

- Created: ses_2f2ab32c3ffeoD0gwPTVU0agTi (2026-03-22)

## Notes

### Files to modify
- `iterminal.cs` - add interface members
- `timewarp-terminal.cs` - implement in TimeWarpTerminal
- `test-terminal.cs` - add test implementations

### Design considerations
- `Beep()` in tests should not actually beep - just track that it was called
- `Beep(frequency, duration)` - frequency is 37-32767 Hz, duration is milliseconds
- `TreatControlCAsInput` - when true, Ctrl+C is passed to `ReadKey()` instead of raising `CancelKeyPress`
- `KeyAvailable` - returns true if a key press is available in the input stream
- `TestTerminal` already has `KeysInQueue` property - `KeyAvailable` can use this

### Reference
- https://learn.microsoft.com/en-us/dotnet/api/system.console.beep
- https://learn.microsoft.com/en-us/dotnet/api/system.console.treatcontrolcasinput
- https://learn.microsoft.com/en-us/dotnet/api/system.console.title
- https://learn.microsoft.com/en-us/dotnet/api/system.console.keyavailable

### Coding Standards
Follow the `/csharp` skill for all implementation work.

## Results

### What was implemented
- Added 5 control/utility members to `ITerminal`:
  - `void Beep()` - play beep sound
  - `void Beep(int frequency, int duration)` - beep with custom frequency/duration
  - `bool TreatControlCAsInput { get; set; }` - whether Ctrl+C is treated as input
  - `string Title { get; set; }` - console title
  - `bool KeyAvailable { get; }` - whether a key is available

- Implemented in `TimeWarpTerminal`:
  - Beep methods wrap Console.Beep (Windows only)
  - TreatControlCAsInput wraps Console.TreatControlCAsInput
  - Title wraps Console.Title (returns empty string on non-Windows)
  - KeyAvailable wraps Console.KeyAvailable with IOException handling
  
- Implemented in `TestTerminal`:
  - BeepCount property - tracks number of beep calls
  - LastBeepFrequency/LastBeepDuration - capture last beep parameters
  - TreatControlCAsInput property (default: false)
  - Title property (default: "")
  - KeyAvailable property - returns KeysInQueue > 0

### Files changed
- `source/timewarp-terminal/iterminal.cs` - added interface members
- `source/timewarp-terminal/timewarp-terminal.cs` - added implementation
- `source/timewarp-terminal/test-terminal.cs` - added test implementation
- `tests/terminal-control-utilities-01-basic.cs` - new test file (12 tests)

### Test results
- All 12 new tests pass
- All existing tests pass
- Build succeeds with 0 warnings

### Design decisions
- Beep methods are Windows-only (PlatformNotSupportedException on other platforms)
- Title returns empty string on non-Windows platforms
- TestTerminal tracks beep calls without actually beeping
