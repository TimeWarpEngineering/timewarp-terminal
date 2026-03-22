# Complete Console API surface coverage for IConsole/ITerminal

## Description

Goal: **Completely replace the need for `System.Console`** so application code never has to touch it directly. Current API surface is partial—missing significant chunks of the Console API.

This is a parent task. See child tasks for detailed implementation and testing checklists.

## Checklist

- [ ] #020-001: Add stream access APIs to IConsole
- [ ] #020-002: Add encoding and redirection APIs to IConsole
- [ ] #020-003: Add rich input APIs to IConsole
- [ ] #020-004: Add cursor properties to ITerminal
- [ ] #020-005: Add window/buffer geometry to ITerminal
- [ ] #020-006: Add color state APIs to ITerminal
- [ ] #020-007: Add control/utility APIs to ITerminal
- [ ] #020-008: Mirror new APIs on Terminal static class (depends on all above)

## Notes

### Current State Analysis

**IConsole currently exposes:**
- `Write(string)` → `IConsole`
- `WriteLine(string?)` → `IConsole`
- `WriteLineAsync(string?)` → `Task`
- `WriteErrorLine(string?)` → `IConsole`
- `WriteErrorLineAsync(string?)` → `Task`
- `ReadLine()` → `string?`

**ITerminal currently exposes:**
- All IConsole members (with covariant return types)
- `ReadKey(bool intercept)` → `ConsoleKeyInfo`
- `SetCursorPosition(int, int)` / `GetCursorPosition()` → `(int, int)`
- `WindowWidth` → `int`
- `IsInteractive` → `bool`
- `SupportsColor` → `bool`
- `SupportsHyperlinks` → `bool`
- `Clear()` → `void`
- `CancelKeyPress` event

### Design Considerations

1. **Fluent chaining**: All sync Write methods must return the interface type (IConsole/ITerminal) for fluent chaining. Async methods return Task.

2. **Interface inheritance pattern**: `ITerminal : IConsole` uses `new` on sync Write methods for covariant return types. Adding sync Write methods to IConsole requires a `new` override in ITerminal.

3. **Files that must change together**:
   - `iconsole.cs` ↔ `iterminal.cs`
   - `timewarp-terminal.cs`, `timewarp-console.cs`
   - `test-terminal.cs`, `test-console.cs`
   - `terminal-static.cs` (for static facade)

4. **RS0030 analyzer**: Currently flags `Console.OpenStandard*` usage. Once we expose these on IConsole, the analyzer should be satisfied.

### Reference

System.Console API docs: https://learn.microsoft.com/en-us/dotnet/api/system.console

## Coding Standards Reminder

Follow the `/csharp` skill for all implementation work on this task and all child tasks.
