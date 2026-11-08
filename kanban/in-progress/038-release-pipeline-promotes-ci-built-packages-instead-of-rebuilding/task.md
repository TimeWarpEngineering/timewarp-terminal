# Release pipeline promotes CI-built packages instead of rebuilding

## Description

The v2.0.0-beta.1 release (2026-11-08, release run 42511233300) rebuilt and re-tested the
packages inside the `release: published` run, then pushed them. The TimeWarp release
convention (tw-release skill, `documentation/developer/guides/releasing.md`) is that the
release run **promotes** the `Packages-*` artifact from the green master CI run of the
tagged commit, byte-identical and never rebuilt:
tag-gate → check-version → locate-run → download-artifact → verify → push.

This repo's `.github/workflows/workflow.yml` still uses the older rebuild-on-release path.
Bring it in line with the current TimeWarp workflow template (compare timewarp-nuru's
`workflow.yml` as the reference).

Same run: each symbol package push returned `Created` and then `Conflict` on
`/api/v2/symbolpackage/`. The symbols appear to be pushed twice (e.g. `dotnet nuget push`
of the .nupkg auto-pushing the adjacent .snupkg, plus an explicit .snupkg push). Harmless
today, but fix it as part of this change.

## Goal (implementer restatement)

A published release must ship the exact packages the green master CI run already
built and tested for the tagged commit. The release job finds that run, downloads
its `Packages-*` artifact, checks the package set and version, and pushes those
bytes with the OIDC key. It does not build or test again.

Master pushes that touch product paths upload that artifact (nupkgs and snupkgs)
only after CI succeeds. Symbol packages are pushed once each, so NuGet does not
log a second `Conflict` on `symbolpackage`. `mode=probe` still only logs in.
Break-glass publish still requires `mode=release` and `confirm=release`, and the
tag-pin check still refuses a resume from any commit other than the tag.
`documentation/developer/guides/releasing.md` describes this flow. Audit, build,
and tests pass.

The bytes in that artifact must stay the Release build. Sample and test
runfiles reference the source projects and compile them in Debug. Packing
runs only when `Configuration` is Release, on the projects under `source/`.
A Debug build does not write a new nupkg or snupkg over `artifacts/packages`.
After a Release build, a layout sample and a test leave those file hashes
unchanged. The merge pipeline records the hashes after the build and fails
if a later step changes, adds, or removes a package.

## Checklist

- [x] Compare `workflow.yml` with the reference template; identify the promotion jobs/steps missing here
- [x] Master push CI uploads a `Packages-*` artifact for packable projects
- [x] `release: published` run locates the green CI run for the tag commit, downloads its artifact, verifies the package set/version, and pushes it via OIDC (no build/test in the release run)
- [x] Symbol packages pushed exactly once (no `Conflict` on `symbolpackage`)
- [x] Probe still works: `gh workflow run workflow.yml -f mode=probe` logs in via OIDC
- [x] Break-glass `workflow_dispatch` (`mode: release`, `confirm: release`) still guarded by the tag-pin gate
- [x] Update `documentation/developer/guides/releasing.md` if behaviour changes
- [x] `ganda repo audit` clean; dev build/test pass
- [x] Pack only when Configuration is Release; a layout sample and a test leave the Release nupkg and snupkg hashes unchanged

## Session

- Created: 271402fa-1014-4c38-986b-db3f26ab9720 (2026-11-09), follow-up from the v2.0.0-beta.1 release
- Implementation: grok implementer (2026-11-09). Split `workflow.cs` under the 500-line audit cap. `ganda repo audit --fix` wrote the gitignored VS Code window title; that file stays untracked.
- Implement oracle: implemented by implementer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a1bcb7-b804-77a3-a04a-927047f57615, max-turns 500, Done — 2026-11-08T20:24:59Z
- Review oracle round 1: claude-opus-5-5 (Claude Code, headless `ganda task work`), effort 3, roster general, 2026-11-09. Result: 1 open bug (M1), fix pass requested.
- Review oracle: review by reviewer-claude (claude, vendor anthropic, model claude-opus-5-5, response model claude-opus-5-5), session 8997b980-c410-426f-a959-599246d7e665, max-turns 200, artifacts general.md, merged.md — 2026-11-08T20:28:42Z
- Fix pass (review round 1 M1): grok implementer (2026-11-09). `GeneratePackageOnBuild` is Release-only in `source/Directory.Build.props`. The merge pipeline hashes every nupkg and snupkg after the Release build and fails if a later step changes them.

## Results

