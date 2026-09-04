# Review framework — task 029

**Date:** 2026-09-04
**Host task:** `kanban/to-do/029-complete-detailed-code-review-of-timewarpterminal/`
**Diff scope:** whole-repo review of origin-home `master` (not a PR delta)
**Pinned SHA at kitchen create:** `bf38d514990febd2815294908b2a599c6f6e0bab`
**Pinned version:** TimeWarp.Terminal `1.0.1`
**Plan / brief:** `task.md` — successor to 022 (1.0 release-readiness); re-review current tree including tests/tools/CI
**Effort:** elevated — 6 area reviewers (not default effort-1)
**Reviewer roster:** core-abstractions, static-facade, test-doubles, widgets, tests-infra, security
**Session IDs:** kitchen created Grok `01a06a77-1631-7543-b181-07ddc524f9fe` / ganda claim 3290412; review-round sessions TBD

**Re-pin before round 1:** if `origin/master` has moved, update **Pinned SHA** here and record the new `git rev-parse origin/master` / `git log -1 --oneline`.

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome for an area
- Address the current tree and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
- Do not re-open 022-done findings unless the defect is still present (cite current file:line)
- Do not review `TimeWarp.Terminal.Layout` (task **023**, not in tree)
- IConsole/ITerminal `new` + explicit impl is required pairing (`source/timewarp-terminal/AGENTS.md`) — missing a pair is a **bug**, not a nit
- Caller-embedded ANSI strings remaining the caller’s responsibility was an explicit 022 decision — not a new finding unless a *library* path ignores SupportsColor

## Finding template

Each reviewer writes `review/round-N/<reviewer>.md` using the `tw-implementation-review` finding template (`bug` / `suggestion` / `nit`, `Status: open`, file:line, suggestion).

## Merge

After all six reviewers finish, write `review/round-N/merged.md` with counts table, stable `M#` IDs, source attribution, and duplicate collapse notes.

## Disposition

Exit bar: 0 `open` findings on this task *or* remaining opens filed as `--parent 029` children with IDs listed in `review/disposition.md`. Outcome is `clean` or `accepted-exceptions`.
