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

- [x] M5 C1 ST (and remaining C1 OSC terminators) percent-encoded in `SanitizeUrl`
- [x] M6 URL-as-display-text / plain fallback does not emit raw controls from the URL parameter
- [x] Explicit `displayText` still unmodified
- [x] Hyperlink regression tests for `\x9c` in URL payload and ESC/BEL in URL-as-display
- [x] All WriteLink / CreateLink / `.Link()` paths still share the single sanitizer

## Notes

- Parent: `kanban/done/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already closed C0/DEL in the payload; do not regress that.

## Session

- Created: 3362509 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
- Implementer: Grok session `01a0a368-5323-7ba0-8bd0-e9c824921ce7` (2026-09-15)

## Results

Closed parent 029 round-1 findings **M5** and **M6** on this child.

**What was implemented**

- `SanitizeUrl` percent-encodes the full C1 set (`U+0080`–`U+009F`) in addition to C0 and DEL, so C1 STRING TERMINATOR (`U+009C`) cannot terminate the OSC 8 payload.
- When `displayText` is omitted, `CreateLink` reuses the sanitized URL as display text. `WriteLink` / `WriteLinkLine` no longer pre-resolve `displayText ?? url` (that treated the raw URL as explicit display text and bypassed the fallback). The `!SupportsHyperlinks` path uses `ResolveDisplayText`, which calls the same sanitizer.
- Explicit caller `displayText` (including the `.Link()` receiver) is still unmodified.

**Files changed**

- `source/timewarp-terminal/ansi-hyperlink-extensions.cs`
- `source/timewarp-terminal/terminal-hyperlink-extensions.cs`
- `tests/hyperlink-01-basic.cs`

**Key decisions**

- Encode the full C1 range, not only ST: OSC/APC/PM/DCS introducers live in `U+0080`–`U+009F`.
- Reuse percent-encoding for URL-as-display rather than a second strip-controls helper, so WriteLink / CreateLink / `.Link()` share one sanitizer.
- Static `Terminal.WriteLink*` still require non-null display text and do not take the URL fallback.

**Test outcomes**

- `dotnet tests/hyperlink-01-basic.cs` — 25 passed (was 16), including C1 ST payload, C1 range endpoints, ESC/BEL URL-as-display, explicit `displayText` carve-out, and WriteLink / WriteLinkLine plain fallbacks.
- `dotnet tests/terminal-static-05-widgets.cs` — 16 passed (static facade WriteLink unchanged).

### How to validate

**Smoke**

```bash
dotnet tests/hyperlink-01-basic.cs
```

**Expect**

- Exit 0
- `Total: 25` / `Passed: 25`
- Named regressions present and passing:
  - `Should_percent_encode_c1_string_terminator_in_url`
  - `Should_sanitize_url_when_used_as_display_text`
  - `Should_sanitize_url_as_plain_text_when_hyperlinks_not_supported`
  - `Should_leave_explicit_display_text_unmodified`

**Automated gate**

```bash
dotnet tests/hyperlink-01-basic.cs
dotnet tests/terminal-static-05-widgets.cs
```

**Not in scope:** live terminal OSC 8 click-through. Static `Terminal.WriteLink` still requires explicit non-null display text.
