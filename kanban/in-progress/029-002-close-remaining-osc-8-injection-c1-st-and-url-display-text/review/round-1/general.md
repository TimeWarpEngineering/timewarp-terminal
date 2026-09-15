# Round 1 — general
**Date:** 2026-09-15
**Scope reviewed:** commit `adac20c` vs `origin/feature/overnight-terminal` — `source/timewarp-terminal/ansi-hyperlink-extensions.cs`, `source/timewarp-terminal/terminal-hyperlink-extensions.cs`, `tests/hyperlink-01-basic.cs`; call-site check of `terminal-static.cs` WriteLink/WriteLinkLine, `tests/terminal-static-05-widgets.cs`, `samples/hyperlink-widget.cs`

## Summary

`SanitizeUrl` now percent-encodes C0, DEL, and the full C1 range (`U+007F`–`U+009F` via `>= '\x7f' and <= '\x9f'`), and omitted `displayText` reuses that sanitized URL in `CreateLink` and on the `!SupportsHyperlinks` plain path via `ResolveDisplayText`. The prior WriteLink pre-resolve (`displayText ?? url`) that treated raw URL as explicit display is gone; static `Terminal.WriteLink*` still require non-null text and share `CreateLink`. Risk is low: M5/M6 land with focused regressions, empty vs null stays distinct, and smoke tests pass (25 + 16).

## Issues

## Parent findings coverage

- **M5** — yes. Predicate `c is < '\x20' or (>= '\x7f' and <= '\x9f')` in `ansi-hyperlink-extensions.cs:79` encodes C1 ST `U+009C` and range endpoints `U+0080`/`U+009F` (plus DEL). Tests: `Should_percent_encode_c1_string_terminator_in_url`, `Should_percent_encode_full_c1_range_in_url` (beside existing `Should_percent_encode_control_characters_in_url`).
- **M6** — yes. `CreateLink` uses `displayText ?? sanitizedUrl` (`ansi-hyperlink-extensions.cs:56-58`); WriteLink/WriteLinkLine pass null through and resolve plain fallback with `ResolveDisplayText` (`terminal-hyperlink-extensions.cs:55-60`, `:84-89`). Explicit display (including `.Link()` receiver) unchanged. Tests: `Should_sanitize_url_when_used_as_display_text`, `Should_leave_explicit_display_text_unmodified`, `Should_leave_link_extension_display_text_unmodified`, `Should_sanitize_url_as_plain_text_when_hyperlinks_not_supported`, `Should_sanitize_url_as_display_text_on_write_link_when_supported`, `Should_leave_explicit_display_text_unmodified_on_plain_write_link`, `Should_sanitize_url_as_plain_text_on_write_link_line_when_not_supported`.

## Smoke tests

- `dotnet tests/hyperlink-01-basic.cs` — **pass** (25/25)
- `dotnet tests/terminal-static-05-widgets.cs` — **pass** (16/16)
