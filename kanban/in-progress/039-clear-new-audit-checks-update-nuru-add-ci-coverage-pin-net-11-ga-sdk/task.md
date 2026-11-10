# Clear new audit checks, update Nuru, add CI coverage, pin .NET 11 GA SDK

## Goal (implementer restatement)

Bring `timewarp-terminal` to a clean `ganda repo audit` and pin the .NET 11.0.100 GA SDK in a single PR without cutting a release.

Acceptance:

- `ganda repo audit --fix` policy updates land: `TimeWarp.Nuru` / `TimeWarp.Nuru.DevCli` `4.0.0-beta.12` (with `tools/dev-cli` handlers updated to `IRequestHandler` returning `ValueTask<Unit>`), `TimeWarp.Amuru` / `TimeWarp.Amuru.Tools` `3.0.0-beta.2`, `.timewarp/suppression-baseline.json` seeded, `/.timewarp/signals/` ignored in `.gitignore`, and the five `.githooks/*.cs` files refreshed to the attest baseline.
- `.github/workflows/workflow.yml` sets top-level `defaults.run.shell` to `pwsh` and every `run:` step uses PowerShell, including the `Keep last two Packages artifacts` prune step and the `release` job steps. The release job continues to promote the `Packages-*` artifact from the green master run (`tag-gate -> check-version -> locate-run -> download-artifact -> verify -> push`) without rebuilding.
- `dev test --coverage` (invoked by `dev workflow`) collects Cobertura code coverage across the Jaribu runfile suite using local tool `dotnet-coverage` (`.config/dotnet-tools.json`) into `artifacts/coverage/coverage.cobertura.xml`, and the `ci` job uploads `artifacts/coverage` as artifact `coverage` while keeping `ci-code-coverage` enabled.
- `global.json` (`allowPrerelease: false`) and `Microsoft.CodeAnalysis.NetAnalyzers` pin stable `11.0.100` GA, and `actions/setup-dotnet@v4` with `global-json-file: global.json` resolves it from the active `11.0` channel.
- Remaining `ganda nuget outdated` updates (`TimeWarp.Builder` `1.1.0` and `TimeWarp.SourceGenerators` `1.0.0-beta.13`, adding `#region Purpose` headers where needed for `TWA0004` with no new suppressions) are applied so no outdated packages remain.
- `dev workflow` (clean, Release build, verify-samples, test with coverage, package-integrity hash gate) and `ganda repo audit` exit 0 locally.

## Description

`ganda repo audit` on master (2026-11-11) fails 6 checks. Five are new checks, and one is a new
Nuru release. .NET 11 went GA on 2026-11-10, so also move the RC pins to stable. Ship all of it in one PR.

## Checklist

- [x] Run `ganda repo audit --fix` and commit what it changes (audit fixes are policy):
      `nuru` (TimeWarp.Nuru 4.0.0-beta.11 -> 4.0.0-beta.12), `analyzer-suppressions`
      (seeds `.timewarp/suppression-baseline.json`), `timewarp-signals-gitignore`
      (`/.timewarp/signals/`), `memsearch-scaffold` (refresh the 5 `.githooks/*.cs` hooks)
- [x] `workflow-pwsh-shell` (no auto-fix): add top-level `defaults: run: shell: pwsh` to
      `.github/workflows/workflow.yml` and convert every bash `run:` step to pwsh, including
      job `ci` step "Keep last two Packages artifacts" (task 038)
- [x] `ci-code-coverage` (no auto-fix): collect coverage in the CI test run and upload it.
      The audit suggests MTP flags `--results-directory artifacts/coverage --coverage
      --coverage-output-format cobertura --coverage-output coverage.cobertura.xml` plus an
      `actions/upload-artifact@v4` step named `coverage` for `artifacts/coverage`.
      Tests here are Jaribu runfiles run by `dev test`, not `dotnet test`; pick the approach
      that fits and makes the check pass. Do not turn the check off without a stated reason.
- [x] Move `global.json` SDK and `Microsoft.CodeAnalysis.NetAnalyzers` from the .NET 11 RC
      (`11.0.100-rc.3...`) to the stable 11.0.100 GA versions. Check that CI's setup-dotnet resolves them.
- [x] Run `ganda nuget outdated` and take any other updates
- [x] `dev workflow` passes locally (build, verify-samples, test, package-hash gate)
- [x] `ganda repo audit` is clean

## Session

- Created: 271402fa-1014-4c38-986b-db3f26ab9720 (2026-11-11)
- Implementation: implementer-cursor (2026-11-11)
- Implement oracle: implemented by implementer-cursor (cursor, vendor google, model gemini-4-carbon-high), session not reported, max-turns uncapped (no --max-turns flag; bound 500), Done — 2026-11-10T19:45:14Z

## Results

