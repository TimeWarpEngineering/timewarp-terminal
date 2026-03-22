# Add encoding and redirection APIs to IConsole

## Description

Add encoding and redirection state properties to `IConsole` to match `System.Console` capabilities. This enables code to detect and control text encoding and check if streams are redirected.

Parent: #020

## Checklist

### Implementation
- [ ] Add `InputEncoding` get/set property to `IConsole`
- [ ] Add `OutputEncoding` get/set property to `IConsole`
- [ ] Add `IsInputRedirected` property to `IConsole`
- [ ] Add `IsOutputRedirected` property to `IConsole`
- [ ] Add `IsErrorRedirected` property to `IConsole`
- [ ] Implement in `TimeWarpConsole`
- [ ] Implement in `TimeWarpTerminal`
- [ ] Consider deprecating `IsInteractive` on `ITerminal` in favor of explicit `!IsInputRedirected`

### Testing
- [ ] Add `TestConsole` implementations for all new members
- [ ] Add `TestTerminal` implementations for all new members
- [ ] Add mock encoding support to test implementations (default to UTF-8)
- [ ] Add redirection state properties to test implementations (default to false)
- [ ] Write unit tests for `InputEncoding` get/set
- [ ] Write unit tests for `OutputEncoding` get/set
- [ ] Write unit tests for `IsInputRedirected`
- [ ] Write unit tests for `IsOutputRedirected`
- [ ] Write unit tests for `IsErrorRedirected`

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
