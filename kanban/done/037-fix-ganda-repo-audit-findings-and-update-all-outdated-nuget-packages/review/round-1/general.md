# Round 1 — general
**Date:** 2026-11-09
**Scope reviewed:** branch vs merge-base 07dcd3c (29 files, +4043 / −3783), per review-framework.md

## Summary

The change applies `ganda repo audit --fix` output (glossary scaffold, Nuru/Nuru.DevCli 4.0.0-beta.11,
Amuru/Amuru.Tools 3.0.0-beta.1, pre-push hook refresh, gitignore lines, `feature/**` PR trigger,
untracking `.vscode/settings.json`), fixes `.editorconfig` charset by hand, bumps NetAnalyzers to
10.0.403, moves dev-cli and `.githooks` to net11.0 (Amuru 3 ships net11.0 only) with CI installing the
11.0.x preview SDK next to 10.0.x, and splits seven over-cap C# files. Risk is low: the splits are pure
moves, and every gate the task names passes on a fresh run. No issues found.

## Verification performed

- **Split fidelity:** for each split group, compared the whitespace-normalized, sorted line multiset of
  the pre-change file with the union of its post-change files. The only differences are additive
  scaffolding: `partial` modifiers, per-file `namespace`, braces, Purpose/Design regions, and
  responsibility comments. No member body line was lost or changed.
  - `terminal-static.cs` → `terminal-static.cs` + `.write.cs` + `.widgets.cs` + `.console.cs` (one public partial `Terminal`)
  - `test-terminal.cs` → + `test-terminal.input.cs`; `timewarp-terminal.cs` → + `.geometry.cs`;
    `ansi-string-utils.cs` → + `.wrap.cs`; `table-widget.cs` → + `.layout.cs`
  - `tests/terminal-static-08-new-apis.cs` (44 test methods) → `-08-color` (13) + `-08-geometry` (13) + `-08-streams` (18),
    each its own class, namespace, and `[ModuleInitializer]` registration
  - `tests/stream-access-01-basic.cs` (36 methods, 2 classes) → `-01-basic` (`ConsoleStreamAccessTests`, 17) + `-02-terminal` (`TerminalStreamAccessTests`, 19)
  - No `-part` / `-N` shard names; the `NN-topic` test prefix follows the existing repo convention.
  - No stale references to `terminal-static-08-new-apis` or `TerminalStaticNewApis` remain outside kanban.
- **Gates (re-run by reviewer):** `ganda repo audit` → "Repository passes all audit checks." (exit 0,
  source-file-line-cap PASS); `ganda nuget outdated` → "All packages are up to date!";
  `dotnet run tools/dev-cli/dev.cs -- build` → 0 warnings, 0 errors;
  `dotnet run tools/dev-cli/dev.cs -- test` → "All 47 test file(s) passed".
- **No pinning to stay green:** every pin is at the latest feed version; the net11.0 move for dev-cli and
  hooks is the consequence of Amuru 3's TFM, not a downgrade. Product libraries stay net10.0.
- **`WorkflowCommand.Handler` ctor → internal:** matches DevCli 4's internal `IPackableProjectService`;
  DI registration in `tools/dev-cli/dev.cs:49` compiles and the dev CLI runs.
- **CI:** `.github/workflows/workflow.yml` installs both 10.0.x and 11.0.x (preview); no `global.json`
  pins the SDK, so `dotnet run tools/dev-cli/dev.cs` resolves to the 11 SDK.
- **No silencing:** no new `#pragma`, `[SuppressMessage]`, NoWarn, or BannedSymbols carve-outs in the diff.
- Audit `--fix` outputs (glossary scaffold, pre-push hook, gitignore, `.vscode/settings.json` untrack,
  Directory.Packages.props BOM removal) are policy baseline and are not flagged.

## Goal restatement vs task

The `## Goal (implementer restatement)` matches the task description and Notes (audit findings list,
split-by-responsibility, no old-version pins). No misread.

## Issues

None.
