# Round 1 — general
**Date:** 2026-10-17
**Scope reviewed:** branch vs `master` (aefbcfd): `readme.md`, `skills/terminal/SKILL.md`, `source/timewarp-terminal/timewarp-terminal.csproj`, `tests/plain-main-01-capture.cs`

## Summary

The change replaces the Nuru-first tagline and intro with framework-agnostic wording. It adds a plain `Main` sample with a matching `TestTerminal` test and moves the Nuru `UseTerminal` sample under `## Works well with Nuru`. It also updates the `TimeWarp.Terminal` package `Description` and `PackageTags` and the terminal skill. It is a documentation and metadata change plus one test, so the risk is low. I checked every acceptance item in the task and the implementer restatement and found no defects.

## Verification

- **Tagline and intro:** `readme.md:3` now reads "A framework-agnostic, testable console and output layer for any .NET CLI or console app." The intro names plain `Main`, System.CommandLine, Spectre.Console.Cli, Cocona, Nuru, and any other host, and says "does not require a CLI framework".
- **What stays up front:** the intro and `## What you get` still cover Console-compatible `IConsole`/`ITerminal`, parallel-safe `TestTerminal`/`TestConsole` with async-scoped capture, the static widgets (panel, table, rule, OSC 8, Unicode width), the optional Layout package, the single-dependency set, and AOT (`IsAotCompatible`).
- **Plain `Main` sample:** `## Testing a plain Main` comes before `## Works well with Nuru`. `tests/plain-main-01-capture.cs` mirrors the README method bodies. `dotnet tests/plain-main-01-capture.cs` → `Passed: 1`.
- **Nuru section:** the handler parameter is `ITerminal`. The page never says Nuru injects `IConsole`; it says the opposite.
- **Dependency claim:** `source/timewarp-terminal/timewarp-terminal.csproj:22` has `TimeWarp.Builder` as its only `PackageReference`. The `Directory.Build.props` references are all `PrivateAssets="all"` analyzers and build tasks. Nothing in `samples/` mentions Nuru. Repo-wide, the remaining Nuru mentions are `tools/dev-cli`, `Directory.Packages.props` versions, the `InterceptorsNamespaces` plumbing (correctly left alone), `documentation/dependents.md` (a consumer, not a dependency), and path strings used as table test data.
- **Metadata:** `Description` reads "Framework-agnostic … No CLI framework required." `PackageTags` adds `testing;aot` and has no `nuru`. Layout's description has no Nuru tie.
- **When to use something else:** still names Spectre.Console, Termina, and Terminal.Gui. Nothing promises `ILiveRegion` or a TUI.
- **Gates:** I re-ran these. `./bin/dev build` → `0 Error(s)`, `✓ Build completed successfully`. `ganda repo audit` → `Repository passes all audit checks.`
- **`#pragma warning disable CA1849`** in the new test is the existing convention for sync-write tests (`tests/terminal-static-01-basic.cs`, `tests/test-terminal-context-01-integration.cs`, and others). It does not hide a broken contract.

## Issues

None.

Observation only, not a finding: the intro's single-dependency sentence repeats the `## What you get` bullet a few lines later. The intro copy is what answers the "not tied to Nuru" positioning question, so the repetition is acceptable.
