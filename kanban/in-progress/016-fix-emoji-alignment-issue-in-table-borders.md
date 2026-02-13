# Fix emoji alignment issue in table borders

## Description

Emojis in table content cause misalignment with the table borders. When emojis are used in cells (like weather icons 🌤️, 📍, 🌡️, ☁️), the table border lines don't line up properly with the content, creating a visual misalignment.

## Example

Sample output showing the problem:
```
═════════════════════════════════════════ 🌤️ Weather Report ═════════════════════════════════════════
╭─────────────────┬───────────────────────╮
│ 📍 Location     │ Dallas, United States │
│ 🌡️ Temperature │ 25.2°C (77.4°F)       │
│ ☁️ Condition    │ Overcast              │
╰─────────────────┴───────────────────────╯
```

## Root Cause

`AnsiStringUtils.GetVisibleLength()` returns `StripAnsiCodes(text).Length` which counts .NET string length (UTF-16 code units), not terminal display columns. Emojis like `📍` take 2 terminal columns but `.Length` counts them as 1-2 characters, making borders too short.

## Implementation Plan

Add a `UnicodeWidth` utility class that calculates terminal display width accounting for wide characters (emoji, CJK) and zero-width characters (combining marks, ZWJ). Update `GetVisibleLength()` to use it. No external dependencies — uses .NET 10 built-in `Rune`, `StringInfo`, and `UnicodeCategory` APIs (all AOT-compatible).

### Steps

1. Create `source/timewarp-terminal/widgets/unicode-width.cs` — static `UnicodeWidth` class with `GetRuneWidth(Rune)` and `GetTextWidth(string)` methods
2. Add `global using System.Globalization;` to `source/timewarp-terminal/global-usings.cs`
3. Update `AnsiStringUtils.GetVisibleLength()` (line 60) to use `UnicodeWidth.GetTextWidth()` instead of `.Length`
4. Update `AnsiStringUtils.WrapText()` to use display width and grapheme cluster iteration instead of `word.Length` and `foreach (char c in word)`
5. Update `TruncateWithEllipsis` and `TruncateMiddle` in `table-widget.cs` to use display-width-aware slicing

### Key Files

- `source/timewarp-terminal/widgets/unicode-width.cs` (new)
- `source/timewarp-terminal/widgets/ansi-string-utils.cs` (modify)
- `source/timewarp-terminal/widgets/table-widget.cs` (modify)
- `source/timewarp-terminal/global-usings.cs` (modify)

## Checklist

- [x] Investigate the table rendering code to understand how cell width calculations work
- [x] Identify why emojis cause border misalignment (emoji display width vs character count)
- [x] Research proper emoji width handling (full-width vs half-width emojis)
- [ ] Implement UnicodeWidth utility class
- [ ] Update GetVisibleLength to use display width
- [ ] Update WrapText for grapheme-cluster-aware iteration
- [ ] Update TruncateWithEllipsis for display-width-aware slicing
- [ ] Add tests for UnicodeWidth, AnsiStringUtils emoji, and table emoji rendering
- [ ] Verify tables without emojis still render correctly
- [ ] Visually verify weather report table alignment
