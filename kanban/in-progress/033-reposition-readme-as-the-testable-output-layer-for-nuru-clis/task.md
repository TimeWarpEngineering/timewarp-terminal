# Reposition README as the testable output layer for Nuru CLIs

## Goal (implementer restatement)

Readers of the README should meet TimeWarp.Terminal as the output layer a Nuru command prints and a test asserts on. The page should say what the library is: Console-compatible `IConsole` and `ITerminal`, static widgets (panel, table, rule, OSC 8 hyperlinks, Unicode width), an optional flexbox layout package, a small dependency set, and AOT-compatible projects. It has no event loop and no reactive dependency. Tests capture output with `TestTerminal`, `TestConsole`, and an async-scoped `TestTerminalContext` that keeps parallel tests off the process-global terminal.

A short Nuru example must call `UseTerminal` with a `TestTerminal`, run a command, and assert on captured output, and that example must compile against the current `TimeWarp.Terminal` and Nuru APIs. A separate section should name Spectre.Console for live progress, and Termina or Terminal.Gui for a full interactive TUI, in factual language. The README must not promise a live region or a TUI layer. Install and widget docs that are already true stay accurate. The change is documentation. Build, tests, and `ganda repo audit` stay green, and the task results carry the command output a later PR body can quote.

A Nuru handler receives `ITerminal`. That interface also implements `IConsole`, which is why console-shaped calls work on the injected instance. Nuru does not register `IConsole` as its own handler service, so the README must not say a handler can take `ITerminal` or `IConsole`. `TestConsole` stays the double for code that depends only on `IConsole` outside that injection.

## Description

Update the timewarp-terminal README so it positions TimeWarp.Terminal as the testable output layer for Nuru CLIs: no event loop, no reactive dependency, AOT-friendly, parallel-safe test capture (TestTerminal, TestConsole, async-scoped TestTerminalContext). Add a short, fair "when to use something else" section: Spectre.Console for live displays/progress, Termina or Terminal.Gui for full interactive TUIs.

Steven approved this on 2026-10-17 (relayed by Amina). Busara rated the README repositioning GREEN: https://github.com/TimeWarpEngineering/timewarp-intel/blob/master/evaluations/2026-10-17-termina-vs-timewarp-terminal.md

Out of scope (Steven): no live status / ILiveRegion, no full TUI layer. Do not promise or roadmap either in the README.

## Checklist

- [x] Rewrite the README intro/tagline around "testable output layer for Nuru CLIs"
- [x] Highlight the differentiators: Console-compatible IConsole/ITerminal, test doubles with parallel-safe capture, static widgets (panel, table, rule, OSC 8 hyperlinks, Unicode width), optional flexbox layout package, minimal dependencies, AOT-friendly
- [x] Show a short Nuru + TestTerminal example that asserts on output (must compile against the current API)
- [x] Add a "When to use something else" section (Spectre.Console, Termina, Terminal.Gui), factual and non-disparaging
- [x] Keep existing install/usage docs accurate; no claims that are not true of the current code
- [x] Build and tests still green, `ganda repo audit` clean (audit also required a Nuru/Amuru pin bump; see Results)
- [x] Proof for the host PR body is in Results (this walk does not open the PR)
- [x] M1: the intro says a Nuru handler takes `ITerminal`, which implements `IConsole`, and does not say Nuru injects `IConsole`

## Review

- Round 1: M1 fixed on the round-1 ledger. Host runs review round 2. Disposition waits for that round.

## Notes

Filed 2026-10-17 by Terminal bot from Amina's relay of Steven's approval.

## Session

- Implementation: grok task-work 01a1475e-7e4e-7e90-a91d-31946805cacd (2026-10-17)
- Implement oracle: implemented by implementer-grok (grok, model grok-4.7), session not reported, max-turns 500, Done — 2026-10-17T01:24:02Z
- Review oracle round 1: claude (claude-opus-5-5), effort 1, roster general. 1 open bug (M1). Fix pass requested — 2026-10-17
- Review oracle: review by reviewer-claude (claude, model claude-opus-5-5), session not reported, max-turns 80 — 2026-10-17T01:26:10Z
- Fix pass (implement-fix-2): implemented by implementer-grok (grok, model grok-4.7), session not reported — 2026-10-17. M1 reworded in `readme.md`.

## Results

