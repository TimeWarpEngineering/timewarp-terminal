# Update TimeWarp.Build.Tasks to 2.0.0 and SourceGenerators to beta.14

## Description

`ganda nuget outdated` on master (2026-11-12) reports two outdated packages:

- `TimeWarp.Build.Tasks` 1.0.0 -> 2.0.0 (major; 1.0.2 also exists on the 1.x line)
- `TimeWarp.SourceGenerators` 1.0.0-beta.13 -> 1.0.0-beta.14

Take both latest versions. Build.Tasks runs in every build (analyzers/MSBuild tasks via
`Directory.Build.props`), and warnings are errors, so the major bump may surface new
diagnostics. Fix the code to satisfy them; do not pin 1.x and do not add blanket suppressions.

## Checklist

- [ ] Bump both pins in `Directory.Packages.props`
- [ ] Read the Build.Tasks 2.0.0 release notes / diff (TimeWarp feed or nuget.org) for breaking changes
- [ ] Fix any new diagnostics properly; any suppression has a stated reason
- [ ] `ganda nuget outdated` reports nothing outdated
- [ ] `dev workflow` passes locally (build, verify-samples, test, coverage, package-hash gate)
- [ ] `ganda repo audit` is clean

## Session

- Created: 271402fa-1014-4c38-986b-db3f26ab9720 (2026-11-12)

## Notes

- TimeWarp packages: https://nuget.pkg.github.com/TimeWarpEngineering/index.json as well as nuget.org.
- Do not cut a release.
