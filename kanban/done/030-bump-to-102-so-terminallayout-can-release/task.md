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

- [x] `source/Directory.Build.props` Version is `1.0.2`
- [x] `.timewarp/dev.jsonc` no longer pins check-version to Terminal only
- [x] `dotnet run tools/dev-cli/dev.cs -- check-version` checks both package
      ids and reports 1.0.2 available (exit 0)
- [x] No `dev release` / tag / nuget push from this branch
- [x] `## Results` + `### How to validate` written before done
- [ ] Host `open-pr` (do not `gh pr create`)

## Review

skip
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
- Re-verified 2026-09-20 (this implementer pass): product bump already on
  `c91d27b`; `check-version` exit 0, both packable ids, source 1.0.2 vs
  nuget 1.0.1, no packages override. No remaining product work. Review skip.

## Session

- Created: 2057841 (2026-09-20)
- Cockpit: grok session 01a0b207-11c7-73b0-9d4f-66c0c07d8c2f (2026-09-20)
- Implementer: grok session 01a0bcab-a440-7ad3-9b63-4d0e98bd67d4 (2026-09-20)
- Implementer re-verify: grok session 01a0bcb0-fa04-7d70-8386-9b69758a43c9 (2026-09-20)

## Results

Repo version is **1.0.2**. `dev check-version` now derives the packable set from `IsPackable` and sees both `TimeWarp.Terminal` and `TimeWarp.Terminal.Layout`. 1.0.2 is not on nuget.org (latest published is Terminal 1.0.1). Did not retag `v1.0.1`, did not resume 1.0.1, did not run `dev release` / `gh release create` / nuget push.

### Files changed

- `source/Directory.Build.props` — `<Version>` `1.0.1` → `1.0.2`
- `.timewarp/dev.jsonc` — removed `checkVersionConfig.packages` (`"TimeWarp.Terminal"`). Kept `checkVersionStrategy: nuget-search`. Packable ids come from `IsPackable` (`timewarp-terminal.csproj` and `timewarp-terminal-layout.csproj`).
- `samples/layout-dashboard.cs` — displayed Layout version `1.0.1` → `1.0.2`
- Historical kanban/docs version strings left as-is

### Key decisions

- Deleted the packages override rather than listing both ids, matching the check-version warning (`delete checkVersionConfig.packages to derive the set from IsPackable`).
- Typed 1.0.2 once in props. Cut stays cockpit after merge + green master CI (`Packages-*` artifact), per `tw-release`.

### Test outcomes

```text
dotnet run tools/dev-cli/dev.cs -- check-version
Version in source: 1.0.2
Latest NuGet version: 1.0.1
Packages checked: TimeWarp.Terminal, TimeWarp.Terminal.Layout

✓ Version in source is new — safe to release.
```

Exit 0. The previous "Using configured package override" line is gone.

Local pack (not pushed):

- `artifacts/packages/TimeWarp.Terminal.1.0.2.nupkg` + `.snupkg`
- `artifacts/packages/TimeWarp.Terminal.Layout.1.0.2.nupkg` + `.snupkg`

### How to validate

**Smoke**

```bash
cd /home/steve/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/task-030-bump-to-102-so-terminallayout-can-release
grep -n '<Version>' source/Directory.Build.props
# expect: <Version>1.0.2</Version>
grep -n packages .timewarp/dev.jsonc || true
# expect: no packages key (only checkVersionStrategy)
dotnet run tools/dev-cli/dev.cs -- check-version
```

**Expect**

- Version in source: `1.0.2`
- Latest NuGet version: `1.0.1`
- Packages checked: `TimeWarp.Terminal, TimeWarp.Terminal.Layout`
- `✓ Version in source is new — safe to release.`
- Exit 0
- No line `Using configured package override`

**Automated gate**

```bash
dotnet run tools/dev-cli/dev.cs -- check-version
# expect: exit 0, both package ids, source 1.0.2

dotnet pack source/timewarp-terminal/timewarp-terminal.csproj -c Release -o artifacts/packages
dotnet pack source/timewarp-terminal-layout/timewarp-terminal-layout.csproj -c Release -o artifacts/packages
ls artifacts/packages/*1.0.2*
# expect: TimeWarp.Terminal.1.0.2.nupkg and TimeWarp.Terminal.Layout.1.0.2.nupkg (and .snupkg)
```

**Depends on:** .NET 10 SDK; network to nuget.org for check-version.

**Not in scope:** `dev release`, GitHub tag `v1.0.2`, nuget push, retag/resume of `v1.0.1`. Host `open-pr` opens the PR; cockpit cuts after merge + green master CI.
