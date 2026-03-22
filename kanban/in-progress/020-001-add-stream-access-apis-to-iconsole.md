# Add stream access APIs to IConsole

## Description

Add raw stream access methods to `IConsole` to match `System.Console` capabilities. This enables scenarios where code needs direct `Stream` access to stdin/stdout/stderr, or needs to redirect I/O via `TextReader`/`TextWriter`.

Parent: #020

## Checklist

### Implementation
- [ ] Add `OpenStandardInput()` → `Stream` to `IConsole`
- [ ] Add `OpenStandardOutput()` → `Stream` to `IConsole`
- [ ] Add `OpenStandardError()` → `Stream` to `IConsole`
- [ ] Add `In` → `TextReader` property to `IConsole`
- [ ] Add `Out` → `TextWriter` property to `IConsole`
- [ ] Add `Error` → `TextWriter` property to `IConsole`
- [ ] Add `SetIn(TextReader)` method to `IConsole`
- [ ] Add `SetOut(TextWriter)` method to `IConsole`
- [ ] Add `SetError(TextWriter)` method to `IConsole`
- [ ] Implement in `TimeWarpConsole`
- [ ] Implement in `TimeWarpTerminal`

### Testing
- [ ] Add `TestConsole` implementations for all new members
- [ ] Add `TestTerminal` implementations for all new members
- [ ] Add mock stream support to test implementations
- [ ] Add mock TextReader/TextWriter support to test implementations
- [ ] Write unit tests for `OpenStandardInput()`
- [ ] Write unit tests for `OpenStandardOutput()`
- [ ] Write unit tests for `OpenStandardError()`
- [ ] Write unit tests for `In`/`Out`/`Error` properties
- [ ] Write unit tests for `SetIn()`/`SetOut()`/`SetError()`

## Session

- Created: ses_2f2ab32c3ffeoD0gwPTVU0agTi (2026-03-22)

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
