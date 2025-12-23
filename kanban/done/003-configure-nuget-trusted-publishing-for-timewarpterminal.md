# Fix NuGet Trusted Publishing API Key Passing

## Description

The CI/CD release workflow fails at the "Push to NuGet" step with a 401 Unauthorized error. Initial diagnosis incorrectly assumed NuGet Trusted Publishing wasn't configured - it was. The real issue was that the `nuget/login@v1` action outputs the API key but doesn't automatically make it available to `dotnet nuget push`. The key must be explicitly passed.

## Checklist

- [x] Investigate CI failure - found 401 error on push
- [x] Verify NuGet Trusted Publishing is configured (it was already correct)
- [x] Discover that `nuget/login@v1` only sets output, not environment variable
- [x] Confirm blog post claiming "no API key needed" is misleading (author's own repo passes it explicitly)
- [x] Add `--api-key` option to `ci-command.cs`
- [x] Update `PushPackagesAsync` to use the API key
- [x] Update `ci-cd.yml` to pass the API key from `nuget-login` step output
- [x] Verify build succeeds
- [ ] Create PR and verify release workflow works

## Notes

### Root Cause
The `nuget/login@v1` action only does `core.setOutput('NUGET_API_KEY', apiKey)` - it does NOT:
- Set an environment variable
- Write to a NuGet config file
- Make the key automatically available to subsequent `dotnet` commands

### The Misleading Blog Post
Gerald Versluis (Microsoft employee) wrote https://blog.verslu.is/nuget/trusted-publishing-easy-setup/ which claims:
> "You don't need to explicitly pass the API key anymore. It's automatically used by the subsequent dotnet commands."

But his actual workflow at https://github.com/jfversluis/maui-version/blob/main/.github/workflows/release.yml **does** pass the key explicitly:
```yaml
--api-key "${{ steps.nuget-login.outputs.NUGET_API_KEY }}"
```

### Fix Applied
1. Added `--api-key` option to `ci` command
2. Updated workflow to pass the key on release events:
```yaml
dotnet run --project tools/dev-cli/timewarp-terminal-dev-cli.csproj -- ci --api-key "${{ steps.nuget-login.outputs.NUGET_API_KEY }}"
```

## Results

Fixed the dev-cli and workflow to properly pass the NuGet API key from the OIDC login step to the push command.
