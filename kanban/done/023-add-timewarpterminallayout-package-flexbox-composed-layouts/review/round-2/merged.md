# Round 2 — merged findings
**Date:** 2026-09-18
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 1 | 0 |
| suggestion | 0 | 5 | 0 |
| nit | 0 | 0 | 0 |

Final counts = round-1 IDs after re-verify (no new findings).

## Issues

### M1 — Severity: bug — Status: fixed
- File: tools/dev-cli/endpoints/workflow.cs:252
- Description: Partial-publish resume packed and pushed every packable nupkg without `--skip-duplicate`.
- Suggestion: Pack/push only `available`; `WithSkipDuplicate()` on push.
- Source: general
- Disposition notes: Pack filters `availableSet`; push `{packageId}.{version}.nupkg` + `WithSkipDuplicate()`.

### M2 — Severity: suggestion — Status: fixed
- File: tools/dev-cli/endpoints/workflow.cs:310
- Description: Site notify hardcoded `TimeWarp.Terminal`.
- Suggestion: Dispatch per successfully pushed package id.
- Source: general
- Disposition notes: `NotifySoftwareSiteAsync(repoRoot, packageId, version)` after each successful push.

### M3 — Severity: suggestion — Status: fixed
- File: source/timewarp-terminal-layout/widget-measure.cs
- Description: Dead `MeasureHeight`.
- Suggestion: Delete it.
- Source: general
- Disposition notes: Method removed.

### M4 — Severity: suggestion — Status: fixed
- File: tests/Directory.Build.props
- Description: Global layout ProjectReference and Flexbox Using on every test.
- Suggestion: Scope to layout runfiles via `#:project`.
- Source: general
- Disposition notes: Props restored to Terminal-only; layout-*.cs add `using TimeWarp.Flexbox;`.

### M5 — Severity: suggestion — Status: fixed
- File: tests/layout-09-terminal-extensions.cs
- Description: No SupportsColor coverage for WriteLayout color overload.
- Suggestion: Add true/false color tests.
- Source: general
- Disposition notes: Two tests added; layout-09 4/4 pass.

### M6 — Severity: suggestion — Status: fixed
- File: source/timewarp-terminal-layout/layout-canvas.cs:68
- Description: Silent skip on overlapping leaf X.
- Suggestion: Throw `InvalidOperationException`.
- Source: general
- Disposition notes: Overlap throws; Design region updated.

## Duplicates / conflicts

- None. Round-2 raised no new IDs. Prior M1–M6 carried forward as fixed.
