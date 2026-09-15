#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test OSC 8 hyperlink functionality

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.Hyperlink
{

  [TestTag("Hyperlinks")]
  public class HyperlinkTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<HyperlinkTests>();

    public static async Task Should_create_hyperlink_with_link_extension()
    {
      // Arrange
      string displayText = "Click here";
      string url = "https://example.com";

      // Act
      string result = displayText.Link(url);

      // Assert - Verify OSC 8 format: \e]8;;URL\e\TEXT\e]8;;\e\
      result.ShouldContain("\x1b]8;;"); // Start sequence
      result.ShouldContain(url);
      result.ShouldContain(displayText);
      result.ShouldEndWith("\x1b]8;;\x1b\\"); // End sequence (empty URL to close)

      await Task.CompletedTask;
    }

    public static async Task Should_create_hyperlink_with_create_link_method()
    {
      // Arrange - CreateLink takes (url, displayText), matching WriteLink's parameter order
      string displayText = "GitHub";
      string url = "https://github.com";

      // Act
      string result = AnsiHyperlinks.CreateLink(url, displayText);

      // Assert
      string expected = $"\x1b]8;;{url}\x1b\\{displayText}\x1b]8;;\x1b\\";
      result.ShouldBe(expected);

      await Task.CompletedTask;
    }

    public static async Task Should_create_link_with_url_first_parameter_order()
    {
      // Regression: CreateLink was changed from (displayText, url) to (url, displayText)
      // to match Terminal.WriteLink and the ITerminal WriteLink/WriteLinkLine extensions
      string result = AnsiHyperlinks.CreateLink("https://x", "text");

      result.ShouldBe("\x1b]8;;https://x\x1b\\text\x1b]8;;\x1b\\");

      await Task.CompletedTask;
    }

    public static async Task Should_use_url_as_display_text_when_create_link_display_text_omitted()
    {
      // Act - displayText defaults to null, which means "use the URL"
      string result = AnsiHyperlinks.CreateLink("https://example.com");

      // Assert
      result.ShouldBe("\x1b]8;;https://example.com\x1b\\https://example.com\x1b]8;;\x1b\\");

      await Task.CompletedTask;
    }

    public static async Task Should_chain_hyperlink_with_color_extensions()
    {
      // Arrange
      string displayText = "Styled Link";
      string url = "https://example.com";

      // Act - Chain Link with color
      string result = displayText.Link(url).Cyan();

      // Assert - Should contain both hyperlink and color codes
      result.ShouldContain("\x1b]8;;"); // OSC 8 start
      result.ShouldContain(url);
      result.ShouldContain(AnsiColors.Cyan); // Color code
      result.ShouldContain(AnsiColors.Reset); // Reset code

      await Task.CompletedTask;
    }

    public static async Task Should_chain_color_then_hyperlink()
    {
      // Arrange
      string displayText = "Colored then linked";
      string url = "https://example.com";

      // Act - Apply color first, then link
      string result = displayText.Blue().Link(url);

      // Assert - The colored text gets wrapped in hyperlink
      result.ShouldContain("\x1b]8;;");
      result.ShouldContain(url);
      result.ShouldContain(AnsiColors.Blue);

      await Task.CompletedTask;
    }

    public static async Task Should_write_link_to_terminal_when_supported()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.SupportsHyperlinks = true;
      string url = "https://docs.microsoft.com";
      string displayText = "Microsoft Docs";

      // Act
      terminal.WriteLink(url, displayText);

      // Assert - Should contain OSC 8 sequences
      terminal.Output.ShouldContain("\x1b]8;;");
      terminal.Output.ShouldContain(url);
      terminal.Output.ShouldContain(displayText);

      await Task.CompletedTask;
    }

    public static async Task Should_write_plain_text_when_hyperlinks_not_supported()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.SupportsHyperlinks = false;
      string url = "https://docs.microsoft.com";
      string displayText = "Microsoft Docs";

      // Act
      terminal.WriteLink(url, displayText);

      // Assert - Should NOT contain OSC 8 sequences, just plain text
      terminal.Output.ShouldNotContain("\x1b]8;;");
      terminal.Output.ShouldBe(displayText);

      await Task.CompletedTask;
    }

    public static async Task Should_write_link_line_with_newline()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.SupportsHyperlinks = true;
      string url = "https://example.com";

      // Act
      terminal.WriteLinkLine(url, "Example");

      // Assert - Should end with newline
      terminal.Output.ShouldEndWith(Environment.NewLine);

      await Task.CompletedTask;
    }

    public static async Task Should_use_url_as_display_text_when_not_specified()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.SupportsHyperlinks = true;
      string url = "https://example.com";

      // Act
      terminal.WriteLink(url); // No display text

      // Assert - URL should appear as display text
      string output = terminal.Output;
      // The URL appears twice: once in the OSC sequence, once as display text
      int urlCount = (output.Length - output.Replace(url, "").Length) / url.Length;
      urlCount.ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_handle_empty_display_text()
    {
      // Arrange
      string displayText = "";
      string url = "https://example.com";

      // Act
      string result = displayText.Link(url);

      // Assert - Should still create valid OSC 8 sequence
      result.ShouldContain("\x1b]8;;");
      result.ShouldContain(url);
      // Display text portion is empty but sequence is valid
      string expected = $"\x1b]8;;{url}\x1b\\\x1b]8;;\x1b\\";
      result.ShouldBe(expected);

      await Task.CompletedTask;
    }

    public static async Task Should_percent_encode_control_characters_in_url()
    {
      // Arrange - URL with embedded ESC and BEL that could terminate the OSC 8 sequence
      string displayText = "Click here";
      string url = "https://example.com/\x1bmalicious\x07path";

      // Act
      string result = AnsiHyperlinks.CreateLink(url, displayText);

      // Assert - ESC and BEL inside the OSC payload must be percent-encoded
      result.ShouldContain("%1B");
      result.ShouldContain("%07");

      // The only raw ESC characters allowed are the OSC 8 framing sequences themselves
      string expected = $"\x1b]8;;https://example.com/%1Bmalicious%07path\x1b\\{displayText}\x1b]8;;\x1b\\";
      result.ShouldBe(expected);

      // The OSC payload (between LinkStart and LinkEnd) must contain no raw ESC or BEL
      int payloadStart = "\x1b]8;;".Length;
      int payloadEnd = result.IndexOf("\x1b\\", payloadStart, StringComparison.Ordinal);
      string payload = result[payloadStart..payloadEnd];
      payload.ShouldNotContain("\x1b");
      payload.ShouldNotContain("\x07");

      await Task.CompletedTask;
    }

    public static async Task Should_percent_encode_c1_string_terminator_in_url()
    {
      // Arrange - C1 ST (U+009C) terminates OSC on ECMA-48 / 8-bit terminals
      string displayText = "Click here";
      string url = "https://example.com/\x9cmalicious";

      // Act
      string result = AnsiHyperlinks.CreateLink(url, displayText);

      // Assert - ST inside the OSC payload must be percent-encoded
      string expected = $"\x1b]8;;https://example.com/%9Cmalicious\x1b\\{displayText}\x1b]8;;\x1b\\";
      result.ShouldBe(expected);

      int payloadStart = "\x1b]8;;".Length;
      int payloadEnd = result.IndexOf("\x1b\\", payloadStart, StringComparison.Ordinal);
      string payload = result[payloadStart..payloadEnd];
      payload.ShouldNotContain("\x9c");
      payload.ShouldContain("%9C");

      await Task.CompletedTask;
    }

    public static async Task Should_percent_encode_full_c1_range_in_url()
    {
      // Arrange - C1 PAD (U+0080), OSC (U+009D), and APC (U+009F)
      string displayText = "Click here";
      string url = "https://example.com/\x80\x9d\x9f";

      // Act
      string result = AnsiHyperlinks.CreateLink(url, displayText);

      // Assert
      string expected = $"\x1b]8;;https://example.com/%80%9D%9F\x1b\\{displayText}\x1b]8;;\x1b\\";
      result.ShouldBe(expected);

      await Task.CompletedTask;
    }

    public static async Task Should_sanitize_url_when_used_as_display_text()
    {
      // Arrange - ESC/BEL in the URL must not appear in the display portion
      string url = "https://example.com/\x1b]0;Hacked\x07";

      // Act
      string result = AnsiHyperlinks.CreateLink(url);

      // Assert - payload and display both use the percent-encoded URL
      string sanitized = "https://example.com/%1B]0;Hacked%07";
      string expected = $"\x1b]8;;{sanitized}\x1b\\{sanitized}\x1b]8;;\x1b\\";
      result.ShouldBe(expected);

      await Task.CompletedTask;
    }

    public static async Task Should_leave_explicit_display_text_unmodified()
    {
      // Arrange - caller-embedded ANSI in displayText is a styled-link carve-out
      string displayText = "Click \x1b[31mhere\x1b[0m";
      string url = "https://example.com/\x1b]0;Hacked\x07";

      // Act
      string result = AnsiHyperlinks.CreateLink(url, displayText);

      // Assert - URL payload sanitized; explicit display text passed through
      string expected = $"\x1b]8;;https://example.com/%1B]0;Hacked%07\x1b\\{displayText}\x1b]8;;\x1b\\";
      result.ShouldBe(expected);

      await Task.CompletedTask;
    }

    public static async Task Should_leave_link_extension_display_text_unmodified()
    {
      string displayText = "Click \x1b[31mhere";
      string url = "https://example.com/\x9c";

      string result = displayText.Link(url);

      string expected = $"\x1b]8;;https://example.com/%9C\x1b\\{displayText}\x1b]8;;\x1b\\";
      result.ShouldBe(expected);

      await Task.CompletedTask;
    }

    public static async Task Should_sanitize_url_as_plain_text_when_hyperlinks_not_supported()
    {
      using TestTerminal terminal = new();
      terminal.SupportsHyperlinks = false;
      string url = "https://example.com/\x1b]0;Hacked\x07";

      terminal.WriteLink(url);

      terminal.Output.ShouldBe("https://example.com/%1B]0;Hacked%07");
      terminal.Output.ShouldNotContain("\x1b");
      terminal.Output.ShouldNotContain("\x07");

      await Task.CompletedTask;
    }

    public static async Task Should_sanitize_url_as_display_text_on_write_link_when_supported()
    {
      using TestTerminal terminal = new();
      terminal.SupportsHyperlinks = true;
      string url = "https://example.com/\x1b]0;Hacked\x07";

      terminal.WriteLink(url);

      string sanitized = "https://example.com/%1B]0;Hacked%07";
      terminal.Output.ShouldBe($"\x1b]8;;{sanitized}\x1b\\{sanitized}\x1b]8;;\x1b\\");

      await Task.CompletedTask;
    }

    public static async Task Should_leave_explicit_display_text_unmodified_on_plain_write_link()
    {
      using TestTerminal terminal = new();
      terminal.SupportsHyperlinks = false;
      string displayText = "Click \x1b[31mhere";
      string url = "https://example.com/\x07";

      terminal.WriteLink(url, displayText);

      terminal.Output.ShouldBe(displayText);

      await Task.CompletedTask;
    }

    public static async Task Should_sanitize_url_as_plain_text_on_write_link_line_when_not_supported()
    {
      using TestTerminal terminal = new();
      terminal.SupportsHyperlinks = false;
      string url = "https://example.com/\x1b]0;Hacked\x07";

      terminal.WriteLinkLine(url);

      terminal.Output.ShouldBe("https://example.com/%1B]0;Hacked%07" + Environment.NewLine);

      await Task.CompletedTask;
    }

    public static async Task Should_leave_normal_url_unchanged()
    {
      // Arrange
      string displayText = "GitHub";
      string url = "https://github.com/TimeWarpEngineering/timewarp-terminal?tab=readme#usage";

      // Act
      string result = AnsiHyperlinks.CreateLink(url, displayText);

      // Assert - URL embedded verbatim, no encoding applied
      string expected = $"\x1b]8;;{url}\x1b\\{displayText}\x1b]8;;\x1b\\";
      result.ShouldBe(expected);

      await Task.CompletedTask;
    }

    public static async Task Should_test_terminal_default_hyperlink_support_is_false()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Assert - TestTerminal defaults to false for SupportsHyperlinks
      terminal.SupportsHyperlinks.ShouldBeFalse();

      await Task.CompletedTask;
    }

    public static async Task Should_write_plain_text_when_facade_hyperlinks_not_supported()
    {
      // Arrange - static facade must honor SupportsHyperlinks like the extension does
      ITerminal original = TimeWarp.Terminal.Terminal.Instance;
      using TestTerminal noLinks = new();  // SupportsHyperlinks defaults to false
      using TestTerminal withLinks = new() { SupportsHyperlinks = true };

      try
      {
        // Act + Assert - unsupported: plain display text, no OSC 8 escapes
        TimeWarp.Terminal.Terminal.Instance = noLinks;
        TimeWarp.Terminal.Terminal.WriteLink("https://example.com", "Example");
        noLinks.Output.ShouldBe("Example");
        noLinks.Output.ShouldNotContain("\u001b");

        // supported: OSC 8 sequence emitted
        TimeWarp.Terminal.Terminal.Instance = withLinks;
        TimeWarp.Terminal.Terminal.WriteLink("https://example.com", "Example");
        withLinks.Output.ShouldContain("\u001b]8;;https://example.com");
        withLinks.Output.ShouldContain("Example");
      }
      finally
      {
        TimeWarp.Terminal.Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_link_line_via_facade_with_newline_and_hyperlink_gating()
    {
      // Regression: Terminal.WriteLinkLine is the newline counterpart of Terminal.WriteLink
      // and must honor SupportsHyperlinks the same way
      ITerminal original = TimeWarp.Terminal.Terminal.Instance;
      using TestTerminal noLinks = new();  // SupportsHyperlinks defaults to false
      using TestTerminal withLinks = new() { SupportsHyperlinks = true };

      try
      {
        // Act + Assert - unsupported: plain display text plus newline, no OSC 8 escapes
        TimeWarp.Terminal.Terminal.Instance = noLinks;
        TimeWarp.Terminal.Terminal.WriteLinkLine("https://example.com", "Example");
        noLinks.Output.ShouldBe("Example" + Environment.NewLine);
        noLinks.Output.ShouldNotContain("\u001b");

        // supported: OSC 8 sequence emitted, still ends with newline
        TimeWarp.Terminal.Terminal.Instance = withLinks;
        TimeWarp.Terminal.Terminal.WriteLinkLine("https://example.com", "Example");
        withLinks.Output.ShouldContain("\u001b]8;;https://example.com");
        withLinks.Output.ShouldContain("Example");
        withLinks.Output.ShouldEndWith(Environment.NewLine);
      }
      finally
      {
        TimeWarp.Terminal.Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.Hyperlink