The release job no longer builds or tests. A master push that matches the path allowlist runs `clean → build → verify-samples → test` with `ContinuousIntegrationBuild`. `GeneratePackageOnBuild` is Release-only, so the Debug sample and test builds do not repack. The merge pipeline then checks that the nupkg and snupkg hashes are unchanged, and uploads `Packages-{run_number}` (each `.nupkg` and `.snupkg`, keep last two, 7-day retention). `release: published` and confirmed break-glass (`mode=release`, `confirm=release`) run `tag-gate → check-version → locate-run → download-artifact → verify → push` and pass the OIDC key. Tag pin refuses a checkout that is not the existing `v{Version}` commit. Each package file is copied into its own directory before `dotnet nuget push`, so a nupkg push cannot also upload the sibling snupkg. `mode=probe` only performs the OIDC login. `documentation/developer/guides/releasing.md` describes the flow.

`ganda repo audit` passes (36 passed, 0 failed, 2 skipped) after the partial-file split, and again after the Release-only packing fix. Release build with `-p:ContinuousIntegrationBuild=true` emitted `TimeWarp.Terminal` and `TimeWarp.Terminal.Layout` at `2.0.0-beta.1`, nupkg and snupkg, 0 warnings. `dev test` passed all 47 files. `dev verify-samples` passed all 6 samples. `workflow --mode release` on this branch exited 1 at tag pin (`v2.0.0-beta.1` is `5744388`, HEAD was `ad7a900`) and did not build.

No release was cut. Probe run [42519838727](https://github.com/TimeWarpEngineering/timewarp-terminal/actions/runs/42519838727) (`mode=probe` on `1de33f1`) succeeded: the release job logged `Trusted publishing OK`, the release pipeline step was skipped, and the ci job was skipped.

### Review

Round 1 (effort 3, roster: general): 1 bug open. Fix pass addressed M1. Round 2 has not run.

- M1 (bug, fixed in this pass): packing is Release-only. `source/Directory.Build.props` sets `GeneratePackageOnBuild` only when `Configuration` is Release, and `IsPackable` is true for source projects. The root props no longer set `GeneratePackageOnBuild`. The merge pipeline (`workflow --mode pr`) hashes every nupkg and snupkg after the Release build and fails if samples or tests change, add, or remove one.
- Check: MSBuild reports `GeneratePackageOnBuild=false` for Debug and `true` for Release on both source projects. A Release build wrote four packages whose assemblies match `bin/Release` and not `bin/Debug`. `samples/layout-dashboard.cs`, `samples/rule-widget.cs`, `tests/layout-01-basic.cs`, Debug builds of both source projects, and a Debug dev-cli run left the four sha256 hashes unchanged. `workflow --mode pr` then ran all 6 samples and all 47 test files and printed `Package integrity verified: 4 packages unchanged since build`.
- Paths: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`.

### How to validate

Smoke:

```bash
rm -rf artifacts/packages
dotnet build timewarp-terminal.slnx -c Release -p:ContinuousIntegrationBuild=true
sha256sum artifacts/packages/*.nupkg artifacts/packages/*.snupkg | sort > /tmp/packages-before.sha
dotnet run --file samples/layout-dashboard.cs
dotnet run --file samples/rule-widget.cs -- --help
dotnet run --file tests/layout-01-basic.cs
dotnet build source/timewarp-terminal/timewarp-terminal.csproj -c Debug
dotnet build source/timewarp-terminal-layout/timewarp-terminal-layout.csproj -c Debug
sha256sum artifacts/packages/*.nupkg artifacts/packages/*.snupkg | sort > /tmp/packages-after.sha
cmp /tmp/packages-before.sha /tmp/packages-after.sha
```

Expect: `cmp` prints nothing and exits 0. `artifacts/packages` contains `TimeWarp.Terminal.{version}.nupkg`, `TimeWarp.Terminal.{version}.snupkg`, `TimeWarp.Terminal.Layout.{version}.nupkg`, and `TimeWarp.Terminal.Layout.{version}.snupkg`. The packed `lib/net11.0` assemblies match the Release build.

```bash
dotnet run --file tools/dev-cli/dev.cs -- workflow --mode release
```

Expect: the command prints `Pipeline: tag-gate -> check-version -> locate-run -> download-artifact -> verify -> push` and exits 1 with `Pipeline ABORTED — tag pin mismatch` when `v{Version}` already points at another commit. It does not print `Step 1/4: Clean`.

After this branch is on the remote:

```bash
gh workflow run workflow.yml --ref task/038-release-pipeline-promotes-ci-built-packages-instea -f mode=probe
```

Expect: the run's release job logs `Trusted publishing OK` and does not run the release pipeline. A real publish is the next `dev release`, not this task.

## Review

- Round 1: open M1, addressed in the fix pass. Round 2 has not run.

## Notes

- Do not cut a release to test this; validate on the next real release, or with `mode=probe` plus a dry review of the job graph.
- Related open item (not in scope): `global.json` and NetAnalyzers are pinned to .NET 11 RC (`11.0.100-rc.3`); move to stable 11.0.100 after GA.
