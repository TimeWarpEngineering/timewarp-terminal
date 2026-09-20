# Round 1 — merged findings
**Date:** 2026-09-21
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 1 | 0 |
| suggestion | 0 | 0 | 0 |
| nit | 0 | 0 | 0 |

## Issues

### M1 — Severity: bug — Status: fixed
- File: source/timewarp-terminal/terminal-static.cs:1245
- Description: `SyncCancelKeyPressForwarder` (invoked from the `Instance` setter after assigning the process-global `field`) calls `BindCancelKeyPressForwarder(Instance)`. The getter is `TestTerminalContext.Current ?? field`, so an assignment made while an async-local `Use` scope is active rebinds the forwarder to the scoped test terminal instead of the value just assigned. If the forwarder is already bound to that scoped terminal, `BindCancelKeyPressForwarder` early-returns and the process-global assignment is a no-op for the forwarder — contradicting the remarks that “the forwarder follows process-global Instance assignment; async-local TestTerminalContext swaps do not move it.” The rebind test only covers assignment with no AsyncLocal current.
- Suggestion: Pass the assigned process-global terminal into sync (e.g. `SyncCancelKeyPressForwarder(field)` / `BindCancelKeyPressForwarder(assigned)`) and keep first-subscribe binding on the resolved `Instance` if subscribe-inside-`Use` should still attach to the scoped terminal. Add a test that assigns `Terminal.Instance` inside `TestTerminalContext.Use` and asserts the forwarder lands on the assigned process-global instance, not the async-local one.
- Source: general
- Disposition notes: Fixed on this id. `SyncCancelKeyPressForwarder(field)` rebinds to the assigned process-global terminal. Regression in `tests/cancel-key-press-01-basic.cs` (subscribe inside Use, assign Instance, assert forwarder on assigned not scoped).

## Duplicates / conflicts

- None. Parent 029 M11/M12/M13/M17 are closed on this child except the residual M13 rebind hole recorded as M1.
