# Review framework

## Budget (by-diff)

- Lines changed: 1430
- Effort: 3
- TCB hits: none
- Roster axes: general
- Turn cap: 200 (--max-turns; cursor uncapped)

## Framework — task 038

**Date:** 2026-11-09
**Host task:** kanban/in-progress/038-release-pipeline-promotes-ci-built-packages-instead-of-rebuilding/
**Diff scope:** branch `task/038-release-pipeline-promotes-ci-built-packages-instea` vs merge-base `ffdf22f` (master); product files: `.github/workflows/workflow.yml`, `Directory.Build.props`, `samples|tests|tools/dev-cli/Directory.Build.props`, `tools/dev-cli/dev.cs`, `tools/dev-cli/endpoints/workflow*.cs`, `documentation/developer/guides/releasing.md`
**Plan / brief:** task.md `## Goal (implementer restatement)`. The release run promotes the green master CI `Packages-*` artifact (tag-gate → check-version → locate-run → download-artifact → verify → push). Each symbol package is pushed once. The probe and break-glass guards stay.
**Reference:** timewarp-nuru master `workflow.yml` and `source/Directory.Build.props` (template)
**Effort:** 3 (roster axes: general)
**Reviewer roster:** general (review oracle, claude-opus-5-5)
**Session IDs:** review oracle under `ganda task work` (headless, print mode)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
