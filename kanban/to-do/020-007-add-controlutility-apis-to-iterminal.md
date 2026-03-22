# Add control/utility APIs to ITerminal

## Description

Add control and utility methods/properties to `ITerminal` to match `System.Console` capabilities. This enables beeps, title control, and Ctrl+C handling configuration.

Parent: #020

## Checklist

### Implementation
- [ ] Add `Beep()` method to `ITerminal`
- [ ] Add `Beep(int frequency, int duration)` overload to `ITerminal`
- [ ] Add `TreatControlCAsInput` get/set property to `ITerminal`
- [ ] Add `Title` get/set property to `ITerminal`
- [ ] Add `KeyAvailable` property to `ITerminal`
- [ ] Implement in `TimeWarpTerminal`

### Testing
- [ ] Add `TestTerminal` implementations for all new members
- [ ] Add `BeepCount` property to `TestTerminal` to track beep calls
- [ ] Add `TreatControlCAsInput` property to `TestTerminal` (default: false)
- [ ] Add `Title` property to `TestTerminal` (default: empty string)
- [ ] Add `KeyAvailable` property to `TestTerminal` (based on key queue)
- [ ] Write unit tests for `Beep()` (verify call count)
- [ ] Write unit tests for `Beep(int, int)` (verify parameters captured)
- [ ] Write unit tests for `TreatControlCAsInput` get/set
- [ ] Write unit tests for `Title` get/set
- [ ] Write unit tests for `KeyAvailable`

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
