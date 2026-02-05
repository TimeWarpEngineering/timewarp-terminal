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

Current test pattern:
```csharp
using TestTerminal terminal = new();
TestTerminalContext.Current = terminal;
await MyApp.RunAsync();
Assert.Contains("expected output", terminal.Output);
```

New test pattern with Terminal static API:
```csharp
using TestTerminal terminal = new();
TestTerminalContext.Current = terminal;

// Terminal static methods now route to test terminal
Terminal.WriteLine("Hello");
Assert.Contains("Hello", terminal.Output);

// Or directly use Terminal.Instance
Terminal.Instance = terminal;
Terminal.WriteLine("Direct");
Assert.Contains("Direct", terminal.Output);
```

This task ensures backward compatibility while adding support for the new static API.
