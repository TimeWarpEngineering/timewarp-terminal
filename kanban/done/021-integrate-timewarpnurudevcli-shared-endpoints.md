# Integrate TimeWarp.Nuru.DevCli shared endpoints

## Description

Replace custom `clean`, `check-version`, and `self-install` endpoint implementations with shared endpoints from the `TimeWarp.Nuru.DevCli` NuGet package. This reduces code duplication across TimeWarp repositories and ensures consistent behavior.

## Checklist

- [x] Add `TimeWarp.Nuru.DevCli` package version to `Directory.Packages.props`
- [x] Add `<PackageReference Include="TimeWarp.Nuru.DevCli" />` to `tools/dev-cli/Directory.Build.props`
- [x] Update `tools/dev-cli/dev.cs` to register Amuru services in `ConfigureServices()`
- [x] Delete `tools/dev-cli/endpoints/clean.cs` (replaced by shared)
- [x] Delete `tools/dev-cli/endpoints/check-version.cs` (replaced by shared)
- [x] Delete `tools/dev-cli/endpoints/self-install.cs` (replaced by shared)
- [x] Run `./bin/dev self-install` to rebuild the CLI
- [x] Verify `./bin/dev clean` works
- [x] Verify `./bin/dev check-version` works
- [x] Verify `./bin/dev self-install` works

## Notes

### Service Registration

The shared endpoints require these services from TimeWarp.Amuru:
- `IRepoCleanService` / `RepoCleanService` - comprehensive cleaning (bin/obj directories)
- `IRepoCheckVersionService` / `RepoCheckVersionService` - version checking with multiple strategies
- `IRepoConfigService` / `RepoConfigService` - per-repo config defaults

Register in `dev.cs`:
```csharp
NuruApp app = NuruApp.CreateBuilder()
  .WithDescription("Development CLI for timewarp-terminal")
  .ConfigureServices(services =>
  {
    services.AddSingleton<IRepoCleanService, RepoCleanService>();
    services.AddSingleton<IRepoCheckVersionService, RepoCheckVersionService>();
    services.AddSingleton<IRepoConfigService, RepoConfigService>();
  })
  .DiscoverEndpoints()
  .Build();
```

### Source-Gen DI

Nuru's source-gen DI handles services registered via `ConfigureServices()`. No `UseMicrosoftDependencyInjection()` needed.

### repo.yaml (Optional)

Can create `.timewarp/repo.yaml` for check-version defaults:
```yaml
check-version:
  strategy: nuget-search
  packages: TimeWarp.Terminal
```

This is optional - can pass `--strategy nuget-search --package TimeWarp.Terminal` on command line instead.

### Shared vs Custom Endpoints

| Endpoint | Shared Package | Notes |
|----------|---------------|-------|
| `clean` | Yes | Uses `IRepoCleanService` |
| `check-version` | Yes | Uses `IRepoCheckVersionService`, `IRepoConfigService` |
| `self-install` | Yes | Standalone (no service deps) |
| `build` | No | Keep custom |
| `test` | No | Keep custom |
| `verify-samples` | No | Keep custom |
| `workflow` | No | Keep custom (orchestrates other commands) |

## Results

Integration landed in commit `1e930f5` (feat: integrate TimeWarp.Nuru.DevCli shared
endpoints) and was finished and verified on 2026-07-02/03 alongside the audit-compliance
work (`26bcb7f`, `a36a9bd`, `472037c`):

- `TimeWarp.Nuru.DevCli` at 3.0.0-beta.71, matched with `TimeWarp.Nuru` 3.0.0-beta.71.
- Actual service registration differs slightly from the sketch above — the shipped
  check-version endpoint uses `NuGetVersionService` + `GitTagCheckService` rather than an
  `IRepoCheckVersionService`:
  `IRepoCleanService`, `NuGetVersionService`, `GitTagCheckService`, `IRepoConfigService`.
- `endpoints/` retains only the keep-custom four: `build`, `test`, `verify-samples`, `workflow`.
- The DevCli package's content files are compiled from the NuGet cache where repo
  `.editorconfig` cannot reach, so `tools/dev-cli/Directory.Build.props` suppresses the
  style rules they violate (IDE0005/0052/0055/0058/0160/0290) plus IDE0211 (runfiles
  require top-level statements).
- Verified: `self-install` rebuilt `bin/dev` (AOT); `check-version` correctly reports
  1.0.0-beta.13 already published (version bump needed before next release); `clean`
  removes bin/obj and root bin/ artifacts while preserving the `dev` executable.
- `.timewarp/repo.yaml` was not created — check-version works without it.

## Session

- Implementation: pre-2026-07-02 (commit `1e930f5`)
- Verification & close-out: 179c59aa-cc0e-4094-99f1-0692f501682b (2026-07-03)
