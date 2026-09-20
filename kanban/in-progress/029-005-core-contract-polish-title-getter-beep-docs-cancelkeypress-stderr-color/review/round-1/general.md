# Round 1 — general
**Date:** 2026-09-21
**Scope reviewed:** branch `task/029-005-core-contract-polish-title-getter-beep-docs-cancel` vs `origin/master` (product commit `db3e526`)

## Summary

Product commit `db3e526` closes the four parent-029 minors: Windows `Title` getter now swallows `IOException`, `Beep()` remarks match Windows redirected kernel beep, static `CancelKeyPress` uses a facade handler list plus one forwarder, and colored `WriteErrorLine` also requires `!IsErrorRedirected`. Overall risk is low and 022 contracts (Title setter, parameterless Beep, event existence, stdout `SupportsColor`) are intact. One residual defect remains: process-global `Instance` rebind of the CancelKeyPress forwarder resolves through the AsyncLocal-aware getter and can pin to the wrong terminal.

## Issues

### Issue 1 — Severity: bug
- File: source/timewarp-terminal/terminal-static.cs:1245
- Description: `SyncCancelKeyPressForwarder` (invoked from the `Instance` setter after assigning the process-global `field`) calls `BindCancelKeyPressForwarder(Instance)`. The getter is `TestTerminalContext.Current ?? field`, so an assignment made while an async-local `Use` scope is active rebinds the forwarder to the scoped test terminal instead of the value just assigned. If the forwarder is already bound to that scoped terminal, `BindCancelKeyPressForwarder` early-returns and the process-global assignment is a no-op for the forwarder — contradicting the remarks that “the forwarder follows process-global Instance assignment; async-local TestTerminalContext swaps do not move it.” The new rebind test only covers assignment with no AsyncLocal current, so this path is untested.
- Suggestion: Pass the assigned process-global terminal into sync (e.g. `SyncCancelKeyPressForwarder(field)` / `BindCancelKeyPressForwarder(assigned)`) and keep first-subscribe binding on the resolved `Instance` if subscribe-inside-`Use` should still attach to the scoped terminal. Add a test that assigns `Terminal.Instance` inside `TestTerminalContext.Use` and asserts the forwarder lands on the assigned process-global instance, not the async-local one.
- Status: open

## Parent findings coverage

- **M11** — yes — `timewarp-terminal.cs:544-553` Windows getter `try`/`catch (IOException)` returns `string.Empty`; `iterminal.cs:316-318` remarks document the empty-string fallback when the Windows getter fails
- **M12** — yes — `iterminal.cs:273-277` remarks distinguish Unix redirected no-op vs Windows kernel beep; `timewarp-terminal.cs:480-492` still only swallows `IOException` (no `IsOutputRedirected` gate), matching the chosen remarks-only fix
- **M13** — yes (add/remove mismatch fixed; residual Issue 1) — facade list + single forwarder at `terminal-static.cs:1104-1127` / `1225-1270`; unsubscribe-after-`Use` covered by `tests/cancel-key-press-01-basic.cs:147-173`. Process-global rebind path still wrong under AsyncLocal (Issue 1)
- **M17** — yes — colored `WriteErrorLine` gates at `terminal-static.cs:325` and `:355` require `SupportsColor && !IsErrorRedirected` before wrapping with `AnsiColors`; stdout colored `Write`/`WriteLine` at `:189`, `:218`, `:249`, `:278` still gate only on `SupportsColor`; `tests/terminal-static-06-color.cs:399-431` asserts no SGR on redirected stderr while stdout color remains
