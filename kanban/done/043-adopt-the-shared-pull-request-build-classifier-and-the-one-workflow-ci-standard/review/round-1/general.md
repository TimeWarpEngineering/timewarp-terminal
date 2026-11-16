# Round 1 — general
**Date:** 2026-11-17
**Scope reviewed:** branch `task/043-adopt-the-shared-pull-request-build-classifier-and` vs `master` (234 lines). Compared with the original requirements and with `## Goal (implementer restatement)`.

## Summary

`dev workflow` calls `PullRequestBuildClassifier.DecideAsync` at the start of the shared pull-request and merge pipeline, prints the decision message, and returns before clean, build, verify-samples, and test when `RequiresBuild` is false. Release mode still goes through `RunReleaseWorkflowAsync` and does not call the classifier. `pull_request` no longer has a paths filter; `push` keeps its allowlist, with the kanban-only comment the task asked for, plus `readme.md` and `assets/logo.png` so a merge of those packed files still builds and uploads packages.

The restatement matches the original task. Samples sit outside the classifier's protected roots (`source`, `tests`, `tools`, `msbuild`), and root `readme.md` plus `assets/logo.png` are packed (`PackageReadmeFile` / `PackageIcon`) but would otherwise be skipped (root markdown, and `assets/**` as a default non-build root). Declaring those three entries under `pullRequestBuild.buildPaths` is the config contract (`PullRequestBuildConfig`), and `RepoConfigService.Parse` reads that file with case-insensitive names and JSONC comments. `fetch-depth: 0` is already on the CI checkout, which is what the three-dot diff needs. The recorded gates cover the change: `./bin/dev build` and `./bin/dev test` passed after the Amuru 3.0.0-beta.5 bump that Nuru 4.0.0-beta.15 requires, and `ganda repo audit` passed including `workflow-file` and `dev-workflow-classifier`. The pull-request `CI` URL is correctly left for the host open-pr node. No local copy of the rule is in `tools/dev-cli/`. The audit `--fix` hook is the Task trailer hook the task required, not a silenced check.

## Issues

None.
