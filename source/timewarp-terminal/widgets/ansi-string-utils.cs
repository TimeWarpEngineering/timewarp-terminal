namespace TimeWarp.Terminal;

#region Purpose
// ANSI-aware string utilities: strip, measure, pad, center, and word-wrap text containing escape codes.
#endregion

#region Design
// AnsiRegex covers the full ANSI escape surface, not just SGR: CSI with any final byte (so cursor
// movement, erase, and private modes are invisible to measurement), any OSC command terminated by
// BEL or ST (terminator optional so truncated sequences still strip), and two-byte ESC sequences
// with final byte 0x40-0x5F excluding '[' and ']' which open CSI/OSC. [GeneratedRegex] keeps the
// library AOT-compatible (IsAotCompatible).
// WrapText tokenizes on whitespace in the VISIBLE text only: ANSI codes embedded in a word stay
// attached to it, so a styled word like "un" + red("break") + "able" wraps as one unit.
// Line-carry state has two independent channels: accumulated SGR codes (cleared by SGR reset) and
// the active OSC 8 hyperlink (cleared by an empty-URL OSC 8; NOT cleared by SGR reset). Only SGR
// and OSC 8 are re-emitted on wrapped lines; one-shot sequences (cursor movement, erase, titles)
// are never carried across line breaks.
// Truncation helpers reuse the same state channels: TakeVisibleFromStart keeps embedded codes in
// place and closes any open hyperlink/SGR at the cut so styling never bleeds into what follows
// (padding, borders, ellipsis); TakeVisibleFromEnd replays the state established by the discarded
// prefix (SGR then hyperlink) before the kept tail so a color opened before the cut still styles
// it. Both cut on grapheme boundaries and never split a wide (two-column) grapheme — a straddling
// grapheme is dropped, so the result may be one column short; callers pad to the exact width.
// Word wrap lives in ansi-string-utils.wrap.cs (same type).
#endregion

/// <summary>
/// Utility methods for working with strings containing ANSI escape codes.
/// </summary>
public static partial class AnsiStringUtils
{
  /// <summary>
  /// OSC 8 sequence that terminates the active hyperlink.
  /// </summary>
  private const string HyperlinkClose = "\x1b]8;;\x1b\\";

  /// <summary>
  /// Source-generated regex matching ANSI escape sequences:
  /// - CSI: ESC [ + parameter bytes (0x30-0x3F) + intermediate bytes (0x20-0x2F) + one final byte
  ///   (0x40-0x7E), e.g. \x1b[31m, \x1b[38;5;214m, \x1b[2K, \x1b[?25l, \x1b[2A
  /// - OSC: ESC ] + content terminated by BEL (\x07) or ST (ESC \); the terminator is optional so
  ///   truncated sequences still strip, e.g. \x1b]8;;URL\x1b\ hyperlinks, \x1b]0;title\x07
  /// - Two-byte ESC sequences: ESC + one final byte 0x40-0x5F excluding '[' and ']' (which open
  ///   CSI/OSC), e.g. \x1bD (index), \x1bM (reverse index), standalone ESC \ (ST)
  /// </summary>
  [GeneratedRegex(@"\x1b\[[0-\x3f]*[ -/]*[@-~]|\x1b\][^\x07\x1b]*(?:\x07|\x1b\\)?|\x1b[@-Z\\^_]")]
  private static partial Regex AnsiRegex();

  /// <summary>
  /// Removes all ANSI escape codes from a string.
  /// </summary>
  /// <param name="text">The text potentially containing ANSI escape codes.</param>
  /// <returns>The text with all ANSI escape codes removed.</returns>
  /// <example>
  /// <code>
  /// string styled = "\x1b[31mError\x1b[0m";
  /// string plain = AnsiStringUtils.StripAnsiCodes(styled);
  /// // plain == "Error"
  /// </code>
  /// </example>
  public static string StripAnsiCodes(string? text)
  {
    if (string.IsNullOrEmpty(text))
    {
      return string.Empty;
    }

    return AnsiRegex().Replace(text, string.Empty);
  }

  /// <summary>
  /// Gets the visible length of a string, excluding ANSI escape codes.
  /// </summary>
  /// <param name="text">The text potentially containing ANSI escape codes.</param>
  /// <returns>The number of visible characters (excluding ANSI codes).</returns>
  /// <example>
  /// <code>
  /// string styled = "\x1b[31mError\x1b[0m";
  /// int length = AnsiStringUtils.GetVisibleLength(styled);
  /// // length == 5 (for "Error")
  /// </code>
  /// </example>
  public static int GetVisibleLength(string? text)
  {
    if (string.IsNullOrEmpty(text))
    {
      return 0;
    }

    return UnicodeWidth.GetTextWidth(StripAnsiCodes(text));
  }

