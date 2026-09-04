# Round 1 — merged findings
**Date:** 2026-09-04
**Sources:** core-abstractions, static-facade, test-doubles, widgets, tests-infra, security

Re-verified against the current tree at implement pin `1a6a29b66c38ba24b6306520de554b22def7bc74` (this branch’s product tree matches origin-home `master` for `source/`, `tests/`, `samples/`, `tools/`, `.github/`). Duplicate collapse prefers the strongest severity.

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 10 | 0 | 0 |
| suggestion | 7 | 0 | 0 |
| nit | 4 | 0 | 0 |

## Issues

### M1 — Severity: bug — Status: open
- File: source/timewarp-terminal/test-console.cs:269
- Description: `TestConsole.Dispose` disposes the current `Standard*Stream` property values. Constructor streams are assigned directly to those properties with no owned-field tracking, so a consumer replacement is disposed with the console. `TestTerminal` already fixed this in 022 (`test-terminal.cs:639-643`).
- Suggestion: Mirror TestTerminal — dispose only constructor-owned streams. Add a TestConsole regression parallel to `tests/stream-access-01-basic.cs` TestTerminal case.
- Source: test-doubles
- Disposition notes: filed as child **029-001**

### M2 — Severity: bug — Status: open
- File: source/timewarp-terminal/test-console.cs:189
- Description: `TestConsole.Read` only drains `CharacterQueue` and returns `-1` when empty; it never reads constructor `InputReader` / `In`. `new TestConsole("abc").Read()` is `-1` while `ReadLine()` returns `"abc"`. TestTerminal’s 022 fix (`test-terminal.cs:242-250`) was not mirrored.
- Suggestion: Fall back to `InputReader.Read()` when the character queue is empty. Add constructor-input / interleave tests.
- Source: test-doubles
- Disposition notes: filed as child **029-001**

### M3 — Severity: bug — Status: open
- File: source/timewarp-terminal/test-terminal.cs:134
- Description: `SetIn` / `SetOut` / `SetError` only reassign the `In` / `Out` / `Error` properties (`test-terminal.cs:134-143`; same on `test-console.cs:108-117`). `Write*` always hits the private capture writers; `ReadLine` / `Read` use `InputReader` / queues. After `SetOut(custom)`, `WriteLine` still lands in the capture writer — opposite of `TimeWarpConsole`/`TimeWarpTerminal` where `SetOut` redirects subsequent writes. Existing tests only assert property identity.
- Suggestion: Route I/O through `Out`/`Error`/`In` (tee into capture if needed) and add tests that `SetOut` then `WriteLine` appears on the new writer.
- Source: test-doubles
- Disposition notes: filed as child **029-001**

### M4 — Severity: bug — Status: open
- File: source/timewarp-terminal/terminal-static.cs:90
- Description: `Terminal.FormatProvider` is a process-global static, while `TestTerminalContext.SetCurrent` / `ClearCurrent` snapshot and restore it from an AsyncLocal stack (`test-terminal-context.cs:85-86`, `test-terminal-context.cs:109`). Parallel `Use` scopes that assign `FormatProvider` race: one flow’s dispose can wipe another’s in-scope provider. Facade format overloads read that static via `ActiveFormatProvider`. The parallel isolation test does not touch FormatProvider. Related: `SnapshotStack` stores a mutable `Stack<ContextSnapshot>` in AsyncLocal (`test-terminal-context.cs:48,136-145`); `Task.Run` from inside an active `Use` forks an ExecutionContext that shares the same Stack instance.
- Suggestion: Make FormatProvider AsyncLocal (resolve like `Instance`), or stop snapshot/restoring it and document serial-only mutation. Prefer an immutable linked snapshot for the stack so forked contexts do not share mutable push/pop state. Add a parallel FormatProvider isolation test.
- Source: static-facade, test-doubles
- Disposition notes: filed as child **029-001**

