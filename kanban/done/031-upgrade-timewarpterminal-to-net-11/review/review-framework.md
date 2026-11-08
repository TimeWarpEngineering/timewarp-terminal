# Review framework

## Budget (by-diff)

- Lines changed: 276
- Effort: 2
- TCB hits: none
- Roster axes: general
- Turn cap: 120 (--max-turns; cursor uncapped)

## Scope

**Date:** 2026-11-09
**Host task:** kanban/in-progress/031-upgrade-timewarpterminal-to-net-11/
**Diff scope:** branch `task/031-upgrade-timewarpterminal-to-net-11` vs `master` (merge base), commit 222c652 and kanban commits
**Plan / brief:** `## Goal (implementer restatement)` in task.md: hard cut to `net11.0`, `global.json` RC SDK pin, CI `global-json-file`, NetAnalyzers 11, version 2.0.0, consumer notes
**Effort:** 2 (by-diff budget), single `general` reviewer (orchestrator inline)
**Reviewer roster:** general
**Session IDs:** review oracle (Claude Opus 5.5, ganda task work review node)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
