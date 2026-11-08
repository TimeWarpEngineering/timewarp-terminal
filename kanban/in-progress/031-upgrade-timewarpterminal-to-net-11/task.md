# Upgrade TimeWarp.Terminal to .NET 11

## Description

Analysis-only task for moving TimeWarpEngineering/timewarp-terminal from **net10.0 / SDK 10.0.x** to **.NET 11 (`net11.0`)**. Do **not** implement product changes in this task until prerequisites below are ready. .NET 11 is STS (GA scheduled 2026-11-10); as of 2026-10-02 it is still in preview (latest noted: 11.0.0-preview.7 / SDK 11.0.100-preview.7.x).

Current baseline (master):
- TFM: `net10.0` via root `Directory.Build.props`
- CI: `actions/setup-dotnet@v4` with `dotnet-version: '10.0.x'` on `ubuntu-latest` (`.github/workflows/workflow.yml`)
- No `global.json` / Nix / Dockerfile SDK pin in-repo
- Central package versions in `Directory.Packages.props`
- Library packages target AOT (`IsAotCompatible`, trim/AOT analyzers)
- Interceptors already use `InterceptorsNamespaces` (.NET 10+ property name)

## Dependencies to update first (ordered)

Update / unblock these **before** flipping Terminal’s TFM and shipping a net11 package:

1. **Local + CI toolchain**
   - Install .NET 11 SDK on developer machines (preview until GA).
   - Optionally add `global.json` pinning `11.0.100` (or latest 11.0.x) with `rollForward`.
   - Bump CI `dotnet-version` from `10.0.x` → `11.0.x` in `.github/workflows/workflow.yml`.
   - Confirm `actions/setup-dotnet@v4` (and runner image) can resolve 11.0.x; bump action majors only if required.

2. **Upstream TimeWarp packages (publish net11-capable builds first, or multi-target)**
   These are referenced centrally and block a clean TFM bump:
   - `TimeWarp.Builder` `1.0.0` (used by `TimeWarp.Terminal`)
   - `TimeWarp.Flexbox` `1.0.0` (used by `TimeWarp.Terminal.Layout`)
   - `TimeWarp.Build.Tasks` `1.0.0` (Directory.Build analyzers/tasks)
   - `TimeWarp.Amuru` `1.0.0` / `TimeWarp.Amuru.Tools` `1.0.0-beta.2`
   - `TimeWarp.Jaribu` `1.0.0-beta.15`
   - `TimeWarp.Nuru` / `TimeWarp.Nuru.DevCli` `3.0.0-beta.76`
   Track sibling repo upgrades (or wait for multi-targeted packages that still support net10 consumers if needed).

3. **Analyzer / Roslyn packages** (`Directory.Packages.props`)
   - `Microsoft.CodeAnalysis.NetAnalyzers` `10.0.400` → 11.x line matching SDK 11
   - `Microsoft.CodeAnalysis.CSharp.CodeStyle` `5.6.0` → version aligned with .NET 11 / VS tooling
   - `Roslynator.*` `4.16.0` → confirm Roslyn host compatibility with SDK 11; bump if analyzers fail under TreatWarningsAsErrors

4. **Test / third-party packages**
   - `Shouldly` `4.3.0` — verify net11 TFM / restore; bump if needed

5. **Repo TFM + product surface (this repo, after 1–4)**
   - Root `Directory.Build.props`: `TargetFramework` `net10.0` → `net11.0` (inherits to source/tests/tools/samples)
   - Re-validate `InterceptorsNamespaces` / Nuru generated interceptors under SDK 11
   - Re-run AOT/trim analyzers on packable projects
   - `tools/dev-cli` file-based `dotnet run … workflow` path used by CI

6. **Docs / versioning / consumers**
   - Package version bump + release notes calling out `net11.0` (and any dropped TFMs)
   - Downstream consumers still on net10 need multi-targeting or stay on last net10 Terminal release
   - Product decision: leave LTS net10 support via multi-target vs hard cut to STS net11

## Checklist

- [ ] Confirm .NET 11 SDK availability (preview vs GA) and pick pin strategy (`global.json` yes/no)
- [ ] Bump CI `setup-dotnet` to `11.0.x` (and verify Actions resolution)
- [ ] Confirm / upgrade TimeWarp.Builder, Flexbox, Build.Tasks, Amuru, Jaribu, Nuru for net11
- [ ] Bump NetAnalyzers / CSharp.CodeStyle / Roslynator as needed for SDK 11
- [ ] Verify Shouldly (and any transitive) on net11
- [ ] Flip `TargetFramework` to `net11.0` (or multi-target if retaining net10)
- [ ] Green local `dev workflow` + CI; AOT/trim clean
- [ ] Document consumer impact; cut release when ready
- [ ] **Do not implement** until prerequisites above are explicitly green

## Notes

- Requested by Steven via Grok Bot → Terminal agent (2026-10-02). Scope: impact review + this kanban task only.
- Cloned bare + master worktree on TWE-001 with `ganda repo clone` to create this task (repo was not previously in local worktrees).
- `ubuntu-latest` image itself is fine; the hard pin is `dotnet-version: '10.0.x'`.
- No in-repo container/Nix SDK image to update today.

## Session

- Created: 56416 (2026-10-02)
- Analysis: Terminal agent (2026-10-02)
