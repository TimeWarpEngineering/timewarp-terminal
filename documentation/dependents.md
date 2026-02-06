# Downstream Dependents

Repos that depend on `TimeWarp.Terminal`. When making breaking changes, create a GitHub issue in each repo to notify them of the update.

## Known Dependents

| Repository | Dependency Type | Notes |
|------------|----------------|-------|
| [timewarp-nuru](https://github.com/TimeWarpEngineering/timewarp-nuru) | NuGet | Uses `IConsole`, `ITerminal`, `TimeWarpConsole`, `TestTerminal` |
| [timewarp-jaribu](https://github.com/TimeWarpEngineering/timewarp-jaribu) | NuGet | Uses `WriteTable` for test results rendering |
| [timewarp-builder](https://github.com/TimeWarpEngineering/timewarp-builder) | NuGet | |
| [timewarp-ganda](https://github.com/TimeWarpEngineering/timewarp-ganda) | NuGet | |
| [crunchit](https://github.com/TimeWarpEngineering/crunchit) | NuGet | |
| [timewarp-flow](https://github.com/TimeWarpEngineering/timewarp-flow) | Skill/docs | Terminal skill in `opencode/skills/terminal/SKILL.md` |

## Discovering New Dependents

Search across TimeWarp repos for `TimeWarp.Terminal` package references:

```bash
gh search code "TimeWarp.Terminal" --owner TimeWarpEngineering --filename Directory.Packages.props
gh search code "TimeWarp.Terminal" --owner TimeWarpEngineering --filename "*.csproj"
```

## Notification Template

When publishing a breaking release, create issues with:

```
Title: Update TimeWarp.Terminal to <version>
Body:
TimeWarp.Terminal <version> has been published with breaking changes:
- <list changes>

Please update your `Directory.Packages.props` to reference the new version.
```
