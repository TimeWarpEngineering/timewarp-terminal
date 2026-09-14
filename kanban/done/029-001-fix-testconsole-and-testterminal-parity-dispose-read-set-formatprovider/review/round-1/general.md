# Round 1 — general
**Date:** 2026-09-15
**Scope reviewed:** commit a826926 vs origin/feature/overnight-terminal; test-console.cs, test-terminal.cs, terminal-static.cs, test-terminal-context.cs, stream-access/rich-input/test-terminal-context tests, skills/terminal/SKILL.md

## Summary

Commit `a826926` lands the parent-029 Dispose/Read/Set*/FormatProvider/ReadKey-shift/Clear-docs batch on both test doubles and the static facade. Risk is moderate but well contained: I/O now routes through `In`/`Out`/`Error` with capture tees, FormatProvider is an async-local override under `TestTerminalContext`, and `SnapshotHead` is an immutable linked list. Surrounding `TimeWarpConsole`/`TimeWarpTerminal` `Set*` contracts and TestTerminal’s owned-stream Dispose (022) are preserved; no new defects found in the diff or call sites.

## Issues

## Parent findings coverage

- **M1** — yes. `TestConsole.Dispose` disposes only `OwnedStandard*` (`test-console.cs:295-297`); regression `Should_not_dispose_consumer_assigned_standard_output_stream_in_test_console` in `tests/stream-access-01-basic.cs`.
- **M2** — yes. `TestConsole.Read` drains `CharacterQueue` then `In.Read()` (`test-console.cs:201-202`); constructor/interleave/prefer-queue tests in `tests/rich-input-01-basic.cs`.
- **M3** — yes. Both doubles route `Write*`/`ReadLine`/`Read` through `Out`/`Error`/`In` with tee helpers; SetOut/SetError/SetIn redirect tests on Console and Terminal in `tests/stream-access-01-basic.cs`.
- **M4** — yes. `Terminal.FormatProvider` uses `FormatProviderBox` AsyncLocal while `TestTerminalContext.HasValue` (`terminal-static.cs:101-117`); `SnapshotHead` linked snapshots (`test-terminal-context.cs:60-67,99-105`); parallel FormatProvider + forked snapshot tests in `tests/test-terminal-context-01-integration.cs`.
- **M14** — yes. Constructor-input `ReadKey` synthesis sets `shift: char.IsAsciiLetterUpper(c)` (`test-terminal.cs:294-295`); regression `Should_set_shift_flag_when_readkey_synthesizes_constructor_uppercase`.
- **M18** — yes. `TestConsole.ClearOutput` added with `Clear` as documented alias (`test-console.cs:222-247`); divergence documented on `TestTerminal.Clear` (`test-terminal.cs:417-425`); skill note for FormatProvider locality only.
