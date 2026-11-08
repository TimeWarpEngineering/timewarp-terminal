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

## Checklist

- [ ] Compare `workflow.yml` with the reference template; identify the promotion jobs/steps missing here
- [ ] Master push CI uploads a `Packages-*` artifact for packable projects
- [ ] `release: published` run locates the green CI run for the tag commit, downloads its artifact, verifies the package set/version, and pushes it via OIDC (no build/test in the release run)
- [ ] Symbol packages pushed exactly once (no `Conflict` on `symbolpackage`)
- [ ] Probe still works: `gh workflow run workflow.yml -f mode=probe` logs in via OIDC
- [ ] Break-glass `workflow_dispatch` (`mode: release`, `confirm: release`) still guarded by the tag-pin gate
- [ ] Update `documentation/developer/guides/releasing.md` if behaviour changes
- [ ] `ganda repo audit` clean; dev build/test pass

## Session

- Created: 271402fa-1014-4c38-986b-db3f26ab9720 (2026-11-09), follow-up from the v2.0.0-beta.1 release

## Notes

- Do not cut a release to test this; validate on the next real release, or with `mode=probe` plus a dry review of the job graph.
- Related open item (not in scope): `global.json` and NetAnalyzers are pinned to .NET 11 RC (`11.0.100-rc.3`); move to stable 11.0.100 after GA.
