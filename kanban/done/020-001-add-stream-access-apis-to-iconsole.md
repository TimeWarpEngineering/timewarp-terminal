# Add stream access APIs to IConsole

## Description

Add raw stream access methods to `IConsole` to match `System.Console` capabilities. This enables scenarios where code needs direct `Stream` access to stdin/stdout/stderr, or needs to redirect I/O via `TextReader`/`TextWriter`.

Parent: #020

## Checklist

### Implementation
- [x] Add `OpenStandardInput()` → `Stream` to `IConsole`
- [x] Add `OpenStandardOutput()` → `Stream` to `IConsole`
- [x] Add `OpenStandardError()` → `Stream` to `IConsole`
- [x] Add `In` → `TextReader` property to `IConsole`
- [x] Add `Out` → `TextWriter` property to `IConsole`
- [x] Add `Error` → `TextWriter` property to `IConsole`
- [x] Add `SetIn(TextReader)` method to `IConsole`
- [x] Add `SetOut(TextWriter)` method to `IConsole`
- [x] Add `SetError(TextWriter)` method to `IConsole`
- [x] Implement in `TimeWarpConsole`
- [x] Implement in `TimeWarpTerminal`

### Testing
- [x] Add `TestConsole` implementations for all new members
- [x] Add `TestTerminal` implementations for all new members
- [x] Add mock stream support to test implementations
- [x] Add mock TextReader/TextWriter support to test implementations
- [x] Write unit tests for `OpenStandardInput()`
- [x] Write unit tests for `OpenStandardOutput()`
- [x] Write unit tests for `OpenStandardError()`
- [x] Write unit tests for `In`/`Out`/`Error` properties
- [x] Write unit tests for `SetIn()`/`SetOut()`/`SetError()`

## Session

- Created: ses_2f2ab32c3ffeoD0gwPTVU0agTi (2026-03-22)
- Completed: ses_2e9f62f66ffeh05Tj3xKWlwLsS (2026-03-22)

## Notes

### Files to modify
- `iconsole.cs` - add interface members
- `timewarp-console.cs` - implement in TimeWarpConsole
- `timewarp-terminal.cs` - implement in TimeWarpTerminal
- `test-console.cs` - add test implementations
- `test-terminal.cs` - add test implementations

### Design considerations
- `OpenStandard*()` methods return `Stream` - test implementations need mock streams (e.g., `MemoryStream`)
- `In`/`Out`/`Error` are `TextReader`/`TextWriter` - test implementations can use `StringReader`/`StringWriter`
- RS0030 analyzer currently flags `Console.OpenStandard*` usage - this task satisfies that analyzer

### Reference
- https://learn.microsoft.com/en-us/dotnet/api/system.console.openstandardinput

### Coding Standards
Follow the `/csharp` skill for all implementation work.

## Results

### What was implemented
Added 9 stream access members to `IConsole`:
- `Stream OpenStandardInput()` - opens stdin as a stream
- `Stream OpenStandardOutput()` - opens stdout as a stream
- `Stream OpenStandardError()` - opens stderr as a stream
- `TextReader In { get; }` - standard input reader
- `TextWriter Out { get; }` - standard output writer
- `TextWriter Error { get; }` - standard error writer
- `void SetIn(TextReader)` - sets standard input
- `void SetOut(TextWriter)` - sets standard output
- `void SetError(TextWriter)` - sets standard error

### Files changed
- `source/timewarp-terminal/iconsole.cs` - added 9 interface members
- `source/timewarp-terminal/timewarp-console.cs` - implemented all 9 members
- `source/timewarp-terminal/timewarp-terminal.cs` - implemented all 9 members
- `source/timewarp-terminal/test-console.cs` - added mock streams and implementations
- `source/timewarp-terminal/test-terminal.cs` - added mock streams and implementations
- `Directory.Build.props` - added CA1716 to NoWarn (matching System.Console API names)
- `tests/stream-access-01-basic.cs` - new test file (26 tests)

### Test results
- All 26 new tests pass
- All existing tests pass
- Build succeeds with 0 warnings

### Design decisions
- TestConsole/TestTerminal use MemoryStream for OpenStandard* methods
- In/Out/Error properties use StringReader/StringWriter in test implementations
- SetIn/SetOut/SetError update the internal readers/writers
- Added CA1716 suppression for naming conflict with System.Console API (In/Out/Error/SetIn/SetOut/SetError)
