# Reposition README and package metadata as a framework-agnostic console layer

## Goal (implementer restatement)

TimeWarp.Terminal is a lower-level console and output library. A plain `Main`, System.CommandLine, Spectre.Console.Cli, Cocona, Nuru, or any other .NET host can pass `IConsole` or `ITerminal` in and assert what was printed with `TestTerminal`. The README tagline, the opening paragraphs, and the NuGet package description have to say that. Nuru is one host that works well with the library, later in the page, not the name of the library.

The shipped `TimeWarp.Terminal` project references only `TimeWarp.Builder`. Wording that ties the package to Nuru is wrong even when a sample happens to use Nuru. `TimeWarp.Nuru` is a dev-CLI package reference in this repo. It is not a dependency of the packed library.

Acceptance:

- The tagline and intro call Terminal a framework-agnostic, testable console/output layer for any .NET CLI or console app.
- The front of the README still lists Console-compatible `IConsole`/`ITerminal`, parallel-safe `TestTerminal`/`TestConsole` with async-scoped capture, static widgets (panel, table, rule, OSC 8 hyperlinks, Unicode width), the optional flexbox layout package, the small dependency set, and AOT.
- At least one plain `Main` example writes through `IConsole` and `ITerminal`, asserts with `TestTerminal`, and compiles against this tree.
- The Nuru `UseTerminal` example sits under a "works well with Nuru" heading, below that plain example. A Nuru handler still takes `ITerminal` (which implements `IConsole`). The page does not say Nuru injects `IConsole`.
- Package `Description` and `PackageTags`, docs, and samples do not imply the shipped package depends on Nuru. `InterceptorsNamespaces` for `TimeWarp.Nuru.Generated` stays unless that string leaks into the packed package.
- "When to use something else" still names Spectre.Console, Termina, and Terminal.Gui. The page does not promise a live region or a TUI.
- Build, tests, and `ganda repo audit` are green. Results include the command output and a before/after of the tagline and intro so the later PR body can quote them.

## Description

Reposition TimeWarp.Terminal in the README and NuGet package metadata as a general-purpose, lower-level, testable console/output layer for ANY .NET CLI or console app (plain `Main`, System.CommandLine, Spectre.Console.Cli, Cocona, Nuru, ...). It does not require Nuru and must not read as Nuru-only. Nuru appears lower down as one integration that works well with it, not as the identity.

