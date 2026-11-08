# Upgrade TimeWarp.Terminal to .NET 11

## Goal (implementer restatement)

Move `TimeWarp.Terminal` and `TimeWarp.Terminal.Layout` from `net10.0` to `net11.0` as a single-target cut, and align the SDK pin, CI, analyzer package, package version, and consumer notes with that cut.

Acceptance:

- The root `TargetFramework` is `net11.0`. Tests, samples, dev-cli, and `.githooks` inherit it. The product no longer targets `net10.0`.
- `global.json` pins the installed SDK `11.0.100-rc.3.26522.148` with `allowPrerelease` and `rollForward`. Stable `11.0.100` is not on this machine (GA is 2026-11-10). Re-pin when that SDK is installed.
- CI stays on `actions/setup-dotnet@v4`, installs the SDK from `global.json`, and drops the `10.0.x` install.
- `Microsoft.CodeAnalysis.NetAnalyzers` moves to the SDK 11 line. `Microsoft.CodeAnalysis.CSharp.CodeStyle` `5.9.0`, Roslynator `5.0.1`, and Shouldly `4.3.0` stay when restore and the build accept them on `net11.0`.
- `TimeWarp.Builder` `1.0.0` and `TimeWarp.Flexbox` `1.0.0` stay. They publish `lib/net10.0` only, which a `net11.0` project can reference. `TimeWarp.Build.Tasks` `1.0.0` is netstandard. `TimeWarp.Jaribu` stays at `1.0.0-beta.15`.
- The package version becomes `2.0.0` because dropping `net10.0` breaks net10 consumers. `1.0.2` stays the last `net10.0` release. Notes say that. This walk does not publish to NuGet.
- Local build and tests are green, including the AOT and trim analyzers already enabled on the packable projects. `ganda repo audit` passes.

Product decision: hard cut, not multi-target. Amuru, Nuru, State, and Flow already target `net11.0` only. A second `net10.0` target would keep a second SDK in CI for a line this repo treats as the single current framework. Net10 consumers stay on `1.0.2`.

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

- [x] Confirm .NET 11 SDK availability (preview vs GA) and pick pin strategy (`global.json` yes/no)
- [x] Bump CI `setup-dotnet` to the SDK in `global.json` (`actions/setup-dotnet@v4`, `global-json-file`)
- [x] Confirm / upgrade TimeWarp.Builder, Flexbox, Build.Tasks, Amuru, Jaribu, Nuru for net11
- [x] Bump NetAnalyzers / CSharp.CodeStyle / Roslynator as needed for SDK 11
- [x] Verify Shouldly (and any transitive) on net11
- [x] Flip `TargetFramework` to `net11.0` (hard cut; net10 consumers stay on 1.0.2)
- [x] Green local `dev workflow`; AOT/trim analyzers clean on the Release build. GitHub Actions has not run yet
- [x] Document consumer impact. Release cut stays with cockpit after merge
- [x] Prerequisites checked on 2026-11-09; the hard cut is implemented in this walk

## Notes

- Requested by Steven via Grok Bot → Terminal agent (2026-10-02). Scope: impact review + this kanban task only.
- Cloned bare + master worktree on TWE-001 with `ganda repo clone` to create this task (repo was not previously in local worktrees).
- `ubuntu-latest` image itself is fine; the hard pin is `dotnet-version: '10.0.x'`.
- No in-repo container/Nix SDK image to update today.

## Session

- Created: 56416 (2026-10-02)
- Analysis: Terminal agent (2026-10-02)
- Implementation: 01a1bc3c-7674-7272-8ab1-67cbb5dd041e (2026-11-09). Hard cut to `net11.0`, package version `2.0.0`. SDK pin is `11.0.100-rc.3.26522.148` because that is the installed .NET 11 SDK. `ganda repo audit --fix` wrote gitignored `.vscode/settings.json` (`window.title` includes `timewarp-terminal`). That file stays untracked. Audit then passed 35, failed 0.

## Results

`TimeWarp.Terminal` and `TimeWarp.Terminal.Layout` 2.0.0 target `net11.0` only. `1.0.2` remains the last `net10.0` release. Public API is unchanged. `TimeWarp.Builder` 1.0.0 and `TimeWarp.Flexbox` 1.0.0 still ship `lib/net10.0` and restore through NuGet framework fallback. `TimeWarp.Build.Tasks` 1.0.0 is netstandard. `TimeWarp.Jaribu` stays `1.0.0-beta.15` (`lib/net10.0`). `Shouldly` 4.3.0, Roslynator 5.0.1, and `Microsoft.CodeAnalysis.CSharp.CodeStyle` 5.9.0 are unchanged. `Microsoft.CodeAnalysis.NetAnalyzers` is `11.0.100-rc.3.26522.148`. NetAnalyzers 11 required `StringComparison.Ordinal` on three char `EndsWith` / `IndexOf` calls in `ansi-string-utils.wrap.cs`.

`global.json` pins SDK `11.0.100-rc.3.26522.148` with `rollForward: latestFeature` and `allowPrerelease: true`. CI uses `actions/setup-dotnet@v4` and `global-json-file: global.json`. The `10.0.x` install is gone. Re-pin `global.json` to stable `11.0.100` when that SDK is installed. This walk did not publish to NuGet and did not open a pull request.

### How to validate

Smoke:

```bash
dotnet --version
dotnet run tools/dev-cli/dev.cs -- workflow
dotnet pack timewarp-terminal.slnx -c Release
unzip -l artifacts/packages/TimeWarp.Terminal.2.0.0.nupkg | rg 'lib/'
unzip -l artifacts/packages/TimeWarp.Terminal.Layout.2.0.0.nupkg | rg 'lib/'
dotnet run tools/dev-cli/dev.cs -- check-version
ganda repo audit
```

Expect: `dotnet --version` prints `11.0.100-rc.3.26522.148`. Workflow exits 0 with Release build 0 warnings and `47 test file(s) passed`. Both packages contain `lib/net11.0/`. `check-version` prints source `2.0.0`, latest NuGet `1.0.2`, and `safe to release`. `ganda repo audit` exits 0.

This walk saw that output. GitHub Actions has not run, so runner resolution of the RC SDK is still unproven on `ubuntu-latest`.
