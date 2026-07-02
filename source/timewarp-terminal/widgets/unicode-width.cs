namespace TimeWarp.Terminal;

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
    // Hiragana, Katakana, Bopomofo, CJK Compatibility
    if (value is >= 0x3041 and <= 0x33BF)
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

    // Mahjong, Dominos, Playing Cards, Enclosed Alphanumerics Supplement
    if (value is >= 0x1F000 and <= 0x1FAFF)
    {
      return 2;
    }

    if (value is >= 0x1FC00 and <= 0x1FFFF)
    {
      return 2;
    }

    // Regional indicator symbols (flag emoji components)
    if (value is >= 0x1F1E0 and <= 0x1F1FF)
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

      // Count runes and check for wide ones
      int runeCount = 0;
      int firstRuneWidth = 0;

      foreach (Rune rune in grapheme.EnumerateRunes())
      {
        if (runeCount == 0)
        {
          firstRuneWidth = GetRuneWidth(rune);
        }

        runeCount++;
      }

      if (runeCount > 1)
      {
        // Multi-codepoint grapheme cluster (ZWJ sequences, flags, skin-tone, keycaps)
        // These render as a single glyph taking 2 columns in terminals
        totalWidth += 2;
      }
      else
      {
        totalWidth += firstRuneWidth;
      }
    }

    return totalWidth;
  }
}
