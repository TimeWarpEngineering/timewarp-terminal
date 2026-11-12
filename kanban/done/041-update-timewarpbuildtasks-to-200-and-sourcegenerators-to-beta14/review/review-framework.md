# Review framework

## Budget (by-diff)

- Lines changed: 86
- Effort: 1
- TCB hits: none
- Roster axes: general
- Turn cap: 80 (--max-turns; cursor uncapped)

## Scope

**Date:** 2026-11-12
**Host task:** kanban/in-progress/041-update-timewarpbuildtasks-to-200-and-sourcegenerators-to-beta14/
**Diff scope:** branch `task/041-update-timewarpbuildtasks-to-200-and-sourcegenerat` vs `master` (merge-base `6eec231`); product change is commit `1b4eb87` (`Directory.Packages.props`)
**Plan / brief:** bump `TimeWarp.Build.Tasks` 1.0.0 -> 2.0.0 and `TimeWarp.SourceGenerators` 1.0.0-beta.13 -> 1.0.0-beta.14; fix any new diagnostics without suppressions
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** reviewer-cursor (review oracle), session not reported

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
- Build, test, and audit gates are taken from the implementer's recorded `## Results`, the host audit node, and CI; the reviewer does not re-run them
