# Round 1 — test-doubles
**Date:** 2026-09-04
**Scope reviewed:** test-terminal.cs, test-console.cs, test-terminal-context.cs

## Summary

Reviewed `TestTerminal`, `TestConsole`, and `TestTerminalContext` against `IConsole`/`ITerminal`, `TimeWarpConsole`/`TimeWarpTerminal`, and the 022 fix list. TestTerminal’s 022 Read/KeyAvailable/QueueKey/Dispose/CancelKeyPress/Clear/thread-safety docs remain intact, and ITerminal `new` + explicit `IConsole` Write pairing is present. New defects are concentrated in TestConsole parity (Dispose still tears down consumer streams; `Read` ignores constructor/`In` input) and in both doubles’ `SetIn`/`SetOut`/`SetError` not redirecting `Write`/`ReadLine`/`Read`, plus AsyncLocal snapshot/`FormatProvider` isolation gaps under forked flows.

## Issues

### Issue 1 — Severity: bug
- File: source/timewarp-terminal/test-console.cs:269
- Description: `TestConsole.Dispose` disposes the current `StandardInputStream` / `StandardOutputStream` / `StandardErrorStream` property values (`test-console.cs:269-271`). Constructor streams are assigned directly to those properties (`test-console.cs:135-137`) with no `Owned*` tracking. After `console.StandardOutputStream = customStream`, disposing the console disposes the consumer’s stream. `TestTerminal` already fixed this pattern in 022 (`test-terminal.cs:639-643` only disposes `Owned*`; regression in `tests/stream-access-01-basic.cs:389-403`), but `TestConsole` was left on the old behavior and has no matching regression test (`tests/stream-access-01-basic.cs:152-180` only assert OpenStandard* identity).
- Suggestion: Mirror TestTerminal — keep constructor `MemoryStream`s in private owned fields, assign them to the public properties initially, and Dispose only the owned instances. Add a TestConsole regression parallel to the TestTerminal one.
- Status: open

### Issue 2 — Severity: bug
- File: source/timewarp-terminal/test-console.cs:189
- Description: `TestConsole.Read` only drains `CharacterQueue` and returns `-1` when empty (`test-console.cs:189-197`); it never reads constructor `InputReader` / `In`. So `new TestConsole("abc").Read()` is `-1` while `ReadLine()` returns `"abc"`, and `console.In.Read()` diverges from `console.Read()`. `TimeWarpConsole.Read` uses `Console.Read()` (`timewarp-console.cs:51-52`), which shares the input stream with `ReadLine`. `TestTerminal.Read` was fixed in 022 to fall back to `InputReader` (`test-terminal.cs:242-250`, covered by `tests/rich-input-01-basic.cs:253-262`); TestConsole still has the pre-fix dual-source behavior. `IConsole.Read` documents reading “from the standard input stream” (`iconsole.cs:77-81`).
- Suggestion: When `CharacterQueue` is empty, fall back to `InputReader.Read()` (same shared-source rule as TestTerminal). Keep `QueueCharacters` as an overlay preferred over unread constructor input. Add constructor-input / interleave tests for TestConsole analogous to the TestTerminal rich-input cases.
- Status: open

### Issue 3 — Severity: bug
- File: source/timewarp-terminal/test-terminal.cs:134
- Description: `SetIn` / `SetOut` / `SetError` only reassign the `In` / `Out` / `Error` properties (`test-terminal.cs:134-143`; same on `test-console.cs:108-117`). `Write` / `WriteLine` / `WriteLineAsync` always write to the private `OutputWriter` (`test-terminal.cs:200-203`, `test-console.cs:156-159`); `WriteErrorLine*` always write to `ErrorWriter`; `ReadLine` / `Read` use `InputReader` / queues, not `In`. After `SetOut(custom)`, `WriteLine("x")` still lands in the capture writer while `Out` points elsewhere — opposite of `TimeWarpConsole`/`TimeWarpTerminal`, where `SetOut` → `Console.SetOut` and subsequent `Write` goes to the new writer (`timewarp-console.cs:99-108`, `timewarp-terminal.cs:127-135`). Task 020-001’s design note said Set* “update the internal readers/writers,” but the I/O methods never consult those properties. Existing tests only assert property identity after Set* (`tests/stream-access-01-basic.cs:114-132`, `tests/terminal-static-08-new-apis.cs:184-198`), not that Write/ReadLine follow the redirect.
- Suggestion: Route `Write*`/`WriteErrorLine*`/`ReadLine`/`Read` through `Out`/`Error`/`In` (constructor capture writers remain the defaults). Keep `Output`/`ErrorOutput` reading the original capture writers, or tee into them, and document the chosen capture semantics. Add tests: `SetOut` then `WriteLine` appears on the new writer; `SetIn` then `ReadLine` reads the new reader.
- Status: open

