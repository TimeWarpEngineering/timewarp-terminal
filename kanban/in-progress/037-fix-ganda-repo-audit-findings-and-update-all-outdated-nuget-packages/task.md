# Fix ganda repo audit findings and update all outdated NuGet packages

## Description

Bring the repo to a clean `ganda repo audit` and update every outdated NuGet package
reported by `ganda nuget outdated`, in one PR.

## Checklist

- [ ] Run `ganda repo audit --fix`; commit what it changes (audit fixes are policy)
- [ ] Fix any remaining `ganda repo audit` findings by hand until it exits clean
- [ ] Run `ganda nuget outdated`; update all outdated packages (major bumps included)
- [ ] Re-run `ganda nuget outdated` and confirm nothing is left
- [ ] Build and test pass via the dev CLI (`dev --capabilities` for commands)
- [ ] Re-run `ganda repo audit` and confirm it is clean after the package updates

## Session

- Created: 271402fa-1014-4c38-986b-db3f26ab9720 (2026-11-08)

## Notes

- TimeWarp packages come from https://nuget.pkg.github.com/TimeWarpEngineering/index.json, not nuget.org.
- If a package update breaks the build, fix the code to match the new API; do not pin the old version.
- Audit failures at task creation (2026-11-08): editorconfig-charset, glossary, kanban-porcelain-gitignore,
  nuru (TimeWarp.Nuru 3.0.0 -> 4.0.0-beta.11), required-gitignore-entries, routine-journals-gitignore,
  source-file-line-cap (7 C# files over 500 lines, no auto-fix: split by responsibility),
  vscode-settings-gitignore, workflow-file (pull_request.branches missing 'feature/**').