The README now opens with "The testable output layer for Nuru CLIs." It describes write-once output: no event loop, no reactive dependency, `IsAotCompatible` on both packages, and parallel-safe capture through a per-test `TestTerminal` or `TestConsole` plus async-local `TestTerminalContext`. It states that one `TestTerminal` is not thread-safe. "When to use something else" names Spectre.Console for live progress, Termina for a reactive TUI (full screen or one inline live region), and Terminal.Gui for a full-screen terminal GUI. The page does not mention `ILiveRegion` and does not roadmap a TUI.

Round-1 M1: the intro used to say a Nuru handler can take `ITerminal` or `IConsole`. It now says a Nuru handler takes `ITerminal`, which also implements `IConsole`. The Nuru sample still injects `ITerminal`. `TestConsole` remains the double for code that only needs `IConsole` outside Nuru injection (`readme.md` testing section). Files: `readme.md`, `review/round-1/merged.md`. `.gitignore` ignores host review markers `review/round-*/base.sha` and `review/round-*/closed`, the same way it already ignores oracle logs and journals.

The Nuru sample uses `UseTerminal`, `Map("greet {name}")`, and `OutputContains("Hello, World")`. A throwaway file-based program compiled that sample against this worktree's `TimeWarp.Terminal` project, `TimeWarp.Nuru` 3.0.0, and Shouldly 4.3.0, then ran it. Exit code 0.

`ganda repo audit` was red before the pin bump: TimeWarp.Nuru `3.0.0-beta.79` is behind stable `3.0.0`. Nuru 3.0.0 depends on TimeWarp.Amuru `>= 2.0.0`, so the pins moved together. dev-cli already used the `TimeWarp.Mediator` contracts from beta.79. No endpoint source changed.

| Package | Was | Now |
| --- | --- | --- |
| TimeWarp.Nuru | 3.0.0-beta.79 | 3.0.0 |
| TimeWarp.Nuru.DevCli | 3.0.0-beta.79 | 3.0.0 |
| TimeWarp.Amuru | 2.0.0-beta.2 | 2.0.0 |
| TimeWarp.Amuru.Tools | 2.0.0-beta.2 | 2.0.0 |

Package version stays `1.0.2`. This walk does not release.

### How to validate

Smoke:

```bash
rg -n "A Nuru handler takes \`ITerminal\`, which also implements \`IConsole\`" readme.md
rg -n "ITerminal\` or \`IConsole" readme.md
rg -n "public interface ITerminal : IConsole" source/timewarp-terminal/iterminal.cs
ganda repo audit
```

Expect:

- The first search prints the intro sentence in `readme.md`.
- The second search prints no matches (exit 1).
- The third search prints `public interface ITerminal : IConsole` in `source/timewarp-terminal/iterminal.cs`.
- Audit prints `Repository passes all audit checks.`

The Nuru code sample in `readme.md` is unchanged and still takes `ITerminal`. The earlier Release build, 43 test files, and 6 samples stay the automated gate for the rest of this branch. This fix does not change C#.

Observed 2026-10-17 in this worktree before this fix: solution Release build 0 warnings / 0 errors after the pin bump; dev-cli Release build exited 0; 43 test files passed after the pin bump; 6 samples verified on the pre-pin tree (samples do not reference Nuru); audit Passed 29, Failed 0. The README Nuru sample exited 0 against Nuru 3.0.0.

Observed on this fix: the intro search printed `readme.md:7` and exited 0; the false-claim search on `readme.md` exited 1; `iterminal.cs:28` is `public interface ITerminal : IConsole`. Nuru 3.0.0 `TimeWarp.Nuru.Analyzers.dll` contains UTF-16 `TimeWarp.Terminal.ITerminal` (5) and contains no `IConsole` string. `ganda repo audit` printed `Repository passes all audit checks.` (Passed 29, Failed 0).

### Review

Round 1 ran at effort 1 with one general reviewer. It found 1 bug (M1), 0 suggestions, and 0 nits. The fix pass marked M1 `fixed` on `review/round-1/merged.md` (bug 0 open, 1 fixed, 0 wontfix). Disposition is not written yet: the host runs review round 2, and `review/disposition.md` waits until that round leaves 0 open findings. Paths: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`.

### PR proof

Host open-pr can quote the Observed paragraph above. This oracle does not run `gh pr create`.
