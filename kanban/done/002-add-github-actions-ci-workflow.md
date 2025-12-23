# Add GitHub Actions CI workflow

## Description

Create a GitHub Actions workflow that runs the dev-cli CI pipeline on push, pull request, and release events.

## Checklist

- [ ] Create `.github/workflows/ci-cd.yml`
- [ ] Configure triggers for push, pull_request, release, and workflow_dispatch
- [ ] Setup .NET 10.0
- [ ] Configure NuGet OIDC trusted publishing for releases
- [ ] Run dev-cli ci command
- [ ] Upload package artifacts

## Notes

### Reference
Based on timewarp-nuru's `.github/workflows/ci-cd.yml` pattern.

### Triggers
- `push` to master (with path filters)
- `pull_request` to master (with path filters)
- `release` published
- `workflow_dispatch` for manual runs

### Path Filters
Only run on changes to:
- `source/**`
- `tools/**`
- `.github/workflows/**`
- `Directory.Build.props`
- `Directory.Packages.props`
