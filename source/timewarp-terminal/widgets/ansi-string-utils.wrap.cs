namespace TimeWarp.Terminal;

#region Purpose
// Word wrap that carries SGR and OSC 8 state across lines.
#endregion

#region Design
// Same type as ansi-string-utils.cs. Wrap reads the private ANSI regex and hyperlink closer.
#endregion

public static partial class AnsiStringUtils
{
  /// <summary>
  /// Wraps text at word boundaries to fit within a specified width, preserving ANSI escape codes.
  /// Visible characters separated only by ANSI codes (no whitespace) are treated as one word.
  /// </summary>
  /// <param name="text">The text to wrap, potentially containing ANSI escape codes.</param>
  /// <param name="maxWidth">The maximum visible width per line.</param>
  /// <returns>A list of wrapped lines with ANSI codes properly carried across line breaks.</returns>
  /// <remarks>
  /// A grapheme wider than <paramref name="maxWidth"/> is dropped (same as <see cref="TruncateVisible"/>),
  /// so every returned line's visible width is at most <paramref name="maxWidth"/>
  /// (after the existing maxWidth &lt; 1 → 1 clamp).
  /// </remarks>
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
  /// <remarks>
  /// A grapheme wider than <paramref name="maxWidth"/> is dropped (same as <see cref="TruncateVisible"/>).
  /// </remarks>
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

        if (graphemeWidth > maxWidth)
        {
          continue;
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
