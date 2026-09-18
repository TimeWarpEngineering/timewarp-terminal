# Round 2 — general
**Date:** 2026-09-18
**Scope reviewed:** post-fix delta vs round-1 (workflow.cs, layout-canvas.cs, widget-measure.cs, tests/Directory.Build.props, tests/layout-*.cs) plus re-verify M1–M6

## Summary

Round-1 M1–M6 all re-verify as fixed in the working tree. Pack/push now iterates `available` with `{PackageId}.{version}.nupkg` and `WithSkipDuplicate()`; site notify is per package id; `MeasureHeight` is gone; layout tests pin `#:project` + `using TimeWarp.Flexbox;` without a global props reference; layout-09 covers SupportsColor true/false; ComposeRow throws on X-overlap. Fix delta introduces no new defects; smoke tests pass.

## Resolved prior

- **M1** — fixed. Pack filters `availableSet`; push uses `{packageId}.{version}.nupkg` + `WithSkipDuplicate()`.
- **M2** — fixed. `NotifySoftwareSiteAsync(repoRoot, packageId, version)` after each successful push; payload uses `{packageId}`.
- **M3** — fixed. `WidgetMeasure.MeasureHeight` deleted; only `ApplyHeightOverrides` remains for height.
- **M4** — fixed. `tests/Directory.Build.props` has no layout ProjectReference / Flexbox Using; layout-*.cs use `#:project` + `using TimeWarp.Flexbox;`.
- **M5** — fixed. layout-09 adds SupportsColor true/false WriteLayout color tests (4/4 pass).
- **M6** — fixed. `leaf.Box.X < cursor` throws `InvalidOperationException`; Design region notes tiling throw.

## Issues

## Smoke tests

- `dotnet tests/layout-09-terminal-extensions.cs` — **pass**
- `dotnet tests/layout-01-basic.cs` — **pass**
- `dotnet tests/panel-widget-01-basic.cs` — **pass**
