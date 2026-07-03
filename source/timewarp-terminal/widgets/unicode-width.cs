namespace TimeWarp.Terminal;

#region Design
// Measurement rules (target: match modern terminal wcwidth, e.g. foot/kitty/wezterm):
// 1. Rune width comes from Unicode 16 East_Asian_Width (W/F => 2) plus
//    Emoji_Presentation=Yes code points (=> 2). Control characters, combining
//    marks, format characters, ZWJ/ZWSP/ZWNJ/WJ, soft hyphen, and variation
//    selectors are width 0. Everything else is width 1. SMP blocks that are
//    EAW=N (Mahjong/Dominos/Playing Cards/Alchemical/Ornamental Dingbats/
//    Supplemental Arrows-C/most Geometric Shapes Extended/most Enclosed
//    Alphanumeric Supplement) fall through to the width-1 default; only their
//    emoji-presentation exceptions (e.g. U+1F004, U+1F0CF) are width 2.
// 2. Text width is measured per grapheme cluster:
//    - single-rune cluster => its rune width;
//    - cluster containing ZWJ (U+200D), VS16 (U+FE0F), or a skin-tone
//      modifier (U+1F3FB-U+1F3FF) => 2 (renders as one emoji glyph);
//    - regional-indicator pair (flag) => 2;
//    - anything else (e.g. NFD combining sequences like "e" + U+0301) =>
//      sum of rune widths, so combining marks add nothing to the base.
#endregion

using System.Globalization;

