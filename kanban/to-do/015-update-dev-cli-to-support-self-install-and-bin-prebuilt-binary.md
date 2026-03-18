# Update dev-cli to support self-install and bin prebuilt binary

## Description

Follow the Nuru repo pattern for dev-cli self-install and prebuilt binary support. Currently the dev-cli can only be invoked via `dotnet run --project tools/dev-cli` which is slow and cumbersome.

## Checklist

- [x] Fix pre-existing MissingMethodException with TimeWarp.Nuru dependency
- [x] Add `dev self-install` command to publish AOT binary to `bin/`
- [ ] Add `bin/` to `.gitignore`
- [x] Set up PATH so `dev test` works from repo root (via .envrc)
- [x] Verify `dev test`, `dev build`, `dev workflow` all work from `bin/dev`

## Notes

### Current Status (2026-03-19)

**Completed:**
- `dev self-install` command exists and works (`tools/dev-cli/endpoints/self-install.cs`)
- `bin/` directory contains AOT binary (`bin/dev`, `bin/dev.dbg`)
- `.envrc` adds `bin/` to PATH: `export PATH="$PWD/bin:$PATH"`
- All commands work: `dev test`, `dev build`, `dev workflow`

**Remaining:**
- Add `bin/` to `.gitignore` (currently only `*.binlog` is ignored)

### Reference Implementation

- Reference: `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-nuru` dev-cli implementation
- Note: Implementation uses `bin/` (not `.bin/`) to match Nuru pattern

### Verification

```bash
./bin/dev --help       # Works
./bin/dev test         # Works
./bin/dev build        # Works
./bin/dev workflow     # Works
```
