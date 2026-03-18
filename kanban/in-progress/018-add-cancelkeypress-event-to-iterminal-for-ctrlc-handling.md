# Add CancelKeyPress event to ITerminal for Ctrl+C handling

**GitHub Issue:** https://github.com/TimeWarpEngineering/timewarp-terminal/issues/18

## Description

Add a `CancelKeyPress` event to `ITerminal` interface to allow graceful Ctrl+C handling without using `System.Console` directly. This enables testability and removes banned API usage in consumers like TimeWarp.Nuru's REPL.

## Problem

The REPL in TimeWarp.Nuru needs to handle Ctrl+C gracefully. Currently, it must use `Console.CancelKeyPress` directly, which violates the banned API rule and breaks testability.

```csharp
// Current code in repl-session.cs
Console.CancelKeyPress += OnCancelKeyPress;
Console.CancelKeyPress -= OnCancelKeyPress;
```

This triggers RS0030 warnings because `System.Console` is banned in favor of `ITerminal`.

## Checklist

- [ ] Add `CancelKeyPress` event to `ITerminal` interface
- [ ] Implement event in `TimeWarpConsole` (delegate to `Console.CancelKeyPress`)
- [ ] Implement event in `TestConsole` (allow test simulation)
- [ ] Add unit tests for event behavior
- [ ] Update any relevant documentation
- [ ] Verify TimeWarp.Nuru REPL can use the new event

## Notes

### Proposed Interface Change

```csharp
public interface ITerminal
{
  // Existing members...
  
  /// <summary>
  /// Occurs when the Ctrl+C key combination is pressed.
  /// </summary>
  event ConsoleCancelEventHandler? CancelKeyPress;
}
```

### Use Case

```csharp
// In ReplSession
Terminal.CancelKeyPress += OnCancelKeyPress;

// Cleanup
Terminal.CancelKeyPress -= OnCancelKeyPress;
```

### Benefits

1. **Testability** - TestConsole can simulate Ctrl+C events
2. **Consistency** - All console interactions go through ITerminal
3. **Removes banned API usage** - REPL code no longer needs to use Console directly

### Related

- Discovered while fixing banned API warnings in timewarp-nuru
- Affects `ReplSession` class in timewarp-nuru

### Files to Modify

- `source/timewarp-terminal/ITerminal.cs` - Add event to interface
- `source/timewarp-terminal/TimeWarpConsole.cs` - Implement event
- `tests/timewarp-terminal.tests/` - Add tests for event behavior