### Issue 4 — Severity: suggestion
- File: source/timewarp-terminal/test-terminal-context.cs:109
- Description: `ClearCurrent` restores process-global `Terminal.FormatProvider` from the snapshot (`test-terminal-context.cs:107-109`), while `Context` itself is AsyncLocal-isolated. Parallel `Use` scopes that assign `FormatProvider` race on that static: one flow’s restore can clobber another’s in-scope value or restore a snapshot captured after a sibling mutation. `Terminal.FormatProvider` is documented as process-global (`terminal-static.cs:86-88`), but `TestTerminalContext` advertises parallel-safe scopes (`test-terminal-context.cs:10-13`) and always pairs FormatProvider snapshot/restore with context push/pop (`test-terminal-context.cs:79-87`). The parallel isolation test (`tests/test-terminal-context-01-integration.cs:145-179`) does not touch FormatProvider. Separately, `SnapshotStack` holds a mutable `Stack<ContextSnapshot>` in AsyncLocal (`test-terminal-context.cs:48,136-145`); `Task.Run` from inside an active `Use` forks an ExecutionContext that shares the same Stack instance, so concurrent nested `Use`/`ClearCurrent` can corrupt push/pop ordering (sibling `Task.Run` from a clean parent, as in the existing parallel test, each allocate their own stack and are fine).
- Suggestion: Document that FormatProvider snapshot/restore is serial-only, or store FormatProvider in AsyncLocal like `Current`. For the stack, prefer an immutable linked snapshot (replace `SnapshotStack.Value` on each push) so forked execution contexts do not share mutable stack state.
- Status: open

### Issue 5 — Severity: suggestion
- File: source/timewarp-terminal/test-terminal.cs:286
- Description: When `ReadKey` synthesizes keys from constructor line input, it enqueues `ConsoleKeyInfo` with `shift: false` for every character (`test-terminal.cs:286-290`), including uppercase letters. `QueueKeys` correctly sets shift for ASCII uppercase (`test-terminal.cs:548-549`, covered by `tests/rich-input-01-basic.cs:342`). Callers that branch on `keyInfo.Modifiers` therefore see different modifier flags for `"A"` depending on whether it came from `QueueKeys("A")` versus `new TestTerminal("A"); ReadKey()`. KeyChar itself is correct in both paths.
- Suggestion: When synthesizing from constructor input, set `shift: char.IsAsciiLetterUpper(c)` (same rule as `QueueKeys`). Add a regression asserting `ReadKey` on constructor `"A"` yields `ConsoleModifiers.Shift`.
- Status: open

### Issue 6 — Severity: nit
- File: source/timewarp-terminal/test-console.cs:220
- Description: `TestConsole.Clear()` discards captured stdout/stderr (`test-console.cs:220-224`), i.e. TestTerminal’s `ClearOutput` semantics. `TestTerminal.Clear()` (ITerminal) appends a `[CLEAR]` marker and preserves history (`test-terminal.cs:418-419`). Same method name, opposite meaning when switching doubles — easy to misuse in tests.
- Suggestion: Rename TestConsole’s helper to `ClearOutput` (parity with TestTerminal) or document the divergence on both APIs.
- Status: open
