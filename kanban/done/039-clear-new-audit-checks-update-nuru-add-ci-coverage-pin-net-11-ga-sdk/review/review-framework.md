# Review framework

## Budget (by-diff)

- Lines changed: 842
- Effort: 3
- TCB hits: none
- Roster axes: general
- Turn cap: 200 (--max-turns; cursor uncapped)

## Framework — task 039

**Date:** 2026-11-11
**Host task:** kanban/in-progress/039-clear-new-audit-checks-update-nuru-add-ci-coverage-pin-net-11-ga-sdk/
**Diff scope:** branch `task/039-clear-new-audit-checks-update-nuru-add-ci-coverage` vs merge-base with `master` (`faf3d0b`); product commit `34d7f03` (86 files, +727/-115)
**Plan / brief:** `task.md` `## Goal (implementer restatement)` and `## Checklist`: clear six `ganda repo audit` failures (`nuru`, `analyzer-suppressions`, `timewarp-signals-gitignore`, `memsearch-scaffold`, `workflow-pwsh-shell`, `ci-code-coverage`), pin .NET 11.0.100 GA, take remaining `ganda nuget outdated` updates, keep the task 038 release promotion working, and cut no release.
**Effort:** 3 (by-diff budget); roster axes: general
**Reviewer roster:** general
**Session IDs:** reviewer-cursor (Claude Opus 5.5), session id not reported by harness

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
