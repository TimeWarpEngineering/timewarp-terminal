# Adopt the shared pull request build classifier and the one-workflow CI standard

## Description

Bring timewarp-terminal to the org CI standard (ganda 476 / 479 / 565 / 587, audited by ganda 655): `workflow.yml` has
no `paths` / `paths-ignore` filter on `pull_request`, so every pull request reports `CI`, and `dev workflow`
calls the shared classifier `PullRequestBuildClassifier` from `TimeWarp.Nuru.DevCli` 4.0.0-beta.15
(timewarp-nuru 595) to skip build and test on a pull request that touches no build path (kanban, docs, ...).

Tracked by timewarp-ganda 656 (architecture reposet rollout).

Survey 2026-11-16 for this repo: has a `pull_request` paths filter; `dev workflow` does not classify. Work: remove the filter, add the classifier call.

## Requirements

1. Run `ganda repo audit --fix` first and commit what it changes.
2. Bump the `TimeWarp.Nuru.DevCli` pin to `4.0.0-beta.15` (and `TimeWarp.Nuru` to the same version if pinned).
3. In the `workflow` command's pull-request path, call `PullRequestBuildClassifier.DecideAsync` first. On skip,
   print its message and exit 0. Push and release paths are unchanged.
4. Remove `paths` / `paths-ignore` from the `pull_request` trigger of `.github/workflows/workflow.yml`. Keep any
   `push` filter and add a comment saying why (a kanban-only merge must not rebuild and upload packages).
5. If this repo builds from roots outside the classifier defaults (`source`, `tests`, `tools`, `msbuild`), declare
   them through the classifier's config (`PullRequestBuildConfig`), not by copying or editing the rule.
6. No local copy of the classification rule remains in `tools/dev-cli/`.
7. `ganda repo audit` passes, including both CI-standard checks from ganda 655.

## Goal (implementer restatement)

Every pull request in timewarp-terminal must report a `CI` check, and a pull request that only touches
non-build paths (kanban, docs, ...) must pass that check quickly without building.

- Bump `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` to `4.0.0-beta.15` (ships `PullRequestBuildClassifier`).
- `dev workflow` (what `workflow.yml` runs) calls `PullRequestBuildClassifier.DecideAsync(repoRoot, ct)` at the
  start of the pull-request/merge pipeline, prints its message, and returns exit 0 without clean/build/test
  when it says no build is needed. Release mode is untouched.
- `workflow.yml`: drop the `paths` list from `pull_request`; keep the `push` allowlist with a comment saying a
  kanban-only merge must not rebuild and upload packages.
- Repo build inputs outside the classifier's protected roots (`samples/**`, plus `readme.md` and
  `assets/logo.png` which are packed into the nupkgs) go into `.timewarp/dev.jsonc` `pullRequestBuild.buildPaths`.
  No copy of the rule lives in `tools/dev-cli/`.
- Acceptance: build and test pass; `ganda repo audit` passes including `workflow-file` and
  `dev-workflow-classifier`; the task PR shows a `CI` run.

## Checklist

- [x] `ganda repo audit --fix` run and committed
- [x] DevCli pin at 4.0.0-beta.15
- [x] `dev workflow` calls the shared classifier on pull requests; skip exits 0 with its message
- [x] No `paths` filter on `pull_request`; `push` filter kept with a why comment
- [x] Extra build roots (if any) declared through config
- [x] `ganda repo audit` clean
- [ ] This task's pull request shows a `CI` run (record the URL in Results)

## Review

- Round 1: Done

## Notes

The ganda 656 survey thought terminal had no `workflow` command in `tools/dev-cli/`. Confirm what CI runs first, and put the classifier call where the pull-request build starts.
After merge, the ganda 656 tracker proves a kanban-only pull request here reports `CI` success and merges with
plain `ganda pr merge` (no `--allow-no-ci`).

## Session

- Created: 64677b22-d705-4b25-b537-4b53732bcb92 (2026-11-16)
- Implement (2026-11-17): merged master (kanban-only commit), ran `ganda repo audit --fix` (added
  `.githooks/prepare-commit-msg` hook, committed 1846dd4), then the changes below.
