# Round 1 — general
**Date:** 2026-11-11
**Scope reviewed:** branch vs merge-base `faf3d0b` (product commit `34d7f03`), all 86 files; deep read of `.github/workflows/workflow.yml`, `tools/dev-cli/endpoints/{test,workflow,build,verify-samples}.cs`, `tools/dev-cli/Directory.Build.props`, `.githooks/*.cs`, `Directory.Packages.props`, `global.json`, `.config/dotnet-tools.json`, `tests/coverage.runsettings`, `.timewarp/suppression-baseline.json`, `.editorconfig`, `.gitignore`; sampled `#region Purpose` additions across `source/`, `samples/`, `tests/`.

## Summary

The change clears the six new audit failures, pins .NET 11.0.100 GA, and takes the remaining package updates in one PR without cutting a release. Risk is low. The pwsh port of the artifact-prune step keeps the bash semantics: ordinal reverse sort of the TSV lines matches `LC_ALL=C sort -r`, the step skips when the just-uploaded artifact is not listed yet, it keeps the newest two, and each `gh` call checks `$LASTEXITCODE`. The release job's promotion path in `workflow.cs` is unchanged. Moving the NuGet API key from an inline `${{ }}` expression into an env var is a small security improvement. Coverage wraps one `dev test` run in a single `dotnet-coverage collect` session, and `workflow.cs` now reuses `TestCommand.Handler` instead of keeping its own copy of the runfile loop. No issues raised.

## Goal / task alignment

The implementer restatement matches the task. Amuru `3.0.0-beta.2` and the `IRequestHandler`/`ValueTask<Unit>` handler migration follow from the `nuru` audit fix. The `#region Purpose` work follows from the `TimeWarp.SourceGenerators` update (`TWA0004`). The release-promotion constraint comes from task Notes. No misread.

## Verification performed

- `ganda context --json`: task 039, branch ahead 6 / behind 5 of `master`, only the kitchen `review/` was untracked.
- `dotnet --version` prints `11.0.100` with the new `global.json` (`allowPrerelease: false`).
- `ganda repo audit` (sandbox): `Passed: 42 | Failed: 1 | Skipped: 1`. All six target checks pass (`nuru`, `analyzer-suppressions` "97 analyzer suppressions", `timewarp-signals-gitignore`, `memsearch-scaffold`, `workflow-pwsh-shell`, `ci-code-coverage`), and so does `dev-cli-capabilities`. The one failure is `self-hosted-runner`, which needs `gh repo view` network access the sandbox denied. It is environmental, not part of this diff.
- `dotnet build timewarp-terminal.slnx -c Release -p:ContinuousIntegrationBuild=true --no-incremental` (offline against `~/.nuget/packages`): exit 0, `0 Warning(s)`, `0 Error(s)`. This covers `TWA0004` with `TimeWarp.SourceGenerators` beta.13 and `Microsoft.CodeAnalysis.NetAnalyzers` 11.0.100.
- `dotnet-coverage collect` propagates the child exit code (probe `sh -c 'exit 3'` returned 3), so a failing runfile fails `dev test --coverage` and `dev workflow`. `--disable-console-output` does not hide child stdout or stderr (probe), so CI logs keep per-runfile output.
- `artifacts/coverage/coverage.cobertura.xml` from the implementer's run (02:36, after the last dev-cli source edit at 02:28): line-rate 0.7717, packages limited to `TimeWarp.Terminal` / `TimeWarp.Terminal.Layout`, matching `tests/coverage.runsettings` and the Results claim of about 77.2%.
- `bin/dev` (AOT self-install, 02:38, after the final source edits) exposes `test [--coverage,-c]`, which is evidence the migrated dev CLI compiles.
- `tests/*.cs` count is 47, matching Results.
- No empty `#region Purpose` blocks. Every added Purpose region has a real description. The diff adds no `#pragma`, `[SuppressMessage]`, or `NoWarn` entries. The dev-cli `<NoWarn>` lost 10 IDE codes plus IDE0065.
- `.editorconfig` removes `dev-cli-capabilities.severity = off`, which re-enables that check.
- `ExemptPackageContentFromAnalyzers` matches the decided org pattern in `timewarp-architecture` `tools/Directory.Build.props` (task 470).

Not re-run here: `./bin/dev workflow` end to end and `ganda nuget outdated`. The sandbox denied writes to the runfile cache under `~/.local/share/dotnet` and `/run/user/1000`, and it blocked nuget.org. Unsandboxed and full-network runs were declined. The static review, the probes, and the implementer's on-disk coverage and package artifacts are consistent with the Results claims.

## Considered and not raised

- **`ExemptPackageContentFromAnalyzers` analyzer carve-out:** this was checked against the audit-gaming rule. It sets `dotnet_analyzer_diagnostic.severity = none` only for `Compile` items under `$(NuGetPackageRoot)` (the `TimeWarp.Nuru.DevCli` content files the repo does not own and cannot edit). It replaces 10 dev-cli-wide IDE `NoWarn` codes that also silenced those rules on repo-owned sources. The net effect is more analyzer coverage on owned code, and it follows the decided pattern from `timewarp-architecture` task 470. It is not a bug.
- **`.githooks/pre-commit.cs` / `pre-push.cs` now refuse when the repo root is not found:** this is the `memsearch-scaffold` attest baseline, which the task treats as policy.
- **Release step `--api-key $env:NUGET_LOGIN_API_KEY`:** an unset value would drop the argument under pwsh. The step only runs after a successful `nuget/login`, which always sets the output, so this is not reachable.
- The `test.cs` `#region Design` comment says "all 47 runfile child processes". The count is correct today but will drift as tests are added. This is cosmetic and not raised.

## Issues

<!-- None. -->
