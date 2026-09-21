# Core contract polish (Title getter, Beep docs, CancelKeyPress, stderr color)

## Description

Parent **029** round-1 merged findings **M11, M12, M13, M17**.

Small IConsole/ITerminal / static-facade contract mismatches that are not test-double or OSC 8 work: Windows `Title` getter can still throw, `Beep()` remarks disagree with Windows redirected BCL behavior, static `CancelKeyPress` add/remove bind only to the `Instance` resolved at that call, and colored `WriteErrorLine` gates on stdout `SupportsColor`.

Do **not** create a sibling “apply 029 findings” task. This child is the product-fix batch.

## Requirements

### M11 — suggestion — Windows `Title` getter does not swallow `IOException`
- File: `source/timewarp-terminal/timewarp-terminal.cs:541`
- Setter catches `IOException`; `CursorVisible` getter returns a safe default on `IOException`. Wrap the Windows getter and return `string.Empty`. Optionally extend `ITerminal.Title` remarks.

### M12 — suggestion — `Beep()` remarks vs Windows redirected `Kernel32.Beep`
- File: `source/timewarp-terminal/iterminal.cs:274`
- Remarks claim redirected/unavailable “silently does nothing instead of throwing.” Implementation (`timewarp-terminal.cs:478-490`) only catches `IOException`. On Windows, BCL `Console.Beep()` still beeps via `Kernel32.Beep` when stdout is redirected.
- Either gate on `Console.IsOutputRedirected` to match the remarks, or revise the remarks to describe Windows redirected fallback vs Unix no-op.

### M13 — suggestion — `Terminal.CancelKeyPress` binds to `Instance` at subscribe time
- File: `source/timewarp-terminal/terminal-static.cs:1037-1041`
- add/remove forward to whatever `Instance` resolves to (AsyncLocal context first). Subscribe inside `Use`, unsubscribe after the scope ends → leak on the test terminal / no-op remove on the restored instance.
- Keep a facade-level handler list and attach a single forwarder to the current Instance, **or** document that subscribe/unsubscribe must occur against a stable Instance.

### M17 — suggestion — colored `WriteErrorLine` gates on stdout `SupportsColor`
- File: `source/timewarp-terminal/timewarp-terminal.cs:333-336` (writers in `terminal-static.cs:260-302`)
- When stdout is a TTY and stderr is redirected, library-applied SGR still lands in the redirected error stream.
- For error-colored writers, also require `!IsErrorRedirected` before wrapping with `AnsiColors`.

## Checklist

- [x] M11 Title Windows getter swallows IOException → empty string
- [x] M12 Beep remarks and/or implementation agree on redirected Windows
- [x] M13 CancelKeyPress Instance/context swap documented or forwarded
- [x] M17 colored WriteErrorLine does not emit ANSI into redirected stderr
- [x] Existing cancel-key-press / color tests still pass; add coverage where the contract changed
- [x] Effort-1 general review under `review/` (rounds 1–2)
- [x] Round-1 M1 CancelKeyPress rebind uses assigned field, not AsyncLocal getter
- [x] `review/disposition.md` — outcome `clean`

## Notes

