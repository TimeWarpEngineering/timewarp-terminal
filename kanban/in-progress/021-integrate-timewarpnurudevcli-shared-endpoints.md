# Integrate TimeWarp.Nuru.DevCli shared endpoints

## Description

Replace custom `clean`, `check-version`, and `self-install` endpoint implementations with shared endpoints from the `TimeWarp.Nuru.DevCli` NuGet package. This reduces code duplication across TimeWarp repositories and ensures consistent behavior.

## Checklist

- [ ] Add `TimeWarp.Nuru.DevCli` package version to `Directory.Packages.props`
- [ ] Add `<PackageReference Include="TimeWarp.Nuru.DevCli" />` to `tools/dev-cli/Directory.Build.props`
- [ ] Update `tools/dev-cli/dev.cs` to register Amuru services in `ConfigureServices()`
- [ ] Delete `tools/dev-cli/endpoints/clean.cs` (replaced by shared)
- [ ] Delete `tools/dev-cli/endpoints/check-version.cs` (replaced by shared)
- [ ] Delete `tools/dev-cli/endpoints/self-install.cs` (replaced by shared)
- [ ] Run `./bin/dev self-install` to rebuild the CLI
- [ ] Verify `./bin/dev clean` works
- [ ] Verify `./bin/dev check-version` works
- [ ] Verify `./bin/dev self-install` works

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
