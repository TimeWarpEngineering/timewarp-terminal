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

## Implementation

Added `UnicodeWidth` utility class that calculates terminal display width accounting for wide characters (emoji, CJK) and zero-width characters (combining marks, ZWJ). Updated `GetVisibleLength()` to use it. No external dependencies — uses .NET 10 built-in `Rune`, `StringInfo`, and `UnicodeCategory` APIs (all AOT-compatible).

### Wide character ranges covered (width 2)

**Emoji_Presentation=Yes (BMP, from Unicode 16.0 emoji-data.txt):**
- U+231A-231B ⌚⌛, U+23E9-23EC ⏩⏪⏫⏬, U+23F0 ⏰, U+23F3 ⏳
- U+25FD-25FE ◽◾
- U+2614-2615 ☔☕, U+2648-2653 ♈-♓, U+267F ♿, U+2693 ⚓, U+26A1 ⚡
- U+26AA-26AB ⚪⚫, U+26BD-26BE ⚽⚾, U+26C4-26C5 ⛄⛅, U+26CE ⛎
- U+26D4 ⛔, U+26EA ⛪, U+26F2-26F3 ⛲⛳, U+26F5 ⛵, U+26FA ⛺, U+26FD ⛽
- U+2705 ✅, U+270A-270B ✊✋, U+2728 ✨, U+274C ❌, U+274E ❎
- U+2753-2755 ❓❔❕, U+2757 ❗, U+2795-2797 ➕➖➗, U+27B0 ➰, U+27BF ➿
- U+2B1B-2B1C ⬛⬜, U+2B50 ⭐, U+2B55 ⭕

**Text-presentation characters (☀♻✈▶◀▪▫ etc.)** are width 1 as single
runes. They become width 2 when combined with VS16 (U+FE0F) as
multi-codepoint grapheme clusters, handled automatically by GetTextWidth.

**Emoji blocks (SMP):**
- U+1F000-U+1FAFF, U+1FC00-U+1FFFF — Emoji blocks
- U+1F1E0-U+1F1FF — Regional indicator symbols (flags)

**CJK:**
- U+2E80-U+303E — CJK Radicals, Kangxi, Ideographic Description
- U+3041-U+33BF — Hiragana, Katakana, Bopomofo, CJK Compatibility
- U+3400-U+4DBF — CJK Extension A
- U+4E00-U+9FFF — CJK Unified Ideographs
- U+A000-U+A4CF — Yi Syllables and Radicals
- U+AC00-U+D7A3 — Hangul Syllables
- U+F900-U+FAFF — CJK Compatibility Ideographs
- U+FE10-U+FE19 — Vertical Forms
- U+FE30-U+FE6F — CJK Compatibility Forms
- U+FF01-U+FF60, U+FFE0-U+FFE6 — Fullwidth forms
- U+1100-U+115F — Hangul Jamo
- U+2329-U+232A — Wide angle brackets
- U+20000-U+2A6DF — CJK Extension B
- U+2A700-U+2B73F — CJK Extension C
- U+2B740-U+2B81F — CJK Extension D
- U+2B820-U+2CEAF — CJK Extension E
- U+2CEB0-U+2EBEF — CJK Extension F
- U+2F800-U+2FA1F — CJK Compatibility Ideographs Supplement
- U+30000-U+3134F — CJK Extension G
- U+31350-U+323AF — CJK Extension H

**Zero-width (width 0):**
- Control characters
- UnicodeCategory.NonSpacingMark, EnclosingMark, Format
- U+00AD (soft hyphen), U+200B-U+200D (ZWSP, ZWNJ, ZWJ), U+2060 (WJ)
- U+FE00-U+FE0F (variation selectors), U+E0100-U+E01EF (VS supplement)

**Multi-codepoint grapheme clusters** (ZWJ sequences, flags, skin-tone) → width 2

### Files changed

- `source/timewarp-terminal/widgets/unicode-width.cs` (new)
- `source/timewarp-terminal/widgets/ansi-string-utils.cs` (modified)
- `source/timewarp-terminal/widgets/table-widget.cs` (modified)
- `source/timewarp-terminal/global-usings.cs` (modified)
- `samples/emoji-table-widget.cs` (new)
- `tests/unicode-width-01-basic.cs` (new)
- `tests/ansi-string-utils-03-emoji-width.cs` (new)
- `tests/table-widget-06-emoji.cs` (new)

## Checklist

- [x] Investigate the table rendering code to understand how cell width calculations work
- [x] Identify why emojis cause border misalignment (emoji display width vs character count)
- [x] Research proper emoji width handling (full-width vs half-width emojis)
- [x] Implement UnicodeWidth utility class with comprehensive ranges
- [x] Update GetVisibleLength to use display width
- [x] Update WrapText for grapheme-cluster-aware iteration
- [x] Update TruncateWithEllipsis for display-width-aware slicing
- [x] Add tests for UnicodeWidth, AnsiStringUtils emoji, and table emoji rendering
- [x] Verify tables without emojis still render correctly (92 tests pass)
- [x] Visually verify weather report table alignment
