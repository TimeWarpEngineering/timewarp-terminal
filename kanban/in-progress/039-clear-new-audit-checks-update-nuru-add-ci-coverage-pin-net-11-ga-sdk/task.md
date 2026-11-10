# Clear new audit checks, update Nuru, add CI coverage, pin .NET 11 GA SDK

## Description

`ganda repo audit` on master (2026-11-11) fails 6 checks. Five are new checks, and one is a new
Nuru release. .NET 11 went GA on 2026-11-10, so also move the RC pins to stable. Ship all of it in one PR.

## Checklist

- [ ] Run `ganda repo audit --fix` and commit what it changes (audit fixes are policy):
      `nuru` (TimeWarp.Nuru 4.0.0-beta.11 -> 4.0.0-beta.12), `analyzer-suppressions`
      (seeds `.timewarp/suppression-baseline.json`), `timewarp-signals-gitignore`
      (`/.timewarp/signals/`), `memsearch-scaffold` (refresh the 5 `.githooks/*.cs` hooks)
- [ ] `workflow-pwsh-shell` (no auto-fix): add top-level `defaults: run: shell: pwsh` to
      `.github/workflows/workflow.yml` and convert every bash `run:` step to pwsh, including
      job `ci` step "Keep last two Packages artifacts" (task 038)
- [ ] `ci-code-coverage` (no auto-fix): collect coverage in the CI test run and upload it.
      The audit suggests MTP flags `--results-directory artifacts/coverage --coverage
      --coverage-output-format cobertura --coverage-output coverage.cobertura.xml` plus an
      `actions/upload-artifact@v4` step named `coverage` for `artifacts/coverage`.
      Tests here are Jaribu runfiles run by `dev test`, not `dotnet test`; pick the approach
      that fits and makes the check pass. Do not turn the check off without a stated reason.
- [ ] Move `global.json` SDK and `Microsoft.CodeAnalysis.NetAnalyzers` from the .NET 11 RC
      (`11.0.100-rc.3...`) to the stable 11.0.100 GA versions. Check that CI's setup-dotnet resolves them.
- [ ] Run `ganda nuget outdated` and take any other updates
- [ ] `dev workflow` passes locally (build, verify-samples, test, package-hash gate)
- [ ] `ganda repo audit` is clean

## Session

- Created: 271402fa-1014-4c38-986b-db3f26ab9720 (2026-11-11)

## Notes

- The release pipeline from task 038 must keep working: master CI uploads `Packages-*`, and the
  release job promotes it without rebuilding. Converting steps to pwsh must not break the
  artifact retention step or the tag-gate, locate-run, download, verify, and push steps.
- Do not cut a release.