### M5 — Severity: bug — Status: open
- File: source/timewarp-terminal/ansi-hyperlink-extensions.cs:71
- Description: `SanitizeUrl` percent-encodes only C0 (`< U+0020`) and DEL (`U+007F`). ECMA-48 / xterm also accept C1 STRING TERMINATOR `U+009C` as an OSC terminator. A URL containing `\x9c` is embedded verbatim in the OSC 8 payload (`CreateLink` at `:50-51`), so an attacker-influenced URL can still terminate the hyperlink sequence on C1-aware terminals. 022 closed ESC/BEL/C0; this C1 path remains. Static and ITerminal `WriteLink*` inherit it via `CreateLink`.
- Suggestion: Percent-encode C1 controls that can terminate or introduce OSC/APC/PM/DCS (at minimum `U+009C`; consider `U+0080`–`U+009F`). Add a regression beside the existing ESC/BEL case in `tests/hyperlink-01-basic.cs`.
- Source: static-facade
- Disposition notes: filed as child **029-002**

### M6 — Severity: bug — Status: open
- File: source/timewarp-terminal/ansi-hyperlink-extensions.cs:51
- Description: `CreateLink` sanitizes the URL only inside the OSC payload, then uses the unsanitized `url` as display text when `displayText` is null (`displayText ?? url`). The same raw URL is chosen in `terminal-hyperlink-extensions.cs:41` / `:71` before either `CreateLink` or the plain fallback. An attacker-influenced URL such as `https://example.com/\x1b]0;Hacked\x07` has a safe OSC payload but still emits raw ESC/BEL in the display (or plain-text) portion. This is the library constructing output from the URL parameter it already treats as untrusted — not the 022 caller-embedded ANSI carve-out.
- Suggestion: When falling back to the URL as display text (and on the `!SupportsHyperlinks` plain path), emit `SanitizeUrl(url)` (or strip C0/DEL/C1 for display). Keep explicit caller `displayText` unmodified.
- Source: security
- Disposition notes: filed as child **029-002**

### M7 — Severity: bug — Status: open
- File: source/timewarp-terminal/widgets/panel-widget.cs:119
- Description: Minimum panel width is hard-coded to 4 and does not account for `PaddingHorizontal` (default 1). When `contentAreaWidth = width - 2 - 2*PaddingHorizontal` drops below 1 it is forced back up to 1 without widening the panel, so `RenderContentRow` emits `1 + PaddingHorizontal + contentAreaWidth + PaddingHorizontal + 1` columns while top/bottom borders still use `width`. With defaults this mismatches at width 4 (border 4 vs content row 5).
- Suggestion: Floor width at `2 + 2*PaddingHorizontal + 1` so border and content-row widths always match. Assert every rendered line has identical visible width for small widths and non-zero padding.
- Source: widgets
- Disposition notes: filed as child **029-003**

### M8 — Severity: bug — Status: open
- File: source/timewarp-terminal/widgets/panel-widget.cs:140
- Description: Bordered and borderless paths split content only on `'\n'` (`RenderWithBorder` here; `RenderWithoutBorder` at `:112`). CRLF input leaves a trailing `'\r'`. `'\r'` measures as width 0 so borders still look aligned by visible length, but writing the line moves the cursor to column 0 and overwrites the left border/padding.
- Suggestion: Normalize newlines before split (strip `'\r'` / split on any line ending). Cover with `"Line1\r\nLine2"`.
- Source: widgets
- Disposition notes: filed as child **029-003**

### M9 — Severity: bug — Status: open
- File: .github/workflows/workflow.yml:7-12
- Description: Push and pull_request `paths` filters list only `source/**`, `tools/**`, `.github/workflows/**`, `Directory.Build.props`, and `Directory.Packages.props`. They omit `tests/` and `samples/` (same on the pull_request block at lines 16–21). A tests-only or samples-only PR does not trigger CI, so `dev workflow`’s verify-samples and test steps never run for that change. Release events have no path filter and still exercise both suites. Verified against the current workflow file.
- Suggestion: Add `'tests/**'` and `'samples/**'` to both path lists (consider `msbuild/**` as well).
- Source: tests-infra
- Disposition notes: filed as child **029-004**

