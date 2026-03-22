# Complete Console API surface coverage for IConsole/ITerminal

## Description

Goal: **Completely replace the need for `System.Console`** so application code never has to touch it directly. Current API surface is partial—missing significant chunks of the Console API.

## Checklist

### Phase 1: Core Stream Access (IConsole)
- [ ] Add `OpenStandardInput()` → `Stream`
- [ ] Add `OpenStandardOutput()` → `Stream`
- [ ] Add `OpenStandardError()` → `Stream`
- [ ] Add `In` → `TextReader` property
- [ ] Add `Out` → `TextWriter` property
- [ ] Add `Error` → `TextWriter` property
- [ ] Add `SetIn(TextReader)` method
- [ ] Add `SetOut(TextWriter)` method
- [ ] Add `SetError(TextWriter)` method

### Phase 2: Encoding (IConsole)
- [ ] Add `InputEncoding` get/set property
- [ ] Add `OutputEncoding` get/set property

### Phase 3: Redirection State (IConsole)
- [ ] Add `IsInputRedirected` property
- [ ] Add `IsOutputRedirected` property
- [ ] Add `IsErrorRedirected` property
- [ ] Consider deprecating `IsInteractive` in favor of explicit `!IsInputRedirected`

### Phase 4: Rich Input (IConsole)
- [ ] Add `Read()` → `int` (reads single character)
- [ ] Add `ReadKey()` overload without parameter (defaults to intercept: false)

### Phase 5: Cursor Properties (ITerminal)
- [ ] Add `CursorLeft` get/set property (currently only have method pair)
- [ ] Add `CursorTop` get/set property (currently only have method pair)
- [ ] Add `CursorVisible` get/set property
- [ ] Add `CursorSize` get/set property

### Phase 6: Window/Buffer Geometry (ITerminal)
- [ ] Add `WindowHeight` property
- [ ] Add `WindowLeft` property
- [ ] Add `WindowTop` property
- [ ] Add `BufferWidth` property
- [ ] Add `BufferHeight` property
- [ ] Add `SetWindowSize(int width, int height)` method
- [ ] Add `SetWindowPosition(int left, int top)` method
- [ ] Add `SetBufferSize(int width, int height)` method
- [ ] Add `MoveBufferArea(...)` method
- [ ] Add `LargestWindowWidth` property
- [ ] Add `LargestWindowHeight` property

### Phase 7: Color State (ITerminal)
- [ ] Add `ForegroundColor` get/set property (ConsoleColor)
- [ ] Add `BackgroundColor` get/set property (ConsoleColor)
- [ ] Add `ResetColor()` method
- [ ] Note: Current ANSI extension methods are for inline styling; these are for terminal state

### Phase 8: Control/Utility (ITerminal)
- [ ] Add `Beep()` method
- [ ] Add `Beep(int frequency, int duration)` overload
- [ ] Add `TreatControlCAsInput` get/set property
- [ ] Add `Title` get/set property
- [ ] Add `KeyAvailable` property

### Phase 9: Static Terminal Facade
- [ ] Mirror all new IConsole members on `Terminal` static class
- [ ] Mirror all new ITerminal members on `Terminal` static class

### Phase 10: Test Implementations
- [ ] Update `TestConsole` to implement all new IConsole members
- [ ] Update `TestTerminal` to implement all new ITerminal members
- [ ] Add mock stream support to test implementations
- [ ] Add mock encoding support to test implementations

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

4. **Test implementation challenges**:
   - `OpenStandard*()` methods return `Stream` - test implementations need mock streams
   - `In`/`Out`/`Error` are `TextReader`/`TextWriter` - test implementations need mock readers/writers
   - Encoding properties need test defaults
   - Window/buffer geometry needs sensible test defaults

5. **RS0030 analyzer**: Currently flags `Console.OpenStandard*` usage. Once we expose these on IConsole, the analyzer should be satisfied.

### Reference

System.Console API docs: https://learn.microsoft.com/en-us/dotnet/api/system.console
