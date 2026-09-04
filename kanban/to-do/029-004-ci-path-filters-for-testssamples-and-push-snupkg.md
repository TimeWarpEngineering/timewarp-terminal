# CI path filters for tests/samples and push snupkg

## Description

Parent **029** round-1 merged findings **M9, M10, M20, M21**.

CI path filters omit `tests/` and `samples/`, so tests-only or samples-only PRs skip the workflow. Pack produces `.snupkg` but the release push and artifact upload only glob `*.nupkg` — NuGet symbolpackage 1.0.0 and 1.0.1 both 404. Also fix the kebab-rename pack Include (`README.md` vs on-disk `readme.md`) and the stale `dev.cs` release banner.

Do **not** create a sibling “apply 029 findings” task. This child is the product-fix batch.

## Requirements

### M9 — bug — CI path filters omit `tests/` and `samples/`
- File: `.github/workflows/workflow.yml:7-12` (PR block `:16-21`)
- Add `'tests/**'` and `'samples/**'` to both push and pull_request `paths` lists. Consider `msbuild/**`.

### M10 — bug — `.snupkg` packed but never pushed
- File: `tools/dev-cli/endpoints/workflow.cs:237`
- Pack already emits snupkg (`timewarp-terminal.csproj:12-13`, `ContinuousIntegrationBuild=true` at `:221`). Push loop is `*.nupkg` only; artifact upload `.github/workflows/workflow.yml:94` likewise.
- Confirmed 2026-09-04: `https://www.nuget.org/api/v2/symbolpackage/TimeWarp.Terminal/1.0.0` and `…/1.0.1` HTTP 404.
- Also push `*.snupkg` to NuGet’s symbol source and include `*.snupkg` in the Actions artifact path.

### M20 — nit — pack Include still `../../README.md`
- File: `source/timewarp-terminal/timewarp-terminal.csproj:26`
- On-disk file is `readme.md` (task 025 kebab rename). Change Include to `../../readme.md` and set `PackagePath="README.md"` so the package entry stays `README.md`.

### M21 — nit — `dev.cs` release banner stale
- File: `tools/dev-cli/dev.cs:28`
- Banner says `build -> check-version -> pack -> push`. Actual release path is clean → build → verify-samples → test → check-version → pack → push (`workflow.cs:10`).

## Checklist

- [ ] M9 tests/** and samples/** in both CI path filters
- [ ] M10 snupkg pushed and uploaded; document how to prove on the next release (do not publish a dummy package)
- [ ] M20 csproj Include matches `readme.md`; pack still embeds PackageReadmeFile README.md
- [ ] M21 dev.cs banner matches the real pipeline
- [ ] A tests-only path change would now match the workflow `on.pull_request.paths` list (inspect YAML; do not need a live PR)

## Notes

- Parent: `kanban/in-progress/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already added verify-samples+test to the release pipeline and snupkg *production*. Do not remove those; finish the push.

## Session

- Created: 3366037 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
