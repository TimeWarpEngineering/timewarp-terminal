# Round 2 — general
**Date:** 2026-09-21
**Scope reviewed:** post-fix delta `87489b5` vs round-1 (terminal-static.cs, tests/cancel-key-press-01-basic.cs) plus re-verify M1

## Summary

Commit `87489b5` correctly fixes M1 by passing the assigned process-global `field` into `SyncCancelKeyPressForwarder` instead of resolving through the AsyncLocal-aware `Instance` getter. First-subscribe still binds via `Instance`, so subscribe-inside-`Use` remains scoped; process-global assignment rebinds to the assigned terminal even while `Current` is set. The new regression asserts assigned fires, scoped does not, and the forwarder survives `Use` ending. No new defects found on the delta.

## Resolved prior

- **M1** — fixed — `terminal-static.cs:94` setter calls `SyncCancelKeyPressForwarder(field)`; `terminal-static.cs:1236-1245` binds `processGlobalTerminal` rather than `Instance`; first-subscribe at `terminal-static.cs:1113` still uses resolved `Instance`. Regression at `tests/cancel-key-press-01-basic.cs:180-216` covers subscribe-inside-Use → assign → assigned fires / scoped does not → still fires after Use ends.

## Issues

## Smoke tests

- `dotnet tests/cancel-key-press-01-basic.cs` — pass
