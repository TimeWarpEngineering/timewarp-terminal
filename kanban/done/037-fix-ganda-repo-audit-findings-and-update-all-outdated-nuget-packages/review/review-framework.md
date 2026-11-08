# Review framework

## Budget (by-diff)

- Lines changed: 7826
- Effort: 3
- TCB hits: none
- Roster axes: general
- Turn cap: 200 (--max-turns; cursor uncapped)

# Review framework — task 037

**Date:** 2026-11-09
**Host task:** kanban/in-progress/037-fix-ganda-repo-audit-findings-and-update-all-outdated-nuget-packages/
**Diff scope:** branch `task/037-fix-ganda-repo-audit-findings-and-update-all-outda` (HEAD e02ebd4) vs merge-base 07dcd3c with master
**Plan / brief:** clean `ganda repo audit`, every `ganda nuget outdated` pin moved to latest (major included), 7 over-cap C# files split by responsibility (partial types / separate test classes), dev build + test green
**Effort:** 3 (by-diff budget); roster axes: general
**Reviewer roster:** general (review oracle, Claude Opus 5.5)
**Session IDs:** review oracle under `ganda task work` (headless print mode; session id not exposed)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
