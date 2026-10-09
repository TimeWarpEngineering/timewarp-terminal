# Update all NuGet packages to latest (incl. TimeWarp.Amuru 2.0.0-beta.2)

## Description

Steven wants every repo on the newest packages, pre-releases included (latest, not stable). Run `ganda nuget outdated --update` and take every package to the newest version it can, not just Amuru. TimeWarp.Amuru and TimeWarp.Amuru.Tools must end on 2.0.0-beta.2 or newer.

## Checklist

- [x] `ganda nuget outdated --dry-run`, then `ganda nuget outdated --update`
- [x] Bump `#:package ...@version` pins in runfiles; .githooks are ganda-owned, refresh via `ganda hooks install attest` instead of editing
- [x] Fix Amuru 1.x to 2.0 breaking changes (timewarp-amuru documentation/release-notes/2.0.0.md): Git *Master* helpers removed (use *Default*), Git methods return result objects not bool, no "master" default branchName, removed/renamed dotnet builder options, WithStandardInput("") closes stdin
- [x] Fix other breaking changes from bumped packages
- [x] Build warning-free, tests green, `ganda repo audit` clean
- [ ] One PR (with proof: build/test output) and merge via `ganda pr merge`

## Session

- Created: 1428834 (2026-10-09)
- Implementation: grok task-work (2026-10-09)
- Review: claude review oracle, effort 2, general (2026-10-09)

## Results

`ganda nuget outdated --update --force` moved the stable band to stable latest and the prerelease band to the newest prerelease. TimeWarp.Amuru was stable `1.0.0`, so the tool stopped at `1.1.1`. TimeWarp.Amuru.Tools `2.0.0-beta.2` requires Amuru `>= 2.0.0-beta.2`, and this task requires that pair, so Amuru is pinned to `2.0.0-beta.2`.

Pins now:

| Package | Was | Now |
| --- | --- | --- |
| TimeWarp.Amuru | 1.0.0 | 2.0.0-beta.2 |
| TimeWarp.Amuru.Tools | 1.0.0-beta.2 | 2.0.0-beta.2 |
| TimeWarp.Nuru | 3.0.0-beta.76 | 3.0.0-beta.79 |
| TimeWarp.Nuru.DevCli | 3.0.0-beta.76 | 3.0.0-beta.79 |
| Roslynator.* | 4.16.0 | 5.0.1 |
| Microsoft.CodeAnalysis.NetAnalyzers | 10.0.400 | 10.0.401 |
| Microsoft.CodeAnalysis.CSharp.CodeStyle | 5.6.0 | 5.9.0 |
| TimeWarp.SourceGenerators | (absent) | 1.0.0-beta.11 |

Builder, Flexbox, Jaribu `1.0.0-beta.15`, Build.Tasks, and Shouldly were already current. No runfile had a versioned `#:package` pin. `.githooks` were refreshed with `ganda hooks install attest` (attest dispatchers plus pre-commit / pre-push). The old post-merge runfile referenced only `TimeWarp.Amuru`, so `Git` did not resolve after FindRoot moved to Amuru.Tools.

Amuru 2.0.0-beta.2 depends on TimeWarp.Terminal 1.0.2. dev-cli also project-references `timewarp-terminal`. Restore unifies that dependency as the project (`type: project` in the assets file). No NU1107. The shipped Terminal package still does not reference Amuru.

This repo had no call sites for the removed Git `*Master` helpers, bool Git results, `"master"` branch default, removed dotnet builder options, or `WithStandardInput("")`. The breaking change that did not compile was TimeWarp.Nuru 3.0.0-beta.79: `ICommand`, `ICommandHandler`, and `Unit` moved to `TimeWarp.Mediator`, handlers return `Task<Unit>`, and `[NuruRoute]` commands are public. dev-cli endpoints follow that.

Audit `--fix` also added the SourceGenerators pin, `.local/` and routine-journal gitignore lines, `kanban/in-progress/.gitkeep`, peacock color, and removed the leftover `.memsearch.toml`. `ganda repo audit` then passed 28/0.

The PR and `ganda pr merge` stay for the host open-pr and merge nodes.

### How to validate

Smoke:

```bash
dotnet build timewarp-terminal.slnx -c Release
dotnet build tools/dev-cli/dev.cs -c Release
dotnet tools/dev-cli/dev.cs -- test
dotnet tools/dev-cli/dev.cs -- verify-samples
GANDA_ATTEST_HOOK=0 dotnet .githooks/post-merge.cs
ganda repo audit
```

Expect:

- Solution build: `Build succeeded.` with `0 Warning(s)` and `0 Error(s)`.
- dev-cli build exits 0.
- Tests print `✓ All 43 test file(s) passed`.
- Samples print `✓ All 6 sample(s) verified successfully`.
- post-merge exits 0 (`Git.FindRoot` resolves).
- Audit prints `Repository passes all audit checks.`

Observed 2026-10-09 in this worktree: solution Release build 0 warnings / 0 errors; dev-cli Release build succeeded; 43 test files passed; 6 samples verified; post-merge smoke exited 0; audit Passed 28, Failed 0.

### Review disposition

- Rounds: 1. Roster: general. Effort: 2 (by-diff, 382 lines).
- Counts: 0 bug / 0 suggestion / 0 nit (0 open, 0 fixed, 0 wontfix).
- Disposition: **clean**. Re-verified: Release build 0 warnings, 43/43 test files, audit passed.
- Artifacts: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md`.

## Notes

Filed 2026-10-09 at Steven's request (Amuru 2.0 sweep).

Terminal-specific: Amuru 2.0.0-beta.2 itself depends on TimeWarp.Terminal 1.0.2, so check for a package cycle before bumping Amuru here. In this repo Amuru/Amuru.Tools are only used by tools/dev-cli and hooks, not by the shipped TimeWarp.Terminal package, so the cycle is build-tooling only; confirm restore resolves cleanly. If it can't move, record why instead of forcing it.

Observed 2026-10-09: on master, `.githooks/post-merge` fails to compile (`error CS0103: The name 'Git' does not exist in the current context`) after `git pull` on TWE-001 WSL. Likely the hook runfile pins an Amuru version mismatch; refresh hooks with `ganda hooks install attest` as part of this task.
