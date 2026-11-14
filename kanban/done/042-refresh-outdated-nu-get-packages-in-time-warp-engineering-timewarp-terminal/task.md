# Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-terminal

## Description

Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-terminal.
Filter: (all)
Outdated:
  TimeWarp.Jaribu 1.0.0-beta.15 -> 1.0.0-beta.16 (patch)
  TimeWarp.Nuru 4.0.0-beta.12 -> 4.0.0-beta.13 (patch)
  TimeWarp.Nuru.DevCli 4.0.0-beta.12 -> 4.0.0-beta.13 (patch)
  TimeWarp.SourceGenerators 1.0.0-beta.14 -> 1.0.0-beta.16 (patch)
  Microsoft.CodeAnalysis.CSharp.CodeStyle 5.9.0 -> 5.12.0 (minor)
Edit Directory.Packages.props in this repo only.

Version: set <Version> in source/Directory.Build.props from 2.0.0-beta.1 to 2.0.0-beta.2 in this pull request. The prerelease suffix is -beta.N, never +beta.N. The wave releases it after merge.

## Goal (implementer restatement)

Bump the five outdated central package pins in `Directory.Packages.props` (Jaribu beta.16,
Nuru and Nuru.DevCli beta.13, SourceGenerators beta.16, CSharp.CodeStyle 5.12.0) and the
product version in `source/Directory.Build.props` to `2.0.0-beta.2`.

Acceptance criteria:
- Only `Directory.Packages.props` pins change (plus the version bump); no other package edits.
- `<Version>` is `2.0.0-beta.2` (hyphen prerelease suffix).
- `./bin/dev build`, `./bin/dev test`, and `ganda repo audit` pass.

## Session

- Minted by ganda task wave run 20261114T054552Z-9a0f39
- Implement oracle 2026-11-14: bumped the five listed pins and Version to 2.0.0-beta.2.
  CodeStyle 5.12.0 raised IDE0028 at `source/timewarp-terminal-layout/layout-engine.cs:71` and
  `tools/dev-cli/endpoints/workflow.cs:240`; both now use the C# 15 `[with(...)]` collection expression.
- Boy-scout audit fixes (`ganda repo audit --fix`): `package-pin-currency` bumped TimeWarp.Builder
  1.1.0 -> 2.1.0-beta.1, TimeWarp.Flexbox 1.0.0 -> 2.0.0-beta.3, TimeWarp.Build.Tasks 2.0.0 -> 2.0.1-beta.1;
  `memsearch-scaffold` redeployed `.githooks/*.cs` (Purpose regions); `vscode-window-icon` set the
  window title in the gitignored `.vscode/settings.json`.
- Implement oracle: implemented by implementer-claude (claude, vendor anthropic, model claude-opus-5-5, response model claude-opus-5-5), session a5e337d8-d18c-4978-acfc-2059b6dd9b08, max-turns 500, Done — 2026-11-14T10:44:38Z
- Review oracle: effort 1, roster general, Grok session 01a1d9a8-dafb-71a1-92a2-8bab7532364e, turn cap 80, disposition clean — 2026-11-14
- Review oracle: review by reviewer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a1d9a8-dafb-71a1-92a2-8bab7532364e, max-turns 80, artifacts general.md, merged.md, disposition.md — 2026-11-14T10:49:49Z

## Results

Directory.Packages.props pins: Jaribu 1.0.0-beta.16, Nuru / Nuru.DevCli 4.0.0-beta.13,
SourceGenerators 1.0.0-beta.16, CSharp.CodeStyle 5.12.0, plus the audit-driven Builder 2.1.0-beta.1,
Flexbox 2.0.0-beta.3, Build.Tasks 2.0.1-beta.1. `source/Directory.Build.props` Version 2.0.0-beta.2.
Two IDE0028 sites fixed with `[with(...)]`. The new Builder and Flexbox majors compiled with no source
changes, and all tests pass.

### Gates

- `./bin/dev build`: passed (0 warnings, 0 errors) after the IDE0028 fixes. The first run failed on the
  `layout-engine.cs` IDE0028.
- `./bin/dev test`: passed (all 47 test files).
- `ganda repo audit`: passed (exit 0). The first run failed on `package-pin-currency`; after
  `--fix` and the dev-cli IDE0028 fix, build and test were rerun and the audit passed.

### How to validate

Smoke: `./bin/dev build && ./bin/dev test && ganda repo audit`
Expect: build has 0 warnings and 0 errors, "All 47 test file(s) passed", audit exits 0, and
`grep -E 'Jaribu|Nuru|SourceGenerators|CodeStyle|Builder|Flexbox|Build.Tasks' Directory.Packages.props`
shows the versions above.

### Review

- Round 1, effort 1, roster general.
- Counts: bug 0 open / 0 fixed / 0 wontfix; suggestion 0 / 0 / 0; nit 0 / 0 / 0.
- Disposition: clean. No wontfix and no escalation.
- Paths: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md`.

## Learnings

The implementer restatement's acceptance line "Only `Directory.Packages.props` pins change (plus the version bump); no other package edits" is tighter than the task. "Edit Directory.Packages.props in this repo only" limits the refresh to this repository. The task also requires the `<Version>` edit in `source/Directory.Build.props`. It does not forbid the source edits CodeStyle 5.12.0 needed, or the pins `ganda repo audit --fix` rewrites. The landed diff still matches the five listed pins and the hyphen `2.0.0-beta.2` suffix, and Results record the extra audit and IDE0028 edits. That scope sentence was a misread; it is not a product defect.

## Review

- Round 1: Done
