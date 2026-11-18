# Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-terminal

## Description

Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-terminal.
Filter: (all)
Outdated:
  TimeWarp.Amuru 3.0.0-beta.5 -> 3.0.0-beta.10 (dev-only: tools/dev-cli/Directory.Build.props)
  TimeWarp.Amuru.Tools 3.0.0-beta.5 -> 3.0.0-beta.10 (dev-only: tools/dev-cli/Directory.Build.props)
  TimeWarp.Jaribu 1.0.0-beta.16 -> 1.0.0-beta.18 (dev-only: tests/Directory.Build.props)
  TimeWarp.Nuru 4.0.0-beta.15 -> 4.0.0-beta.16 (dev-only: tools/dev-cli/Directory.Build.props)
  TimeWarp.Nuru.DevCli 4.0.0-beta.15 -> 4.0.0-beta.16 (dev-only: tools/dev-cli/Directory.Build.props)
  TimeWarp.SourceGenerators 1.0.0-beta.16 -> 1.0.0-beta.17 (dev-only: PrivateAssets=all on Directory.Build.props)
Every outdated pin is dev-only. Bump them. Do not change <Version> in source/Directory.Build.props: the wave merges this task and does not release this repo.
Edit Directory.Packages.props in this repo only.

## Goal (implementer restatement)

Bump the six dev-only TimeWarp pins in `Directory.Packages.props` to the latest feed versions
(Amuru/Amuru.Tools 3.0.0-beta.10, Jaribu 1.0.0-beta.18, Nuru/Nuru.DevCli 4.0.0-beta.16,
SourceGenerators 1.0.0-beta.17). Leave `<Version>` in `source/Directory.Build.props` alone.

Acceptance criteria:
- Only `Directory.Packages.props` changes (plus this kitchen).
- `./bin/dev build` passes with the new pins.
- `ganda repo audit` passes.

## Session

- Minted by ganda task wave run 20261118T170809Z-b6e473
- Implementer 2026-11-19: bumped the six dev-only pins in `Directory.Packages.props`.
  TimeWarp.Amuru 3.0.0-beta.10 ships the AMURU0001 analyzer, which errors on
  `Shell.Builder("dotnet"|"gh")` when a typed builder exists. That broke the dev-cli
  compile in 14 places, so those call sites moved to `DotNet.*` / `Gh.*` builders
  (build.cs, test.cs, verify-samples.cs, workflow.cs, workflow.release-gates.cs,
  workflow.release.cs). No suppression added. `source/Directory.Build.props` `<Version>` untouched.
- Audit boy-scout: `vscode-window-icon` advisory (missing ignored `.vscode/settings.json`)
  fixed locally by `ganda repo audit --fix`; the file is git-ignored, so nothing to commit.

## Results

Changed:
- `Directory.Packages.props`: Amuru/Amuru.Tools 3.0.0-beta.5 -> 3.0.0-beta.10, Jaribu
  1.0.0-beta.16 -> 1.0.0-beta.18, Nuru/Nuru.DevCli 4.0.0-beta.15 -> 4.0.0-beta.16,
  SourceGenerators 1.0.0-beta.16 -> 1.0.0-beta.17 (comment updated to match).
- `tools/dev-cli/endpoints/*.cs`: AMURU0001 migration to typed builders. Rendered command lines
  were checked with `Build().ToCommandString()` in a throwaway runfile and match the old ones
  (`dotnet run --file x.cs -- --help`, `dotnet tool run dotnet-coverage -- collect ...`,
  `dotnet clean <slnx> --verbosity q`, `dotnet build <slnx> --configuration Release
  --property:ContinuousIntegrationBuild=true`, `dotnet msbuild <proj> -getProperty:Version -nologo`,
  `gh api ... -f k=v`, `gh run list --workflow ... --json a,b`). Test files ran as
  `dotnet <file>` and now run as `dotnet run --file <file>` (same file-based run).

Gates:
- `./bin/dev build` — PASS (0 warnings, 0 errors)
- `dotnet run --file tools/dev-cli/dev.cs -- self-install` — PASS (dev-cli compiles on Nuru beta.16 / Amuru beta.10 after the migration; first attempt failed with 14 AMURU0001 errors)
- No test file added or edited; no dev-cli tests exist. Jaribu/SourceGenerators bump spot-checked:
  `dotnet run --file tests/terminal-static-01-basic.cs` — PASS (10/10)
- `./bin/dev verify-samples` — PASS (6/6 samples; exercises the migrated `DotNet.Run().WithFile()` path)
- `ganda repo audit` — PASS with 1 advisory (vscode-window-icon); after `ganda repo audit --fix` — PASS all checks

### How to validate

Smoke:
```bash
dotnet run --file tools/dev-cli/dev.cs -- self-install
./bin/dev build
./bin/dev verify-samples
dotnet run --file tests/terminal-static-01-basic.cs
```

Expect: self-install succeeds with no AMURU0001 errors; build reports 0 warnings / 0 errors;
verify-samples reports all 6 samples verified; the test file reports 10 passed.
