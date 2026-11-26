# Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-terminal

## Description

Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-terminal.
Filter: (all)
Outdated:
  TimeWarp.Nuru 4.0.0-beta.16 -> 4.0.0-beta.17 (dev-only: tools/dev-cli/Directory.Build.props)
  TimeWarp.Nuru.DevCli 4.0.0-beta.16 -> 4.0.0-beta.17 (dev-only: tools/dev-cli/Directory.Build.props)
Every outdated pin is dev-only. Bump them. Do not change <Version> in source/Directory.Build.props: the wave merges this task and does not release this repo.
Edit Directory.Packages.props in this repo only.

## Session

- Minted by ganda task wave run 20261126T021837Z-0cb631
- Implement oracle: implemented by implementer-claude (claude, vendor anthropic, model claude-opus-5-5, response model claude-opus-5-5), session 54432537-2167-4ef7-93d7-22200a17db5a, max-turns 500, Done — 2026-11-26T03:03:53Z
- Review oracle: grok session 01a215cf-6063-7700-9e15-5fce375371b9 (2026-11-26), effort 1, roster general, turn cap 80, disposition clean
- Review oracle: review by reviewer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a215cf-6063-7700-9e15-5fce375371b9, max-turns 80, artifacts general.md, merged.md, disposition.md — 2026-11-26T03:06:54Z

## Goal (implementer restatement)

Bump the two dev-only TimeWarp pins in `Directory.Packages.props` from 4.0.0-beta.16 to 4.0.0-beta.17:
`TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` (consumed only by `tools/dev-cli`).

Acceptance criteria:
- Both `PackageVersion` entries read `4.0.0-beta.17`; no other package or file changes.
- `<Version>` in `source/Directory.Build.props` is unchanged (no release of this repo).
- The dev CLI rebuilds against the new packages and `./bin/dev build` passes.
- `ganda repo audit` passes.

## Results

- `Directory.Packages.props`: `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` 4.0.0-beta.16 -> 4.0.0-beta.17. No other change; `source/Directory.Build.props` `<Version>` untouched.

Gates:
- `dotnet run --file tools/dev-cli/dev.cs -- self-install` (rebuild dev CLI on beta.17): success.
- `./bin/dev build`: success, 0 warnings, 0 errors.
- Test files: none added or edited. No test under `tests/` covers `tools/dev-cli` (grep for `dev-cli`/`DevCli` in `tests/` finds nothing); smoke `./bin/dev --capabilities` exit 0 instead. CI runs the full suite.
- `ganda repo audit`: passes, exit 0. One advisory (non-blocking) warning, `vscode-window-icon`: `.vscode/settings.json` is missing. That file is gitignored local editor state, so there is nothing committable to fix here.

### How to validate

Smoke:
```bash
grep -n 'TimeWarp.Nuru' Directory.Packages.props
dotnet run --file tools/dev-cli/dev.cs -- self-install && ./bin/dev build && ./bin/dev --capabilities > /dev/null
```
Expect: both lines show `Version="4.0.0-beta.17"`; self-install reports success; build ends with `Build completed successfully`; `--capabilities` exits 0.

### Review

- Rounds: 1. Roster: general. Effort: 1.
- Counts: bug 0 open / 0 fixed / 0 wontfix; suggestion 0 / 0 / 0; nit 0 / 0 / 0.
- Disposition: clean. No wontfix and no escalation. The implementer restatement matches the task (two dev-only pins in `Directory.Packages.props`; product `<Version>` left alone), so there is no misread under `## Learnings`.
- Paths: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md`.

## Review

- Round 1: Done
