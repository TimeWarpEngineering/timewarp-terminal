# Round 1 — static-facade
**Date:** 2026-09-04
**Scope reviewed:** terminal-static.cs, ansi-*.cs, terminal-hyperlink-extensions.cs

## Summary

Re-reviewed the static `Terminal` facade, ANSI color helpers, and OSC 8 hyperlink paths against the 022 fixes and the current 1.0.1 tree. `Terminal.Instance` AsyncLocal resolution, SupportsColor / SupportsHyperlinks gating on colored Write and WriteLink paths, FormatProvider current-culture defaults, Dark*/bright SGR mapping, and the ITerminal surface mirror all look intact. Three new defects remain: FormatProvider is still a process-global mutated from AsyncLocal scopes (parallel restore clobber), `CancelKeyPress` add/remove bind only to the Instance resolved at subscribe time, and `SanitizeUrl` still leaves C1 STRING TERMINATOR (`U+009C`) unsanitized in the OSC 8 payload.

## Issues

### Issue 1 — Severity: bug
- File: source/timewarp-terminal/terminal-static.cs:90
- Description: `Terminal.FormatProvider` is a process-global static, while `TestTerminalContext.SetCurrent` / `ClearCurrent` snapshot and restore it from an AsyncLocal stack (`test-terminal-context.cs:85-86`, `test-terminal-context.cs:109`). After 022 made `Instance` truly parallel-isolated via AsyncLocal, FormatProvider is the remaining shared mutable that parallel `Use` scopes can clobber: flow A disposing its scope writes `FormatProvider` back to its snapshot and can wipe flow B’s in-progress provider (or the reverse). Facade format overloads read that same static via `ActiveFormatProvider` (`terminal-static.cs:92`, `terminal-static.cs:320-321`). Serial restore works (`tests/test-terminal-context-01-integration.cs` restore test); there is no parallel FormatProvider isolation test analogous to `Should_isolate_parallel_contexts`.
- Suggestion: Either make FormatProvider AsyncLocal (resolve like `Instance`), or stop snapshot/restoring it from `TestTerminalContext` and document that mutating `FormatProvider` is serial-only startup configuration with no parallel safety.
- Status: open

### Issue 2 — Severity: bug
- File: source/timewarp-terminal/ansi-hyperlink-extensions.cs:71
- Description: `SanitizeUrl` percent-encodes only C0 controls (`< U+0020`) and DEL (`U+007F`). ECMA-48 / xterm also accept C1 STRING TERMINATOR `U+009C` (`\x9c`) as an OSC terminator. A URL containing `\x9c` is embedded verbatim in the OSC 8 payload (`CreateLink` at `ansi-hyperlink-extensions.cs:50-51`), so an attacker-influenced URL can still terminate the hyperlink sequence early on terminals that honor C1 ST. 022 closed ESC/BEL/C0 injection; this C1 path remains. Static `WriteLink` / `WriteLinkLine` and `ITerminal` extensions all route through `CreateLink`, so they inherit the gap.
- Suggestion: Extend `SanitizeUrl` to percent-encode the C1 controls that can terminate or introduce OSC/APC/PM/DCS (at minimum `U+009C` ST; consider the full C1 set `U+0080`–`U+009F`), and add a regression test beside the existing ESC/BEL case in `tests/hyperlink-01-basic.cs`.
- Status: open

### Issue 3 — Severity: suggestion
- File: source/timewarp-terminal/terminal-static.cs:1037-1041
- Description: `Terminal.CancelKeyPress` forwards `add`/`remove` to whatever `Instance` resolves to at that moment. Because `Instance` prefers `TestTerminalContext.Current` over the process-global field (`terminal-static.cs:71`), a handler subscribed inside a `Use` scope is attached to the test terminal, but unsubscribing after the scope ends (or after `Terminal.Instance = …` swap) removes from a different instance. The handler then leaks on the original instance (and `remove` is a no-op on the new one). Production use with a stable `TimeWarpTerminal.Default` is fine; the footgun appears with the AsyncLocal/`Instance` swap model 022 introduced. Current tests only cover add/remove while Instance is held constant (`tests/cancel-key-press-01-basic.cs`).
- Suggestion: Keep a facade-level handler list and attach a single forwarder to the current Instance (reattach on Instance/context change), or document that `Terminal.CancelKeyPress` subscribe/unsubscribe must occur against a stable Instance with no intervening context clear.
- Status: open
