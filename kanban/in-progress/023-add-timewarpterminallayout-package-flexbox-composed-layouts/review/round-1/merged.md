# Round 1 — merged findings
**Date:** 2026-09-18
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 1 | 0 |
| suggestion | 0 | 5 | 0 |
| nit | 0 | 0 | 0 |

## Issues

### M1 — Severity: bug — Status: fixed
- File: tools/dev-cli/endpoints/workflow.cs:245
- Description: Partial-publish resume is broken. `available` / `alreadyPublished` are computed and the log claims "resume/--skip-duplicate semantics", but Step 6 packs every packable project and the push loop uploads every `*.nupkg` in `artifacts/packages` with no `--skip-duplicate` and without filtering to `available`. Repo version is 1.0.1 with TimeWarp.Terminal already on nuget.org; pushing `TimeWarp.Terminal.1.0.1.nupkg` will conflict and abort before Layout can publish. Confirmed: no `SkipDuplicate` / `--skip-duplicate` anywhere in the repo; `available` is unused after the check-version messages.
- Suggestion: Pack and push only packages in `available`. Also call Amuru `WithSkipDuplicate()` on nuget push so leftover nupkgs in `artifacts/packages` cannot abort a resume.
- Source: general
- Disposition notes: Pack/push iterate `available` only (`{PackageId}.{version}.nupkg`); `WithSkipDuplicate()` on push.

### M2 — Severity: suggestion — Status: fixed
- File: tools/dev-cli/endpoints/workflow.cs:320
- Description: `NotifySoftwareSiteAsync` still hardcodes `client_payload[package]=TimeWarp.Terminal`. A Layout-only (or multi-package) release will not notify the site about `TimeWarp.Terminal.Layout`.
- Suggestion: Dispatch once per successfully pushed package id using the packable-project list / `available` set.
- Source: general
- Disposition notes: Notify takes `packageId` and is called after each successful push.

### M3 — Severity: suggestion — Status: fixed
- File: source/timewarp-terminal-layout/widget-measure.cs:95
- Description: `WidgetMeasure.MeasureHeight` has no references. Height is computed inline in `LayoutEngine.ApplyHeightOverrides` instead, so the helper is dead code.
- Suggestion: Delete `MeasureHeight`, or call it from `ApplyHeightOverrides` so height policy lives in one place.
- Source: general
- Disposition notes: `MeasureHeight` deleted.

### M4 — Severity: suggestion — Status: fixed
- File: tests/Directory.Build.props:37
- Description: Every test under `tests/` now gets a ProjectReference to `timewarp-terminal-layout` and a global `Using` for `TimeWarp.Flexbox`. Non-layout runfiles inherit a Flexbox compile dependency they do not need; layout runfiles already pin `#:project …/timewarp-terminal-layout.csproj`.
- Suggestion: Remove the layout ProjectReference and Flexbox Using from `tests/Directory.Build.props`. Keep layout tests on their `#:project` pin; add `using TimeWarp.Flexbox;` in layout tests that need `FlexDirection` / `Wrap`.
- Source: general
- Disposition notes: Global layout ref / Flexbox using removed; layout-*.cs add `using TimeWarp.Flexbox;`.

### M5 — Severity: suggestion — Status: fixed
- File: tests/layout-09-terminal-extensions.cs:21
- Description: WriteLayout’s SupportsColor-gated color overload is implemented and matches WritePanel/WriteTable, but layout-09 only covers WindowWidth write + static facade — no color / SupportsColor=false coverage for the new overload.
- Suggestion: Add tests that assert ANSI color prefixes when SupportsColor is true and plain output when false (mirror `tests/terminal-static-06-color.cs` widget gating).
- Source: general
- Disposition notes: Added color-supported and SupportsColor=false tests on WriteLayout.

### M6 — Severity: suggestion — Status: fixed
- File: source/timewarp-terminal-layout/layout-canvas.cs:68
- Description: When `leaf.Box.X < cursor`, ComposeRow silently `continue`s and drops that leaf’s cells. Under the documented tiling contract this path should be unreachable; if a future engine regression overlaps boxes, output clips without failing.
- Suggestion: Throw `InvalidOperationException` (same spirit as `ToCell`) when overlap is detected so contract breaks surface in tests instead of silent clipping.
- Source: general
- Disposition notes: Overlap now throws; Design region updated.

## Duplicates / conflicts

- None (single general reviewer).
