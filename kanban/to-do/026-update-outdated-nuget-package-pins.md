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

- [ ] Create task and move to in-progress
- [ ] `ganda nuget outdated --update --force`
- [ ] Verify `Directory.Packages.props` matches the dry-run versions
- [ ] Build; fix DI / API / analyzer breaks from the bumps
- [ ] `dev self-install` if DevCli content or commands changed
- [ ] `dev test` and `dev verify-samples`
- [ ] `ganda repo audit` passes
- [ ] Commit pins, follow-up fixes, and this task card

## Notes

Followed task 025 (beta.72 adoption): last bump needed Amuru.Tools, DI
registration (`IPackableProjectService`), DevCli cache NoWarns, and kebab
renames. Expect similar NURU050 / NU1605 / new analyzer diagnostics.

Jaribu comment in props still warns that older betas pulled a vulnerable
MessagePack via Amuru; confirm beta.15 does not reintroduce NU1902/NU1903.

## Session

- Created: grok (2026-08-16)