- Implement oracle: implemented by implementer-claude (claude, vendor anthropic, model claude-opus-5-5, response model claude-opus-5-5), session ca195ba2-89fc-4c22-8baf-6f603c3bb39e, max-turns 500, Done — 2026-11-16T18:06:58Z
- Review (2026-11-17): round 1, effort 2, roster general. Disposition clean (0 findings). The implementer restatement matches the original requirements.
- Reviewer: grok session 01a1e58a-9417-7770-ad3c-045b9c3c3c3b (2026-11-17)
- Review oracle: review by reviewer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a1e58a-9417-7770-ad3c-045b9c3c3c3b, max-turns 120, artifacts general.md, merged.md, disposition.md — 2026-11-16T18:14:35Z

## Results

- `Directory.Packages.props`: `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` 4.0.0-beta.13 -> 4.0.0-beta.15.
  `TimeWarp.Amuru` / `TimeWarp.Amuru.Tools` 3.0.0-beta.2 -> 3.0.0-beta.5, because Nuru beta.15 requires
  Amuru >= 3.0.0-beta.5 (NU1605 package downgrade on the dev-cli restore otherwise).
- `tools/dev-cli/endpoints/workflow.cs`: `RunPrWorkflowAsync` starts with
  `PullRequestBuildClassifier.DecideAsync(repoRoot, cancellationToken)`, prints `decision.Message`, and returns
  (exit 0) when `RequiresBuild` is false. Release path unchanged. Design region updated; `dev.cs` header updated.
- `.github/workflows/workflow.yml`: `pull_request` has no `paths` filter (comment says why). `push` keeps its
  allowlist with a comment that a kanban-only merge must not rebuild and upload packages; `readme.md` and
  `assets/logo.png` (packed into the nupkgs) were added to that allowlist so it matches the classifier config.
- `.timewarp/dev.jsonc`: `pullRequestBuild.buildPaths` = `samples/**` (verify-samples build root),
  `readme.md`, `assets/logo.png` (packed). No local copy of the rule in `tools/dev-cli/`.

### Gates

- `./bin/dev build` — first run failed (NU1605 Amuru downgrade from Nuru beta.15) during dev-cli
  self-install; after the Amuru bump, self-install and `./bin/dev build` succeeded, 0 warnings, 0 errors.
- `./bin/dev test` — pass, all 47 test files passed.
- `ganda repo audit` — "Repository passes all audit checks." `workflow-file` PASS, `dev-workflow-classifier`
  PASS. (A second read-only invocation was only to read the truncated `dev-workflow-classifier` row.)
- PR `CI` run URL: recorded by the host once the PR is opened (open-pr is a later host node).

### How to validate

Smoke:
```bash
./bin/dev build   # or: dotnet run --file tools/dev-cli/dev.cs -- self-install
T=$(mktemp -d)
echo '{"pull_request":{"base":{"sha":"'$(git rev-parse 38bc7b1)'"},"head":{"sha":"'$(git rev-parse 3701383)'"}}}' > $T/event.json
GITHUB_EVENT_NAME=pull_request GITHUB_EVENT_PATH=$T/event.json ./bin/dev workflow; echo "exit=$?"
```
Expect: `Pull request changes only declared non-build paths. Skipping build and test. ... (1 changed paths)`
and `exit=0` (38bc7b1..3701383 is a kanban-only diff). With a base/head pair that touches
`.github/workflows/` or `source/`, the output is `Build path changed: <path>. Full build.` followed by
`CI Pipeline: clean -> build -> verify-samples -> test`. On GitHub, this task's PR shows a `CI` check.

### Review

- Round 1. Effort 2. Roster: general. Session `01a1e58a-9417-7770-ad3c-045b9c3c3c3b`.
- Counts: bug 0 open / 0 fixed / 0 wontfix; suggestion 0; nit 0.
- Disposition: clean. No wontfix and no escalation.
- The implementer restatement matches the original requirements. No misread.
- Paths: `review/review-framework.md`, `review/round-1/merged.md`, `review/disposition.md`.
- Build, test, and audit were not re-run. They stay on the gates recorded above.

