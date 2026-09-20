# CI path filters for tests/samples and push snupkg

## Description

Parent **029** round-1 merged findings **M9, M10, M20, M21**.

Four independent hygiene items. Do **not** create a sibling “apply 029 findings” task.

**Kebab SSOT for readmes (M20):** `readme.md` everywhere — git path, csproj
`Include`, `PackageReadmeFile`, and the nupkg entry. Do **not** remap to
`README.md`. Org packs (Nuru, Flexbox, Ganda, State, Jaribu, Builder, Mediator,
Tazor, …) already use `<PackageReadmeFile>readme.md</PackageReadmeFile>` with
`Include="…/readme.md"` and empty `PackagePath`.

## Requirements

### M9 — bug — CI path filters omit `tests/` and `samples/`
- File: `.github/workflows/workflow.yml` push `paths` and pull_request `paths`
- Today: `source/**`, `tools/**`, `.github/workflows/**`, `Directory.Build.props`,
  `Directory.Packages.props` only.
- Add `'tests/**'` and `'samples/**'` to **both** lists. Add `'msbuild/**'` if
  those files can change CI/build without touching `source/**`.

### M10 — bug — release artifact/push glob is `*.nupkg` only
- Pack already emits `.snupkg` (`IncludeSymbols` + `SymbolPackageFormat`).
- `workflow.cs` push builds `{id}.{version}.nupkg` only; Actions upload is
  `artifacts/packages/*.nupkg`.
- **Stale 2026-09-04 proof:** Terminal 1.0.0/1.0.1 symbols **are** on nuget.org now
  (CLI often auto-pushes a sibling `.snupkg` next to the nupkg). Do not cite the
  old symbolpackage 404.
- Still do: include `*.snupkg` in the Actions artifact glob, and push the sibling
  `.snupkg` explicitly (or document that `dotnet nuget push` of the nupkg from the
  same folder publishes symbols, and assert that in Results). Layout 1.0.2 has not
  shipped yet — this should be correct before `dev release`.

### M20 — nit — use kebab `readme.md` for git, Include, and PackageReadmeFile

NuGet accepts `readme.md`. Match the rest of the org; do not invent a
`PackagePath="README.md"` remap.

**Wrong today**

- Repo root is already `readme.md`.
- `source/timewarp-terminal/timewarp-terminal.csproj`: `PackageReadmeFile` is
  `README.md` and `Include="../../README.md"` (no such git file). Set both to
  `readme.md` (`Include="../../readme.md"`, `PackagePath=""`).
- `source/timewarp-terminal-layout/README.md` is Pascal on disk (audit
  `kebab-path-names`). Rename to `readme.md`. csproj Include +
  `PackageReadmeFile` become `readme.md`.

Do **not** add a git `README.md`. Do **not** leave Layout’s on-disk `README.md`.

### M21 — nit — `dev.cs` release banner stale
- File: `tools/dev-cli/dev.cs` command banner
- Banner says `build -> check-version -> pack -> push`.
- Actual release path: clean → build → verify-samples → test → check-version →
  pack → push (`workflow.cs`).

## Checklist

- [x] M9 `tests/**` and `samples/**` in both CI path filters
- [x] M10 snupkg in artifact glob and explicit push or documented sibling-push;
      no dummy publish
- [x] M20 every readme is kebab `readme.md` on disk, in `Include`, and in
      `PackageReadmeFile` (root already kebab; Layout renamed; no `README.md`
      remap)
- [x] M21 `dev.cs` banner matches the real pipeline
- [x] A tests-only path change would match `on.pull_request.paths` (inspect YAML)
- [x] Effort-1 general review under `review/` (round 1)
- [x] `review/disposition.md` — outcome `clean`

## Notes

