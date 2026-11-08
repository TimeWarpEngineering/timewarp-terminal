# Fix ganda repo audit findings and update all outdated NuGet packages

## Description

Bring the repo to a clean `ganda repo audit` and update every outdated NuGet package
reported by `ganda nuget outdated`, in one PR.

## Goal (implementer restatement)

This walk leaves timewarp-terminal with a clean `ganda repo audit` and with every
NuGet pin that `ganda nuget outdated` reports moved to the newest published version,
including major bumps. Auto-fixable audit findings are applied with
`ganda repo audit --fix` and kept. Findings that have no auto-fix are corrected in
the tree: gitignore and workflow gaps, a root `glossary.md`, charset in
`.editorconfig`, the TimeWarp.Nuru pin the nuru check names, and every counted C#
file over 500 lines split by responsibility. A facade that must stay one public
type is split into partial files named by that responsibility. A test file that
holds more than one concern becomes separate test classes. Names are not `-part`
or `-N` shards. When a package update changes a public API, call sites move to
the new API. Old versions are not pinned to keep the build green.

Acceptance criteria:

- `ganda repo audit` exits clean after the fixes and again after the package updates.
- `ganda nuget outdated` reports no remaining outdated packages.
- The dev CLI build and test commands pass.
- The checklist items above are done and the results section says how to smoke them.

## Checklist

- [x] Run `ganda repo audit --fix`; commit what it changes (audit fixes are policy)
- [x] Fix any remaining `ganda repo audit` findings by hand until it exits clean
- [x] Run `ganda nuget outdated`; update all outdated packages (major bumps included)
- [x] Re-run `ganda nuget outdated` and confirm nothing is left
- [x] Build and test pass via the dev CLI (`dev --capabilities` for commands)
- [x] Re-run `ganda repo audit` and confirm it is clean after the package updates

## Session

- Created: 271402fa-1014-4c38-986b-db3f26ab9720 (2026-11-08)
- Implementation: task-work implementer (2026-11-08)

## Results

`ganda repo audit --fix` scaffolded `glossary.md`, pinned TimeWarp.Nuru and TimeWarp.Nuru.DevCli to 4.0.0-beta.11, raised the Amuru and Amuru.Tools companion pins to 3.0.0-beta.1, refreshed the pre-push hook, gitignore porcelain lines, and `pull_request.branches` (`feature/**`), and stopped tracking `.vscode/settings.json`. The charset quotes in `.editorconfig` were removed by hand. Microsoft.CodeAnalysis.NetAnalyzers moved from 10.0.401 to 10.0.403, the only package `ganda nuget outdated` still reported after the nuru pin.

Amuru 3.0.0-beta.1 ships `net11.0` only. dev-cli and the `.githooks` runfiles now target `net11.0`. The product libraries stay on `net10.0`. CI installs the 10.0.x SDK and the 11.0.x preview SDK. `WorkflowCommand.Handler`'s constructor is internal because `IPackableProjectService` is internal in DevCli 4.

Seven files over the 500-line cap were split by responsibility. `Terminal`, `TimeWarpTerminal`, `TestTerminal`, `Table`, and `AnsiStringUtils` stay one public type each, in partial files. The static-facade tests and the terminal stream-access tests moved into their own test classes, and those new runfiles keep the executable bit the other test files use. `ganda repo audit` then passed (35 passed, 0 failed, 2 skipped). `ganda nuget outdated` printed `All packages are up to date!`.

`dotnet run tools/dev-cli/dev.cs -- build` succeeded with 0 warnings and 0 errors. `dotnet run tools/dev-cli/dev.cs -- test` passed all 47 test files. `dotnet run tools/dev-cli/dev.cs -- verify-samples` passed all 6 samples.

### How to validate

Smoke: from this worktree run `ganda repo audit`, then `ganda nuget outdated`, then `dotnet run tools/dev-cli/dev.cs -- build`, then `dotnet run tools/dev-cli/dev.cs -- test`.

Expect: audit exits 0 and reports no failed checks. Outdated prints `All packages are up to date!`. Build reports 0 warnings and 0 errors. The test command prints `All 47 test file(s) passed`.

## Notes

- TimeWarp packages come from https://nuget.pkg.github.com/TimeWarpEngineering/index.json, not nuget.org.
- If a package update breaks the build, fix the code to match the new API; do not pin the old version.
- Audit failures at task creation (2026-11-08): editorconfig-charset, glossary, kanban-porcelain-gitignore,
  nuru (TimeWarp.Nuru 3.0.0 -> 4.0.0-beta.11), required-gitignore-entries, routine-journals-gitignore,
  source-file-line-cap (7 C# files over 500 lines, no auto-fix: split by responsibility),
  vscode-settings-gitignore, workflow-file (pull_request.branches missing 'feature/**').
