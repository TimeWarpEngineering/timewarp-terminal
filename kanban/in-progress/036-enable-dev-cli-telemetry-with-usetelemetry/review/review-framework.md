# Review framework

## Budget (by-diff)

- Lines changed: 207
- Effort: 2
- TCB hits: none
- Roster axes: general
- Turn cap: 120 (--max-turns; cursor uncapped)

# Review framework — task 036

**Date:** 2026-11-09
**Host task:** kanban/in-progress/036-enable-dev-cli-telemetry-with-usetelemetry/
**Diff scope:** branch task/036-enable-dev-cli-telemetry-with-usetelemetry vs merge-base with master (fc1ba3b; product files tools/dev-cli/dev.cs, tools/dev-cli/endpoints/incoming-trace-context.cs, tools/dev-cli/telemetry.md)
**Plan / brief:** task.md Requirements + `## Goal (implementer restatement)`; reference timewarp-nuru tools/dev-cli (kanban 556)
**Effort:** 2 (by-diff budget)
**Reviewer roster:** general
**Session IDs:** review oracle (claude, opus-5.5), 2026-11-09

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
