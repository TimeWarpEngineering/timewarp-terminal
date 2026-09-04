# Fix panel layout (padding min-width, CRLF, builder snapshot)

## Description

Parent **029** round-1 merged findings **M7, M8, M15, M16, M19**.

Panel layout still breaks boxes at the min-width + default padding, CRLF content leaves `\r` that overwrites the left border, PanelBuilder/RuleBuilder still return the live instance (unlike TableBuilder’s 022 snapshot), WrapText can exceed `maxWidth` for a lone wide grapheme, and negative padding throws at Render.

Do **not** create a sibling “apply 029 findings” task. This child is the product-fix batch.

## Requirements

### M7 — bug — panel min width ignores `PaddingHorizontal`
- File: `source/timewarp-terminal/widgets/panel-widget.cs:119`
- Floor is hard-coded to 4; `contentAreaWidth` is then forced up to 1 without widening the panel. Default pad 1 at width 4: border 4 vs content row 5.
- Floor width at `2 + 2*PaddingHorizontal + 1` (or reduce padding). Assert every rendered line has identical visible width.

### M8 — bug — `Split('\n')` leaves CR from CRLF
- File: `source/timewarp-terminal/widgets/panel-widget.cs:140` (borderless `:112`)
- Normalize newlines before split. Cover `"Line1\r\nLine2"`.

### M15 — suggestion — PanelBuilder/RuleBuilder return the live instance
- Files: `panel-widget.cs:374`, `rule-widget.cs:157`
- Snapshot on Build (copy property values onto a new instance), **or** document that Build returns the live object. Align with TableBuilder if snapshot is the IBuilder contract.

### M16 — suggestion — WrapText can exceed `maxWidth` for a lone wide grapheme
- File: `source/timewarp-terminal/widgets/ansi-string-utils.cs:629`
- `BreakLongWord` only starts a new line when `currentLineWidth > 0`. `WrapText("😀", 1)` can return a line of visible width 2.
- Drop the grapheme (match TruncateVisible) or document overrun by at most one column. Add a WrapText test.

### M19 — nit — negative `PaddingHorizontal` throws at Render
- File: `source/timewarp-terminal/widgets/panel-widget.cs:237`
- Clamp horizontal (and vertical) padding to `>= 0` at render or in the builder setters (Pad*/Center/Rule already clamp after 022).

## Checklist

- [ ] M7 min-width accounts for padding; all rendered lines same visible width
- [ ] M8 CRLF content does not overwrite borders
- [ ] M15 Panel/Rule Build snapshot or documented live-instance contract
- [ ] M16 WrapText maxWidth for unsplittable wide graphemes
- [ ] M19 negative padding clamped
- [ ] Panel/rule/wrap tests green

## Notes

- Parent: `kanban/in-progress/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already fixed rule negative width, panel WordWrap(false) truncate, TableBuilder snapshot. Do not regress those.

## Session

- Created: 3364129 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