- Applied `ganda repo audit --fix` updates: `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` moved to `4.0.0-beta.12`, `TimeWarp.Amuru` and `TimeWarp.Amuru.Tools` moved to `3.0.0-beta.2`, `.timewarp/suppression-baseline.json` was seeded and lowered from 107 to 97 suppressions, `/.timewarp/signals/` is ignored in `.gitignore`, and all five `.githooks/*.cs` files match the attest baseline. Also removed the obsolete `dev-cli-capabilities.severity = off` override in `.editorconfig` now that Nuru `4.0.0-beta.12` emits valid capabilities JSON.
- Migrated `tools/dev-cli` handlers (`BuildCommand`, `VerifySamplesCommand`, `TestCommand`, `WorkflowCommand`) to Nuru `4.0.0-beta.12`'s `IRequestHandler<T, Unit>` returning `ValueTask<Unit>`. Added `ExemptPackageContentFromAnalyzers` to `tools/dev-cli/Directory.Build.props` (matching `timewarp-architecture` task 470) so `TimeWarp.Nuru.DevCli` content files compiled from `$(NuGetPackageRoot)` do not trip `TWA0038` or style rules, and dropped 10 redundant IDE entries from `<NoWarn>`.
- Converted `.github/workflows/workflow.yml` to PowerShell (`defaults.run.shell: pwsh`), including `Keep last two Packages artifacts` in job `ci` and the break-glass/probe/release `run:` steps in job `release`. Added `.config/dotnet-tools.json` to the push/PR path allowlists. Verified all 6 `run:` blocks parse cleanly via the PowerShell AST parser, verified the prune script against a mock `gh` CLI (both listed and not-yet-listed cases), and verified `dev workflow --mode release` still runs the promotion gate (`tag-gate -> check-version -> locate-run -> download-artifact -> verify -> push`) without rebuilding.
- Added Cobertura code coverage collection for the Jaribu runfile suite: pinned `dotnet-coverage` `18.12.0` in `.config/dotnet-tools.json`, added `tests/coverage.runsettings` scoping instrumentation to `TimeWarp.Terminal` and `TimeWarp.Terminal.Layout`, added `--coverage` to `dev test` (invoked by `dev workflow`) to wrap the full 47-runfile suite in a single `dotnet-coverage collect` session producing `artifacts/coverage/coverage.cobertura.xml` (~77.2% line coverage locally), and added the `coverage` `actions/upload-artifact@v4` step in job `ci`. `ci-code-coverage` stays enabled and passes.
- Pinned stable `.NET 11.0.100` GA in `global.json` (`allowPrerelease: false`, `rollForward: latestFeature`) and `Microsoft.CodeAnalysis.NetAnalyzers` `11.0.100`. Verified the public `.NET 11.0` `releases-index.json` and `releases.json` metadata report `support-phase: active` and `latest-sdk: 11.0.100`, which `actions/setup-dotnet@v4` with `global-json-file: global.json` resolves directly.
- Applied all remaining `ganda nuget outdated` updates: `TimeWarp.Builder` `1.0.0 -> 1.1.0` and `TimeWarp.SourceGenerators` `1.0.0-beta.11 -> 1.0.0-beta.13`. Added `#region Purpose` blocks to the 14 `source/`, 6 `samples/`, and 46 `tests/` C# files that lacked them so `TWA0004` passes cleanly with zero added suppressions. `ganda nuget outdated` reports all 15 packages up to date.
- `./bin/dev self-install`, `./bin/dev workflow`, and `ganda repo audit` (42 passed, 0 failed, 2 skipped) all exit 0 locally. No release was cut.

### How to validate

Smoke:

```bash
dotnet --version
ganda nuget outdated
./bin/dev workflow
ganda repo audit
```

Expect:

- `dotnet --version` prints `11.0.100`.
- `ganda nuget outdated` exits 0 and prints `All packages are up to date!`.
- `./bin/dev workflow` exits 0: cleans, builds `TimeWarp.Terminal` and `TimeWarp.Terminal.Layout` in Release with `0 Warning(s)` and `0 Error(s)`, verifies all 6 samples, passes all 47 Jaribu test runfiles, writes `artifacts/coverage/coverage.cobertura.xml` covering `TimeWarp.Terminal` and `TimeWarp.Terminal.Layout`, and prints `Package integrity verified: 4 packages unchanged since build.`.
- `ganda repo audit` exits 0 with `Passed: 42 | Failed: 0 | Skipped: 2`, including `ci-code-coverage`, `workflow-pwsh-shell`, `nuru`, `analyzer-suppressions`, `timewarp-signals-gitignore`, `memsearch-scaffold`, and `dev-cli-capabilities`.

## Notes

- The release pipeline from task 038 must keep working: master CI uploads `Packages-*`, and the
  release job promotes it without rebuilding. Converting steps to pwsh must not break the
  artifact retention step or the tag-gate, locate-run, download, verify, and push steps.
- Do not cut a release.
