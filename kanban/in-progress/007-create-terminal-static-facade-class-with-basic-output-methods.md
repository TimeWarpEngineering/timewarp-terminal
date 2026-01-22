# Create Terminal static facade class with basic output methods

## Description

Create a static `Terminal` class that provides a Console-compatible API for easy migration from `Console.WriteLinexxx()` to `Terminal.WriteLinexxx()`. This enables users to simply add `using static TimeWarp.Terminal.Terminal;` and get static access to terminal output without needing to pass instances around.

## Checklist

- [ ] Create `terminal-static.cs` file with static `Terminal` class
- [ ] Add `Instance` property that defaults to `TimeWarpTerminal.Default`
- [ ] Implement basic `Write(string? message)` method
- [ ] Implement `WriteLine(string? message = null)` method
- [ ] Implement async variants: `WriteLineAsync(string? message = null)`
- [ ] Implement `WriteErrorLine(string? message = null)`
- [ ] Implement `WriteErrorLineAsync(string? message = null)`
- [ ] Add input methods: `ReadLine()`, `ReadKey(bool intercept = false)`
- [ ] Add terminal properties: `WindowWidth`, `IsInteractive`, `SupportsColor`, `SupportsHyperlinks`
- [ ] Add terminal operations: `Clear()`, `SetCursorPosition(int left, int top)`, `GetCursorPosition()`
- [ ] Add XML documentation for all public APIs
- [ ] Write unit tests for static methods

## Notes

Reference analysis document: `.agent/workspace/2026-01-22T00-00-00_static-console-api-analysis.md`

### Implementation Plan: Terminal Static Facade Class

#### File Structure
- `terminal-static.cs` in `/source/timewarp-terminal/` - Static Terminal facade class
- `terminal-static.tests.cs` in `/source/timewarp-terminal/` - Unit tests

#### Key Implementation Details

**Terminal Static Class:**
- Static property `Instance` defaults to `TimeWarpTerminal.Default`
- Methods: `Write`, `WriteLine`, `WriteLineAsync`, `WriteErrorLine`, `WriteErrorLineAsync`
- Input methods: `ReadLine()`, `ReadKey(bool intercept = false)`
- Properties: `WindowWidth`, `IsInteractive`, `SupportsColor`, `SupportsHyperlinks`
- Operations: `Clear()`, `SetCursorPosition()`, `GetCursorPosition()`
- All methods route to `Instance.Method()`

**Design Decisions:**
- Null handling: `Write(null!)` converts to empty string
- `ReadKey` default parameter matches Console: `bool intercept = false`
- Single static class (not partial) for clarity

#### Test Strategy
- 30+ unit tests covering all methods and properties
- Tests require: `FluentAssertions` or XUnit.Assert

#### Questions for Implementation
1. Test framework preference (FluentAssertions vs XUnit.Assert)?
2. Test file location (alongside source or dedicated folder)?

#### Next Steps
1. Create `terminal-static.cs`
2. Create test file
3. Run build verification
4. Run tests

---

Example usage after implementation:
```csharp
using static TimeWarp.Terminal.Terminal;

WriteLine("Hello, World!");           // Direct migration from Console.WriteLine
WriteErrorLine("Error occurred!");    // Error output
string? input = ReadLine();           // Input support
Clear();                              // Clear terminal
```
