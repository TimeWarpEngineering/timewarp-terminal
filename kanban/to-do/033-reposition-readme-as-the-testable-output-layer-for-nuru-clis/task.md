# Reposition README as the testable output layer for Nuru CLIs

## Description

Update the timewarp-terminal README so it positions TimeWarp.Terminal as the testable output layer for Nuru CLIs: no event loop, no reactive dependency, AOT-friendly, parallel-safe test capture (TestTerminal, TestConsole, async-scoped TestTerminalContext). Add a short, fair "when to use something else" section: Spectre.Console for live displays/progress, Termina or Terminal.Gui for full interactive TUIs.

Steven approved this on 2026-10-17 (relayed by Amina). Busara rated the README repositioning GREEN: https://github.com/TimeWarpEngineering/timewarp-intel/blob/master/evaluations/2026-10-17-termina-vs-timewarp-terminal.md

Out of scope (Steven): no live status / ILiveRegion, no full TUI layer. Do not promise or roadmap either in the README.

## Checklist

- [ ] Rewrite the README intro/tagline around "testable output layer for Nuru CLIs"
- [ ] Highlight the differentiators: Console-compatible IConsole/ITerminal, test doubles with parallel-safe capture, static widgets (panel, table, rule, OSC 8 hyperlinks, Unicode width), optional flexbox layout package, minimal dependencies, AOT-friendly
- [ ] Show a short Nuru + TestTerminal example that asserts on output (must compile against the current API)
- [ ] Add a "When to use something else" section (Spectre.Console, Termina, Terminal.Gui), factual and non-disparaging
- [ ] Keep existing install/usage docs accurate; no claims that are not true of the current code
- [ ] Docs-only change; build and tests still green, `ganda repo audit` clean
- [ ] PR body carries proof (build/test/audit output)

## Notes

Filed 2026-10-17 by Terminal bot from Amina's relay of Steven's approval.
