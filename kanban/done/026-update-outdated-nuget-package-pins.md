# Update outdated NuGet package pins

## Description

`ganda repo audit` is red on the `nuru` check (`TimeWarp.Nuru` 3.0.0-beta.72 <
3.0.0-beta.76). Bump every outdated central pin in `Directory.Packages.props`
via `ganda nuget outdated --update --force` and restore a green build, test
suite, samples, and repo audit.

Dry-run targets (do not jump to preview streams such as NetAnalyzers
`11.0.100-preview` or Roslynator `4.0.0-rc`):

- TimeWarp.Jaribu `1.0.0-beta.13` → `1.0.0-beta.15`
- TimeWarp.Nuru `3.0.0-beta.72` → `3.0.0-beta.76`
- TimeWarp.Nuru.DevCli `3.0.0-beta.72` → `3.0.0-beta.76`
- Roslynator.Analyzers / CodeAnalysis / Formatting `4.15.0` → `4.16.0`
- Microsoft.CodeAnalysis.NetAnalyzers `10.0.301` → `10.0.400`

## Requirements

- All pins come from `ganda nuget outdated --update --force` (or equivalent
  edits to `Directory.Packages.props` that match that dry-run)
- Solution builds with 0 warnings / 0 errors under existing TreatWarningsAsErrors
- `./bin/dev test` and `./bin/dev verify-samples` pass
- `ganda repo audit` exits 0
- If DevCli/Nuru APIs or content files change: update `tools/dev-cli/dev.cs`
  registrations and NoWarns, then `dev self-install`
- Record any structurally unfixable audit check with a reason instead of forcing

## Checklist

- [x] Create task and move to in-progress
- [x] `ganda nuget outdated --update --force`
- [x] Verify `Directory.Packages.props` matches the dry-run versions
- [x] Build; fix DI / API / analyzer breaks from the bumps
- [x] `dev self-install` if DevCli content or commands changed
- [x] `dev test` and `dev verify-samples`
- [x] `ganda repo audit` passes
- [x] Commit pins, follow-up fixes, and this task card

## Notes

Followed task 025 (beta.72 adoption): last bump needed Amuru.Tools, DI
registration (`IPackableProjectService`), DevCli cache NoWarns, and kebab
renames. Expect similar NURU050 / NU1605 / new analyzer diagnostics.

### Implementation notes (2026-08-16)

`ganda nuget outdated --update --force` applied all seven dry-run pins. No
preview-stream jumps (NetAnalyzers stayed on 10.0.400, not 11.0.100-preview;
Roslynator stayed on 4.16.0, not 4.0.0-rc).

No product or DevCli code changes: `dotnet run --file tools/dev-cli/dev.cs -- --help`
still exposes the same commands; existing DI registrations compiled. Self-install
skipped (AOT snapshot already has `release` / `check-version`; no new endpoints).

Jaribu beta.15 restored clean (no NU1902/NU1903). Build needed a local empty
`smoke` NuGet feed directory
(`timewarp-architecture/dev/artifacts/template-smoke/packages`) because that
user-level source is missing; not a repo change.

## Results

All central pins are current. Audit is green on Nuru 3.0.0-beta.76.

### How to validate

**Smoke**
```bash
cd /home/steve/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/dev
grep -E 'TimeWarp\.(Nuru|Jaribu)|Roslynator|NetAnalyzers' Directory.Packages.props
# Expect: Nuru + DevCli 3.0.0-beta.76; Jaribu 1.0.0-beta.15;
#         Roslynator* 4.16.0; NetAnalyzers 10.0.400

ganda nuget outdated
# Expect: All packages are up to date!

ganda repo audit
# Expect: Repository passes all audit checks (nuru included).
```

**Automated gate**
```bash
dotnet build timewarp-terminal.slnx -c Release   # 0 warnings / 0 errors
./bin/dev test                                   # 33/33
./bin/dev verify-samples                         # 5/5
ganda repo audit                                 # exit 0
```

**Depends on / Not in scope**
- Did not take NetAnalyzers 11.0.100-preview or Roslynator 4.0.0-rc
- Did not self-install `bin/dev` (no command/DI surface change)
- Local commits only; no push

## Session

- Created: grok (2026-08-16)
- Implementation: grok (2026-08-16)
