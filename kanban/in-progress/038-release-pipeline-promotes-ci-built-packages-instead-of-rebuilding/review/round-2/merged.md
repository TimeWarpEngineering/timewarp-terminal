# Round 2 — merged findings
**Date:** 2026-11-09
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 1 | 0 |
| suggestion | 0 | 0 | 0 |
| nit | 0 | 0 | 0 |

## Issues

### M1 — Severity: bug — Status: fixed
- File: Directory.Build.props:11 (round 1), source/Directory.Build.props:13 (fix)
- Description: Unconditional `GeneratePackageOnBuild` let Debug sample and test builds repack over the Release nupkg and snupkg that CI uploads and the release job promotes.
- Suggestion: Pack only Release builds and gate CI on unchanged package hashes.
- Source: general (round 1)
- Disposition notes: Fixed in `6336379`. `GeneratePackageOnBuild` is now Release-only in `source/Directory.Build.props`, and the root props no longer set it. `dev workflow` hashes the packages after the build and fails if a later step changes, adds, or removes one. Round 2 re-verified this. After a Release build, `samples/rule-widget.cs` and `samples/layout-dashboard.cs` rebuilt both source projects in Debug, and all 4 package hashes stayed the same.

## New findings

None.

## Duplicates / conflicts

- None (single reviewer).