/// <summary>
/// Calculates terminal display width for Unicode text.
/// Accounts for wide characters (CJK, emoji), zero-width characters
/// (combining marks, variation selectors, ZWJ), and multi-codepoint
/// grapheme clusters (emoji sequences).
/// </summary>
public static class UnicodeWidth
{
  /// <summary>
  /// Gets the terminal display width of a single Unicode scalar value.
  /// </summary>
  /// <param name="rune">The Unicode Rune to measure.</param>
  /// <returns>0, 1, or 2 columns.</returns>
  public static int GetRuneWidth(Rune rune)
  {
    int value = rune.Value;

    // Control characters
    if (Rune.IsControl(rune))
    {
      return 0;
    }

    UnicodeCategory category = Rune.GetUnicodeCategory(rune);

    // Zero-width categories
    if (category is UnicodeCategory.NonSpacingMark
        or UnicodeCategory.EnclosingMark
        or UnicodeCategory.Format)
    {
      return 0;
    }

    // Explicit zero-width code points
    if (value is 0x00AD) // Soft hyphen
    {
      return 0;
    }

    if (value is 0x200B or 0x200C or 0x200D or 0x2060) // ZWSP, ZWNJ, ZWJ, WJ
    {
      return 0;
    }

    if (value is >= 0xFE00 and <= 0xFE0F) // Variation selectors VS1-VS16
    {
      return 0;
    }

    if (value is >= 0xE0100 and <= 0xE01EF) // Variation selectors supplement
    {
      return 0;
    }

    // ── Emoji_Presentation=Yes code points (U+2000-U+2BFF) ──
    // Source: Unicode 16.0 emoji-data.txt — only code points that render
    // as width 2 without VS16. Text-presentation characters become width 2
    // when combined with VS16 as multi-codepoint clusters (handled by GetTextWidth).
    if (value is 0x231A or 0x231B                       // ⌚⌛
        or (>= 0x23E9 and <= 0x23EC)                    // ⏩⏪⏫⏬
        or 0x23F0 or 0x23F3                              // ⏰⏳
        or 0x25FD or 0x25FE                              // ◽◾
        or 0x2614 or 0x2615                              // ☔☕
        or (>= 0x2648 and <= 0x2653)                     // ♈♉♊♋♌♍♎♏♐♑♒♓
        or 0x267F or 0x2693 or 0x26A1                    // ♿⚓⚡
        or 0x26AA or 0x26AB                              // ⚪⚫
        or 0x26BD or 0x26BE                              // ⚽⚾
        or 0x26C4 or 0x26C5 or 0x26CE                   // ⛄⛅⛎
        or 0x26D4 or 0x26EA                              // ⛔⛪
        or 0x26F2 or 0x26F3 or 0x26F5                    // ⛲⛳⛵
        or 0x26FA or 0x26FD                              // ⛺⛽
        or 0x2705                                        // ✅
        or 0x270A or 0x270B                              // ✊✋
        or 0x2728                                        // ✨
        or 0x274C or 0x274E                              // ❌❎
        or (>= 0x2753 and <= 0x2755) or 0x2757           // ❓❔❕❗
        or (>= 0x2795 and <= 0x2797)                     // ➕➖➗
        or 0x27B0 or 0x27BF                              // ➰➿
        or 0x2B1B or 0x2B1C                              // ⬛⬜
        or 0x2B50 or 0x2B55)                             // ⭐⭕
    {
      return 2;
    }

    // Wide angle brackets
    if (value is >= 0x2329 and <= 0x232A)
    {
      return 2;
    }

    // ── CJK ranges (width 2) ──

    // CJK Radicals, Kangxi, Ideographic Description
    if (value is >= 0x2E80 and <= 0x303E)
    {
      return 2;
    }
    // Hiragana, Katakana, Bopomofo, CJK Compatibility (full block to U+33FF)
    if (value is >= 0x3041 and <= 0x33FF)
    {
      return 2;
    }
    // CJK Unified Ideographs Extension A
    if (value is >= 0x3400 and <= 0x4DBF)
    {
      return 2;
    }
    // CJK Unified Ideographs
    if (value is >= 0x4E00 and <= 0x9FFF)
    {
      return 2;
    }
    // Yi Syllables and Radicals
    if (value is >= 0xA000 and <= 0xA4CF)
    {
      return 2;
    }
    // Hangul Jamo Extended-A
    if (value is >= 0xA960 and <= 0xA97F)
    {
      return 2;
    }
    // Hangul Syllables
    if (value is >= 0xAC00 and <= 0xD7A3)
    {
      return 2;
    }
    // CJK Compatibility Ideographs
    if (value is >= 0xF900 and <= 0xFAFF)
    {
      return 2;
    }

    // ── CJK and Fullwidth forms ──

    // Vertical Forms
    if (value is >= 0xFE10 and <= 0xFE19)
    {
      return 2;
    }
    // CJK Compatibility Forms
    if (value is >= 0xFE30 and <= 0xFE6F)
    {
      return 2;
    }
    // Fullwidth Latin, Punctuation, Katakana, Hangul
    if (value is >= 0xFF01 and <= 0xFF60)
    {
      return 2;
    }
    // Fullwidth Signs
    if (value is >= 0xFFE0 and <= 0xFFE6)
    {
      return 2;
    }

    // ── Hangul Jamo ──

    if (value is >= 0x1100 and <= 0x115F)
    {
      return 2;
    }

    // ── Emoji blocks (Supplementary Multilingual Plane) ──
    // Only EAW=W / Emoji_Presentation=Yes ranges are wide. Narrow (EAW=N)
    // blocks fall through to the width-1 default: Mahjong U+1F000-U+1F02B
    // (except U+1F004), Dominos U+1F030-U+1F093, Playing Cards
    // U+1F0A0-U+1F0F5 (except U+1F0CF), most Enclosed Alphanumeric Supplement
    // U+1F100-U+1F1AC, Ornamental Dingbats U+1F650-U+1F67F, Alchemical
    // Symbols U+1F700-U+1F77F, most Geometric Shapes Extended
    // U+1F780-U+1F7D8, and Supplemental Arrows-C U+1F800-U+1F8B1.

    // Mahjong Tile Red Dragon (emoji presentation)
    if (value is 0x1F004)
    {
      return 2;
    }
    // Playing Card Black Joker (emoji presentation)
    if (value is 0x1F0CF)
    {
      return 2;
    }
    // Enclosed Alphanumeric Supplement emoji (AB button; squared CL..VS)
    if (value is 0x1F18E or (>= 0x1F191 and <= 0x1F19A))
    {
      return 2;
    }
    // Regional indicator symbols (flag emoji components)
    if (value is >= 0x1F1E0 and <= 0x1F1FF)
    {
      return 2;
    }
    // Enclosed Ideographic Supplement
    if (value is (>= 0x1F200 and <= 0x1F202)
        or (>= 0x1F210 and <= 0x1F23B)
        or (>= 0x1F240 and <= 0x1F248)
        or 0x1F250 or 0x1F251
        or (>= 0x1F260 and <= 0x1F265))
    {
      return 2;
    }
    // Miscellaneous Symbols and Pictographs, Emoticons
    if (value is >= 0x1F300 and <= 0x1F64F)
    {
      return 2;
    }
    // Transport and Map Symbols (wide/emoji-presentation subranges)
    if (value is (>= 0x1F680 and <= 0x1F6D7)
        or (>= 0x1F6DC and <= 0x1F6EC)
        or (>= 0x1F6F4 and <= 0x1F6FC))
    {
      return 2;
    }
    // Geometric Shapes Extended: large colored circles and squares
    if (value is (>= 0x1F7E0 and <= 0x1F7EB) or 0x1F7F0)
    {
      return 2;
    }
    // Supplemental Symbols and Pictographs
    if (value is >= 0x1F900 and <= 0x1F9FF)
    {
      return 2;
    }
    // Symbols and Pictographs Extended-A
    if (value is >= 0x1FA70 and <= 0x1FAFF)
    {
      return 2;
    }

    // ── Other wide SMP scripts ──

    // Tangut, Tangut Components
    if (value is >= 0x17000 and <= 0x187F7)
    {
      return 2;
    }
    // Tangut Supplement, Khitan Small Script
    if (value is >= 0x18800 and <= 0x18CD5)
    {
      return 2;
    }
    // Kana Extended-B
    if (value is >= 0x1AFF0 and <= 0x1AFFE)
    {
      return 2;
    }
    // Kana Supplement, Kana Extended-A, Small Kana Extension
    if (value is (>= 0x1B000 and <= 0x1B122)
        or 0x1B132
        or (>= 0x1B150 and <= 0x1B152)
        or 0x1B155
        or (>= 0x1B164 and <= 0x1B167))
    {
      return 2;
    }
    // Nushu
    if (value is >= 0x1B170 and <= 0x1B2FB)
    {
      return 2;
    }

    // ── CJK Extensions in Supplementary Ideographic Plane ──

    // CJK Extension B
    if (value is >= 0x20000 and <= 0x2A6DF)
    {
      return 2;
    }
    // CJK Extension C
    if (value is >= 0x2A700 and <= 0x2B73F)
    {
      return 2;
    }
    // CJK Extension D
    if (value is >= 0x2B740 and <= 0x2B81F)
    {
      return 2;
    }
    // CJK Extension E
    if (value is >= 0x2B820 and <= 0x2CEAF)
    {
      return 2;
    }
    // CJK Extension F
    if (value is >= 0x2CEB0 and <= 0x2EBEF)
    {
      return 2;
    }
    // CJK Compatibility Ideographs Supplement
    if (value is >= 0x2F800 and <= 0x2FA1F)
    {
      return 2;
    }
    // CJK Extension G
    if (value is >= 0x30000 and <= 0x3134F)
    {
      return 2;
    }
    // CJK Extension H
    if (value is >= 0x31350 and <= 0x323AF)
    {
      return 2;
    }

    return 1;
  }

