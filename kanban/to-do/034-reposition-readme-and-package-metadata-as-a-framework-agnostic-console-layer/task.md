# Reposition README and package metadata as a framework-agnostic console layer

## Description

Reposition TimeWarp.Terminal in the README and NuGet package metadata as a general-purpose, lower-level, testable console/output layer for ANY .NET CLI or console app (plain `Main`, System.CommandLine, Spectre.Console.Cli, Cocona, Nuru, ...). It does not require Nuru and must not read as Nuru-only. Nuru appears lower down as one integration that works well with it, not as the identity.

Background: task 033 (PR #44) positioned Terminal as "the testable output layer for Nuru CLIs". Steven rejected that framing on 2026-10-17 after review. #44 was merged as reviewed, and this task corrects the positioning. Evaluation context: https://github.com/TimeWarpEngineering/timewarp-intel/blob/master/evaluations/2026-10-17-termina-vs-timewarp-terminal.md

Out of scope (Steven): no live status / ILiveRegion and no full TUI layer. Don't promise or roadmap either.

## Checklist

- [ ] Tagline and intro lead with Terminal as a framework-agnostic, testable console/output layer for any .NET CLI or console app
- [ ] Differentiators stay up front: Console-compatible IConsole/ITerminal, TestTerminal/TestConsole with parallel-safe async-scoped capture, static widgets (panel, table, rule, OSC 8 hyperlinks, Unicode width), optional flexbox layout package, minimal dependencies, AOT-friendly
- [ ] At least one non-Nuru example (plain `Main`, and/or System.CommandLine) that writes through IConsole/ITerminal and asserts output with TestTerminal. Examples must compile against the current API
- [ ] Nuru example/section moved lower as "works well with Nuru"
- [ ] Check package Description, PackageTags, csproj/Directory.Build.props metadata, docs/, and samples for anything implying a Nuru dependency, and fix the wording. `InterceptorsNamespaces` for TimeWarp.Nuru.Generated is test/sample build plumbing: leave it unless it leaks into the shipped package
- [ ] Keep the fair "when to use something else" section (Spectre.Console, Termina, Terminal.Gui)
- [ ] Build and tests green, `ganda repo audit` clean
- [ ] PR body carries proof (build/test/audit output) plus a before/after of the tagline and intro

## Notes

Filed 2026-10-17 by Terminal bot at Steven's request (relayed by Amina). File only. Not launched.

Root cause of the 033 miss: the bot accepted the brief's framing ("output layer for Nuru CLIs") without checking it against the real dependencies. The shipped TimeWarp.Terminal package references only TimeWarp.Builder, and Nuru is used only by tools/dev-cli and tests. Rule for this and future positioning work: check any identity or positioning claim against the shipped package's actual references before writing it, and never describe the package as tied to a framework it doesn't reference.
