# Review framework — task 029-005

**Date:** 2026-09-21
**Host task:** kanban/in-progress/029-005-core-contract-polish-title-getter-beep-docs-cancelkeypress-stderr-color/
**Diff scope:** branch `task/029-005-core-contract-polish-title-getter-beep-docs-cancel` vs `origin/master` — product commit `db3e526` (`fix: align Title, Beep, CancelKeyPress, and stderr color contracts`). Kanban kitchen commit `8fd0a7e` is out of product-code scope except as the implementer brief.
**Plan / brief:** Child of parent **029** round-1 merged findings **M11, M12, M13, M17**. Windows `Title` getter swallows `IOException` and returns empty string; `Beep()` remarks match Windows redirected `Kernel32.Beep` vs Unix no-op (implementation not gated on `IsOutputRedirected`); static `CancelKeyPress` uses a facade handler list plus a single forwarder rebound on process-global `Instance` assignment (not async-local `TestTerminalContext` swaps); colored `WriteErrorLine` requires `SupportsColor && !IsErrorRedirected`. Do not regress 022 Title setter Unix gate, parameterless Beep Unix gate, CancelKeyPress existence, or stdout `SupportsColor`.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** Review oracle Grok `01a0bfd2-9633-7c32-ae38-a04538001cad` (2026-09-21); implementer Grok `01a0bfc6-4c10-78a2-9f98-9ce1c936a86a` (2026-09-21)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`

## Parent findings this child claims to close

| ID | Severity | Claimed fix |
|----|----------|-------------|
| M11 | suggestion | Windows `Title` getter catches `IOException` and returns `string.Empty`; `ITerminal.Title` remarks cover the empty-string fallback |
| M12 | suggestion | Remarks describe Unix redirected no-op vs Windows kernel beep; implementation still only swallows `IOException` |
| M13 | suggestion | Facade handler list + single forwarder; add/remove match after `Use` ends; rebind on process-global `Instance` assignment only |
| M17 | suggestion | Colored `WriteErrorLine` overloads require `SupportsColor && !IsErrorRedirected`; stdout writers unchanged |

## Round 2

**Date:** 2026-09-21
**Diff scope:** product commit `87489b5` (`fix: rebind CancelKeyPress forwarder to assigned Instance field`) plus re-verify round-1 M1. Parent 029 M11/M12/M13/M17 remain in scope only for regression. Round 1 files are frozen.
**Reviewer roster:** general

