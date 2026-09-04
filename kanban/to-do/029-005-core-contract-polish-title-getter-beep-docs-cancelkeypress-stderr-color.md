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

- [ ] M11 Title Windows getter swallows IOException → empty string
- [ ] M12 Beep remarks and/or implementation agree on redirected Windows
- [ ] M13 CancelKeyPress Instance/context swap documented or forwarded
- [ ] M17 colored WriteErrorLine does not emit ANSI into redirected stderr
- [ ] Existing cancel-key-press / color tests still pass; add coverage where the contract changed

## Notes

- Parent: `kanban/in-progress/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already fixed Title *setter* Unix gate, parameterless Beep Unix gate, CancelKeyPress event existence, and SupportsColor on stdout-colored writers. Do not regress those.

## Session

- Created: 3367923 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
