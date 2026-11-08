# Downstream Dependents

Repos that depend on `TimeWarp.Terminal`. When making breaking changes, create a GitHub issue in each repo to notify them of the update.

## Consumer impact (2.0.0)

`TimeWarp.Terminal` and `TimeWarp.Terminal.Layout` 2.0.0 target `net11.0` only. A `net10.0` project cannot reference them. Stay on 1.0.2 for `net10.0`. There is no multi-target build. The public API is unchanged; the break is the dropped target framework.

`TimeWarp.Builder` 1.0.0 and `TimeWarp.Flexbox` 1.0.0 still publish `lib/net10.0`. A `net11.0` project consumes those assets through NuGet's compatible-framework fallback.

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
