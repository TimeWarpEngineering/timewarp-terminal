# Update TimeWarp.Build.Tasks to 2.0.0 and SourceGenerators to beta.14

## Goal (implementer restatement)

Update the central package pins in `Directory.Packages.props` for `TimeWarp.Build.Tasks` (`1.0.0 -> 2.0.0`) and `TimeWarp.SourceGenerators` (`1.0.0-beta.13 -> 1.0.0-beta.14`), resolve any new build/analyzer diagnostics cleanly without pinning `1.x` or adding blanket suppressions, and pass all repository validation and audit gates without cutting a release.

Acceptance criteria:

- `Directory.Packages.props` pins `TimeWarp.Build.Tasks` `2.0.0` and `TimeWarp.SourceGenerators` `1.0.0-beta.14`.
- Breaking changes in `TimeWarp.Build.Tasks` `2.0.0` (task assembly retargeted from `netstandard2.0` to `net11.0`, requiring MSBuild on .NET 11+ via `dotnet` CLI and emitting `TWBT0001` on .NET Framework MSBuild / older SDKs) and changes in `TimeWarp.SourceGenerators` `1.0.0-beta.14` are reviewed and verified compatible with `timewarp-terminal`'s `.NET 11.0.100` SDK / `dotnet` CLI build and CI pipeline.
- Any surfaced build or analyzer diagnostics are fixed in source code rather than suppressed via blanket rules or raising `.timewarp/suppression-baseline.json`.
- `ganda nuget outdated` reports 0 outdated packages across all central pins.
- `./bin/dev self-install` and `./bin/dev workflow` (clean, Release build, verify-samples, test with Cobertura coverage, and package-integrity hash gate) pass locally with 0 warnings and 0 errors.
- `ganda repo audit` passes cleanly with 0 failed checks.

## Description

`ganda nuget outdated` on master (2026-11-12) reports two outdated packages:

- `TimeWarp.Build.Tasks` 1.0.0 -> 2.0.0 (major; 1.0.2 also exists on the 1.x line)
- `TimeWarp.SourceGenerators` 1.0.0-beta.13 -> 1.0.0-beta.14

Take both latest versions. Build.Tasks runs in every build (analyzers/MSBuild tasks via
`Directory.Build.props`), and warnings are errors, so the major bump may surface new
diagnostics. Fix the code to satisfy them; do not pin 1.x and do not add blanket suppressions.

## Checklist

- [x] Bump both pins in `Directory.Packages.props`
- [x] Read the Build.Tasks 2.0.0 release notes / diff (TimeWarp feed or nuget.org) for breaking changes
- [x] Fix any new diagnostics properly; any suppression has a stated reason
- [x] `ganda nuget outdated` reports nothing outdated
- [x] `dev workflow` passes locally (build, verify-samples, test, coverage, package-hash gate)
- [x] `ganda repo audit` is clean

## Session

- Created: 271402fa-1014-4c38-986b-db3f26ab9720 (2026-11-12)
- Implementation: implementer-cursor (2026-11-12)
- Implement oracle: implemented by implementer-cursor (cursor, vendor google, model gemini-4-carbon-high), session not reported, max-turns uncapped (no --max-turns flag; bound 500), Done — 2026-11-12T13:35:41Z
- Review oracle: reviewer-cursor (tw-implementation-review, effort 1, roster general), session not reported, max-turns uncapped (no --max-turns flag; bound 80), disposition clean — 2026-11-12
- Review oracle: review by reviewer-cursor (cursor, vendor anthropic, model claude-opus-5-5-high), session not reported, max-turns uncapped (no --max-turns flag; bound 80), artifacts general.md, merged.md, disposition.md — 2026-11-12T13:37:12Z

## Results

- Updated `Directory.Packages.props`:
  - `TimeWarp.Build.Tasks` `1.0.0 -> 2.0.0`
  - `TimeWarp.SourceGenerators` `1.0.0-beta.13 -> 1.0.0-beta.14`
