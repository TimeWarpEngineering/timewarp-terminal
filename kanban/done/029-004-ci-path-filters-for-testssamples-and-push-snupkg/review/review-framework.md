# Review framework — task 029-004

**Date:** 2026-09-20
**Host task:** kanban/in-progress/029-004-ci-path-filters-for-testssamples-and-push-snupkg/
**Diff scope:** branch `task/029-004-ci-path-filters-for-testssamples-and-push-snupkg` vs `origin/master` — product commit `83a3612` (`fix: CI path filters, snupkg push, and kebab package readme`). Kanban kitchen is in the same commit (folderize + implementer Results); product-code review is the YAML/csproj/dev-cli/readme rename, not the task file.
**Plan / brief:** Child of parent **029** round-1 merged findings **M9, M10, M20, M21**. Add `tests/**` and `samples/**` (and `msbuild/**` if it can change CI/build without `source/**`) to both CI path lists; upload and explicitly push sibling `.snupkg`; kebab `readme.md` for git path, Include, and PackageReadmeFile with empty PackagePath (cockpit overrode parent M20’s `PackagePath="README.md"` remap); align `dev.cs` release banner with the real pipeline. No dummy NuGet publish.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** Review oracle Grok `01a0be4c-c876-7172-9b8e-4d0aa8dc7d7a` (2026-09-20); implementer Grok `01a0be47-53ca-75b0-848a-aa3dc432dd1d` (2026-09-20)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`

## Parent findings this child claims to close

| ID | Severity | Claimed fix |
|----|----------|-------------|
| M9 | bug | `'tests/**'`, `'samples/**'`, and `'msbuild/**'` on both push and pull_request `paths` |
| M10 | bug | Artifact glob includes `*.snupkg`; release push requires and pushes `{id}.{version}.snupkg` with `WithSkipDuplicate()` |
| M20 | nit | Kebab `readme.md` on disk, Include, PackageReadmeFile, empty PackagePath; no git `README.md`; no remap |
| M21 | nit | `dev.cs` banner matches clean → build → verify-samples → test → check-version → pack → push |
