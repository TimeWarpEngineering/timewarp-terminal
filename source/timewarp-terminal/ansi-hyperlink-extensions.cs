// CA1822: Analyzer bug - doesn't recognize C# 14 extension block receiver 'text' as instance data
#pragma warning disable CA1822
// CA1054: OSC 8 hyperlinks use raw URL strings by design for ergonomic fluent API
#pragma warning disable CA1054

namespace TimeWarp.Terminal;

#region Purpose
// OSC 8 hyperlink primitives: escape-sequence constants, CreateLink, and the string.Link extension.
#endregion

#region Design
// CreateLink takes (url, displayText) — the parameter order was aligned with Terminal.WriteLink
// and the ITerminal WriteLink/WriteLinkLine extensions for 1.0 so the same two strings read in
// the same order everywhere; displayText is optional and defaults to the URL.
// URLs are sanitized (C0 controls and DEL percent-encoded) before embedding so an
// attacker-influenced URL cannot terminate the OSC 8 sequence and inject escape sequences.
#endregion

/// <summary>
/// Constants for OSC 8 hyperlink escape sequences.
/// </summary>
public static class AnsiHyperlinks
{
  /// <summary>
  /// OSC 8 escape sequence to start a hyperlink.
  /// Format: \e]8;;{URL}\e\
  /// </summary>
  internal const string LinkStart = "\x1b]8;;";

  /// <summary>
  /// OSC 8 escape sequence to end a hyperlink.
  /// </summary>
  internal const string LinkEnd = "\x1b\\";

  /// <summary>
  /// Creates an OSC 8 hyperlink string.
  /// </summary>
  /// <param name="url">The URL to link to.</param>
  /// <param name="displayText">The text to display. If null, the URL is used as display text.</param>
  /// <returns>An OSC 8 formatted hyperlink string.</returns>
  /// <remarks>
  /// The URL is sanitized before being embedded in the escape sequence:
  /// every C0 control character (below U+0020) and DEL (U+007F) is percent-encoded
  /// as %XX (uppercase hexadecimal). This prevents characters such as ESC or BEL in an
  /// attacker-influenced URL from terminating the OSC 8 sequence and injecting
  /// arbitrary terminal escape sequences. Normal URLs pass through unchanged.
  /// The display text is outside the OSC payload and is not modified.
  /// </remarks>
  public static string CreateLink(string url, string? displayText = null)
    => $"{LinkStart}{SanitizeUrl(url)}{LinkEnd}{displayText ?? url}{LinkStart}{LinkEnd}";

  /// <summary>
  /// Percent-encodes every C0 control character (below U+0020) and DEL (U+007F)
  /// in the URL as %XX (uppercase hexadecimal) so the URL cannot prematurely
  /// terminate the OSC 8 escape sequence. Returns the original string when no
  /// encoding is required.
  /// </summary>
  /// <param name="url">The URL to sanitize.</param>
  /// <returns>The sanitized URL, safe to embed in an OSC 8 payload.</returns>
  private static string SanitizeUrl(string url)
  {
    ArgumentNullException.ThrowIfNull(url);

    StringBuilder? builder = null;

    for (int i = 0; i < url.Length; i++)
    {
      char c = url[i];

      if (c is < '\x20' or '\x7f')
      {
        builder ??= new StringBuilder(url.Length + 8).Append(url, 0, i);
        _ = builder.Append('%').Append(((int)c).ToString("X2", CultureInfo.InvariantCulture));
      }
      else
      {
        _ = builder?.Append(c);
      }
    }

    return builder?.ToString() ?? url;
  }
}

/// <summary>
/// Fluent extension methods for creating OSC 8 hyperlinks in terminal output.
/// OSC 8 hyperlinks are supported by Windows Terminal, iTerm2, VS Code terminal,
/// Hyper, Konsole, and GNOME Terminal 3.26+.
/// </summary>
/// <example>
/// <code>
/// // Create a clickable link
/// "Click here".Link("https://example.com")
///
/// // Chain with color extensions
/// "Visit Ardalis.com".Link("https://ardalis.com").Cyan().Bold()
/// </code>
/// </example>
public static class AnsiHyperlinkExtensions
{
  extension(string text)
  {
    /// <summary>
    /// Wraps the text in an OSC 8 hyperlink sequence.
    /// In supported terminals, the text becomes a clickable link.
    /// In unsupported terminals, only the display text is shown.
    /// </summary>
    /// <param name="url">The URL to link to.</param>
    /// <returns>The text wrapped in OSC 8 hyperlink escape sequences.</returns>
    /// <remarks>
    /// The URL is sanitized before being embedded: C0 control characters (below U+0020)
    /// and DEL (U+007F) are percent-encoded as %XX (uppercase hexadecimal) to prevent
    /// terminal escape injection. Normal URLs pass through unchanged.
    /// </remarks>
    /// <example>
    /// <code>
    /// // Simple hyperlink
    /// "GitHub".Link("https://github.com")
    ///
    /// // With styling
    /// "Download".Link("https://example.com/file.zip").Blue().Underline()
    /// </code>
    /// </example>
    public string Link(string url) => AnsiHyperlinks.CreateLink(url, text);
  }
}
