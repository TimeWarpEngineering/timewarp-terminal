# Fix TestConsole and TestTerminal parity (Dispose, Read, Set, FormatProvider)

## Description

Parent **029** round-1 merged findings **M1, M2, M3, M4, M14, M18**.

Mirror the 022 TestTerminal fixes that never landed on `TestConsole`, make `SetIn`/`SetOut`/`SetError` actually redirect I/O on both doubles, and stop `TestTerminalContext` from racing process-global `Terminal.FormatProvider` (and sharing a mutable AsyncLocal stack) under parallel `Use` scopes.

Do **not** create a sibling “apply 029 findings” task. This child is the product-fix batch.

## Requirements

### M1 — bug — `TestConsole.Dispose` tears down consumer streams
- File: `source/timewarp-terminal/test-console.cs:269`
- Mirror `TestTerminal` (`test-terminal.cs:639-643`): dispose only constructor-owned `MemoryStream`s.
- Add a TestConsole regression parallel to `tests/stream-access-01-basic.cs` TestTerminal case.

### M2 — bug — `TestConsole.Read` ignores constructor / `In` input
- File: `source/timewarp-terminal/test-console.cs:189`
- When `CharacterQueue` is empty, fall back to `InputReader.Read()` (same shared-source rule as TestTerminal `:242-250`).
- Add constructor-input / interleave tests.

### M3 — bug — `SetIn`/`SetOut`/`SetError` do not redirect `Write`/`ReadLine`/`Read`
- Files: `source/timewarp-terminal/test-terminal.cs:134-143`, `test-console.cs:108-117`
- Route I/O through `Out`/`Error`/`In` (tee into capture writers if `Output`/`ErrorOutput` must keep working).
- Tests: `SetOut` then `WriteLine` appears on the new writer; `SetIn` then `ReadLine` reads the new reader.

### M4 — bug — `FormatProvider` process-global vs AsyncLocal restore
- Files: `source/timewarp-terminal/terminal-static.cs:90`, `test-terminal-context.cs:85-86,109,48,136-145`
- Make `FormatProvider` AsyncLocal (resolve like `Instance`), **or** stop snapshot/restoring it and document serial-only mutation.
- Prefer an immutable linked snapshot for `SnapshotStack` so `Task.Run` inside `Use` does not share a mutable `Stack`.
- Add a parallel FormatProvider isolation test (the existing parallel test does not touch FormatProvider).

### M14 — suggestion — `ReadKey` constructor-input omits shift
- File: `source/timewarp-terminal/test-terminal.cs:286-290`
- Set `shift: char.IsAsciiLetterUpper(c)` when synthesizing from constructor input (same rule as `QueueKeys`).

### M18 — nit — `TestConsole.Clear` vs `TestTerminal.Clear`
- File: `source/timewarp-terminal/test-console.cs:220`
- Rename TestConsole’s helper to `ClearOutput` **or** document the divergence on both APIs.

## Checklist

- [ ] M1 TestConsole Dispose only owned streams + regression
- [ ] M2 TestConsole.Read falls back to constructor/`In` + tests
- [ ] M3 SetIn/SetOut/SetError redirect Write/ReadLine/Read + tests
- [ ] M4 FormatProvider isolation (AsyncLocal or documented serial-only) + parallel test; SnapshotStack not shared across forks
- [ ] M14 ReadKey constructor uppercase sets Shift
- [ ] M18 Clear naming/docs
- [ ] `./bin/dev test` (or the repo test command) green for the touched runfiles

## Notes

- Parent: `kanban/in-progress/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already fixed TestTerminal Dispose/Read; do not regress those.
- Lax cursor/window arg validation on TestTerminal stays accepted (022).

## Session

- Created: 3361369 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
