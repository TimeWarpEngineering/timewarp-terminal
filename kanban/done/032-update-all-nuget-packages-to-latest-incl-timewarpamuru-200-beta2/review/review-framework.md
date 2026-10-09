# Review framework

## Budget (by-diff)

- Lines changed: 382
- Effort: 2
- TCB hits: none
- Roster axes: general
- Turn cap: 120 (--max-turns; cursor uncapped)

# Review framework — task 032

**Date:** 2026-10-09
**Host task:** kanban/to-do/032-update-all-nuget-packages-to-latest-incl-timewarpamuru-200-beta2/
**Diff scope:** branch task/032-update-all-nuget-packages-to-latest-incl-timewarpa vs master (commit 1c14737)
**Plan / brief:** Bump all NuGet pins to latest (Amuru/Amuru.Tools 2.0.0-beta.2, Nuru beta.79, Roslynator 5.0.1, analyzers), refresh ganda-owned hooks, fix Nuru beta.79 breaking changes in dev-cli.
**Effort:** 2 (by-diff budget)
**Reviewer roster:** general
**Session IDs:** claude review oracle (ganda task work), 2026-10-09

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
