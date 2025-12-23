# Convert dev-cli to runfile structure

## Description

Convert the dev-cli from a traditional .csproj project to a runfile structure, matching the pattern used in timewarp-nuru. This enables running the CLI directly as `dotnet tools/dev-cli/dev.cs` without explicit compilation, while still supporting AOT compilation via `runfiles/publish-dev.cs`.

## Checklist

- [ ] Rename `tools/dev-cli/program.cs` to `tools/dev-cli/dev.cs`
  - [ ] Add shebang `#!/usr/bin/dotnet --` at top
  - [ ] Update header comments to reflect runfile usage
- [ ] Delete `tools/dev-cli/timewarp-terminal-dev-cli.csproj` (not needed for runfiles)
- [ ] Delete `tools/dev-cli/global-usings.cs` (move to Directory.Build.props)
- [ ] Update `tools/dev-cli/Directory.Build.props`:
  - [ ] Add global usings via `<Using Include="..." />` elements
  - [ ] Add AOT properties (`PublishAot`, `InvariantGlobalization`)
  - [ ] Add `<Compile Include="commands/**/*.cs" />`
  - [ ] Add project reference to timewarp-terminal
  - [ ] Add package references (Mediator, TimeWarp.Amuru)
  - [ ] Add suppressed warnings (CA1031, CA1303, etc.)
- [ ] Create `runfiles/` directory
- [ ] Create `runfiles/publish-dev.cs` for AOT binary publishing
- [ ] Update `timewarp-terminal.slnx` to remove the csproj reference (if present)
- [ ] Test: `dotnet tools/dev-cli/dev.cs --help`
- [ ] Test: `dotnet tools/dev-cli/dev.cs build`
- [ ] Clean up `tools/dev-cli/bin/` and `tools/dev-cli/obj/` directories

## Notes

### Reference: timewarp-nuru structure

```
tools/dev-cli/
├── Directory.Build.props   # Contains usings, AOT config, compile includes, references
├── commands/               # Command handler files
│   ├── build-command.cs
│   ├── ci-command.cs
│   └── ...
├── dev.cs                  # Runfile entry point (with shebang)
└── generated/              # Source generator output (auto-created)

runfiles/
└── publish-dev.cs          # AOT publish script
```

### Key differences from csproj approach

1. **No .csproj file** - dotnet SDK handles runfiles natively
2. **Shebang line** - `#!/usr/bin/dotnet --` enables direct execution on Unix
3. **Directory.Build.props** - Contains all configuration that would normally be in .csproj
4. **runfiles/publish-dev.cs** - Separate script to publish AOT binary to repo root

### Usage after conversion

```bash
# Run as runfile (interpreted)
dotnet tools/dev-cli/dev.cs --help
dotnet tools/dev-cli/dev.cs ci --mode pr

# Build AOT binary
dotnet runfiles/publish-dev.cs
./dev --help
```