### M10 — Severity: bug — Status: open
- File: tools/dev-cli/endpoints/workflow.cs:237
- Description: Pack emits a `.snupkg` (`timewarp-terminal.csproj:12-13`, pack at `workflow.cs:221` with `ContinuousIntegrationBuild=true`). The push loop only enumerates `*.nupkg`, so sibling `.snupkg` files are never pushed. Artifact upload in `.github/workflows/workflow.yml:94` likewise only uploads `*.nupkg`. Confirmed: NuGet symbolpackage URLs for TimeWarp.Terminal 1.0.0 and 1.0.1 both HTTP 404. 022 fixed production of snupkg; symbols still are not published.
- Suggestion: Also push `*.snupkg` to NuGet’s symbol source and include `*.snupkg` in the Actions artifact path.
- Source: tests-infra
- Disposition notes: filed as child **029-004**

### M11 — Severity: suggestion — Status: open
- File: source/timewarp-terminal/timewarp-terminal.cs:541
- Description: `Title`’s Windows getter calls `Console.Title` with no `try`/`catch`, while the setter catches `IOException` and sibling getters such as `CursorVisible` return a safe default on `IOException`. BCL `Console.Title` get on Windows can throw when `GetConsoleTitleW` fails.
- Suggestion: Catch `IOException` on the Windows getter and return `string.Empty`. Optionally extend `ITerminal.Title` remarks.
- Source: core-abstractions
- Disposition notes: filed as child **029-005**

### M12 — Severity: suggestion — Status: open
- File: source/timewarp-terminal/iterminal.cs:274
- Description: `Beep()` remarks claim that when the console is redirected or unavailable, the default implementation “silently does nothing instead of throwing.” `TimeWarpTerminal.Beep()` (`timewarp-terminal.cs:478-490`) only forwards to `Console.Beep()` and catches `IOException`. On Windows, BCL `Console.Beep()` falls back to `Kernel32.Beep` when stdout is redirected, so a redirected Windows process still beeps.
- Suggestion: Gate on `Console.IsOutputRedirected` to match the remarks, or revise the remarks to describe Windows redirected fallback vs Unix no-op.
- Source: core-abstractions
- Disposition notes: filed as child **029-005**

### M13 — Severity: suggestion — Status: open
- File: source/timewarp-terminal/terminal-static.cs:1037-1041
- Description: `Terminal.CancelKeyPress` forwards add/remove to whatever `Instance` resolves to at that moment. Because `Instance` prefers `TestTerminalContext.Current`, a handler subscribed inside a `Use` scope is attached to the test terminal, but unsubscribing after the scope ends removes from a different instance. Production use with a stable `TimeWarpTerminal.Default` is fine. Tests only cover add/remove while Instance is held constant.
- Suggestion: Keep a facade-level handler list and attach a single forwarder to the current Instance, or document that subscribe/unsubscribe must occur against a stable Instance.
- Source: static-facade
- Disposition notes: filed as child **029-005**

### M14 — Severity: suggestion — Status: open
- File: source/timewarp-terminal/test-terminal.cs:286
- Description: When `ReadKey` synthesizes keys from constructor line input, it enqueues `ConsoleKeyInfo` with `shift: false` for every character (`:286-290`), including uppercase letters. `QueueKeys` correctly sets shift for ASCII uppercase (`:548-549`). Callers that branch on `keyInfo.Modifiers` see different flags for `"A"` from `QueueKeys("A")` versus `new TestTerminal("A"); ReadKey()`.
- Suggestion: Set `shift: char.IsAsciiLetterUpper(c)` when synthesizing from constructor input. Add a regression.
- Source: test-doubles
- Disposition notes: filed as child **029-001**

### M15 — Severity: suggestion — Status: open
- File: source/timewarp-terminal/widgets/panel-widget.cs:374
- Description: `PanelBuilder.Build()` / `ToPanel()` return the builder’s live `Panel` instance. Same for `RuleBuilder.Build()` at `rule-widget.cs:157`. TableBuilder was fixed in 022 to return an independent snapshot; post-Build mutation of Panel/Rule builders still mutates previously “built” objects.
- Suggestion: Snapshot Panel/Rule on Build, or document that Build returns the live object; align with TableBuilder if snapshot is the IBuilder contract.
- Source: widgets
- Disposition notes: filed as child **029-003**