  /// <summary>
  /// Pads a string to a specified length, accounting for ANSI codes.
  /// </summary>
  /// <param name="text">The text to pad.</param>
  /// <param name="totalWidth">The desired total visible width. Negative values are clamped to 0.</param>
  /// <param name="paddingChar">The character to use for padding (default is space).</param>
  /// <returns>
  /// The padded string with original ANSI codes preserved. Text already at or beyond
  /// <paramref name="totalWidth"/> is returned unchanged.
  /// </returns>
  public static string PadRightVisible(string? text, int totalWidth, char paddingChar = ' ')
  {
    if (totalWidth < 0)
    {
      totalWidth = 0;
    }

    if (string.IsNullOrEmpty(text))
    {
      return new string(paddingChar, totalWidth);
    }

    int visibleLength = GetVisibleLength(text);
    if (visibleLength >= totalWidth)
    {
      return text;
    }

    return text + new string(paddingChar, totalWidth - visibleLength);
  }

  /// <summary>
  /// Pads a string on the left to a specified length, accounting for ANSI codes.
  /// </summary>
  /// <param name="text">The text to pad.</param>
  /// <param name="totalWidth">The desired total visible width. Negative values are clamped to 0.</param>
  /// <param name="paddingChar">The character to use for padding (default is space).</param>
  /// <returns>
  /// The padded string with original ANSI codes preserved. Text already at or beyond
  /// <paramref name="totalWidth"/> is returned unchanged.
  /// </returns>
  public static string PadLeftVisible(string? text, int totalWidth, char paddingChar = ' ')
  {
    if (totalWidth < 0)
    {
      totalWidth = 0;
    }

    if (string.IsNullOrEmpty(text))
    {
      return new string(paddingChar, totalWidth);
    }

    int visibleLength = GetVisibleLength(text);
    if (visibleLength >= totalWidth)
    {
      return text;
    }

    return new string(paddingChar, totalWidth - visibleLength) + text;
  }

  /// <summary>
  /// Centers a string within a specified width, accounting for ANSI codes.
  /// </summary>
  /// <param name="text">The text to center.</param>
  /// <param name="totalWidth">The desired total visible width. Negative values are clamped to 0.</param>
  /// <param name="paddingChar">The character to use for padding (default is space).</param>
  /// <returns>
  /// The centered string with original ANSI codes preserved. Text already at or beyond
  /// <paramref name="totalWidth"/> is returned unchanged.
  /// </returns>
  public static string CenterVisible(string? text, int totalWidth, char paddingChar = ' ')
  {
    if (totalWidth < 0)
    {
      totalWidth = 0;
    }

    if (string.IsNullOrEmpty(text))
    {
      return new string(paddingChar, totalWidth);
    }

    int visibleLength = GetVisibleLength(text);
    if (visibleLength >= totalWidth)
    {
      return text;
    }

    int totalPadding = totalWidth - visibleLength;
    int leftPadding = totalPadding / 2;
    int rightPadding = totalPadding - leftPadding;

    return new string(paddingChar, leftPadding) + text + new string(paddingChar, rightPadding);
  }

  /// <summary>
  /// Truncates a string to a maximum visible width, preserving embedded ANSI escape codes.
  /// If the text already fits, it is returned unchanged. The cut is grapheme-aware and never
  /// splits a wide (two-column) grapheme: if the last column would split one, the cut happens
  /// before it and the result is one column short (callers pad to the exact width). If SGR
  /// styling or an OSC 8 hyperlink is active at the cut point, closing sequences are appended
  /// so styling does not bleed past the truncated text.
  /// </summary>
  /// <param name="text">The text to truncate, potentially containing ANSI escape codes.</param>
  /// <param name="maxWidth">The maximum visible width in terminal columns.</param>
  /// <returns>The text truncated to at most <paramref name="maxWidth"/> visible columns.</returns>
  public static string TruncateVisible(string? text, int maxWidth)
  {
    if (string.IsNullOrEmpty(text) || maxWidth <= 0)
    {
      return string.Empty;
    }

    if (GetVisibleLength(text) <= maxWidth)
    {
      return text;
    }

    return TakeVisibleFromStart(text, maxWidth);
  }

