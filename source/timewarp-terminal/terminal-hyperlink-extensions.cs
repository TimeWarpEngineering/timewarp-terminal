// CA1054: OSC 8 hyperlinks use raw URL strings by design for ergonomic API
#pragma warning disable CA1054

namespace TimeWarp.Terminal;

#region Purpose
// ITerminal WriteLink / WriteLinkLine extensions: OSC 8 when supported, else plain display text.
#endregion

#region Design
// Both branches share AnsiHyperlinks.SanitizeUrl. CreateLink sanitizes the OSC payload and,
// when displayText is omitted, reuses the sanitized URL as display text. The
// !SupportsHyperlinks path uses ResolveDisplayText so a raw URL fallback cannot emit
// C0/DEL/C1. Explicit displayText is never rewritten (styled-link carve-out). Static
// Terminal.WriteLink* require non-null text and therefore never take this fallback.
#endregion

/// <summary>
/// Extension methods for writing hyperlinks to an <see cref="ITerminal"/>.
/// </summary>
/// <example>
/// <code>
/// // Write a clickable link
/// terminal.WriteLink("https://github.com", "GitHub");
///
/// // Write a link with newline
/// terminal.WriteLinkLine("https://example.com", "Click here");
///
/// // Conditionally use hyperlinks based on terminal support
/// if (terminal.SupportsHyperlinks)
///     terminal.WriteLinkLine("https://docs.com", "View documentation");
/// else
///     terminal.WriteLine("View documentation at https://docs.com");
/// </code>
/// </example>
public static class TerminalHyperlinkExtensions
{
  /// <summary>
  /// Writes a hyperlink to the terminal without a trailing newline.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="url">The URL to link to.</param>
  /// <param name="displayText">The text to display. If null, the sanitized URL is used as display text.</param>
  /// <remarks>
  /// If the terminal does not support hyperlinks (<see cref="ITerminal.SupportsHyperlinks"/> is false),
  /// only the display text is written without the hyperlink escape sequences.
  /// </remarks>
  public static ITerminal WriteLink(this ITerminal terminal, string url, string? displayText = null)
  {
    ArgumentNullException.ThrowIfNull(terminal);
    ArgumentNullException.ThrowIfNull(url);

    if (terminal.SupportsHyperlinks)
    {
      _ = terminal.Write(AnsiHyperlinks.CreateLink(url, displayText));
    }
    else
    {
      string text = AnsiHyperlinks.ResolveDisplayText(url, displayText);
      _ = terminal.Write(text);
    }

    return terminal;
  }

  /// <summary>
  /// Writes a hyperlink to the terminal followed by a newline.
  /// </summary>
  /// <param name="terminal">The terminal to write to.</param>
  /// <param name="url">The URL to link to.</param>
  /// <param name="displayText">The text to display. If null, the sanitized URL is used as display text.</param>
  /// <returns>The terminal instance for fluent chaining.</returns>
  /// <remarks>
  /// If the terminal does not support hyperlinks (<see cref="ITerminal.SupportsHyperlinks"/> is false),
  /// only the display text is written without the hyperlink escape sequences.
  /// </remarks>
  public static ITerminal WriteLinkLine(this ITerminal terminal, string url, string? displayText = null)
  {
    ArgumentNullException.ThrowIfNull(terminal);
    ArgumentNullException.ThrowIfNull(url);

    if (terminal.SupportsHyperlinks)
    {
      _ = terminal.WriteLine(AnsiHyperlinks.CreateLink(url, displayText));
    }
    else
    {
      string text = AnsiHyperlinks.ResolveDisplayText(url, displayText);
      _ = terminal.WriteLine(text);
    }

    return terminal;
  }
}
