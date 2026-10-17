# Round 1 — merged findings
**Date:** 2026-10-17
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 1 | 0 |
| suggestion | 0 | 0 | 0 |
| nit | 0 | 0 | 0 |

## Issues

### M1 — Severity: bug — Status: fixed
- File: readme.md:7
- Description: The README says a Nuru handler can take `ITerminal` or `IConsole`. Nuru 3.0.0 injects only `ITerminal` as a built-in. Its service validator and resolver have no `IConsole` entry, so an `IConsole` handler parameter gets an unregistered-service diagnostic and does not receive the `UseTerminal` instance. This is a claim the current code does not support.
- Suggestion: Reword the sentence so a Nuru handler takes `ITerminal`, which also implements `IConsole`, or drop `IConsole` from it.
- Source: general
- Disposition notes: Reworded the intro so a Nuru handler takes `ITerminal`, which also implements `IConsole`. The sentence no longer says a handler can take `IConsole`. Line 82 still points `TestConsole` at code that only needs `IConsole` outside Nuru injection. Checked Nuru 3.0.0 `TimeWarp.Nuru.Analyzers.dll`: UTF-16 `TimeWarp.Terminal.ITerminal` is present and `IConsole` is absent. `ITerminal : IConsole` is `source/timewarp-terminal/iterminal.cs`.

## Duplicates / conflicts

- None (single reviewer).
