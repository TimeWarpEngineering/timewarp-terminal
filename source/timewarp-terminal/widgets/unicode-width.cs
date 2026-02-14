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
      return 0;

    UnicodeCategory category = Rune.GetUnicodeCategory(rune);

    // Zero-width categories
    if (category is UnicodeCategory.NonSpacingMark
        or UnicodeCategory.EnclosingMark
        or UnicodeCategory.Format)
      return 0;

    // Explicit zero-width code points
    if (value is 0x00AD) // Soft hyphen
      return 0;
    if (value is 0x200B or 0x200C or 0x200D or 0x2060) // ZWSP, ZWNJ, ZWJ, WJ
      return 0;
    if (value is >= 0xFE00 and <= 0xFE0F) // Variation selectors VS1-VS16
      return 0;
    if (value is >= 0xE0100 and <= 0xE01EF) // Variation selectors supplement
      return 0;

    // CJK Unified Ideographs and extensions
    if (value is >= 0x4E00 and <= 0x9FFF)
      return 2;
    if (value is >= 0x3400 and <= 0x4DBF)
      return 2;
    if (value is >= 0x20000 and <= 0x2A6DF)
      return 2;
    if (value is >= 0x2A700 and <= 0x2B73F)
      return 2;
    if (value is >= 0x2B740 and <= 0x2B81F)
      return 2;

    // CJK Compatibility Ideographs
    if (value is >= 0xF900 and <= 0xFAFF)
      return 2;
    if (value is >= 0x2F800 and <= 0x2FA1F)
      return 2;

    // CJK Radicals, Kangxi, Ideographic Description
    if (value is >= 0x2E80 and <= 0x303E)
      return 2;

    // Hiragana, Katakana, Bopomofo, CJK Compatibility
    if (value is >= 0x3041 and <= 0x33BF)
      return 2;

    // Fullwidth forms
    if (value is >= 0xFF01 and <= 0xFF60)
      return 2;
    if (value is >= 0xFFE0 and <= 0xFFE6)
      return 2;

    // Hangul
    if (value is >= 0xAC00 and <= 0xD7A3)
      return 2;
    if (value is >= 0x1100 and <= 0x115F)
      return 2;

    // Wide angle brackets
    if (value is >= 0x2329 and <= 0x232A)
      return 2;

    // Miscellaneous Symbols and Dingbats (✅❌⚡⭐☀⚽ etc.)
    // Most render as width 2 in modern terminals
    if (value is >= 0x2600 and <= 0x27BF)
      return 2;

    // Miscellaneous Technical emoji (⌚⌛⏰⏳ etc.)
    if (value is >= 0x2300 and <= 0x23FF)
      return 2;

    // Geometric Shapes with emoji presentation (▶◀ etc.)
    if (value is >= 0x25A0 and <= 0x25FF)
      return 2;

    // Emoji blocks
    if (value is >= 0x1F000 and <= 0x1FAFF)
      return 2;
    if (value is >= 0x1FC00 and <= 0x1FFFF)
      return 2;

    // Regional indicator symbols (flag emoji components)
    if (value is >= 0x1F1E0 and <= 0x1F1FF)
      return 2;

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
      return 0;

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
          firstRuneWidth = GetRuneWidth(rune);
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
