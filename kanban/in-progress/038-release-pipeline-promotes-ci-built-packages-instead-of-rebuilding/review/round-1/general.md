# Round 1 — general
**Date:** 2026-11-09
**Scope reviewed:** Same as the framework: branch vs `ffdf22f`, compared with the timewarp-nuru master template.

## Summary

The release job now promotes artifacts and no longer rebuilds. The job graph, tag gate, tag pin, ancestor check, locate-run, download, verify, isolated nupkg and snupkg push, probe, and the break-glass `confirm=release` guard all match the goal and the template. The docs describe the flow accurately.

One bug defeats the goal. `GeneratePackageOnBuild` is set to `true` unconditionally, so the Debug builds that samples and tests trigger later in the same CI job repack the packages. They overwrite the Release nupkg and snupkg in `artifacts/packages`, and master CI uploads those Debug packages. The release then promotes them to nuget.org.

## Issues

### Issue 1 — Severity: bug
- File: Directory.Build.props:11
- Description: `<GeneratePackageOnBuild>true</GeneratePackageOnBuild>` is unconditional. The merge pipeline (`tools/dev-cli/endpoints/workflow.cs:113-134`) runs the Release solution build first, then `verify-samples` (`dotnet run samples/*.cs`) and the test runfiles (`dotnet tests/*.cs`). Every sample and test uses `#:project $(SourceDirectory)timewarp-terminal[-layout]/…csproj`. Those runfiles build in Debug, and `Configuration=Debug` flows to the referenced source project. That project inherits `GeneratePackageOnBuild=true` and `PackageOutputPath=artifacts/packages`, so it packs again at the same version over the Release package. The opt-outs in `samples/` and `tests/Directory.Build.props` cover only the runfile projects, not the referenced source projects. The `Upload Artifacts` step then uploads the Debug packages, and the release job promotes and pushes them. timewarp-nuru hit this exact defect (task 503: 3.0.0-beta.79 and beta.80 shipped Debug assemblies) and fixed it with `<GeneratePackageOnBuild Condition="'$(Configuration)' == 'Release'">true</GeneratePackageOnBuild>` in `source/Directory.Build.props`. This port of the template dropped that guard.
- Evidence (reproduced in a throwaway `git archive HEAD` export at `/tmp/rv038.DLx0`, outside the worktree):
  - `dotnet build timewarp-terminal.slnx -c Release` → `TimeWarp.Terminal.2.0.0-beta.1.nupkg` 72952 bytes, sha256 `05dac55b…`; `.snupkg` 28816 bytes.
  - `cd samples && dotnet run rule-widget.cs -- --help` → `TimeWarp.Terminal.2.0.0-beta.1.nupkg` rewritten: 75637 bytes, sha256 `2f76a102…`, and `.snupkg` 32539 bytes. `source/timewarp-terminal/bin/Debug` now exists. The Layout package was untouched only because this sample does not reference Layout. The layout samples and tests would repack it as well.
  - The task Results check the packages only after the Release build. They do not check them after `verify-samples` and `test`, so this was not caught.
- Suggestion: Port the template guard. Make `GeneratePackageOnBuild` Release-only (`Condition="'$(Configuration)' == 'Release'"`), ideally scoped to `source/Directory.Build.props` with packability as the template does. Also consider a cheap CI tripwire: record the nupkg and snupkg hashes after Step 2 and fail the job if they changed by the end of Step 4. Re-verify by repeating the reproduction above, with a layout sample and one test file, and confirming the hashes are unchanged.
- Status: open

<!-- Checked with no finding: ci/release job `if` conditions for push / pull_request / dispatch merge|release|probe; confirm=release fails before nuget/login; OIDC login and rebuild-token conditions; GH_TOKEN (job token) vs REBUILD_DISPATCH_TOKEN split; upload only on green master push; keep-last-two prune (skips if the new artifact is not listed yet); concurrency groups; fetch-depth 0 gives origin/master for the ancestor check; tag assertion on release event + tag pin on every release-mode run; locate-run → artifact selection → download into artifacts/packages; package-set and snupkg verification; per-file isolated push (nupkg no longer auto-pushes sibling snupkg) with --skip-duplicate; dev-cli/tests/samples marked non-packable; releasing.md matches the code. Goal restatement matches the task description and checklist: no misread. -->