- Parent: `kanban/in-progress/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already fixed Title *setter* Unix gate, parameterless Beep Unix gate, CancelKeyPress event existence, and SupportsColor on stdout-colored writers. Do not regress those.

## Session

- Created: 3367923 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
- Implementer: Grok session `01a0bfc6-4c10-78a2-9f98-9ce1c936a86a` (2026-09-21)
- Review oracle: Grok `01a0bfd2-9633-7c32-ae38-a04538001cad` (2026-09-21)

## Results

Parent **029** M11/M12/M13/M17 applied on this child. No sibling “apply findings” task.

### What was implemented

- **M11** — `TimeWarpTerminal.Title` getter on Windows catches `IOException` and returns `string.Empty`, matching the setter and `CursorVisible`. `ITerminal.Title` remarks cover the empty-string fallback.
- **M12** — Remarks revised to match BCL: Unix redirected parameterless `Beep` is a no-op (BEL is not written to the redirected stream); Windows still plays the system beep when stdout is redirected. Implementation still swallows `IOException` when the console is unavailable; it is not gated on `IsOutputRedirected` (would change Windows redirected behavior).
- **M13** — `Terminal.CancelKeyPress` stores handlers on the facade and attaches a single forwarder to the `Instance` resolved at first subscribe. Add/remove no longer require the same instance to be current. Process-global `Instance` assignment rebinds the forwarder to the assigned field (not `TestTerminalContext.Current`); `TestTerminalContext` async-local swaps do not (avoids moving leftover handlers onto `Console` and racing parallel `Use` scopes). Review round-1 M1: setter calls `SyncCancelKeyPressForwarder(field)` (`87489b5`).
- **M17** — Colored `WriteErrorLine` overloads require `SupportsColor && !IsErrorRedirected` before wrapping with `AnsiColors`. Stdout-colored writers still gate only on `SupportsColor`.

### Files changed

- `source/timewarp-terminal/timewarp-terminal.cs`
- `source/timewarp-terminal/iterminal.cs`
- `source/timewarp-terminal/terminal-static.cs`
- `tests/cancel-key-press-01-basic.cs`
- `tests/terminal-static-06-color.cs`

### Key decisions

- M12: document Windows redirected beep rather than silencing it.
- M13: facade list + bound-instance forwarder; do not rebind on `TestTerminalContext.SetCurrent`/`ClearCurrent`. Process-global `Instance` assignment rebinds using the assigned field so a set inside `Use` does not pin to Current.
- M17: do not fold stderr into `SupportsColor` (that would regress stdout color when only stderr is redirected).

### Test outcomes

`dotnet tools/dev-cli/dev.cs test` — **43/43 test files passed**, including:

- `cancel-key-press-01-basic.cs` (existing add/remove plus Instance-rebind, unsubscribe-after-Use, and assignment-inside-Use)
- `terminal-static-06-color.cs` (14 tests, including `Should_write_error_line_plain_when_error_redirected`)
- `terminal-control-utilities-01-basic.cs` (Title/Beep on `TestTerminal`)
- `test-terminal-context-01-integration.cs` (parallel `Use` isolation unchanged)

### How to validate

**Smoke**

```bash
dotnet tests/cancel-key-press-01-basic.cs
dotnet tests/terminal-static-06-color.cs
dotnet tests/terminal-control-utilities-01-basic.cs
```

**Expect**

- CancelKeyPress runfile prints `✓ Static Terminal.CancelKeyPress forwarder follows Instance assignment`, `✓ Static Terminal.CancelKeyPress unsubscribe after Use does not leak`, `✓ Static Terminal.CancelKeyPress assignment inside Use follows process-global Instance`, and `🧪 All CancelKeyPress tests passed!`
- Color suite: 14 passed, including `Should_write_error_line_plain_when_error_redirected` (stderr has the message text and no `\u001b`; stdout color still applied)
- Control utilities: 13 passed (Title get/set and Beep counts on `TestTerminal`)

**Automated gate**

```bash
dotnet tools/dev-cli/dev.cs test
# expect: ✓ All 43 test file(s) passed
```

**Not in scope:** live Windows `Console.Title` `IOException` and audible `Kernel32.Beep` when stdout is redirected (no console-less Windows fixture in this suite).

### Review disposition

- **Rounds:** 2
- **Effort / roster:** 1 — general only
- **Final counts:** 0 open / 1 fixed / 0 wontfix (bug 1 fixed; suggestion 0; nit 0)
- **Outcome:** `clean` (parent 029 M11, M12, M13, M17 closed on this child; round-1 M1 CancelKeyPress rebind fixed on this id)
- **Wontfix / escalations:** none
- **Paths:**
  - `review/review-framework.md`
  - `review/round-1/general.md`
  - `review/round-1/merged.md`
  - `review/round-2/general.md`
  - `review/round-2/merged.md`
  - `review/disposition.md`
- Review-session smoke: `dotnet tests/cancel-key-press-01-basic.cs` pass (including assignment-inside-Use); `dotnet tests/terminal-static-06-color.cs` 14/14.
