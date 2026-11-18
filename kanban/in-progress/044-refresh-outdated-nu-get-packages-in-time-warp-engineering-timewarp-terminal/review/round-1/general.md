# Round 1 — general
**Date:** 2026-11-19
**Scope reviewed:** branch `task/044-refresh-outdated-nu-get-packages-in-time-warp` vs merge-base `2a58e31`. Product diff: `Directory.Packages.props` and the six `tools/dev-cli/endpoints` files. Kitchen `## Results` and `### How to validate`.

## Summary

The six dev-only pins move to the versions the task names (Amuru and Amuru.Tools 3.0.0-beta.10, Jaribu 1.0.0-beta.18, Nuru and Nuru.DevCli 4.0.0-beta.16, SourceGenerators 1.0.0-beta.17). `source/Directory.Build.props` `<Version>` stays 2.0.0-beta.2. Amuru 3.0.0-beta.10's AMURU0001 analyzer rejects `Shell.Builder("dotnet")` and `Shell.Builder("gh")` where a typed builder exists, so the dev-cli call sites moved to `DotNet.*` and `Gh.*` with no suppression. Remaining `Shell.Builder` calls are `git`, which the analyzer does not flag.

Recorded gates cover the bump: `./bin/dev build` (0 warnings, 0 errors), dev-cli self-install, `./bin/dev verify-samples` (6/6), one Jaribu file (10/10), and `ganda repo audit` (pass after the ignored vscode advisory fix). I did not re-run those gates. I decompiled TimeWarp.Amuru.Tools 3.0.0-beta.10 (`ilspycmd` on `DotNetToolRunBuilder`, `DotNetRunBuilder`, `DotNetNuGetPushBuilder`, `GhRunListBuilder`, `GhJsonCommandBuilder`, `GhListCommandBuilder`, and `GhApiBuilder`) because the coverage, nuget push, and gh dispatch lines could have dropped or doubled a separator. The assembled arguments match the previous commands: coverage keeps a single `--` before `collect`, `dotnet run --file` inserts `--` before program arguments, nuget push keeps `--source`, `--api-key`, and `--skip-duplicate` with the parent working directory, and `gh` keeps `--json a,b`, `-f key=value`, and no extra `--limit`.

## Issues

None.
