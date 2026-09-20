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

- [ ] M9 `tests/**` and `samples/**` in both CI path filters
- [ ] M10 snupkg in artifact glob and explicit push or documented sibling-push;
      no dummy publish
- [ ] M20 every readme is kebab `readme.md` on disk, in `Include`, and in
      `PackageReadmeFile` (root already kebab; Layout renamed; no `README.md`
      remap)
- [ ] M21 `dev.cs` banner matches the real pipeline
- [ ] A tests-only path change would match `on.pull_request.paths` (inspect YAML)

## Notes

- Parent: `kanban/done/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already added verify-samples+test and snupkg *production*. Do not remove those.
- Cockpit 2026-09-20: M20 is kebab `readme.md` all the way (org PackageReadmeFile
  convention). Earlier “nupkg must be README.md” note was wrong. M10 404 claim is stale.

## Session

- Created: 3366037 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
- Brief clarified: grok `01a0b207-11c7-73b0-9d4f-66c0c07d8c2f` (2026-09-20)
