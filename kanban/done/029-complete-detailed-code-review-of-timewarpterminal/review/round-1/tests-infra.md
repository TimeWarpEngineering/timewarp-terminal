# Round 1 — tests-infra
**Date:** 2026-09-04
**Scope reviewed:** tests/, samples/, tools/dev-cli/, .github/workflows/workflow.yml, packaging/AOT/snupkg, 1.0.1 delta

## Summary

CI path filters on push/PR omit `tests/` and `samples/`, so a tests-only or samples-only PR skips the workflow entirely even though `dev workflow` runs both suites when CI does fire. Symbol packages are produced (`IncludeSymbols` + `snupkg` + `ContinuousIntegrationBuild`) but the release push and artifact upload only glob `*.nupkg`, and NuGet returns 404 for `TimeWarp.Terminal` 1.0.0/1.0.1 symbol packages — 022’s snupkg intent is still incomplete. All 33 `tests/*.cs` runfiles are picked up by `dev test` / workflow’s TopDirectoryOnly `*.cs` glob; packaging still has PackageReadmeFile, AOT analyzers without library IL NoWarn, and version 1.0.1. The 1.0.1 bump itself is pins/CI-probe/kebab-rename plus a version tick — no library API delta vs 1.0.0.

## Issues

### Issue 1 — Severity: bug
- File: .github/workflows/workflow.yml:7-12
- Description: Push and pull_request `paths` filters list only `source/**`, `tools/**`, `.github/workflows/**`, `Directory.Build.props`, and `Directory.Packages.props`. They do **not** include `tests/` or `samples/` (same omission on the pull_request block at lines 16–21). A PR that changes only `tests/**` or only `samples/**` will not trigger CI, so `dev workflow`’s verify-samples and test steps never run for that change. Release events have no path filter and still exercise both suites.
- Suggestion: Add `'tests/**'` and `'samples/**'` to both the push and pull_request `paths` lists (and consider `msbuild/**` if repository props edits should also force CI).
- Status: open

### Issue 2 — Severity: bug
- File: tools/dev-cli/endpoints/workflow.cs:237
- Description: Pack correctly emits a `.snupkg` (`timewarp-terminal.csproj:12-13` with `IncludeSymbols`/`SymbolPackageFormat=snupkg`, pack at workflow.cs:221 with `ContinuousIntegrationBuild=true`). The push loop only enumerates `Directory.GetFiles(artifactsDir, "*.nupkg")`, so sibling `.snupkg` files are never pushed. Artifact upload in `.github/workflows/workflow.yml:94` likewise only uploads `artifacts/packages/*.nupkg`. Confirmed against NuGet: `https://www.nuget.org/api/v2/symbolpackage/TimeWarp.Terminal/1.0.0` and `…/1.0.1` both 404, and flat-container `.snupkg` URLs 404. 022 fixed production of snupkg; symbols still are not published.
- Suggestion: Also push `*.snupkg` (or a combined glob that includes them) to NuGet’s symbol endpoint/source, and include `*.snupkg` in the Actions artifact path.
- Status: open

### Issue 3 — Severity: nit
- File: source/timewarp-terminal/timewarp-terminal.csproj:26
- Description: After task 025’s kebab rename (`README.md` → `readme.md`), the pack Include is still `../../README.md` while the on-disk file is `readme.md`. `PackageReadmeFile` remains `README.md` (the in-package name), and `dotnet pack` currently still embeds the readme (NuGet resolves the wrong-cased source path on this Linux host; nuspec `<readme>README.md</readme>` is present for published 1.0.0/1.0.1). Fragile vs a strictly case-sensitive open.
- Suggestion: Change the Include to `../../readme.md` and set `PackagePath="README.md"` (or `Link="README.md"`) so the source path matches the filesystem while the package entry stays `README.md`.
- Status: open

### Issue 4 — Severity: nit
- File: tools/dev-cli/dev.cs:28
- Description: Header comment documents release as `build -> check-version -> pack -> push`, omitting `clean`, `verify-samples`, and `test`. That contradicts `tools/dev-cli/endpoints/workflow.cs:10` and `RunReleaseWorkflowAsync`, which run clean → build → verify-samples → test → check-version → pack → push.
- Suggestion: Update the `dev.cs` banner to match the real release pipeline order.
- Status: open
