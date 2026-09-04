# Round 1 — core-abstractions
**Date:** 2026-09-04
**Scope reviewed:** iconsole.cs, iterminal.cs, timewarp-console.cs, timewarp-terminal.cs (pairing + platform gates)

## Summary

IConsole/ITerminal define the library’s console and interactive-terminal contracts; TimeWarpConsole/TimeWarpTerminal are the System.Console-backed production implementations. Fluent `new` pairing for sync Write methods is complete across ITerminal, TimeWarpTerminal, and TestTerminal, and 022 platform gates (CursorVisible/Title/Beep, KeyAvailable InvalidOperationException, IsInteractive, NO_COLOR/TERM=dumb, GetCursorPosition atomic) still match the current tree. Residual risk is narrow: one Windows Title-getter resilience gap versus the class’s IOException-swallow policy, and one Beep() remarks mismatch with Windows redirected BCL behavior.

## Issues

### Issue 1 — Severity: suggestion
- File: source/timewarp-terminal/timewarp-terminal.cs:541
- Description: `Title`’s Windows getter calls `Console.Title` with no `try`/`catch`, while the setter at lines 552–559 catches `IOException`, the file Design region (line 11) states IOExceptions are silently swallowed for redirected/unavailable consoles, and sibling getters such as `CursorVisible` (lines 210–217) return a safe default on `IOException`. BCL `Console.Title` get on Windows can throw when `GetConsoleTitleW` fails, so reading `Title` without an attached console can still throw on Windows despite the surrounding swallow policy.
- Suggestion: Wrap the Windows `Console.Title` get in `try`/`catch (IOException)` and return `string.Empty` (mirroring the non-Windows path and the setter’s resilience). Optionally extend the `ITerminal.Title` remarks to state that Windows unavailable/redirected reads also return empty.
- Status: open

### Issue 2 — Severity: suggestion
- File: source/timewarp-terminal/iterminal.cs:274
- Description: `Beep()` remarks claim that when the console is redirected or unavailable, the default implementation “silently does nothing instead of throwing.” `TimeWarpTerminal.Beep()` (timewarp-terminal.cs:478–490) only forwards to `Console.Beep()` and catches `IOException`. On Windows, BCL `Console.Beep()` falls back to `Kernel32.Beep` when stdout is redirected, so a redirected Windows process still emits a beep — contradicting the remarks (Unix BCL does no-op when redirected).
- Suggestion: Either gate `TimeWarpTerminal.Beep()` on `Console.IsOutputRedirected` (and treat unavailable the same) so behavior matches the remarks on all platforms, or revise the remarks to describe Windows redirected fallback vs Unix no-op and that the wrapper only suppresses throws.
- Status: open