### M16 — Severity: suggestion — Status: open
- File: source/timewarp-terminal/widgets/ansi-string-utils.cs:629
- Description: `BreakLongWord` only starts a new line when `currentLineWidth > 0`. A single wide grapheme (emoji/CJK, width 2) on a `maxWidth` of 1 is appended anyway, so `WrapText` can return lines whose visible width exceeds `maxWidth`. Panel mitigates by truncating after wrap; direct WrapText callers and the documented max-width contract do not.
- Suggestion: When a grapheme alone exceeds `maxWidth` and the line is empty, drop it (match TruncateVisible) or document overrun by at most one column. Add `WrapText("😀", 1)`.
- Source: widgets
- Disposition notes: filed as child **029-003**

### M17 — Severity: suggestion — Status: open
- File: source/timewarp-terminal/timewarp-terminal.cs:333-336
- Description: `SupportsColor` is false only when **stdout** is redirected (plus `NO_COLOR` / `TERM=dumb`). Colored `Terminal.WriteErrorLine` in `terminal-static.cs:260-302` gates on that property while writing to stderr. When stdout is a TTY and stderr is redirected, library-applied SGR still lands in the redirected error stream.
- Suggestion: For error-colored writers, also require `!IsErrorRedirected` before wrapping with `AnsiColors`.
- Source: security
- Disposition notes: filed as child **029-005**

### M18 — Severity: nit — Status: open
- File: source/timewarp-terminal/test-console.cs:220
- Description: `TestConsole.Clear()` discards captured stdout/stderr (TestTerminal’s `ClearOutput` semantics). `TestTerminal.Clear()` appends a `[CLEAR]` marker and preserves history. Same method name, opposite meaning when switching doubles.
- Suggestion: Rename TestConsole’s helper to `ClearOutput` or document the divergence on both APIs.
- Source: test-doubles
- Disposition notes: filed as child **029-001**

### M19 — Severity: nit — Status: open
- File: source/timewarp-terminal/widgets/panel-widget.cs:237
- Description: `RenderContentRow` does `new string(' ', PaddingHorizontal)` with no clamp. A negative `PaddingHorizontal` throws `ArgumentOutOfRangeException` from Render, while sibling APIs (Pad*/Center, Rule width) clamp negatives after 022.
- Suggestion: Clamp horizontal (and vertical) padding to `>= 0` at render or in the builder setters.
- Source: widgets
- Disposition notes: filed as child **029-003**

### M20 — Severity: nit — Status: open
- File: source/timewarp-terminal/timewarp-terminal.csproj:26
- Description: After task 025’s kebab rename (`README.md` → `readme.md`), the pack Include is still `../../README.md` while the on-disk file is `readme.md`. `PackageReadmeFile` remains `README.md` (in-package name). `dotnet pack` currently still embeds the readme on this Linux host (NuGet resolves the wrong-cased source path); fragile vs a strictly case-sensitive open. Confirmed: `README.md` does not exist in the worktree; `readme.md` does.
- Suggestion: Change the Include to `../../readme.md` and set `PackagePath="README.md"` so the source path matches the filesystem while the package entry stays `README.md`.
- Source: tests-infra
- Disposition notes: filed as child **029-004**

### M21 — Severity: nit — Status: open
- File: tools/dev-cli/dev.cs:28
- Description: Header comment documents release as `build -> check-version -> pack -> push`, omitting `clean`, `verify-samples`, and `test`. That contradicts `tools/dev-cli/endpoints/workflow.cs:10` and `RunReleaseWorkflowAsync`.
- Suggestion: Update the `dev.cs` banner to match the real release pipeline order.
- Source: tests-infra
- Disposition notes: filed as child **029-004**

## Duplicates / conflicts

- FormatProvider parallel clobber was raised as **bug** by static-facade and **suggestion** by test-doubles. Collapsed to **M4 bug** (strongest severity). The AsyncLocal mutable `Stack` fork from test-doubles is recorded on M4, not a separate ID.
- OSC 8 remaining injection split into two bugs on purpose: **M5** is C1 ST in the OSC *payload*; **M6** is unsanitized URL reused as *display text* / plain fallback. Same sanitizer, different channels.
- No severity conflicts besides FormatProvider. No 022 findings re-opened: TestConsole Dispose/Read are the 022 TestTerminal fixes that were never mirrored; snupkg *production* landed in 022 but *push* did not.