Background: task 033 (PR #44) positioned Terminal as "the testable output layer for Nuru CLIs". Steven rejected that framing on 2026-10-17 after review. #44 was merged as reviewed, and this task corrects the positioning. Evaluation context: https://github.com/TimeWarpEngineering/timewarp-intel/blob/master/evaluations/2026-10-17-termina-vs-timewarp-terminal.md

Out of scope (Steven): no live status / ILiveRegion and no full TUI layer. Don't promise or roadmap either.

## Checklist

- [x] Tagline and intro lead with Terminal as a framework-agnostic, testable console/output layer for any .NET CLI or console app
- [x] Differentiators stay up front: Console-compatible IConsole/ITerminal, TestTerminal/TestConsole with parallel-safe async-scoped capture, static widgets (panel, table, rule, OSC 8 hyperlinks, Unicode width), optional flexbox layout package, minimal dependencies, AOT-friendly
- [x] At least one non-Nuru example (plain `Main`, and/or System.CommandLine) that writes through IConsole/ITerminal and asserts output with TestTerminal. Examples must compile against the current API
- [x] Nuru example/section moved lower as "works well with Nuru"
- [x] Check package Description, PackageTags, csproj/Directory.Build.props metadata, docs/, and samples for anything implying a Nuru dependency, and fix the wording. `InterceptorsNamespaces` for TimeWarp.Nuru.Generated is test/sample build plumbing: leave it unless it leaks into the shipped package
- [x] Keep the fair "when to use something else" section (Spectre.Console, Termina, Terminal.Gui)
- [x] Build and tests green, `ganda repo audit` clean
- [x] PR body carries proof (build/test/audit output) plus a before/after of the tagline and intro

## Notes

Filed 2026-10-17 by Terminal bot at Steven's request (relayed by Amina). File only. Not launched.

Root cause of the 033 miss: the bot accepted the brief's framing ("output layer for Nuru CLIs") without checking it against the real dependencies. The shipped TimeWarp.Terminal package references only TimeWarp.Builder, and Nuru is used only by tools/dev-cli and tests. Rule for this and future positioning work: check any identity or positioning claim against the shipped package's actual references before writing it, and never describe the package as tied to a framework it doesn't reference.

## Results

The README, the Terminal skill, and the `TimeWarp.Terminal` NuGet description now lead with a framework-agnostic console layer. Nuru is one host, under `## Works well with Nuru`, below the plain `Main` sample. The page does not promise `ILiveRegion` or a TUI.

Checked the shipped project before writing that claim. `source/timewarp-terminal/timewarp-terminal.csproj` has one `PackageReference`: `TimeWarp.Builder`. `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` are referenced only from `tools/dev-cli/Directory.Build.props`. Tests and samples do not reference Nuru. A Release pack of `TimeWarp.Terminal` 1.0.2 has nuspec dependency `TimeWarp.Builder` 1.0.0 only. The nupkg nuspec has no `Nuru` dependency and no `InterceptorsNamespaces`. `Directory.Build.props` still sets `InterceptorsNamespaces` for `TimeWarp.Nuru.Generated`. That property stays. It is compiler plumbing, and it does not enter the package. Layout's packed dependencies are `TimeWarp.Terminal` and `TimeWarp.Flexbox`. `documentation/dependents.md` still lists timewarp-nuru as a consumer. That list is true, and it is not a package dependency.

`TimeWarp.Nuru` 3.0.0 `TimeWarp.Nuru.Analyzers.dll` contains UTF-16 `TimeWarp.Terminal.ITerminal` five times and contains no `IConsole` string. The Nuru section still says a handler takes `ITerminal`.

The plain `Main` sample is `tests/plain-main-01-capture.cs`. `AppStatus.Write` takes `IConsole`. `Greeting.Write` takes `ITerminal`. One `TestTerminal` is passed to both, and the test asserts `OutputContains`. The same two fences in `readme.md` were copied to throwaway file-based programs and run. No System.CommandLine package was added. The intro names that host. A second framework sample would have been a new dependency for wording the plain `Main` path already covers.

Version stays `1.0.2`. This change is source and the local pack. nuget.org updates when the next version is released.

### Before / after

Tagline before:

> The testable output layer for Nuru CLIs.

Tagline after:

> A framework-agnostic, testable console and output layer for any .NET CLI or console app.

Intro before:

> TimeWarp.Terminal writes the text, widgets, and layouts a command prints. It does not run an event loop, and it does not depend on a reactive framework. Tests capture that output with `TestTerminal`, `TestConsole`, and an async-scoped `TestTerminalContext`. Both packages are marked `IsAotCompatible` and run the trim and AOT analyzers.
>
> A Nuru handler takes `ITerminal`, which also implements `IConsole`. Call `UseTerminal` with a `TestTerminal` in a test. When you do not call it, Nuru uses `TimeWarpTerminal.Default`.

Intro after:

> TimeWarp.Terminal writes the text, widgets, and layouts a program prints. Plain `Main` passes `IConsole` or `ITerminal` in. System.CommandLine, Spectre.Console.Cli, Cocona, Nuru, and any other host can pass the same interfaces. The library does not require a CLI framework. It does not run an event loop, and it does not depend on a reactive framework. Tests capture output with `TestTerminal`, `TestConsole`, and an async-scoped `TestTerminalContext`. Both packages are marked `IsAotCompatible` and run the trim and AOT analyzers.
>
> The only library dependency of the `TimeWarp.Terminal` package is TimeWarp.Builder.

Package `Description` before:

> Terminal abstractions and widgets for console applications - IConsole, ITerminal, panels, tables, rules, and ANSI color support

Package `Description` after (also the packed nuspec):

> Framework-agnostic, testable console and output layer for any .NET CLI or console app. IConsole, ITerminal, TestTerminal, panels, tables, rules, OSC 8 hyperlinks, and ANSI color. No CLI framework required.

`PackageTags` gained `testing` and `aot`. They do not contain `nuru`.

### Proof

Observed 2026-10-17 in this worktree:

- `./bin/dev build` — `Build succeeded.` `0 Warning(s)` `0 Error(s)`.
- `./bin/dev verify-samples` — `✓ All 6 sample(s) verified successfully` (exit 0).
- `./bin/dev test` — `✓ All 44 test file(s) passed` (exit 0). Includes `plain-main-01-capture.cs`: `Should_capture_iconsole_and_iterminal_output` passed.
- `ganda repo audit` — `Repository passes all audit checks.` Passed 29, Failed 0.
- Throwaway `dotnet` run of the README plain `Main` fence — exit 0, printed `ready` and `Hello, World`.
- Throwaway `dotnet` run of the README `TestTerminal` fence (`Shouldly` 4.3.0) — exit 0.
- Throwaway `dotnet` run of the README Nuru fence (`TimeWarp.Nuru` 3.0.0, `Shouldly` 4.3.0, this worktree's terminal project) — exit 0. A git-metadata warning came from building that file under `/tmp`, outside the repo. The repo `./bin/dev build` had 0 warnings.

### Files

- `readme.md`
- `source/timewarp-terminal/timewarp-terminal.csproj` (`Description`, `PackageTags`)
- `skills/terminal/SKILL.md`
- `tests/plain-main-01-capture.cs`

### How to validate

**Smoke**

```bash
sed -n '1,8p' readme.md
rg -n "Works well with Nuru" readme.md
rg -n "<Description>|<PackageTags>" source/timewarp-terminal/timewarp-terminal.csproj
dotnet tests/plain-main-01-capture.cs
```

**Expect**

- Line 3 of `readme.md` is `A framework-agnostic, testable console and output layer for any .NET CLI or console app.`
- `## Works well with Nuru` is below `## Testing a plain Main`.
- The csproj description starts with `Framework-agnostic, testable console and output layer` and the tags do not contain `nuru`.
- `plain-main-01-capture.cs` prints `Passed: 1`.

**Automated gate**

```bash
./bin/dev build
./bin/dev test
ganda repo audit
```

Expect: build `0 Warning(s)` / `0 Error(s)`, `✓ All 44 test file(s) passed`, audit `Repository passes all audit checks.`

**Not in scope:** a version bump and a nuget.org publish. The new description is in source and in a local pack of 1.0.2. Live status / `ILiveRegion` and a TUI layer stay out.

## Session

- Implementer: grok session 01a148f9-9992-7193-af8b-f26b409ad97e (2026-10-17)
