# Round 1 — general
**Date:** 2026-11-12
**Scope reviewed:** branch vs `master` (merge-base `6eec231`); product diff is `Directory.Packages.props` lines 22-23 in commit `1b4eb87`, plus the implementer's `## Results` record in `task.md`.

## Summary

The change bumps two central package pins: `TimeWarp.Build.Tasks` 1.0.0 -> 2.0.0 and `TimeWarp.SourceGenerators` 1.0.0-beta.13 -> 1.0.0-beta.14. No source, suppression, or `.editorconfig` changes were needed, and none were made. The implementer's restatement matches the original task. Risk is low and limited to the Build.Tasks 2.0.0 runtime requirement, which this repo meets. No issues found.

## Verification (read-only)

- **Pins:** `Directory.Packages.props:22-23` carry the new versions. `rg` over the repo (excluding `kanban/`) finds no other version pin. `Directory.Build.props:77-78` references both packages without versions, so central pinning covers every project.
- **Build.Tasks 2.0.0 breaking change:** the restored package (`~/.nuget/packages/timewarp.build.tasks/2.0.0/build/TimeWarp.Build.Tasks.props`) loads `GitMetadataTask` from `net11.0/` only when `MSBuildRuntimeType == Core`, and raises `TWBT0001` on .NET Framework MSBuild or a runtime below 11.0. `global.json` pins SDK `11.0.100`. Both CI jobs in `.github/workflows/workflow.yml` run on `ubuntu-latest` with `actions/setup-dotnet@v4` and `global-json-file: global.json`, so CI always builds with the `dotnet` CLI on .NET 11. This agrees with the implementer's analysis.
- **Suppressions:** the diff touches neither `.timewarp/suppression-baseline.json` nor `.editorconfig`, and it adds no `#pragma` or `[SuppressMessage]`. No check was silenced or weakened.
- **Gate record:** `## Results` records exit codes and outcomes for `ganda nuget outdated`, a Release `dotnet build` (0 warnings, 0 errors), `self-install`, `./bin/dev workflow` (samples, 47 test files, coverage, package-integrity gate), and `ganda repo audit` (42 passed, 0 failed). `### How to validate` covers the change. No gates were re-run during this review.
- **Base drift:** the 3 commits on `master` that this branch lacks only land this task's own kanban `task.md`. None of them touch product files.

## Issues

None.

## Observations (not findings)

- From 2.0.0, building the repo with Visual Studio or `MSBuild.exe` (.NET Framework MSBuild) fails with `TWBT0001`. The task explicitly accepts this by requiring "do not pin 1.x". The escape hatch is `TimeWarpEnableGitMetadata=false`. No current workflow in this repo uses .NET Framework MSBuild.
