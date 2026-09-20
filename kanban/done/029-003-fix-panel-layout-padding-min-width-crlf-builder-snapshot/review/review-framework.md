# Review framework — task 029-003

**Date:** 2026-09-20
**Host task:** kanban/in-progress/029-003-fix-panel-layout-padding-min-width-crlf-builder-snapshot/
**Diff scope:** branch `task/029-003-fix-panel-layout-padding-min-width-crlf-builder-sn` vs `origin/master` — product commit `4da3341` (`fix: keep panel boxes aligned at padded min-width`). Kanban-only commit `6164eb6` is out of product-code scope except as the implementer brief.
**Plan / brief:** Child of parent **029** round-1 merged findings **M7, M8, M15, M16, M19**. Floor bordered panel min-width at `2 + 2*padH + 1`; normalize CRLF/CR before split; snapshot PanelBuilder/RuleBuilder like TableBuilder; drop WrapText graphemes wider than `maxWidth`; clamp negative padding. Layout `GetPanelMinWidth` / natural width must match Render so assigned boxes are not narrower than the panel will emit. Do not regress 022 rule negative width, panel WordWrap(false) truncate, or TableBuilder snapshot.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** Review oracle Grok `01a0bd03-4b65-7a80-9099-836f4e663b77` (2026-09-20); implementer Grok 4.6 `01a0bcf8-4a45-79a2-b582-dcf1a8c90ce6` (2026-09-20)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`

## Parent findings this child claims to close

| ID | Severity | Claimed fix |
|----|----------|-------------|
| M7 | bug | Bordered width floors at `2 + 2*paddingHorizontal + 1`; uniform visible line width |
| M8 | bug | Newlines normalized (`\r\n` and lone `\r` → `\n`) before split in bordered and borderless paths |
| M15 | suggestion | `PanelBuilder.Build`/`ToPanel` and `RuleBuilder.Build`/`ToRule` return independent snapshots |
| M16 | suggestion | `WrapText`/`BreakLongWord` drop a grapheme wider than `maxWidth` (same as TruncateVisible) |
| M17 | — | Not this child (029-005) |
| M19 | nit | Horizontal and vertical padding clamp `>= 0` in builder setters and at render |
