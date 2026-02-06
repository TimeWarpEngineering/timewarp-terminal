# Make terminal Write methods return ITerminal for fluent chaining

## Description

Change Write/WriteLine/WriteTable/WritePanel/WriteRule to return `ITerminal` (or `IConsole`) instead of `void`, enabling fluent chaining:

```csharp
terminal
  .WriteLine("Build Output")
  .WriteRule("Results")
  .WriteTable(t => t
    .AddColumn("Test")
    .AddColumn("Status")
    .AddRow("Unit", "PASSED".Green()))
  .WriteRule()
  .WriteLine("Done");
```

## Checklist

- [ ] Change `IConsole` interface: `Write`, `WriteLine`, `WriteErrorLine` return `IConsole`
- [ ] Change `ITerminal` interface: override return type to `ITerminal` where needed
- [ ] Update `TimeWarpTerminal` implementation
- [ ] Update `TestTerminal` implementation
- [ ] Update `TestConsole` implementation
- [ ] Update `TimeWarpConsole` implementation
- [ ] Change extension methods (`WriteTable`, `WritePanel`, `WriteRule`) to return `ITerminal`
- [ ] Update `Terminal` static class methods
- [ ] Update samples to show fluent chaining
- [ ] Verify build

## Notes

- Extension methods are backward compatible — existing void-ignoring callers still compile
- Interface changes require updating all implementations — breaking change for external implementors
- This is a beta package so breaking changes are acceptable
