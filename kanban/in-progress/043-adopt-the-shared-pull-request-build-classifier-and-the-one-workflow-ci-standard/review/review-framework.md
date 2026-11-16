# Review framework — task 043

**Date:** 2026-11-17
**Host task:** kanban/in-progress/043-adopt-the-shared-pull-request-build-classifier-and-the-one-workflow-ci-standard/
**Diff scope:** branch `task/043-adopt-the-shared-pull-request-build-classifier-and` vs `master` (5 commits, 234 lines). Product files: `.github/workflows/workflow.yml`, `.timewarp/dev.jsonc`, `Directory.Packages.props`, `tools/dev-cli/dev.cs`, `tools/dev-cli/endpoints/workflow.cs`, `.githooks/prepare-commit-msg`, `.githooks/prepare-commit-msg.cs`, and this task's `task.md`.
**Plan / brief:** Adopt `PullRequestBuildClassifier` from TimeWarp.Nuru.DevCli 4.0.0-beta.15, drop the `pull_request` paths filter, and declare extra build paths in `.timewarp/dev.jsonc`.
**Effort:** 2 (general only)
**Reviewer roster:** general
**Session IDs:** grok 01a1e58a-9417-7770-ad3c-045b9c3c3c3b (review); implementer claude ca195ba2-89fc-4c22-8baf-6f603c3bb39e

## Budget (by-diff)

- Lines changed: 234
- Effort: 2
- TCB hits: none
- Roster axes: general
- Turn cap: 120 (--max-turns; cursor uncapped)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims by reading the diff and the kitchen `## Results` and `### How to validate` record
- Do not re-run a full build, a full test run, an audit, or the hooks; those belong to the implementer's recorded results, the host audit node, and CI
- Exception: one targeted command may check one suspected bug; the finding names the command and the reason. A full build, full test run, or audit is never that command
- A missing or thin recorded result is a finding, not a reason to run the gates
- Prior rounds are immutable; new work goes in `round-(N+1)/`
