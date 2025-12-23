# Create dev-cli tool for timewarp-terminal

## Description

Create a development CLI tool for the `timewarp-terminal` repository following the patterns established in `timewarp-nuru`'s dev-cli. This will provide CI/CD orchestration and development workflow automation.

## Checklist

- [ ] Fix `msbuild/repository.props` - Update `RepositoryName` and `SolutionFile` from `timewarp-nuru` to `timewarp-terminal`
- [ ] Update `Directory.Packages.props` - Add `TimeWarp.Nuru` version `3.0.0-beta.22`
- [ ] Create `tools/dev-cli/timewarp-terminal-dev-cli.csproj` with AOT support, project reference to timewarp-terminal, and package references to TimeWarp.Nuru, Mediator, TimeWarp.Amuru
- [ ] Create `tools/dev-cli/Directory.Build.props` with CLI-specific warning suppressions
- [ ] Create `tools/dev-cli/global-usings.cs`
- [ ] Create `tools/dev-cli/program.cs` entry point using NuruApp builder pattern
- [ ] Create `tools/dev-cli/commands/build-command.cs` - Build timewarp-terminal library
- [ ] Create `tools/dev-cli/commands/clean-command.cs` - Clean solution and artifacts
- [ ] Create `tools/dev-cli/commands/test-command.cs` - Run tests (TODO placeholder until tests exist)
- [ ] Create `tools/dev-cli/commands/verify-samples-command.cs` - Verify samples compile (TODO placeholder until samples exist)
- [ ] Create `tools/dev-cli/commands/check-version-command.cs` - Check NuGet publishing status for TimeWarp.Terminal
- [ ] Create `tools/dev-cli/commands/ci-command.cs` - CI pipeline orchestration with mode detection
- [ ] Update `timewarp-terminal.slnx` - Add dev-cli project to solution
- [ ] Verify build succeeds with `dotnet build`

## Notes

### Reference Implementation
Pattern follows `timewarp-nuru/tools/dev-cli/`:
- Uses TimeWarp.Nuru framework with `[NuruRoute]` attributed routes
- Mediator pattern with nested `Handler : ICommandHandler<TCommand, Unit>` classes
- All output through `ITerminal` interface for testability
- Shell operations via `TimeWarp.Amuru.Shell.Builder()` and `DotNet.*` helpers
- Commands throw `InvalidOperationException` for failures

### Key Adaptations from timewarp-nuru
- Look for `timewarp-terminal.slnx` instead of `timewarp-nuru.slnx`
- Build only `source/timewarp-terminal/timewarp-terminal.csproj`
- Check only `TimeWarp.Terminal` package on NuGet
- Read version from `source/timewarp-terminal/timewarp-terminal.csproj` (version is defined there, not in `source/Directory.Build.props`)

### Commands
| Command | Route | Description |
|---------|-------|-------------|
| build | `build --clean,-c? --verbose,-v?` | Build timewarp-terminal library |
| clean | `clean` | Clean solution and delete bin/obj |
| test | `test` | Run tests (placeholder with TODO) |
| verify-samples | `verify-samples` | Verify samples compile (placeholder with TODO) |
| check-version | `check-version` | Check if TimeWarp.Terminal is already published |
| ci | `ci --mode,-m?` | Orchestrate CI pipeline |

### Dependencies
- `TimeWarp.Nuru` v3.0.0-beta.22 (PackageReference)
- `TimeWarp.Amuru` (PackageReference - already in Directory.Packages.props)
- `Mediator.Abstractions` + `Mediator.SourceGenerator` (already in Directory.Packages.props)
- `timewarp-terminal.csproj` (ProjectReference - in-repo)
