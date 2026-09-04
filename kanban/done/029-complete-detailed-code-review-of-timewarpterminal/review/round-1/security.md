# Round 1 — security
**Date:** 2026-09-04
**Scope reviewed:** OSC 8 / ANSI injection, exception swallowing, redirected stdin/stdout

## Summary

022’s OSC 8 URL sanitizer (C0/DEL percent-encoding), `SupportsColor` / `SupportsHyperlinks` gating on library color and link writers, `KeyAvailable`’s `InvalidOperationException` catch, and `IsInteractive` requiring both stdin and stdout unredirected are all still present and correct. One residual injection channel remains: when display text is omitted, `CreateLink` / `WriteLink` embed the raw URL outside the OSC payload (and write that raw URL on the non-hyperlink fallback), so attacker-influenced controls still reach the terminal. Separately, colored `WriteErrorLine` consults stdout-based `SupportsColor` and can emit ANSI into a redirected stderr.

## Issues

### Issue 1 — Severity: bug
- File: source/timewarp-terminal/ansi-hyperlink-extensions.cs:51
- Description: `CreateLink` sanitizes the URL only inside the OSC 8 payload (`SanitizeUrl(url)`), then uses the unsanitized `url` as display text when `displayText` is null (`displayText ?? url`). The same raw URL is chosen in `terminal-hyperlink-extensions.cs:41` / `:71` before either `CreateLink` or the plain `Write`/`WriteLine` fallback when hyperlinks are unsupported. An attacker-influenced URL such as `https://example.com/\x1b]0;Hacked\x07` has a safe OSC payload (`%1B` / `%07`) but still emits raw ESC/BEL in the display (or plain-text) portion, e.g. OSC 0 title injection. This is the library constructing output from the URL parameter it already treats as untrusted — not caller-supplied styled display text (the 022 “caller-embedded ANSI” carve-out).
- Suggestion: When falling back to the URL as display text (and on the `!SupportsHyperlinks` plain-text path that uses `displayText ?? url`), emit `SanitizeUrl(url)` (or strip/replace C0/DEL for display) so the URL parameter cannot reintroduce controls outside the payload. Keep explicit caller `displayText` unmodified so styled links remain possible.
- Status: open

### Issue 2 — Severity: suggestion
- File: source/timewarp-terminal/timewarp-terminal.cs:333-336
- Description: `SupportsColor` is false only when **stdout** is redirected (`!Console.IsOutputRedirected`, plus `NO_COLOR` / `TERM=dumb`). Colored `Terminal.WriteErrorLine` / `WriteErrorLine(..., ConsoleColor, ...)` in `terminal-static.cs:260-302` gate on that property while writing to stderr. When stdout is a TTY and stderr is redirected, library-applied SGR still lands in the redirected error stream (raw escapes in logs/pipes). The inverse (stdout redirected, stderr a TTY) suppresses color even though stderr could accept it.
- Suggestion: For error-colored writers, also require `!IsErrorRedirected` (or a stderr-aware capability) before wrapping with `AnsiColors`, so redirected stderr does not receive library-emitted escapes.
- Status: open
