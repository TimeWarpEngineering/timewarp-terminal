# Round 1 — merged findings
**Date:** 2026-11-09
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 1 | 0 | 0 |
| suggestion | 0 | 0 | 0 |
| nit | 0 | 0 | 0 |

## Issues

### M1 — Severity: bug — Status: open
- File: Directory.Build.props:11
- Description: `GeneratePackageOnBuild` is unconditionally `true`. After the Release build, master CI runs `verify-samples` and the test runfiles, which `#:project` the source projects in Debug. Those Debug builds repack `TimeWarp.Terminal` and `TimeWarp.Terminal.Layout` at the same version over the Release nupkg and snupkg in `artifacts/packages`. CI uploads those Debug packages as `Packages-*`, and the release job promotes them to nuget.org. This is the same defect timewarp-nuru fixed in task 503. Reproduced: a Release build gave a nupkg of 72952 bytes (`05dac55b…`). Running `dotnet run samples/rule-widget.cs` rewrote it to 75637 bytes (`2f76a102…`) from `bin/Debug`.
- Suggestion: Make packing Release-only, the way the template does it (`<GeneratePackageOnBuild Condition="'$(Configuration)' == 'Release'">true</GeneratePackageOnBuild>`, preferably in `source/Directory.Build.props`). Optionally, have CI fail if the package hashes change between the build step and the end of the test step. Re-verify by running a layout sample and one test after a Release build and confirming the hashes are unchanged.
- Source: general
- Disposition notes:

## Duplicates / conflicts

- None (single reviewer).
