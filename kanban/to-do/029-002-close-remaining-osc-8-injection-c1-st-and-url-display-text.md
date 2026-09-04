# Close remaining OSC 8 injection (C1 ST and URL display text)

## Description

Parent **029** round-1 merged findings **M5, M6**.

022 percent-encodes C0 and DEL in the OSC 8 URL payload. Two channels remain: C1 STRING TERMINATOR (`U+009C`) in the payload, and the unsanitized URL reused as display text / plain-text fallback.

Do **not** create a sibling “apply 029 findings” task. This child is the product-fix batch.

## Requirements

### M5 — bug — `SanitizeUrl` leaves C1 ST in the OSC payload
- File: `source/timewarp-terminal/ansi-hyperlink-extensions.cs:71`
- Percent-encode C1 controls that can terminate or introduce OSC/APC/PM/DCS (at minimum `U+009C`; consider the full C1 set `U+0080`–`U+009F`).
- Add a regression beside the existing ESC/BEL case in `tests/hyperlink-01-basic.cs`.

### M6 — bug — unsanitized URL reused as display text
- File: `source/timewarp-terminal/ansi-hyperlink-extensions.cs:51`
- When `displayText` is null, `CreateLink` embeds the raw `url` outside the OSC payload (`displayText ?? url`). `terminal-hyperlink-extensions.cs:41` / `:71` use the same raw URL on the `!SupportsHyperlinks` plain path.
- Example: `https://example.com/\x1b]0;Hacked\x07` has a safe OSC payload (`%1B`/`%07`) but still emits ESC/BEL in the display (or plain-text) portion.
- When falling back to the URL as display text, emit `SanitizeUrl(url)` (or strip C0/DEL/C1 for display). Keep explicit caller `displayText` unmodified (022 caller-embedded ANSI carve-out).

## Checklist

- [ ] M5 C1 ST (and remaining C1 OSC terminators) percent-encoded in `SanitizeUrl`
- [ ] M6 URL-as-display-text / plain fallback does not emit raw controls from the URL parameter
- [ ] Explicit `displayText` still unmodified
- [ ] Hyperlink regression tests for `\x9c` in URL payload and ESC/BEL in URL-as-display
- [ ] All WriteLink / CreateLink / `.Link()` paths still share the single sanitizer

## Notes

- Parent: `kanban/in-progress/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already closed C0/DEL in the payload; do not regress that.

## Session

- Created: 3362509 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
