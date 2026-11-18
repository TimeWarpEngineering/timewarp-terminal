# Review framework — task 044

## Budget (by-diff)

- Lines changed: 173
- Effort: 1
- TCB hits: none
- Roster axes: general
- Turn cap: 80 (--max-turns; cursor uncapped)

**Date:** 2026-11-19
**Host task:** kanban/in-progress/044-refresh-outdated-nu-get-packages-in-time-warp-engineering-timewarp-terminal/
**Diff scope:** branch `task/044-refresh-outdated-nu-get-packages-in-time-warp` vs merge-base `2a58e31` (`master`). Product files: `Directory.Packages.props`, `tools/dev-cli/endpoints/build.cs`, `test.cs`, `verify-samples.cs`, `workflow.cs`, `workflow.release-gates.cs`, `workflow.release.cs`.
**Plan / brief:** Bump the six dev-only TimeWarp pins. Leave `source/Directory.Build.props` `<Version>` alone. Stay in this repo.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** grok `01a1f02f-b899-72c0-bb7a-1797503a962e` (reviewer, 2026-11-19). Implementer session was not reported.

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims by reading the diff and the kitchen `## Results` and `### How to validate` record
- Do not re-run a full build, a full test run, an audit, or the hooks; those belong to the implementer's recorded results, the host audit node, and CI
- Exception: one targeted command may check one suspected bug; the finding names the command and the reason. A full build, full test run, or audit is never that command
- A missing or thin recorded result is a finding, not a reason to run the gates
- Prior rounds are immutable; new work goes in `round-(N+1)/`
