# Round 1 — general
**Date:** 2026-09-20
**Scope reviewed:** branch task/029-003-fix-panel-layout-padding-min-width-crlf-builder-sn vs origin/master (product commit 4da3341)

## Summary

Product commit `4da3341` closes the five claimed parent findings with matching layout floors: bordered panel width floors at `2 + 2*padH + 1` after clamping padding (`panel-widget.cs:121-125`, `widget-measure.cs:146-154`, `ApplyItemFlex` uses `Max(min, fixed)` at `layout-engine.cs:236`), CRLF/CR normalize before split in bordered and borderless paths (`SplitContentLines` at `panel-widget.cs:252-257`), PanelBuilder/RuleBuilder snapshot like TableBuilder (`panel-widget.cs:385-397`, `rule-widget.cs:159-167`), `BreakLongWord` drops graphemes wider than `maxWidth` (`ansi-string-utils.cs:643-646`), and padding clamps in builder setters plus at render (`panel-widget.cs:121-122`, `:330-353`). Risk is low; 022 rule negative-width, WordWrap(false) truncate, and TableBuilder snapshot remain green. **M7, M8, M15, M16, and M19 all appear closed.**

## Issues

No issues.
