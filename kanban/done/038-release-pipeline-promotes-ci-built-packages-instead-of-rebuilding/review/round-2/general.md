# Round 2 — general
**Date:** 2026-11-09
**Scope reviewed:** fix delta `ede47ae..6336379` (commit `6336379`, "fix: pack only Release builds so CI cannot promote Debug packages"). Product files: `Directory.Build.props`, `source/Directory.Build.props`, `tools/dev-cli/endpoints/workflow.cs`, `.github/workflows/workflow.yml` (comment only), `documentation/developer/guides/releasing.md`. Also re-checked round 1 M1.

## Summary

The fix takes the unconditional `GeneratePackageOnBuild` out of the root props. It adds `IsPackable=true` and a Release-only `GeneratePackageOnBuild` in `source/Directory.Build.props`, which matches the timewarp-nuru template. The merge pipeline (`dev workflow`, which CI runs at `workflow.yml:87`) now does three things. It hashes `artifacts/packages/*.nupkg|*.snupkg` right after the Release build. It requires at least one nupkg and one snupkg. After samples and tests, it fails if any package changed, appeared, or disappeared. The docs and comments describe this. M1 is resolved, and I found no new defects in the delta.

## Verification (re-run by this reviewer)

- `dotnet build source/timewarp-terminal-layout/timewarp-terminal-layout.csproj -c Release -p:ContinuousIntegrationBuild=true` wrote 4 packages, recorded as sha256 hashes: Terminal nupkg `46869c0a…`, Terminal snupkg `be443ace…`, Layout nupkg `0a3f67cb…`, Layout snupkg `c91bb8e4…`.
- `dotnet run samples/rule-widget.cs` (exit 0) and then `dotnet run samples/layout-dashboard.cs` (exit 0) rebuilt both source projects in Debug. The `bin/Debug` and `obj/Debug` DLLs for both projects are newer than the hash snapshot. All 4 package hashes stayed the same. In round 1, the same `rule-widget.cs` run rewrote the nupkg from `bin/Debug`.
- `dotnet build tools/dev-cli/dev.cs` exited 0, so the new `workflow.cs` code compiles under the repo analyzers with warnings as errors. A Debug dev-cli build also left the hashes the same.
- Projects under `samples/`, `tests/`, and `tools/dev-cli/` still set `IsPackable=false`. Only the two `source/` projects pack, and both csproj files keep `IncludeSymbols` and `snupkg`.

## Issues

None.

## Observations (not findings)

- `tools/dev-cli/Directory.Build.props:6` still says "Parent GeneratePackageOnBuild must not pack it", but the parent no longer sets that property. The explicit `false` is harmless defense in depth. The comment can be reworded the next time someone edits that file.
- `RequireReleasePackages` checks that at least one nupkg and one snupkg exist, not that every packable project produced one. The release-side `verify` step already checks the package set, so a partial pack cannot ship.

## Prior findings

- M1 (bug): **fixed**. Verified as described above.