  /// <summary>
  /// Takes up to <paramref name="maxColumns"/> visible columns from the start of the text,
  /// keeping embedded ANSI codes in their original positions. Never splits a wide grapheme.
  /// If the text is cut while SGR styling or a hyperlink is active, closing sequences are
  /// appended so the style does not bleed past the kept portion.
  /// </summary>
  internal static string TakeVisibleFromStart(string text, int maxColumns)
  {
    StringBuilder result = new();
    StringBuilder sgrState = new();
    string? hyperlinkState = null;
    int width = 0;
    bool cut = false;

    foreach (TextSegment segment in ParseTextSegments(text))
    {
      if (cut)
      {
        break;
      }

      if (segment.IsAnsiCode)
      {
        _ = result.Append(segment.Text);
        UpdateAnsiState(sgrState, ref hyperlinkState, segment.Text);
        continue;
      }

      TextElementEnumerator graphemeEnum = StringInfo.GetTextElementEnumerator(segment.Text);
      while (graphemeEnum.MoveNext())
      {
        string grapheme = graphemeEnum.GetTextElement();
        int graphemeWidth = UnicodeWidth.GetTextWidth(grapheme);

        if (width + graphemeWidth > maxColumns)
        {
          cut = true;
          break;
        }

        _ = result.Append(grapheme);
        width += graphemeWidth;
      }
    }

    if (cut)
    {
      if (hyperlinkState is not null)
      {
        _ = result.Append(HyperlinkClose);
      }

      if (sgrState.Length > 0)
      {
        _ = result.Append(AnsiColors.Reset);
      }
    }

    return result.ToString();
  }

  /// <summary>
  /// Takes up to <paramref name="maxColumns"/> visible columns from the end of the text,
  /// keeping embedded ANSI codes in their original positions within the kept span. Never
  /// splits a wide grapheme. SGR styling and any hyperlink established by the discarded
  /// prefix are replayed before the kept tail so it renders with its original style.
  /// </summary>
  internal static string TakeVisibleFromEnd(string text, int maxColumns)
  {
    List<TextSegment> segments = ParseTextSegments(text);

    // First pass: collect grapheme widths to find the grapheme-aligned keep width from the end
    List<int> graphemeWidths = [];
    int totalWidth = 0;

    foreach (TextSegment segment in segments)
    {
      if (segment.IsAnsiCode)
      {
        continue;
      }

      TextElementEnumerator graphemeEnum = StringInfo.GetTextElementEnumerator(segment.Text);
      while (graphemeEnum.MoveNext())
      {
        int graphemeWidth = UnicodeWidth.GetTextWidth(graphemeEnum.GetTextElement());
        graphemeWidths.Add(graphemeWidth);
        totalWidth += graphemeWidth;
      }
    }

    if (totalWidth <= maxColumns)
    {
      return text;
    }

    int keepWidth = 0;
    for (int i = graphemeWidths.Count - 1; i >= 0; i--)
    {
      if (keepWidth + graphemeWidths[i] > maxColumns)
      {
        break;
      }

      keepWidth += graphemeWidths[i];
    }

    int skipWidth = totalWidth - keepWidth;

    // Second pass: fold discarded-prefix codes into carry state, emit the kept span verbatim
    StringBuilder kept = new();
    StringBuilder sgrState = new();
    string? hyperlinkState = null;
    int accumulated = 0;

    foreach (TextSegment segment in segments)
    {
      if (segment.IsAnsiCode)
      {
        if (accumulated < skipWidth)
        {
          UpdateAnsiState(sgrState, ref hyperlinkState, segment.Text);
        }
        else
        {
          _ = kept.Append(segment.Text);
        }

        continue;
      }

      TextElementEnumerator graphemeEnum = StringInfo.GetTextElementEnumerator(segment.Text);
      while (graphemeEnum.MoveNext())
      {
        string grapheme = graphemeEnum.GetTextElement();

        if (accumulated < skipWidth)
        {
          accumulated += UnicodeWidth.GetTextWidth(grapheme);
        }
        else
        {
          _ = kept.Append(grapheme);
        }
      }
    }

    StringBuilder result = new();
    _ = result.Append(sgrState);

    if (hyperlinkState is not null)
    {
      _ = result.Append(hyperlinkState);
    }

    _ = result.Append(kept);

    return result.ToString();
  }

}
