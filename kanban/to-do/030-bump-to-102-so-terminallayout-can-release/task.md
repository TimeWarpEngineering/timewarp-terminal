# Bump to 1.0.2 so Terminal.Layout can release

## Description

Version-bump PR so `TimeWarp.Terminal.Layout` can ship. Layout is on `master`
(task 023 / PR #34) but **not** on nuget.org. Repo version is still **1.0.1**.
`TimeWarp.Terminal` 1.0.1 is already published and GitHub tag **`v1.0.1`**
already exists (2026-08-16, `bd375e2`, **before** Layout).

Do **not** cut the GitHub Release or run `dev release` on this task. This PR
only makes version **1.0.2** releasable. After merge + green master CI, cockpit
runs `dev release` from origin-home master.

## Requirements

- Bump `<Version>` in `source/Directory.Build.props` from `1.0.1` to `1.0.2`.
- Make `dev check-version` see **both** packable ids (`TimeWarp.Terminal` and
  `TimeWarp.Terminal.Layout`). Today `.timewarp/dev.jsonc`
  `checkVersionConfig.packages` is `"TimeWarp.Terminal"` only, so check-version
  ignores Layout and reports 1.0.1 as fully released. Prefer **deleting** the
  `packages` override so the set is derived from `IsPackable`. Do not leave a
  Terminal-only list.
- Do **not** retag `v1.0.1`. Do **not** break-glass resume 1.0.1. Layout is not
  on the tagged commit; a 1.0.1 resume would publish the old tree.
- Do **not** run `dev release`, `gh release create`, or push nupkgs from this
  task branch. Host `open-pr` only. Merge and cut stay cockpit.
- Keep version strings in historical kanban/docs as-is. If
  `samples/layout-dashboard.cs` hardcodes `1.0.1` as displayed package version,
  update it to match the props version.

## Checklist

- [ ] `source/Directory.Build.props` Version is `1.0.2`
- [ ] `.timewarp/dev.jsonc` no longer pins check-version to Terminal only
- [ ] `dotnet run tools/dev-cli/dev.cs -- check-version` checks both package
      ids and reports 1.0.2 available (exit 0)
- [ ] No `dev release` / tag / nuget push from this branch
- [ ] `## Results` + `### How to validate` written before done
- [ ] Host `open-pr` (do not `gh pr create`)

## Notes

Verified 2026-09-20 from origin-home:

```text
dotnet run tools/dev-cli/dev.cs -- check-version
Using configured package override; delete checkVersionConfig.packages to derive the set from IsPackable.
Version in source: 1.0.1
Latest NuGet version: 1.0.1
Packages checked: TimeWarp.Terminal

✗ Version 1.0.1 was already released.
  Bump the version before releasing.
  Already published: TimeWarp.Terminal
```

- nuget.org: `TimeWarp.Terminal` has 1.0.1; `TimeWarp.Terminal.Layout` 404.
- `dev release --dry-run` also refused dirty origin-home (`?? .local/`) and
  would refuse `v1.0.1` already existing even if the tree were clean.
- Task 023 left version at 1.0.1 expecting a Layout-only partial publish.
  That is the wrong vehicle now: tag `v1.0.1` pins a pre-Layout commit.
- `tw-release`: humans/agents type the version once in this props bump PR.
  After merge, wait for the master push CI (Packages-* artifact), then
  `dev release` from a clean synced master.

## Session

- Created: 2057841 (2026-09-20)
- Cockpit: grok session 01a0b207-11c7-73b0-9d4f-66c0c07d8c2f (2026-09-20)
