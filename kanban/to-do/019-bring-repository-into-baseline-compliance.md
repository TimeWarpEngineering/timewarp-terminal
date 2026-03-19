# Bring repository into baseline compliance

## Description

Fix all failing baseline audit checks to bring the repository into compliance. The audit revealed 6 failing checks that need to be addressed.

## Checklist

### Error-level issues (must fix)

- [ ] Fix `.envrc` to use `PATH_add bin` instead of `export PATH="$PWD/bin:$PATH"`
- [ ] Create `BannedSymbols.txt` in repository root
- [ ] Add BannedApiAnalyzers configuration to `Directory.Build.props`
- [ ] Fix dev CLI capabilities JSON trailing comma issue in `tools/dev-cli/dev.cs`

### Warning-level issues (should fix)

- [ ] Add `#region Purpose` annotations to 8 dev-cli files:
  - [ ] `tools/dev-cli/dev.cs`
  - [ ] `tools/dev-cli/endpoints/verify-samples.cs`
  - [ ] `tools/dev-cli/endpoints/build.cs`
  - [ ] `tools/dev-cli/endpoints/self-install.cs`
  - [ ] `tools/dev-cli/endpoints/check-version.cs`
  - [ ] `tools/dev-cli/endpoints/test.cs`
  - [ ] `tools/dev-cli/endpoints/clean.cs`
  - [ ] `tools/dev-cli/endpoints/workflow.cs`
- [ ] Clean up orphaned CPM packages in `Directory.Packages.props`:
  - [ ] `TimeWarp.Build.Tasks` (version 1.0.0)
  - [ ] `GlobalUsingsAnalyzer` (version 1.4.0)
  - [ ] `Microsoft.CodeAnalysis.CSharp` (version 5.0.0)
  - [ ] `Microsoft.CodeAnalysis.Analyzers` (version 4.14.0)

### Verification

- [ ] Run `ganda repo audit` to verify all checks pass
- [ ] Commit changes

## Notes

### Audit Results (2026-03-18)

```
Passed: 5 | Failed: 6

Failing checks:
1. baseline-envrc (Error) - .envrc does not contain PATH_add bin
2. baseline-banned-symbols (Error) - BannedSymbols.txt is missing
3. baseline-banned-api-analyzers (Error) - Directory.Build.props missing BannedApiAnalyzers config
4. baseline-dev-cli-capabilities (Error) - Capabilities JSON has trailing comma
5. baseline-region-annotations (Warning) - 8 files missing #region Purpose
6. baseline-cpm-consistency (Warning) - 4 orphaned PackageVersion entries
```

### Passing checks (for reference)

- `baseline-bin-dev` - bin/dev is present
- `baseline-source-props` - source/Directory.Build.props exists
- `baseline-msbuild-props` - msbuild/repository.props exists
- `baseline-directory-packages` - Directory.Packages.props exists
- `baseline-runfile-variables` - All #:project directives use MSBuild variables

### Related Issue

The NuGet release workflow failed due to workflow name mismatch:
- Workflow renamed from `ci-cd.yml` to `workflow.yml`
- NuGet trusted publishing policy still expects `ci-cd.yml`
- This should be addressed separately (update NuGet policy or rename workflow back)
