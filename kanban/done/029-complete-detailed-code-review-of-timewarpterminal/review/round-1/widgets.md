# Round 1 — widgets
**Date:** 2026-09-04
**Scope reviewed:** source/timewarp-terminal/widgets/ (table, panel, rule, unicode-width, ansi-string-utils, borders/truncate)

## Summary

Table, rule, unicode-width, and ANSI strip/measure/truncate paths look consistent with the 022 fixes (snapshot Build, Grow floor, OSC 8 carry, grapheme-aware ellipsis, negative Pad/Rule width clamps). New defects concentrate on panel layout: the default horizontal padding can make content rows wider than the border when the panel is at its minimum width, and `Split('\n')` leaves CR from CRLF content which breaks boxed output on a real terminal. PanelBuilder/RuleBuilder still return the live instance (unlike TableBuilder’s snapshot), and WrapText can emit lines wider than `maxWidth` for a lone wide grapheme.

## Issues

### Issue 1 — Severity: bug
- File: source/timewarp-terminal/widgets/panel-widget.cs:119
- Description: Minimum panel width is hard-coded to 4 (corners + 1 content column) and does not account for `PaddingHorizontal` (default 1). When `contentAreaWidth = width - 2 - 2*PaddingHorizontal` drops below 1 it is forced back up to 1 without widening the panel, so `RenderContentRow` emits `1 + PaddingHorizontal + contentAreaWidth + PaddingHorizontal + 1` columns while top/bottom borders still use `width`. With defaults this already mismatches at `Width`/`WindowWidth` 4 (border 4 vs content row 5); larger padding mismatches at larger widths.
- Suggestion: Set the floor to `2 + 2*PaddingHorizontal + 1` (and clamp/reduce padding if needed) so border width and content-row width always match; add a regression asserting every rendered line has identical visible width for small widths and non-zero padding.
- Status: open

### Issue 2 — Severity: bug
- File: source/timewarp-terminal/widgets/panel-widget.cs:140
- Description: Both bordered and borderless paths split content only on `'\n'` (`RenderWithBorder` here; `RenderWithoutBorder` at panel-widget.cs:112). CRLF input therefore leaves a trailing `'\r'` on each line. `'\r'` is measured as width 0, so borders still look aligned by visible length, but when the line is written the carriage return moves the cursor to column 0 and overwrites the left border/padding.
- Suggestion: Normalize newlines before split (e.g. replace `"\r\n"` / strip `'\r'`), or split on any line ending; cover with a panel test using `"Line1\r\nLine2"`.
- Status: open

### Issue 3 — Severity: suggestion
- File: source/timewarp-terminal/widgets/panel-widget.cs:374
- Description: `PanelBuilder.Build()` / `ToPanel()` return the builder’s live `Panel` instance. The same pattern exists for `RuleBuilder.Build()` at rule-widget.cs:157. TableBuilder was fixed in 022 to return an independent snapshot (table-builder.cs:149-178) with a regression test; post-Build mutation of Panel/Rule builders still mutates previously “built” objects.
- Suggestion: Snapshot Panel/Rule on Build (copy property values onto a new instance), or document that Build returns the live object; align with TableBuilder if snapshot semantics are the intended IBuilder contract.
- Status: open

### Issue 4 — Severity: suggestion
- File: source/timewarp-terminal/widgets/ansi-string-utils.cs:629
- Description: `BreakLongWord` only starts a new line when `currentLineWidth > 0`. A single wide grapheme (emoji/CJK, width 2) on a `maxWidth` of 1 is appended anyway, so `WrapText` can return lines whose visible width exceeds `maxWidth`. TruncateVisible drops such graphemes instead (result may be short). Panel mitigates by truncating after wrap, but direct WrapText callers and the documented “max visible width per line” contract do not.
- Suggestion: When a grapheme alone exceeds `maxWidth` and the line is empty, either drop it (match TruncateVisible) or document that lines may overrun by at most one column for unsplittable wide graphemes; add a WrapText test for `WrapText("😀", 1)`.
- Status: open

### Issue 5 — Severity: nit
- File: source/timewarp-terminal/widgets/panel-widget.cs:237
- Description: `RenderContentRow` does `new string(' ', PaddingHorizontal)` with no clamp. A negative `PaddingHorizontal` (public setter / builder) throws `ArgumentOutOfRangeException` from Render, while sibling APIs (Pad*/Center, Rule width) clamp negatives after 022.
- Suggestion: Clamp horizontal (and vertical) padding to `>= 0` at render or in the builder setters.
- Status: open