- Reviewed release notes and commit diffs for both packages:
  - `TimeWarp.Build.Tasks` `2.0.0` ([release v2.0.0](https://github.com/TimeWarpEngineering/timewarp-build-tasks/releases/tag/v2.0.0), [PR #11](https://github.com/TimeWarpEngineering/timewarp-build-tasks/pull/11), compare `v1.0.0...v2.0.0`): retargets `GitMetadataTask` from `build/netstandard2.0` to `build/net11.0` and `buildTransitive/net11.0` against `Microsoft.Build.Framework` / `Microsoft.Build.Utilities.Core` `18.12.2`, and emits `TWBT0001` when MSBuild is not running on .NET 11+ (`MSBuildRuntimeType != 'Core'` or runtime `< 11.0`). `timewarp-terminal` pins `.NET SDK 11.0.100` in `global.json` and builds exclusively via the `dotnet` CLI locally and in CI (`actions/setup-dotnet@v4` with `global-json-file: global.json`), so `GitMetadataTask` loads cleanly and continues to inject `CommitHash` and `CommitDate` `AssemblyMetadata` attributes into `TimeWarp.Terminal` and `TimeWarp.Terminal.Layout`. `1.0.2` also dropped `TimeWarp.SourceGenerators` from `timewarp-build-tasks` itself; `TimeWarp.Build.Tasks` ships no analyzers of its own.
  - `TimeWarp.SourceGenerators` `1.0.0-beta.14` ([release v1.0.0-beta.14](https://github.com/TimeWarpEngineering/timewarp-source-generators/releases/tag/v1.0.0-beta.14), compare `v1.0.0-beta.13...v1.0.0-beta.14`): moves the upstream repo/tests onto the `.NET 11.0` GA SDK while keeping the shipped `netstandard2.0` analyzer/generator surface identical to `1.0.0-beta.13`.
- Zero new compiler or analyzer diagnostics surfaced across `source/`, `samples/`, `tests/`, or `tools/dev-cli/`. No suppressions were added and `.timewarp/suppression-baseline.json` remains unchanged at 97 suppressions.
- Ran `ganda repo audit --fix` to populate the gitignored worktree `.vscode/settings.json` for `vscode-window-icon` (boy-scout rule). No release was cut.

### Gate commands and outcomes

- `ganda nuget outdated` -> exit 0 (`All packages are up to date!` across all 15 packages in `Directory.Packages.props`).
- `dotnet build timewarp-terminal.slnx -c Release -p:ContinuousIntegrationBuild=true --no-incremental` -> exit 0 (`0 Warning(s)`, `0 Error(s)`), packed `TimeWarp.Terminal.2.0.0-beta.1.{nupkg,snupkg}` and `TimeWarp.Terminal.Layout.2.0.0-beta.1.{nupkg,snupkg}` with `CommitHash` and `CommitDate` assembly metadata injected by `TimeWarp.Build.Tasks` `2.0.0`.
- `dotnet run --file tools/dev-cli/dev.cs -- self-install` -> exit 0 (AOT-published `./bin/dev` cleanly under the updated pins).
- `./bin/dev workflow` -> exit 0 (`Step 1/4: Clean`, `Step 2/4: Build` with `0 Warning(s)` and `0 Error(s)`, `Step 3/4: Verify Samples` across all 6 sample runfiles, `Step 4/4: Test` with `All 47 test file(s) passed`, `Coverage written to artifacts/coverage/coverage.cobertura.xml` at 77.2% line coverage, and `Package integrity verified: 4 packages unchanged since build.`).
- `ganda repo audit` -> exit 0 (`Passed: 42 | Failed: 0 | Skipped: 2` — `Repository passes all audit checks.`).

### Review disposition

- Rounds: 1. Effort 1, roster `general`.
- Final counts: bug 0, suggestion 0, nit 0, all statuses 0. No findings were raised.
- Disposition: `clean`. No wontfix and no escalations.
- Goal restatement matches the original task (no misread).
- Artifacts: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md`.

### How to validate

Smoke:

```bash
ganda nuget outdated
dotnet build timewarp-terminal.slnx -c Release -p:ContinuousIntegrationBuild=true --no-incremental
./bin/dev workflow
ganda repo audit
```

Expect:

- `ganda nuget outdated` exits 0 and prints `All packages are up to date!`.
- `dotnet build timewarp-terminal.slnx -c Release -p:ContinuousIntegrationBuild=true --no-incremental` exits 0 with `0 Warning(s)` and `0 Error(s)`.
- `./bin/dev workflow` exits 0: cleans, builds Release packages, verifies all 6 samples, passes all 47 Jaribu test runfiles, writes `artifacts/coverage/coverage.cobertura.xml`, and prints `Package integrity verified: 4 packages unchanged since build.`.
- `ganda repo audit` exits 0 with `Passed: 42 | Failed: 0 | Skipped: 2`.

## Review

- Round 1: Done

## Notes

- TimeWarp packages: https://nuget.pkg.github.com/TimeWarpEngineering/index.json as well as nuget.org.
- Do not cut a release.