- Parent: `kanban/done/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already added verify-samples+test and snupkg *production*. Do not remove those.
- Cockpit 2026-09-20: M20 is kebab `readme.md` all the way (org PackageReadmeFile
  convention). Earlier “nupkg must be README.md” note was wrong. M10 404 claim is stale.

## Session

- Created: 3366037 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
- Brief clarified: grok `01a0b207-11c7-73b0-9d4f-66c0c07d8c2f` (2026-09-20)
- Implementer: Grok `01a0be47-53ca-75b0-848a-aa3dc432dd1d` (2026-09-20)
- Review oracle: Grok `01a0be4c-c876-7172-9b8e-4d0aa8dc7d7a` (2026-09-20)

## Results

Implemented parent **029** round-1 findings **M9, M10, M20, M21** on this id. No dummy NuGet publish.

**M9.** `.github/workflows/workflow.yml` push and pull_request `paths` both now include `'tests/**'`, `'samples/**'`, and `'msbuild/**'` (root `Directory.Build.props` imports `msbuild/repository.props`, so those files can change CI/build without touching `source/**`).

**M10.** Artifact upload globs `artifacts/packages/*.nupkg` and `artifacts/packages/*.snupkg`. Release push in `tools/dev-cli/endpoints/workflow.cs` requires `{id}.{version}.nupkg` **and** the sibling `{id}.{version}.snupkg` and pushes both with `WithSkipDuplicate()`. Site notify still fires once per package id after both files push. No nuget.org upload was run from this session.

**M20.** Kebab `readme.md` all the way: git path, csproj `Include`, `PackageReadmeFile`, empty `PackagePath`, nupkg entry, nuspec `<readme>`. Root was already kebab. Layout `README.md` renamed to `readme.md`. No git `README.md` and no `PackagePath="README.md"` remap.

**M21.** `tools/dev-cli/dev.cs` release banner is `clean -> build -> verify-samples -> test -> check-version -> pack -> push` (PR banner gained `clean` too). Runtime `workflow.cs` release line now includes `-> push`.

### Files changed

- `.github/workflows/workflow.yml`
- `tools/dev-cli/endpoints/workflow.cs`
- `tools/dev-cli/dev.cs`
- `source/timewarp-terminal/timewarp-terminal.csproj`
- `source/timewarp-terminal-layout/timewarp-terminal-layout.csproj`
- `source/timewarp-terminal-layout/README.md` → `readme.md`
- `.gitignore` (`oracle-*.log`; host pane logs beside kitchens)
- `kanban/in-progress/029-004-ci-path-filters-for-testssamples-and-push-snupkg/task.md`

### Decisions

- Explicit sibling `.snupkg` push rather than relying on CLI auto-push of a same-folder sibling (Layout 1.0.2 has not shipped).
- Included `'msbuild/**'` because the brief said to add it when those files can change CI/build without `source/**`.

### Test outcomes

- `dotnet pack` both packable projects `-c Release -p:ContinuousIntegrationBuild=true` — success.
- Both nupkgs contain `readme.md`; both nuspecs have `<readme>readme.md</readme>`.
- Both `.snupkg` files emitted: `TimeWarp.Terminal.1.0.2.snupkg`, `TimeWarp.Terminal.Layout.1.0.2.snupkg`.
- `dotnet run tools/dev-cli/dev.cs -- --help` — exit 0 (runfile compiles).
- Inspected YAML: `tests/hyperlink-01-basic.cs` matches `on.pull_request.paths` via `'tests/**'`.
- `git ls-files` readmes: `readme.md`, `source/timewarp-terminal-layout/readme.md` only.

### How to validate

**Smoke**

```bash
# From the claimed worktree
sed -n '3,27p' .github/workflows/workflow.yml
sed -n '95,103p' .github/workflows/workflow.yml
git ls-files | grep -i readme
grep -n PackageReadmeFile source/timewarp-terminal/*.csproj source/timewarp-terminal-layout/*.csproj
grep -n 'snupkg\|Pushing' tools/dev-cli/endpoints/workflow.cs
grep -n 'Release workflow' tools/dev-cli/dev.cs
```

**Expect**

- Both `push.paths` and `pull_request.paths` list `'tests/**'`, `'samples/**'`, `'msbuild/**'`.
- A tests-only path such as `tests/hyperlink-01-basic.cs` matches `'tests/**'` in `on.pull_request.paths` (GitHub `**` glob). `kanban/**` and root `readme.md` do not match those lists.
- Upload `path` includes `artifacts/packages/*.snupkg`.
- `git ls-files` shows `readme.md` and `source/timewarp-terminal-layout/readme.md` only — no `README.md`.
- Both csproj files: `<PackageReadmeFile>readme.md</PackageReadmeFile>` and `Include="…/readme.md"` with `PackagePath=""`.
- Push loop builds `{packageId}.{version}.snupkg` and pushes it after the nupkg.
- Banner line: `clean -> build -> verify-samples -> test -> check-version -> pack -> push`.

**Automated gate**

```bash
PACK_DIR=/tmp/029-004-pack
rm -rf "$PACK_DIR" && mkdir -p "$PACK_DIR"
dotnet pack source/timewarp-terminal/timewarp-terminal.csproj -c Release -o "$PACK_DIR" -p:ContinuousIntegrationBuild=true
dotnet pack source/timewarp-terminal-layout/timewarp-terminal-layout.csproj -c Release -o "$PACK_DIR" -p:ContinuousIntegrationBuild=true
ls "$PACK_DIR"
unzip -l "$PACK_DIR"/TimeWarp.Terminal.1.0.2.nupkg | grep -i readme
unzip -p "$PACK_DIR"/TimeWarp.Terminal.1.0.2.nupkg TimeWarp.Terminal.nuspec | grep -i readme
unzip -l "$PACK_DIR"/TimeWarp.Terminal.Layout.1.0.2.nupkg | grep -i readme
unzip -p "$PACK_DIR"/TimeWarp.Terminal.Layout.1.0.2.nupkg TimeWarp.Terminal.Layout.nuspec | grep -i readme
dotnet run tools/dev-cli/dev.cs -- --help
```

Expect: four files (`TimeWarp.Terminal.1.0.2.nupkg` + `.snupkg`, `TimeWarp.Terminal.Layout.1.0.2.nupkg` + `.snupkg`); nupkg entries `readme.md`; nuspec `<readme>readme.md</readme>`; help exits 0.

**Not in scope:** live `dotnet nuget push` / `dev release` (no dummy publish). Layout 1.0.2 has not shipped; this is the pre-release fix.

### Review disposition

- **Rounds:** 1
- **Effort / roster:** 1 — general only
- **Final counts:** 0 open / 0 fixed / 0 wontfix (no issues raised)
- **Outcome:** `clean` (parent 029 M9, M10, M20, M21 closed on this child; no new findings)
- **Wontfix / escalations:** none
- **Paths:**
  - `review/review-framework.md`
  - `review/round-1/general.md`
  - `review/round-1/merged.md`
  - `review/disposition.md`
- Review-session smoke: `dotnet pack` both packable projects `-c Release -p:ContinuousIntegrationBuild=true` → `TimeWarp.Terminal.1.0.2` and `TimeWarp.Terminal.Layout.1.0.2` nupkg+snupkg; nupkg entries `readme.md`; nuspec `<readme>readme.md</readme>`; `dotnet run tools/dev-cli/dev.cs -- --help` exit 0; nuget.org 1.0.2 nuspecs HTTP 404.
