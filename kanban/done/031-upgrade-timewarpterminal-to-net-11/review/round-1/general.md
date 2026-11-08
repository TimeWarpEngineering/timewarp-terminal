# Round 1 — general
**Date:** 2026-11-09
**Scope reviewed:** branch vs `master`, all product paths (17 files, 169+/107-)

## Summary

The change moves the root `TargetFramework` to `net11.0` and removes the per-folder `net11.0`
overrides in `.githooks` and `tools/dev-cli`. It adds `global.json` pinning
`11.0.100-rc.3.26522.148`, switches CI to one `setup-dotnet@v4` step with `global-json-file`, moves
NetAnalyzers to the matching 11 RC, bumps the package version to 2.0.0, and adds release notes and
consumer notes. Three `char` `EndsWith` / `IndexOf` calls gain `StringComparison.Ordinal` for
NetAnalyzers 11. Ordinal is what the char overloads already did, so behavior does not change. Risk is
low. No issues found.

## Verification

- `dotnet --version` in the worktree → `11.0.100-rc.3.26522.148` (global.json honored).
- `dotnet run tools/dev-cli/dev.cs -- workflow` → exit 0, `47 test file(s) passed`,
  `CI Pipeline completed successfully`.
- `ganda repo audit` (read-only, no `--fix`) → "Repository passes all audit checks."
- Public availability checked, because the Results flagged runner resolution as unproven:
  - `builds.dotnet.microsoft.com/dotnet/release-metadata/11.0/releases.json` lists
    `11.0.0-rc.3` with SDK `11.0.100-rc.3.26522.148` as `latest-sdk` (2026-10-25). `setup-dotnet`
    can install it from `global.json`.
  - nuget.org lists `Microsoft.CodeAnalysis.NetAnalyzers` `11.0.100-rc.3.26522.148`.
- `rg 'net10|10\.0\.x|\.NET 10'` outside kanban: the only matches are intentional consumer notes and a
  historical comment about the `InterceptorsNamespaces` rename (`Directory.Build.props:38`).
- Samples use `#:project`, not `#:package @1.0.2`, so they follow the TFM bump. `layout-dashboard.cs`
  version text is updated to 2.0.0.
- The `Directory.Packages.props` comment about Amuru's `TimeWarp.Terminal 1.0.2` dependency is still
  correct: dev-cli's project reference (2.0.0) satisfies `>= 1.0.2`.

## Goal vs task

The original task body said "analysis-only … until prerequisites below are ready". The restatement
turns it into the hard-cut implementation. This is not a misread. The task's own checklist includes
"Flip `TargetFramework` to `net11.0`" and a product-decision item. The upstream prerequisites are
answered: Builder and Flexbox `lib/net10.0` are consumable from `net11.0`, Build.Tasks is netstandard,
and Amuru 3 / Nuru already restore on net11. The original "bump CI to `11.0.x`" item became a
`global.json`-driven install. That is equivalent or stricter, and the checklist text was updated to
match.

## Issues

None.

## Follow-ups (not findings)

- Re-pin `global.json` and NetAnalyzers to stable `11.0.100` once the GA SDK (2026-11-10) is
  installed, ideally before the 2.0.0 release is cut. This is already recorded in task Results. The
  cockpit owns the release.
- CI has not run on the RC pin yet. The metadata check above makes runner resolution very likely, and
  the PR's CI run confirms it.
