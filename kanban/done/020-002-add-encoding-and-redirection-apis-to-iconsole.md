# Add encoding and redirection APIs to IConsole

## Description

Add encoding and redirection state properties to `IConsole` to match `System.Console` capabilities. This enables code to detect and control text encoding and check if streams are redirected.

Parent: #020

## Checklist

### Implementation
- [x] Add `InputEncoding` get/set property to `IConsole`
- [x] Add `OutputEncoding` get/set property to `IConsole`
- [x] Add `IsInputRedirected` property to `IConsole`
- [x] Add `IsOutputRedirected` property to `IConsole`
- [x] Add `IsErrorRedirected` property to `IConsole`
- [x] Implement in `TimeWarpConsole`
- [x] Implement in `TimeWarpTerminal`
- [x] Consider deprecating `IsInteractive` on `ITerminal` in favor of explicit `!IsInputRedirected`

### Testing
- [x] Add `TestConsole` implementations for all new members
- [x] Add `TestTerminal` implementations for all new members
- [x] Add mock encoding support to test implementations (default to UTF-8)
- [x] Add redirection state properties to test implementations (default to false)
- [x] Write unit tests for `InputEncoding` get/set
- [x] Write unit tests for `OutputEncoding` get/set
- [x] Write unit tests for `IsInputRedirected`
- [x] Write unit tests for `IsOutputRedirected`
- [x] Write unit tests for `IsErrorRedirected`

## Session

- Created: ses_2f2ab32c3ffeoD0gwPTVU0agTi (2026-03-22)

## Notes

### Files to modify
- `iconsole.cs` - add interface members
- `iterminal.cs` - consider IsInteractive deprecation
- `timewarp-console.cs` - implement in TimeWarpConsole
- `timewarp-terminal.cs` - implement in TimeWarpTerminal
- `test-console.cs` - add test implementations
- `test-terminal.cs` - add test implementations

### Design considerations
- Encoding properties should default to `Encoding.UTF8` in test implementations
- Redirection properties should default to `false` in test implementations
- `IsInteractive` on ITerminal currently returns `!Console.IsInputRedirected` - consider if this should be deprecated or kept as convenience

### Reference
- https://learn.microsoft.com/en-us/dotnet/api/system.console.inputencoding
- https://learn.microsoft.com/en-us/dotnet/api/system.console.isinputredirected

### Coding Standards
Follow the `/csharp` skill for all implementation work.

## Results

### What was implemented
- Added 5 new properties to `IConsole` interface:
  - `Encoding InputEncoding { get; set; }`
  - `Encoding OutputEncoding { get; set; }`
  - `bool IsInputRedirected { get; }`
  - `bool IsOutputRedirected { get; }`
  - `bool IsErrorRedirected { get; }`

- Implemented in `TimeWarpConsole` - delegates to Console properties
- Implemented in `TimeWarpTerminal` - same as TimeWarpConsole
- Implemented in `TestConsole` and `TestTerminal` with defaults:
  - InputEncoding/OutputEncoding default to Encoding.UTF8
  - IsInputRedirected/IsOutputRedirected/IsErrorRedirected default to false (settable)

### Files changed
- `source/timewarp-terminal/iconsole.cs` - added interface members
- `source/timewarp-terminal/timewarp-console.cs` - implemented properties
- `source/timewarp-terminal/timewarp-terminal.cs` - implemented properties
- `source/timewarp-terminal/test-console.cs` - added test implementation
- `source/timewarp-terminal/test-terminal.cs` - added test implementation
- `tests/console-encoding-01-basic.cs` - new test file (22 tests)

### Test results
- All 22 new tests pass
- All existing tests pass
- Build succeeds with 0 warnings

### Design decisions
- Kept `IsInteractive` on ITerminal (not deprecated) - it's a convenience property that users may prefer over `!IsInputRedirected`
- Redirection properties on test implementations are settable to allow testing different scenarios
