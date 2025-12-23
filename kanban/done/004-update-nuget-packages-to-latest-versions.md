# Update NuGet packages to latest versions

## Description

Update all outdated NuGet packages identified by `dotnet outdated --pre-release always`. Key updates include TimeWarp.Nuru (which now defaults `.UseTerminal()` in `CreateBuilder()`), TimeWarp.Amuru, Mediator, and code analyzers.

## Checklist

- [x] Update Directory.Packages.props with new versions:
  - [x] TimeWarp.Nuru: 3.0.0-beta.22 → 3.0.0-beta.23
  - [x] TimeWarp.Amuru: 1.0.0-beta.13 → 1.0.0-beta.17
  - [x] Mediator.Abstractions: 3.0.1 → 3.1.0-preview.14
  - [x] Mediator.SourceGenerator: 3.0.1 → 3.1.0-preview.14
  - [x] Microsoft.CodeAnalysis.NetAnalyzers: 10.0.100 → 10.0.101
  - [x] Roslynator.Analyzers: 4.14.1 → 4.15.0
  - [x] Roslynator.CodeAnalysis.Analyzers: 4.14.1 → 4.15.0
  - [x] Roslynator.Formatting.Analyzers: 4.14.1 → 4.15.0
- [x] Remove `.UseTerminal(TimeWarpTerminal.Default)` from tools/dev-cli/program.cs (now default in Nuru)
- [x] Build solution to verify updates work
- [x] Run tests to ensure no regressions (no test projects in solution)

## Results

- **Build:** ✅ Succeeded with 0 warnings, 0 errors
- **Tests:** N/A (no test projects in solution)
- All 8 package versions updated in Directory.Packages.props
- Removed `.UseTerminal()` call from program.cs (now default behavior)

## Notes

### Package Changes Summary

**TimeWarp.Nuru 3.0.0-beta.23**
- `.UseTerminal()` is now default when using `CreateBuilder()` - can remove explicit call
- ITerminal injection updated for testable output

**TimeWarp.Amuru 1.0.0-beta.17**
- Git Default Branch Auto-Detection added
- Breaking: `Git.UpdateMasterAsync()` removed → use `Git.UpdateDefaultBranchAsync()`
- Breaking: `Git.GetCommitsAheadOfMasterAsync()` removed → use `Git.GetCommitsAheadOfDefaultBranchAsync()`
- (Verified: no usages of deprecated methods in this codebase)

**Mediator 3.1.0-preview.14**
- Preview release with updates (no breaking changes expected)

**Analyzers**
- Roslynator 4.15.0: Bug fixes and improvements
- Microsoft.CodeAnalysis.NetAnalyzers 10.0.101: Patch update
