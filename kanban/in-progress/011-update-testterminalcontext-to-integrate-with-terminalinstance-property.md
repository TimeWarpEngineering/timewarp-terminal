# Update TestTerminalContext to integrate with Terminal.Instance property

## Description

Update the existing `TestTerminalContext` to provide seamless integration with the new `Terminal.Instance` static property. This enables users to use the ambient test context pattern while also having access to the static `Terminal` class methods in tests.

## Checklist

- [ ] Add `Terminal` property to `TestTerminalContext` that returns `TestTerminal`
- [ ] Modify `TestTerminalContext.Resolve(ITerminal? terminal)` to check `Terminal.Instance` first
- [ ] Update `TestTerminalContext` to set `Terminal.Instance = Context.Terminal` on initialization
- [ ] Restore previous `Terminal.Instance` on disposal (for proper test isolation)
- [ ] Update XML documentation with test usage patterns
- [ ] Write integration tests demonstrating the test pattern
- [ ] Update existing tests to use the new static API

## Notes

### Current Status (2026-03-19)

**What exists:**
- `TestTerminalContext` class in `source/timewarp-terminal/test-terminal-context.cs`
  - Uses `AsyncLocal<TestTerminal?>` for test isolation
  - Has `Current` property and `Resolve()` methods
  - **NOT integrated with `Terminal.Instance`**
- `Terminal` static class in `source/timewarp-terminal/terminal-static.cs`
  - Has `Instance` property that can be set to any `ITerminal`
  - **Does NOT check `TestTerminalContext.Current`**

**Current test pattern (working):**
```csharp
ITerminal original = Terminal.Instance;
using TestTerminal testTerminal = new();
try
{
  Terminal.Instance = testTerminal;
  // test code
}
finally
{
  Terminal.Instance = original;
}
```

**Proposed pattern (cleaner):**
```csharp
using TestTerminal terminal = new();
TestTerminalContext.Current = terminal;
// Terminal.Instance automatically set
// Terminal.WriteLine routes to testTerminal
// Automatic restoration on dispose
```

### Why This Task

The integration would provide:
1. **Cleaner test code** - No manual try/finally blocks
2. **Automatic restoration** - `Terminal.Instance` restored when context disposed
3. **AsyncLocal isolation** - Each async context gets its own terminal
4. **Backward compatibility** - Existing pattern still works

### Files to Modify

- `source/timewarp-terminal/test-terminal-context.cs` - Add integration logic
- `tests/*.cs` - Optionally update to use new pattern (low priority)
