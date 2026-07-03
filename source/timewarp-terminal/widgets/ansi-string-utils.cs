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

  /// <summary>
  /// Wraps text at word boundaries to fit within a specified width, preserving ANSI escape codes.
  /// Visible characters separated only by ANSI codes (no whitespace) are treated as one word.
  /// </summary>
  /// <param name="text">The text to wrap, potentially containing ANSI escape codes.</param>
  /// <param name="maxWidth">The maximum visible width per line.</param>
  /// <returns>A list of wrapped lines with ANSI codes properly carried across line breaks.</returns>
  /// <example>
  /// <code>
  /// string longText = "This is a very long text that needs wrapping";
  /// var lines = AnsiStringUtils.WrapText(longText, 20);
  /// // Returns multiple lines, each with visible length &lt;= 20
  /// </code>
  /// </example>
  public static IReadOnlyList<string> WrapText(string? text, int maxWidth)
  {
    if (string.IsNullOrEmpty(text))
    {
      return [""];
    }

    if (maxWidth < 1)
    {
      maxWidth = 1;
    }

    List<string> result = [];

    // Tokenize into words: whitespace in the visible text ends a word, embedded ANSI codes stay attached
    List<string> words = TokenizeWords(ParseTextSegments(text));

    // Track active state to carry across lines (SGR and hyperlink are independent channels)
    StringBuilder sgrState = new();
    string? hyperlinkState = null;
    StringBuilder currentLine = new();
    int currentLineWidth = 0;

    foreach (string word in words)
    {
      int wordWidth = GetVisibleLength(word);

      if (wordWidth == 0)
      {
        // ANSI-only token (no visible characters) — emit in place, occupies no width
        AppendWord(currentLine, sgrState, ref hyperlinkState, word);
        continue;
      }

      if (currentLineWidth + wordWidth <= maxWidth)
      {
        // Word fits on the current line
        AppendWord(currentLine, sgrState, ref hyperlinkState, word);
        currentLineWidth += wordWidth;
      }
      else if (wordWidth <= maxWidth)
      {
        // Word fits on a line of its own — start a new line
        StartNewLine(result, currentLine, sgrState, hyperlinkState);
        currentLineWidth = 0;

        string trimmedWord = TrimStartVisible(word);
        AppendWord(currentLine, sgrState, ref hyperlinkState, trimmedWord);
        currentLineWidth = GetVisibleLength(trimmedWord);
      }
      else
      {
        // Word is longer than maxWidth — break it by grapheme
        if (currentLineWidth > 0)
        {
          StartNewLine(result, currentLine, sgrState, hyperlinkState);
          currentLineWidth = 0;
        }

        BreakLongWord(word, maxWidth, result, currentLine, ref currentLineWidth, sgrState, ref hyperlinkState);
      }
    }

    // Add the last line if it has content
    if (currentLine.Length > 0)
    {
      result.Add(currentLine.ToString());
    }

    // Ensure we return at least one empty line
    if (result.Count == 0)
    {
      result.Add("");
    }

    return result;
  }

  /// <summary>
  /// Represents a segment of text (either ANSI code or visible text).
  /// </summary>
  private readonly struct TextSegment(string text, bool isAnsiCode)
  {
    public string Text { get; } = text;
    public bool IsAnsiCode { get; } = isAnsiCode;
  }

  /// <summary>
  /// Parses text into segments of ANSI codes and visible text.
  /// </summary>
  private static List<TextSegment> ParseTextSegments(string text)
  {
    List<TextSegment> segments = [];
    int lastEnd = 0;

    foreach (Match match in AnsiRegex().Matches(text))
    {
      // Add any text before this ANSI code
      if (match.Index > lastEnd)
      {
        segments.Add(new TextSegment(text[lastEnd..match.Index], isAnsiCode: false));
      }

      // Add the ANSI code
      segments.Add(new TextSegment(match.Value, isAnsiCode: true));
      lastEnd = match.Index + match.Length;
    }

    // Add any remaining text after the last ANSI code
    if (lastEnd < text.Length)
    {
      segments.Add(new TextSegment(text[lastEnd..], isAnsiCode: false));
    }

    return segments;
  }

  /// <summary>
  /// Splits parsed segments into word tokens. A word ends at each whitespace character in the
  /// visible text (the whitespace is kept as the word's last character); ANSI codes never end a
  /// word, so visible characters separated only by ANSI codes stay in one token.
  /// </summary>
  private static List<string> TokenizeWords(List<TextSegment> segments)
  {
    List<string> words = [];
    StringBuilder current = new();

    foreach (TextSegment segment in segments)
    {
      if (segment.IsAnsiCode)
      {
        _ = current.Append(segment.Text);
        continue;
      }

      foreach (char c in segment.Text)
      {
        _ = current.Append(c);

        if (char.IsWhiteSpace(c))
        {
          // End of word (including trailing whitespace)
          words.Add(current.ToString());
          _ = current.Clear();
        }
      }
    }

    // Add remaining text
    if (current.Length > 0)
    {
      words.Add(current.ToString());
    }

    return words;
  }

  /// <summary>
  /// Appends a word to the current line and updates the active state from any ANSI codes it contains.
  /// </summary>
  private static void AppendWord(StringBuilder currentLine, StringBuilder sgrState, ref string? hyperlinkState, string word)
  {
    _ = currentLine.Append(word);

    if (!word.Contains('\x1b', StringComparison.Ordinal))
    {
      return;
    }

    foreach (Match match in AnsiRegex().Matches(word))
    {
      UpdateAnsiState(sgrState, ref hyperlinkState, match.Value);
    }
  }

  /// <summary>
  /// Finishes the current line (closing any active SGR styling and hyperlink so state does not
  /// bleed past the break), adds it to the result, and seeds the new line with the active state.
  /// </summary>
  private static void StartNewLine(List<string> result, StringBuilder currentLine, StringBuilder sgrState, string? hyperlinkState)
  {
    if (hyperlinkState is not null)
    {
      _ = currentLine.Append(HyperlinkClose);
    }

    if (sgrState.Length > 0)
    {
      _ = currentLine.Append(AnsiColors.Reset);
    }

    result.Add(currentLine.ToString());

    // Re-open active channels on the new line
    _ = currentLine.Clear();
    _ = currentLine.Append(sgrState);

    if (hyperlinkState is not null)
    {
      _ = currentLine.Append(hyperlinkState);
    }
  }

  /// <summary>
  /// Breaks a word wider than <paramref name="maxWidth"/> across lines grapheme by grapheme,
  /// emitting embedded ANSI codes in place and keeping the carry state current.
  /// </summary>
  private static void BreakLongWord
  (
    string word,
    int maxWidth,
    List<string> result,
    StringBuilder currentLine,
    ref int currentLineWidth,
    StringBuilder sgrState,
    ref string? hyperlinkState
  )
  {
    foreach (TextSegment part in ParseTextSegments(word))
    {
      if (part.IsAnsiCode)
      {
        _ = currentLine.Append(part.Text);
        UpdateAnsiState(sgrState, ref hyperlinkState, part.Text);
        continue;
      }

      TextElementEnumerator graphemeEnum = StringInfo.GetTextElementEnumerator(part.Text);
      while (graphemeEnum.MoveNext())
      {
        string grapheme = graphemeEnum.GetTextElement();

        if (grapheme == " " && currentLineWidth == 0)
        {
          continue; // Skip leading spaces on new line
        }

        int graphemeWidth = UnicodeWidth.GetTextWidth(grapheme);

        if (currentLineWidth + graphemeWidth > maxWidth && currentLineWidth > 0)
        {
          StartNewLine(result, currentLine, sgrState, hyperlinkState);
          currentLineWidth = 0;
        }

        _ = currentLine.Append(grapheme);
        currentLineWidth += graphemeWidth;
      }
    }
  }

  /// <summary>
  /// Removes leading whitespace from the visible text of a word while preserving its ANSI codes.
  /// </summary>
  private static string TrimStartVisible(string word)
  {
    if (!word.Contains('\x1b', StringComparison.Ordinal))
    {
      return word.TrimStart();
    }

    StringBuilder trimmed = new(word.Length);
    bool visibleSeen = false;

    foreach (TextSegment part in ParseTextSegments(word))
    {
      if (part.IsAnsiCode)
      {
        _ = trimmed.Append(part.Text);
        continue;
      }

      string visibleText = visibleSeen ? part.Text : part.Text.TrimStart();

      if (visibleText.Length > 0)
      {
        visibleSeen = true;
        _ = trimmed.Append(visibleText);
      }
    }

    return trimmed.ToString();
  }

  /// <summary>
  /// Updates the active line-carry state from an ANSI code. SGR codes accumulate and an SGR reset
  /// clears them; OSC 8 hyperlinks are an independent channel opened by a non-empty URL and
  /// cleared by an empty one (an SGR reset does NOT clear the hyperlink). Other sequences
  /// (cursor movement, erase, titles) are one-shot actions and never carried across lines.
  /// </summary>
  private static void UpdateAnsiState(StringBuilder sgrState, ref string? hyperlinkState, string ansiCode)
  {
    if (ansiCode is "\x1b[0m" or "\x1b[m")
    {
      // SGR reset clears styling only — the hyperlink channel is independent
      _ = sgrState.Clear();
    }
    else if (ansiCode.StartsWith("\x1b]8;", StringComparison.Ordinal))
    {
      // Non-empty URL opens a hyperlink; empty URL closes it
      string url = ExtractHyperlinkUrl(ansiCode);
      hyperlinkState = url.Length == 0 ? null : ansiCode;
    }
    else if (ansiCode.StartsWith("\x1b[", StringComparison.Ordinal) && ansiCode.EndsWith('m'))
    {
      // SGR styling (colors, bold, ...) accumulates
      _ = sgrState.Append(ansiCode);
    }

    // Everything else (other CSI, other OSC, two-byte ESC sequences) is not style state
  }

  /// <summary>
  /// Extracts the URL from an OSC 8 hyperlink sequence (ESC ] 8 ; params ; URL, terminated by BEL or ST).
  /// </summary>
  private static string ExtractHyperlinkUrl(string ansiCode)
  {
    int terminatorLength = 0;

    if (ansiCode.EndsWith("\x1b\\", StringComparison.Ordinal))
    {
      terminatorLength = 2;
    }
    else if (ansiCode.EndsWith('\x07'))
    {
      terminatorLength = 1;
    }

    // URL starts after the ';' that ends the params section ("\x1b]8;" is 4 characters)
    int separatorIndex = ansiCode.IndexOf(';', 4);

    if (separatorIndex < 0 || separatorIndex + 1 > ansiCode.Length - terminatorLength)
    {
      return string.Empty;
    }

    return ansiCode[(separatorIndex + 1)..^terminatorLength];
  }
}
