# Update all NuGet packages to latest (incl. TimeWarp.Amuru 2.0.0-beta.2)

## Description

Steven wants every repo on the newest packages, pre-releases included (latest, not stable). Run `ganda nuget outdated --update` and take every package to the newest version it can, not just Amuru. TimeWarp.Amuru and TimeWarp.Amuru.Tools must end on 2.0.0-beta.2 or newer.

## Checklist

- [ ] `ganda nuget outdated --dry-run`, then `ganda nuget outdated --update`
- [ ] Bump `#:package ...@version` pins in runfiles; .githooks are ganda-owned, refresh via `ganda hooks install attest` instead of editing
- [ ] Fix Amuru 1.x to 2.0 breaking changes (timewarp-amuru documentation/release-notes/2.0.0.md): Git *Master* helpers removed (use *Default*), Git methods return result objects not bool, no "master" default branchName, removed/renamed dotnet builder options, WithStandardInput("") closes stdin
- [ ] Fix other breaking changes from bumped packages
- [ ] Build warning-free, tests green, `ganda repo audit` clean
- [ ] One PR (with proof: build/test output) and merge via `ganda pr merge`

## Session

- Created: 1428834 (2026-10-09)

## Notes

Filed 2026-10-09 at Steven's request (Amuru 2.0 sweep).

Terminal-specific: Amuru 2.0.0-beta.2 itself depends on TimeWarp.Terminal 1.0.2, so check for a package cycle before bumping Amuru here. In this repo Amuru/Amuru.Tools are only used by tools/dev-cli and hooks, not by the shipped TimeWarp.Terminal package, so the cycle is build-tooling only; confirm restore resolves cleanly. If it can't move, record why instead of forcing it.

Observed 2026-10-09: on master, `.githooks/post-merge` fails to compile (`error CS0103: The name 'Git' does not exist in the current context`) after `git pull` on TWE-001 WSL. Likely the hook runfile pins an Amuru version mismatch; refresh hooks with `ganda hooks install attest` as part of this task.
