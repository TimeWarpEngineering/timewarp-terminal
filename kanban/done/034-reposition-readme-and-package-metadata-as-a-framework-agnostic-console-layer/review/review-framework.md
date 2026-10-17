# Review framework

## Budget (by-diff)

- Lines changed: 304
- Effort: 2
- TCB hits: none
- Roster axes: general
- Turn cap: 120 (--max-turns; cursor uncapped)

## Framework — task 034

**Date:** 2026-10-17
**Host task:** kanban/to-do/034-reposition-readme-and-package-metadata-as-a-framework-agnostic-console-layer/
**Diff scope:** branch `task/034-reposition-readme-and-package-metadata-as-a-framew` vs `master` (commit aefbcfd): `readme.md`, `skills/terminal/SKILL.md`, `source/timewarp-terminal/timewarp-terminal.csproj`, `tests/plain-main-01-capture.cs`, plus task.md
**Plan / brief:** move README, skill, and NuGet metadata from "testable output layer for Nuru CLIs" to framework-agnostic console layer. Add a plain `Main` sample backed by a test. Move the Nuru sample under "Works well with Nuru".
**Effort:** 2
**Reviewer roster:** general
**Session IDs:** review oracle (claude, claude-opus-5-5), session id not exposed in print mode

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
