# Configure NuGet Trusted Publishing for TimeWarp.Terminal

## Description

The CI/CD release workflow fails at the "Push to NuGet" step with a 401 Unauthorized error because NuGet Trusted Publishing (OIDC) is not configured for the `TimeWarp.Terminal` package on NuGet.org.

## Checklist

- [ ] Log in to NuGet.org as TimeWarp.Enterprises
- [ ] Navigate to the TimeWarp.Terminal package (or create it if first publish)
- [ ] Go to Package Settings → Trusted Publishers
- [ ] Add GitHub Actions as a trusted publisher:
  - Repository: `TimeWarpEngineering/timewarp-terminal`
  - Workflow: `.github/workflows/ci-cd.yml`
  - Environment: (leave blank or specify if using environments)
- [ ] Re-run the failed release workflow to verify fix

## Notes

### Error from CI Run #20451103544
```
warn : No API Key was provided and no API Key could be found for 'https://www.nuget.org/api/v2/package'.
error: Response status code does not indicate success: 401 (An API key must be provided in the 'X-NuGet-ApiKey' header to use this service).
```

### How Trusted Publishing Works
1. The `nuget/login@v1` action in the workflow requests an OIDC token from GitHub
2. NuGet.org validates the token against the trusted publisher configuration
3. If the repository/workflow matches, NuGet.org accepts the package upload without an API key

### Reference
- NuGet Trusted Publishing docs: https://learn.microsoft.com/en-us/nuget/nuget-org/publish-a-package#trusted-publishing
- The same configuration is already working for `TimeWarp.Nuru` package

### First-Time Package Publish
If `TimeWarp.Terminal` has never been published to NuGet.org before, you may need to:
1. First publish manually with an API key to create the package
2. Then configure trusted publishing for subsequent releases
