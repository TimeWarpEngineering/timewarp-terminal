# Update NuGet packages to latest versions

## Description

Update all outdated NuGet packages identified by `dotnet outdated --pre-release always`. Key updates include TimeWarp.Nuru (which now defaults `.UseTerminal()` in `CreateBuilder()`), TimeWarp.Amuru, Mediator, and code analyzers.

## Checklist

- [ ] Update Directory.Packages.props with new versions:
  - [ ] TimeWarp.Nuru: 3.0.0-beta.22 → 3.0.0-beta.23
  - [ ] TimeWarp.Amuru: 1.0.0-beta.13 → 1.0.0-beta.17
  - [ ] Mediator.Abstractions: 3.0.1 → 3.1.0-preview.14
  - [ ] Mediator.SourceGenerator: 3.0.1 → 3.1.0-preview.14
  - [ ] Microsoft.CodeAnalysis.NetAnalyzers: 10.0.100 → 10.0.101
  - [ ] Roslynator.Analyzers: 4.14.1 → 4.15.0
  - [ ] Roslynator.CodeAnalysis.Analyzers: 4.14.1 → 4.15.0
  - [ ] Roslynator.Formatting.Analyzers: 4.14.1 → 4.15.0
- [ ] Remove `.UseTerminal(TimeWarpTerminal.Default)` from tools/dev-cli/program.cs (now default in Nuru)
- [ ] Build solution to verify updates work
- [ ] Run tests to ensure no regressions

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
