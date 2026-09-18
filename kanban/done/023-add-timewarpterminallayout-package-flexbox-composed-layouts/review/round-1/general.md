# Round 1 — general
**Date:** 2026-09-18
**Scope reviewed:** branch task/023-add-timewarpterminallayout-package-flexbox-compose vs origin/master — `source/timewarp-terminal-layout/**` (engine, canvas, builder, measure, extensions, facade, csproj), `tools/dev-cli/endpoints/workflow.cs`, `tests/Directory.Build.props`, `tests/layout-01`…`layout-10`, `source/timewarp-terminal/timewarp-terminal.csproj`, panel/table color-extension call sites, `Directory.Packages.props`

## Summary

Companion package `TimeWarp.Terminal.Layout` correctly keeps Flexbox out of `TimeWarp.Terminal`, hosts the public API in `TimeWarp.Terminal`, and implements the two-pass integer-cell contract (PointScaleFactor=1, pass-2 grow/shrink freeze on leaves and nested containers, min-width floors, SupportsColor-gated WriteLayout matching WritePanel/WriteTable). Layout smoke tests and the Release build all pass. The dominant defect is release resume: check-version advertises skip-duplicate semantics but pack/push still emit every packable nupkg with no filter and no `--skip-duplicate`, so a partial publish of Layout while Terminal 1.0.1 already exists will fail on push.

## Issues

### Issue 1 — Severity: bug
- File: tools/dev-cli/endpoints/workflow.cs:245
- Description: Partial-publish resume is broken. `available` / `alreadyPublished` are computed and the log claims "resume/--skip-duplicate semantics", but Step 6 packs every packable project and the push loop uploads every `*.nupkg` in `artifacts/packages` with no `--skip-duplicate` and without filtering to `available`. Repo version is 1.0.1 with TimeWarp.Terminal already on nuget.org; pushing `TimeWarp.Terminal.1.0.1.nupkg` will conflict and abort before Layout can publish. Confirmed: no `SkipDuplicate` / `--skip-duplicate` anywhere in the repo; `available` is unused after the check-version messages.
- Suggestion: Either push only packages in `available`, or pass `--skip-duplicate` (or Amuru equivalent) on every push. Prefer filtering to `available` so already-published ids are not re-packed/re-pushed as the happy path for this release.
- Status: open

### Issue 2 — Severity: suggestion
- File: tools/dev-cli/endpoints/workflow.cs:320
- Description: `NotifySoftwareSiteAsync` still hardcodes `client_payload[package]=TimeWarp.Terminal`. A Layout-only (or multi-package) release will not notify the site about `TimeWarp.Terminal.Layout`.
- Suggestion: Dispatch once per successfully pushed package id (or include all newly published ids in the payload) using the packable-project list / `available` set.
- Status: open

### Issue 3 — Severity: suggestion
- File: source/timewarp-terminal-layout/widget-measure.cs:95
- Description: `WidgetMeasure.MeasureHeight` has no references (Roslynk find-references empty). Height is computed inline in `LayoutEngine.ApplyHeightOverrides` instead, so the helper is dead code.
- Suggestion: Delete `MeasureHeight`, or call it from `ApplyHeightOverrides` so height policy lives in one place.
- Status: open

### Issue 4 — Severity: suggestion
- File: tests/Directory.Build.props:37
- Description: Every test under `tests/` now gets a ProjectReference to `timewarp-terminal-layout` and a global `Using` for `TimeWarp.Flexbox`. Non-layout runfiles inherit a Flexbox compile dependency they do not need; layout runfiles already pin `#:project …/timewarp-terminal-layout.csproj`.
- Suggestion: Scope the layout ProjectReference / Flexbox using to layout tests only (e.g. a props file imported by `layout-*.cs` / a layout test project), keeping other terminal tests on Terminal alone.
- Status: open

### Issue 5 — Severity: suggestion
- File: tests/layout-09-terminal-extensions.cs:21
- Description: WriteLayout’s SupportsColor-gated color overload is implemented and matches WritePanel/WriteTable, but layout-09 only covers WindowWidth write + static facade — no color / SupportsColor=false coverage for the new overload.
- Suggestion: Add tests that assert ANSI color prefixes when SupportsColor is true and plain output when false (mirror whatever panel/table color tests exist, or introduce a minimal SupportsColor pair here).
- Status: open

### Issue 6 — Severity: suggestion
- File: source/timewarp-terminal-layout/layout-canvas.cs:68
- Description: When `leaf.Box.X < cursor`, ComposeRow silently `continue`s and drops that leaf’s cells. Under the documented tiling contract this path should be unreachable; if a future engine regression overlaps boxes, output clips without failing.
- Suggestion: Throw `InvalidOperationException` (same spirit as `ToCell`) when overlap is detected, or at least fail in Debug builds, so contract breaks surface in tests instead of silent clipping.
- Status: open

## Smoke tests

- `dotnet build timewarp-terminal.slnx -c Release` — **pass** (0 Warning(s), 0 Error(s))
- `dotnet tests/layout-01-basic.cs` — **pass** (Passed: 1)
- `dotnet tests/layout-07-rounding.cs` — **pass** (Passed: 3)
- `dotnet tests/layout-08-min-width.cs` — **pass** (Passed: 1)
- `dotnet tests/layout-09-terminal-extensions.cs` — **pass** (Passed: 2)
- `dotnet tests/layout-02-row-column.cs` — **pass** (Passed: 2)
- `dotnet tests/layout-03-grow.cs` — **pass** (Passed: 1)
- `dotnet tests/layout-04-wrap.cs` — **pass** (Passed: 1)
- `dotnet tests/layout-05-nesting.cs` — **pass** (Passed: 1)
- `dotnet tests/layout-06-emoji-ansi.cs` — **pass** (Passed: 1)
- `dotnet tests/layout-10-widgets.cs` — **pass** (Passed: 1)
