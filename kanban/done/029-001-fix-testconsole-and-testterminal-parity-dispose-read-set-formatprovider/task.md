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

- [x] M1 TestConsole Dispose only owned streams + regression
- [x] M2 TestConsole.Read falls back to constructor/`In` + tests
- [x] M3 SetIn/SetOut/SetError redirect Write/ReadLine/Read + tests
- [x] M4 FormatProvider isolation (AsyncLocal or documented serial-only) + parallel test; SnapshotStack not shared across forks
- [x] M14 ReadKey constructor uppercase sets Shift
- [x] M18 Clear naming/docs
- [x] `./bin/dev test` (or the repo test command) green for the touched runfiles
- [x] Effort-1 implementation review (round 1 general) + disposition on this id

## Notes

- Parent: `kanban/done/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already fixed TestTerminal Dispose/Read; do not regress those.
- Lax cursor/window arg validation on TestTerminal stays accepted (022).

## Session

- Created: 3361369 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
- Implementer: Grok `01a0a236-27b6-7b13-a0f9-0b2f6578c348` (2026-09-15)
- Review oracle: Grok `01a0a243-c399-7711-b5e7-0ea48c6a87f0` (2026-09-15); general reviewer `01a0a245-1d44-7970-9cdd-5aaad14b7346`

## Results

Landed parent **029** round-1 findings **M1, M2, M3, M4, M14, M18** on this child. No sibling “apply findings” task.

### What was implemented

- **M1** — `TestConsole.Dispose` disposes only constructor-owned `MemoryStream`s (`OwnedStandard*` fields), matching `TestTerminal`. Consumer-assigned `Standard*Stream` replacements stay usable.
- **M2** — `TestConsole.Read` drains `CharacterQueue` then falls back to `In.Read()`, so constructor input and `SetIn` share one source. Interleave with `ReadLine` matches TestTerminal.
- **M3** — `Write*` / `ReadLine` / `Read` on both doubles route through `Out` / `Error` / `In`. Capture writers are teed so `Output` / `ErrorOutput` still work after `SetOut` / `SetError`. `TestTerminal.ReadKey` / `KeyAvailable` also consult `In`.
- **M4** — `Terminal.FormatProvider` is an async-local override while `TestTerminalContext` is active, else process-global. `TestTerminalContext` snapshots that override (not the process-global static) on an immutable linked `SnapshotHead`, so `Task.Run` inside `Use` does not share a mutable `Stack`.
- **M14** — Constructor-input `ReadKey` synthesis sets `shift: char.IsAsciiLetterUpper(c)`, same as `QueueKeys`.
- **M18** — Added `TestConsole.ClearOutput`. Kept `Clear` as a documented alias (package is already 1.0.1; a hard rename would break callers). Divergence from `TestTerminal.Clear` (`[CLEAR]` marker) is documented on both APIs.

### Files changed

- `source/timewarp-terminal/test-console.cs`
- `source/timewarp-terminal/test-terminal.cs`
- `source/timewarp-terminal/terminal-static.cs`
- `source/timewarp-terminal/test-terminal-context.cs`
- `tests/stream-access-01-basic.cs`
- `tests/rich-input-01-basic.cs`
- `tests/test-terminal-context-01-integration.cs`
- `skills/terminal/SKILL.md`

### Key decisions

- Tee capture writers after `SetOut`/`SetError` so existing `Output` assertions keep working.
- Route `Read`/`ReadLine` through `In` (not only the constructor `InputReader`) so M2 and M3 are both true.
- `Clear` stays as an alias of `ClearOutput` on `TestConsole` rather than a breaking rename.
- FormatProvider process-global assignment outside a context is unchanged for serial tests (`Terminal.Instance = …` style).

### Test outcomes

All **33** `tests/*.cs` runfiles passed (this worktree has no `./bin/dev` binary; the suite was run with `dotnet tests/<file>.cs`, which is what `tools/dev-cli/endpoints/test.cs` does). Touched runfiles: stream-access 36 passed, rich-input 25 passed, test-terminal-context 12 passed, plus terminal-static-04-format and terminal-control-utilities.

### How to validate

**Smoke**

```bash
dotnet tests/stream-access-01-basic.cs
dotnet tests/rich-input-01-basic.cs
dotnet tests/test-terminal-context-01-integration.cs
```

Full suite (same as `dev test`):

```bash
for f in tests/*.cs; do dotnet "$f" || exit 1; done
```

**Expect**

- Each command exits 0.
- Stream-access reports ConsoleStreamAccess **17** passed and TerminalStreamAccess **19** passed, including `Should_not_dispose_consumer_assigned_standard_output_stream_in_test_console`, `Should_write_to_set_out_writer_in_test_*`, and `Should_read_line_from_set_in_reader_in_test_*`.
- Rich-input reports ConsoleReadBasic **8** passed and TerminalReadBasic **17** passed, including constructor-input `Read` interleave and `Should_set_shift_flag_when_readkey_synthesizes_constructor_uppercase`.
- TestTerminalContextIntegration reports **12** passed, including `Should_isolate_format_provider_across_parallel_use_scopes` and `Should_not_share_snapshot_stack_across_forked_use_scopes`.
- Full suite: `Ran 33 file(s).` / `All test files passed.`

**Automated gate**

```bash
# from repo root; ./bin/dev is absent until `dotnet run tools/dev-cli/dev.cs -- self-install`
for f in tests/*.cs; do dotnet "$f" || exit 1; done
```

**Not in scope:** live console / `TimeWarpTerminal.Default` I/O; `gh pr create` and `ganda kanban done` (later host nodes).

### Review disposition

- **Rounds:** 1
- **Effort / roster:** 1 — general only
- **Final counts:** 0 open / 0 fixed / 0 wontfix (bug, suggestion, nit all zero)
- **Outcome:** `clean` (no issues raised; parent M1, M2, M3, M4, M14, M18 re-verified landed)
- **Paths:**
  - `review/review-framework.md`
  - `review/round-1/general.md`
  - `review/round-1/merged.md`
  - `review/disposition.md`
- Review-session smoke: stream-access 36 passed, rich-input 25 passed, test-terminal-context 12 passed. Unstaged `.gitignore` was out of scope.
