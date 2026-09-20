# Round 1 — general
**Date:** 2026-09-20
**Scope reviewed:** branch `task/029-004-ci-path-filters-for-testssamples-and-push-snupkg` vs `origin/master` (product commit `83a3612`)

## Summary

Product commit `83a3612` closes the four claimed parent findings with matching files: both CI `paths` lists include `'tests/**'`, `'samples/**'`, and `'msbuild/**'` (root `Directory.Build.props` imports `msbuild/repository.props`); Actions upload globs `*.nupkg` and `*.snupkg`; release push requires the sibling `{id}.{version}.snupkg` and pushes it after the nupkg with `WithSkipDuplicate()`, then notifies once per package id; both packable csproj files use kebab `readme.md` Include + PackageReadmeFile with empty PackagePath; Layout’s on-disk `README.md` was renamed; `dev.cs` banners match the real PR and release pipelines (runtime release line also gained `-> push`). Risk is low hygiene. Re-verified: `dotnet pack` emits both 1.0.2 nupkg+snupkg pairs; nupkg entries and nuspec `<readme>` are `readme.md`; `git ls-files` has no `README.md`; nuget.org 1.0.2 nuspecs 404 (no dummy publish). **M9, M10, M20, and M21 all appear closed.**

## Issues

No issues.
