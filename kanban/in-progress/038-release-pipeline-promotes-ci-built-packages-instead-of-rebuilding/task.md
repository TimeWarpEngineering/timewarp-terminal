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

## Checklist

- [x] Compare `workflow.yml` with the reference template; identify the promotion jobs/steps missing here
- [x] Master push CI uploads a `Packages-*` artifact for packable projects
- [x] `release: published` run locates the green CI run for the tag commit, downloads its artifact, verifies the package set/version, and pushes it via OIDC (no build/test in the release run)
- [x] Symbol packages pushed exactly once (no `Conflict` on `symbolpackage`)
- [x] Probe still works: `gh workflow run workflow.yml -f mode=probe` logs in via OIDC
- [x] Break-glass `workflow_dispatch` (`mode: release`, `confirm: release`) still guarded by the tag-pin gate
- [x] Update `documentation/developer/guides/releasing.md` if behaviour changes
- [x] `ganda repo audit` clean; dev build/test pass

## Session

- Created: 271402fa-1014-4c38-986b-db3f26ab9720 (2026-11-09), follow-up from the v2.0.0-beta.1 release
- Implementation: grok implementer (2026-11-09). Split `workflow.cs` under the 500-line audit cap. `ganda repo audit --fix` wrote the gitignored VS Code window title; that file stays untracked.
- Implement oracle: implemented by implementer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a1bcb7-b804-77a3-a04a-927047f57615, max-turns 500, Done — 2026-11-08T20:24:59Z

## Results

The release job no longer builds or tests. A master push that matches the path allowlist runs `clean → build → verify-samples → test` with `GeneratePackageOnBuild` and `ContinuousIntegrationBuild`, then uploads `Packages-{run_number}` (each `.nupkg` and `.snupkg`, keep last two, 7-day retention). `release: published` and confirmed break-glass (`mode=release`, `confirm=release`) run `tag-gate → check-version → locate-run → download-artifact → verify → push` and pass the OIDC key. Tag pin refuses a checkout that is not the existing `v{Version}` commit. Each package file is copied into its own directory before `dotnet nuget push`, so a nupkg push cannot also upload the sibling snupkg. `mode=probe` only performs the OIDC login. `documentation/developer/guides/releasing.md` describes the flow.

`ganda repo audit` passes (36 passed, 0 failed, 2 skipped) after the partial-file split. Release build with `-p:ContinuousIntegrationBuild=true` emitted `TimeWarp.Terminal` and `TimeWarp.Terminal.Layout` at `2.0.0-beta.1`, nupkg and snupkg, 0 warnings. `dev test` passed all 47 files. `dev verify-samples` passed all 6 samples. `workflow --mode release` on this branch exited 1 at tag pin (`v2.0.0-beta.1` is `5744388`, HEAD was `ad7a900`) and did not build.

No release was cut. Probe run [42519838727](https://github.com/TimeWarpEngineering/timewarp-terminal/actions/runs/42519838727) (`mode=probe` on `1de33f1`) succeeded: the release job logged `Trusted publishing OK`, the release pipeline step was skipped, and the ci job was skipped.

### How to validate

Smoke:

```bash
dotnet build timewarp-terminal.slnx -c Release -p:ContinuousIntegrationBuild=true
dotnet run --file tools/dev-cli/dev.cs -- workflow --mode release
```

Expect: `artifacts/packages` contains `TimeWarp.Terminal.{version}.nupkg`, `TimeWarp.Terminal.{version}.snupkg`, `TimeWarp.Terminal.Layout.{version}.nupkg`, and `TimeWarp.Terminal.Layout.{version}.snupkg`. The release command prints `Pipeline: tag-gate -> check-version -> locate-run -> download-artifact -> verify -> push` and exits 1 with `Pipeline ABORTED — tag pin mismatch` when `v{Version}` already points at another commit. It does not print `Step 1/4: Clean`.

After this branch is on the remote:

```bash
gh workflow run workflow.yml --ref task/038-release-pipeline-promotes-ci-built-packages-instea -f mode=probe
```

Expect: the run's release job logs `Trusted publishing OK` and does not run the release pipeline. A real publish is the next `dev release`, not this task.

## Notes

- Do not cut a release to test this; validate on the next real release, or with `mode=probe` plus a dry review of the job graph.
- Related open item (not in scope): `global.json` and NetAnalyzers are pinned to .NET 11 RC (`11.0.100-rc.3`); move to stable 11.0.100 after GA.
