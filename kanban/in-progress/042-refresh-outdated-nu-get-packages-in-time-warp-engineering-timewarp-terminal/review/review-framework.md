# Review framework

## Budget (by-diff)

- Lines changed: 108
- Effort: 1
- TCB hits: none
- Roster axes: general
- Turn cap: 80 (--max-turns; cursor uncapped)

# Review framework — task 042

**Date:** 2026-11-14
**Host task:** kanban/in-progress/042-refresh-outdated-nu-get-packages-in-time-warp-engineering-timewarp-terminal/
**Diff scope:** branch `task/042-refresh-outdated-nu-get-packages-in-time-warp` vs merge-base `d66e6f9842628808709c5ba37a25270cdfcc9560` (`master`). Product files: `Directory.Packages.props`, `source/Directory.Build.props`, `source/timewarp-terminal-layout/layout-engine.cs`, `tools/dev-cli/endpoints/workflow.cs`, and Purpose regions in `.githooks/*.cs`. Kitchen `task.md` is on the same branch. Master is two kanban-land commits ahead; those commits touch only `task.md`.
**Plan / brief:** Refresh the five listed central package pins and set `<Version>` to `2.0.0-beta.2`. Audit `--fix` also moved Builder, Flexbox, and Build.Tasks, and added hook Purpose regions. CodeStyle 5.12.0 required two IDE0028 collection-expression edits.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** review oracle Grok session `01a1d9a8-dafb-71a1-92a2-8bab7532364e` (2026-11-14). Implementer session `a5e337d8-d18c-4978-acfc-2059b6dd9b08` (claude-opus-5-5, 2026-11-14).

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims by reading the diff and the kitchen `## Results` and `### How to validate` record
- Do not re-run a full build, a full test run, an audit, or the hooks; those belong to the implementer's recorded results, the host audit node, and CI
- Exception: one targeted command may check one suspected bug; the finding names the command and the reason. A full build, full test run, or audit is never that command
- A missing or thin recorded result is a finding, not a reason to run the gates
- Prior rounds are immutable; new work goes in `round-(N+1)/`