  /// <summary>
  /// Gets the terminal display width of a plain text string (no ANSI codes).
  /// Uses grapheme cluster enumeration to correctly handle multi-codepoint
  /// sequences like emoji ZWJ sequences and flag emoji.
  /// </summary>
  /// <param name="text">Plain text string (ANSI codes should already be stripped).</param>
  /// <returns>The display width in terminal columns.</returns>
  public static int GetTextWidth(string? text)
  {
    if (string.IsNullOrEmpty(text))
    {
      return 0;
    }

    TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(text);
    int totalWidth = 0;

    while (enumerator.MoveNext())
    {
      string grapheme = enumerator.GetTextElement();

      // Fast path: single ASCII character
      if (grapheme.Length == 1 && grapheme[0] < 128)
      {
        totalWidth += char.IsControl(grapheme[0]) ? 0 : 1;
        continue;
      }

      // Analyze the cluster's runes
      int runeCount = 0;
      int runeWidthSum = 0;
      int regionalIndicatorCount = 0;
      bool hasZeroWidthJoiner = false;
      bool hasEmojiPresentationSelector = false;
      bool hasEmojiModifier = false;

      foreach (Rune rune in grapheme.EnumerateRunes())
      {
        int codePoint = rune.Value;

        if (codePoint is 0x200D)
        {
          hasZeroWidthJoiner = true;
        }
        else if (codePoint is 0xFE0F)
        {
          hasEmojiPresentationSelector = true;
        }
        else if (codePoint is >= 0x1F1E6 and <= 0x1F1FF)
        {
          regionalIndicatorCount++;
        }
        else if (codePoint is >= 0x1F3FB and <= 0x1F3FF)
        {
          hasEmojiModifier = true;
        }

        runeWidthSum += GetRuneWidth(rune);
        runeCount++;
      }

      if (runeCount == 1)
      {
        totalWidth += runeWidthSum;
      }
      else if (hasZeroWidthJoiner || hasEmojiPresentationSelector || hasEmojiModifier)
      {
        // Emoji ZWJ sequence, emoji-presentation (VS16) sequence, or
        // skin-tone modifier sequence: renders as one two-column glyph
        totalWidth += 2;
      }
      else if (runeCount == 2 && regionalIndicatorCount == 2)
      {
        // Regional indicator pair (flag emoji): one two-column glyph
        totalWidth += 2;
      }
      else
      {
        // Other multi-rune clusters, e.g. NFD combining sequences like
        // "e" + U+0301: combining marks are width 0, so the sum yields
        // the base character's width
        totalWidth += runeWidthSum;
      }
    }

    return totalWidth;
  }
}
