# Update dev-cli to support self-install and bin prebuilt binary

## Description

Follow the Nuru repo pattern for dev-cli self-install and prebuilt binary support. Currently the dev-cli can only be invoked via `dotnet run --project tools/dev-cli` which is slow and cumbersome.

## Checklist

- [x] Fix pre-existing MissingMethodException with TimeWarp.Nuru dependency
- [x] Add `dev self-install` command to publish AOT binary to `bin/`
- [x] Add `bin/` to `.gitignore`
- [x] Set up PATH so `dev test` works from repo root (via .envrc)
- [x] Verify `dev test`, `dev build`, `dev workflow` all work from `bin/dev`

## Notes

### Implementation Details

- `dev self-install` command: `tools/dev-cli/endpoints/self-install.cs`
- AOT binary location: `bin/dev` (7.3 MB)
- Debug symbols: `bin/dev.dbg` (13 MB)
- PATH setup: `.envrc` with `export PATH="$PWD/bin:$PATH"`

### Verification

```bash
./bin/dev --help       # Works
./bin/dev test         # Works
./bin/dev build        # Works
./bin/dev workflow     # Works
```

## Results

All items completed. The dev-cli now supports:
- AOT compilation via `dev self-install`
- Fast execution from `bin/dev`
- PATH integration via `.envrc` (direnv)
- Proper gitignore for generated binaries
