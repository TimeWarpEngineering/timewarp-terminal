# Round 2 — merged findings
**Date:** 2026-09-21
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 1 | 0 |
| suggestion | 0 | 0 | 0 |
| nit | 0 | 0 | 0 |

Final counts = round-1 IDs after re-verify (no new findings).

## Issues

### M1 — Severity: bug — Status: fixed
- File: source/timewarp-terminal/terminal-static.cs:1245
- Description: `SyncCancelKeyPressForwarder` rebound via the AsyncLocal-aware `Instance` getter, so process-global assignment inside `Use` pinned the forwarder to the scoped terminal (or no-op if already bound there).
- Suggestion: Pass the assigned `field` into sync; keep first-subscribe on resolved `Instance`; add assign-inside-Use regression.
- Source: general
- Disposition notes: `SyncCancelKeyPressForwarder(field)` at `terminal-static.cs:94`; helper binds `processGlobalTerminal` at `:1236-1245`. First-subscribe still uses `Instance` at `:1113`. Regression in `tests/cancel-key-press-01-basic.cs:180-216`.

## Duplicates / conflicts

- None.
