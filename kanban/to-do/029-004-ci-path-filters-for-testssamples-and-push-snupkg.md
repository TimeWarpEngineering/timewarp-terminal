# CI path filters for tests/samples and push snupkg

## Description

Parent **029** round-1 merged findings **M9, M10, M20, M21**.

Four independent hygiene items. Do **not** create a sibling “apply 029 findings” task.

**Kebab SSOT for readmes (M20):** git paths are `readme.md`, never `README.md`.
NuGet’s *package payload* may still be named `README.md` via `PackagePath` — that is
not a git filename.

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

### M20 — nit — git readme is kebab `readme.md`; csproj still points at `README.md`

**Two different names. Do not mix them.**

| Layer | Required name | Why |
|-------|----------------|-----|
| Git / disk | `readme.md` | TimeWarp kebab (tw-csharp). Not an exception. |
| csproj `Include` | path to that kebab file | Linux pack is case-sensitive; `README.md` misses `readme.md`. |
| nupkg entry + `PackageReadmeFile` | `README.md` | NuGet gallery readme filename inside the package. Set with `PackagePath="README.md"`. |

**Wrong today**

- Repo root git file is already `readme.md`.
- `source/timewarp-terminal/timewarp-terminal.csproj` still
  `Include="../../README.md"` (no file of that name). Fix Include to
  `../../readme.md` and set `PackagePath="README.md"` so the nupkg still contains
  `README.md`.
- `source/timewarp-terminal-layout/README.md` is Pascal `README.md` on disk
  (audit `kebab-path-names`). Rename the **git file** to `readme.md`. Point
  Include at `readme.md`. Same `PackagePath="README.md"`.

Do **not** add a git `README.md`. Do **not** leave Layout’s on-disk `README.md`.

### M21 — nit — `dev.cs` release banner stale
- File: `tools/dev-cli/dev.cs` command banner
- Banner says `build -> check-version -> pack -> push`.
- Actual release path: clean → build → verify-samples → test → check-version →
  pack → push (`workflow.cs`).

## Checklist

- [ ] M9 `tests/**` and `samples/**` in both CI path filters
- [ ] M10 snupkg in artifact glob and explicit push or documented sibling-push;
      no dummy publish
- [ ] M20 every **git** readme path is `readme.md` (root already is; Layout
      renamed). csproj `Include` matches those kebab paths. nupkg entry may be
      `README.md` via `PackagePath` only
- [ ] M21 `dev.cs` banner matches the real pipeline
- [ ] A tests-only path change would match `on.pull_request.paths` (inspect YAML)

## Notes

- Parent: `kanban/done/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already added verify-samples+test and snupkg *production*. Do not remove those.
- Cockpit 2026-09-20: clarified M20 kebab vs nupkg `README.md`; M10 404 claim is stale.

## Session

- Created: 3366037 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
- Brief clarified: grok `01a0b207-11c7-73b0-9d4f-66c0c07d8c2f` (2026-09-20)
