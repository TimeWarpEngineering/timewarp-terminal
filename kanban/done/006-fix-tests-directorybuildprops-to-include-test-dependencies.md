# Fix tests Directory.Build.props to include test dependencies

## Description

The `tests/` directory is missing a `Directory.Build.props` file to include test-specific dependencies. Tests currently use `#:project` directives in shebang scripts but need common test packages like Shouldly and TimeWarp.Jaribu to be available.

## Checklist

- [ ] Create `tests/Directory.Build.props` following the pattern from timewarp-nuru
- [ ] Import parent `Directory.Build.props`
- [ ] Set `<IsPackable>false</IsPackable>`
- [ ] Add `<NoWarn>` for test-appropriate suppressions
- [ ] Add PackageReference for `Shouldly`
- [ ] Add PackageReference for `TimeWarp.Jaribu`
- [ ] Add PackageReference for `Microsoft.Extensions.Logging` (if needed)
- [ ] Add appropriate `<Using>` directives for test namespaces
- [ ] Add ProjectReference to `timewarp-terminal.csproj`
- [ ] Verify tests run successfully with `dotnet tests/ansi-string-utils-01-basic.cs`

## Notes

Reference implementation: `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-nuru/Cramer-2025-12-22-clean/tests/Directory.Build.props`

Key packages needed (from Directory.Packages.props):
- `Shouldly` - 4.3.0
- `TimeWarp.Jaribu` - 1.0.0-beta.7

The tests use TimeWarp.Jaribu test framework patterns:
- `RunAllTests()` for test execution
- `RegisterTests<T>()` for test registration
- `[TestTag]` attribute for categorization
- `[ModuleInitializer]` attribute

Current test files:
- `tests/ansi-string-utils-01-basic.cs`
- `tests/ansi-string-utils-02-wrap-text.cs`
