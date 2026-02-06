# Update dev-cli to support self-install and .bin prebuilt binary

## Description

Follow the Nuru repo pattern for dev-cli self-install and prebuilt binary support. Currently the dev-cli can only be invoked via `dotnet run --project tools/dev-cli` which is slow and cumbersome.

## Checklist

- [ ] Fix pre-existing MissingMethodException with TimeWarp.Nuru dependency
- [ ] Add `dev --self-install` command to publish AOT binary to `.bin/`
- [ ] Add `.bin/` to `.gitignore`
- [ ] Set up PATH so `dev test` works from repo root (via .envrc or similar)
- [ ] Verify `dev test`, `dev build`, `dev ci` all work from `.bin/dev`

## Notes

- Reference: `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-nuru` dev-cli implementation
- The MissingMethodException is a pre-existing issue: `NuruCoreAppBuilder..ctor(NuruCoreApplicationOptions)` not found — likely a version mismatch with TimeWarp.Nuru NuGet
