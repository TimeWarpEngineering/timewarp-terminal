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

- [x] M7 min-width accounts for padding; all rendered lines same visible width
- [x] M8 CRLF content does not overwrite borders
- [x] M15 Panel/Rule Build snapshot or documented live-instance contract
- [x] M16 WrapText maxWidth for unsplittable wide graphemes
- [x] M19 negative padding clamped
- [x] Panel/rule/wrap tests green

## Notes

- Parent: `kanban/in-progress/029-complete-detailed-code-review-of-timewarpterminal/review/round-1/merged.md`
- 022 already fixed rule negative width, panel WordWrap(false) truncate, TableBuilder snapshot. Do not regress those.

## Session

- Created: 3364129 (2026-09-04)
- Parent review: Grok `01a06a96-935c-79a0-b334-1e5bc6c6b568` (2026-09-04)
- Implementer: Grok 4.6 session `01a0bcf8-4a45-79a2-b582-dcf1a8c90ce6` (2026-09-20)

## Results

Closed parent 029 round-1 findings **M7, M8, M15, M16, M19** on this child.

**What was implemented**

- Bordered panel width floors at `2 + 2*paddingHorizontal + 1` after clamping padding, so content-area width is naturally >= 1 and every rendered line shares the same visible width.
- Content newlines are normalized (`\r\n` and lone `\r` → `\n`) before split in both bordered and borderless render paths.
- `PanelBuilder.Build` / `ToPanel` and `RuleBuilder.Build` / `ToRule` return independent snapshots (same IBuilder contract as TableBuilder).
- `WrapText` / `BreakLongWord` drop a grapheme that is wider than `maxWidth` (same as TruncateVisible), so a lone `😀` at width 1 does not overrun.
- Horizontal and vertical padding clamp to `>= 0` in PanelBuilder setters and again at render (public setters can still go negative after Build).
- Layout `GetPanelMinWidth` / natural width use the same padding-aware floor so assigned boxes are not narrower than `Panel.Render` will emit.

**Files changed**

- `source/timewarp-terminal/widgets/panel-widget.cs`
- `source/timewarp-terminal/widgets/rule-widget.cs`
- `source/timewarp-terminal/widgets/ansi-string-utils.cs`
- `source/timewarp-terminal-layout/widget-measure.cs`
- `tests/panel-widget-01-basic.cs`
- `tests/rule-widget-01-basic.cs`
- `tests/ansi-string-utils-02-wrap-text.cs`
- `tests/layout-08-min-width.cs`

**Key decisions**

- Floor width rather than shrink padding when the requested width is too small for the pad.
- Drop unsplittable wide graphemes rather than document a one-column overrun.
- Snapshot builders rather than document live-instance `Build`.
- Clamp at both builder and render: properties stay public settable.

**Test outcomes**

- `dotnet tests/panel-widget-01-basic.cs` — 19 passed
- `dotnet tests/panel-widget-03-word-wrap.cs` — 10 passed (WordWrap(false) truncate unregressed)
- `dotnet tests/rule-widget-01-basic.cs` — 10 passed
- `dotnet tests/ansi-string-utils-02-wrap-text.cs` — 15 passed
- `dotnet tests/layout-08-min-width.cs` — 1 passed (container width 12; min >= 5)
- `dotnet tests/layout-10-widgets.cs` — 1 passed
- `dotnet tests/table-widget-01-basic.cs` — 10 passed (TableBuilder snapshot unregressed)

### How to validate

**Smoke**

```bash
dotnet tests/panel-widget-01-basic.cs
dotnet tests/ansi-string-utils-02-wrap-text.cs
dotnet tests/rule-widget-01-basic.cs
```

**Expect**

- Exit 0 on each
- Panel: `Total: 19` / `Passed: 19`
- WrapText: `Total: 15` / `Passed: 15`
- Rule: `Total: 10` / `Passed: 10`
- Named regressions present and passing:
  - `Should_widen_panel_so_default_padding_keeps_uniform_line_width`
  - `Should_normalize_crlf_content_with_border`
  - `Should_build_independent_snapshots` (panel and rule)
  - `Should_drop_grapheme_wider_than_max_width`
  - `Should_clamp_negative_padding_without_throwing`

**Automated gate**

```bash
dotnet tests/panel-widget-01-basic.cs
dotnet tests/panel-widget-03-word-wrap.cs
dotnet tests/rule-widget-01-basic.cs
dotnet tests/ansi-string-utils-02-wrap-text.cs
dotnet tests/layout-08-min-width.cs
dotnet tests/layout-10-widgets.cs
dotnet tests/table-widget-01-basic.cs
```

**Not in scope:** live TTY box-drawing inspection. `dotnet test` on the `.slnx` finds nothing (Jaribu runfiles, not VSTest).
