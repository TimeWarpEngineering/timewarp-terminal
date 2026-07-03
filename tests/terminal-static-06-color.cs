#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test Terminal static class color overloads
// CA1849: We deliberately test sync methods in async test methods
#pragma warning disable CA1849

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalStaticColor
{

  [TestTag("Terminal")]
  public class TerminalStaticColorTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalStaticColorTests>();

    // ========== Basic Color Output Tests ==========

    public static async Task Should_write_with_foreground_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Write("Red text", ConsoleColor.Red);

        // Assert - ConsoleColor.Red maps to bright red (91)
        testTerminal.Output.ShouldContain(AnsiColors.BrightRed);
        testTerminal.Output.ShouldContain("Red text");
        testTerminal.Output.ShouldContain(AnsiColors.Reset);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_writeline_with_foreground_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteLine("Green text", ConsoleColor.Green);

        // Assert - ConsoleColor.Green maps to bright green (92)
        testTerminal.Output.ShouldContain(AnsiColors.BrightGreen);
        testTerminal.Output.ShouldContain("Green text");
        testTerminal.Output.ShouldContain(AnsiColors.Reset);
        testTerminal.Output.ShouldEndWith(Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_writeline_with_foreground_and_background_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteLine("Colored text", ConsoleColor.White, ConsoleColor.Blue);

        // Assert - White maps to bright white (97), Blue maps to bright blue background (104)
        testTerminal.Output.ShouldContain(AnsiColors.BrightWhite);
        testTerminal.Output.ShouldContain(AnsiColors.BgBrightBlue);
        testTerminal.Output.ShouldContain("Colored text");
        testTerminal.Output.ShouldContain(AnsiColors.Reset);
        testTerminal.Output.ShouldEndWith(Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_error_line_with_foreground_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteErrorLine("Error message", ConsoleColor.Red);

        // Assert - ConsoleColor.Red maps to bright red (91)
        testTerminal.ErrorOutput.ShouldContain(AnsiColors.BrightRed);
        testTerminal.ErrorOutput.ShouldContain("Error message");
        testTerminal.ErrorOutput.ShouldContain(AnsiColors.Reset);
        testTerminal.ErrorOutput.ShouldEndWith(Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Widget Color Tests ==========

    public static async Task Should_writetable_with_colors()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteTable(
          table => table
            .AddColumn("Name")
            .AddColumn("Value")
            .AddRow("Foo", "123"),
          ConsoleColor.White,
          ConsoleColor.DarkBlue);

        // Assert - White maps to bright white (97), DarkBlue maps to dim blue background (44)
        testTerminal.Output.ShouldContain(AnsiColors.BrightWhite);
        testTerminal.Output.ShouldContain(AnsiColors.BgBlue);
        testTerminal.Output.ShouldContain("Foo");
        testTerminal.Output.ShouldContain("123");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_writepanel_with_colors()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WritePanel(
          panel => panel
            .Header("Test Panel")
            .Content("Panel content"),
          ConsoleColor.White,
          ConsoleColor.DarkBlue);

        // Assert - White maps to bright white (97), DarkBlue maps to dim blue background (44)
        testTerminal.Output.ShouldContain(AnsiColors.BrightWhite);
        testTerminal.Output.ShouldContain(AnsiColors.BgBlue);
        testTerminal.Output.ShouldContain("Test Panel");
        testTerminal.Output.ShouldContain("Panel content");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Edge Case Tests ==========

    public static async Task Should_handle_null_message_with_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act - a null message writes plain, exactly like the non-colored overload
        Terminal.Write(null, ConsoleColor.Red);
        Terminal.Write(null, ConsoleColor.Red, ConsoleColor.Blue);
        Terminal.WriteLine(null, ConsoleColor.Red);
        Terminal.WriteLine(null, ConsoleColor.Red, ConsoleColor.Blue);
        Terminal.WriteErrorLine(null, ConsoleColor.Red);
        Terminal.WriteErrorLine(null, ConsoleColor.Red, ConsoleColor.Blue);

        // Assert - only line terminators are written; no escape sequences at all
        testTerminal.AllOutput.ShouldNotContain("\u001b");
        testTerminal.Output.ShouldBe(Environment.NewLine + Environment.NewLine);
        testTerminal.ErrorOutput.ShouldBe(Environment.NewLine + Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_map_all_console_colors_correctly()
    {
      // Arrange - standard mapping: Dark* colors use SGR 30-37/40-47,
      // normal colors use the bright range 90-97/100-107, with the grays crossing over
      Dictionary<ConsoleColor, (string Foreground, string Background)> expected = new()
      {
        [ConsoleColor.Black] = ("\x1b[30m", "\x1b[40m"),
        [ConsoleColor.DarkRed] = ("\x1b[31m", "\x1b[41m"),
        [ConsoleColor.DarkGreen] = ("\x1b[32m", "\x1b[42m"),
        [ConsoleColor.DarkYellow] = ("\x1b[33m", "\x1b[43m"),
        [ConsoleColor.DarkBlue] = ("\x1b[34m", "\x1b[44m"),
        [ConsoleColor.DarkMagenta] = ("\x1b[35m", "\x1b[45m"),
        [ConsoleColor.DarkCyan] = ("\x1b[36m", "\x1b[46m"),
        [ConsoleColor.Gray] = ("\x1b[37m", "\x1b[47m"),
        [ConsoleColor.DarkGray] = ("\x1b[90m", "\x1b[100m"),
        [ConsoleColor.Red] = ("\x1b[91m", "\x1b[101m"),
        [ConsoleColor.Green] = ("\x1b[92m", "\x1b[102m"),
        [ConsoleColor.Yellow] = ("\x1b[93m", "\x1b[103m"),
        [ConsoleColor.Blue] = ("\x1b[94m", "\x1b[104m"),
        [ConsoleColor.Magenta] = ("\x1b[95m", "\x1b[105m"),
        [ConsoleColor.Cyan] = ("\x1b[96m", "\x1b[106m"),
        [ConsoleColor.White] = ("\x1b[97m", "\x1b[107m")
      };

      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act & Assert - verify each ConsoleColor produces its exact ANSI escape code
        foreach (ConsoleColor color in Enum.GetValues<ConsoleColor>())
        {
          AnsiColors.GetForeground(color).ShouldBe(expected[color].Foreground);
          AnsiColors.GetBackground(color).ShouldBe(expected[color].Background);

          testTerminal.ClearOutput();
          Terminal.Write($"Test {color}", color);

          testTerminal.Output.ShouldStartWith(expected[color].Foreground);
          testTerminal.Output.ShouldContain(AnsiColors.Reset);
          testTerminal.Output.ShouldContain($"Test {color}");
        }
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_map_dark_and_normal_colors_to_distinct_sgr_codes()
    {
      // Regression: dark and normal ConsoleColors previously collapsed to the same SGR code
      (ConsoleColor Dark, ConsoleColor Normal)[] pairs =
      [
        (ConsoleColor.DarkRed, ConsoleColor.Red),
        (ConsoleColor.DarkGreen, ConsoleColor.Green),
        (ConsoleColor.DarkYellow, ConsoleColor.Yellow),
        (ConsoleColor.DarkBlue, ConsoleColor.Blue),
        (ConsoleColor.DarkMagenta, ConsoleColor.Magenta),
        (ConsoleColor.DarkCyan, ConsoleColor.Cyan),
        (ConsoleColor.DarkGray, ConsoleColor.Gray),
        (ConsoleColor.Black, ConsoleColor.White)
      ];

      foreach ((ConsoleColor dark, ConsoleColor normal) in pairs)
      {
        AnsiColors.GetForeground(dark).ShouldNotBe(AnsiColors.GetForeground(normal));
        AnsiColors.GetBackground(dark).ShouldNotBe(AnsiColors.GetBackground(normal));
      }

      // The two grays cross over: DarkGray is bright black, Gray is dim white
      AnsiColors.GetForeground(ConsoleColor.DarkGray).ShouldBe("\x1b[90m");
      AnsiColors.GetForeground(ConsoleColor.Gray).ShouldBe("\x1b[37m");

      await Task.CompletedTask;
    }

    public static async Task Should_write_with_foreground_and_background_color()
    {
      // Arrange - Write and WriteErrorLine (message, fg, bg) overloads mirror WriteLine's
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Write("Highlighted", ConsoleColor.Black, ConsoleColor.Yellow);
        Terminal.WriteErrorLine("Fatal", ConsoleColor.White, ConsoleColor.DarkRed);

        // Assert
        testTerminal.Output.ShouldContain(AnsiColors.Black);
        testTerminal.Output.ShouldContain(AnsiColors.BgBrightYellow); // Yellow maps to bright yellow background
        testTerminal.Output.ShouldContain("Highlighted");
        testTerminal.Output.ShouldContain(AnsiColors.Reset);
        testTerminal.Output.ShouldNotEndWith(Environment.NewLine);

        testTerminal.ErrorOutput.ShouldContain(AnsiColors.BrightWhite);
        testTerminal.ErrorOutput.ShouldContain(AnsiColors.BgRed); // DarkRed maps to dim red background
        testTerminal.ErrorOutput.ShouldContain("Fatal");
        testTerminal.ErrorOutput.ShouldContain(AnsiColors.Reset);
        testTerminal.ErrorOutput.ShouldEndWith(Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_reapply_table_color_after_embedded_border_reset()
    {
      // Regression: a BorderColor emits its own Reset inside each rendered line, which used to
      // cancel the requested foreground for everything after the first border segment
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteTable(
          table => table
            .AddColumn("Name")
            .AddRow("Foo")
            .BorderColor(AnsiColors.Yellow),
          ConsoleColor.Cyan);

        // Assert - the foreground code re-appears after each embedded Reset
        testTerminal.Output.ShouldContain(AnsiColors.Yellow);
        testTerminal.Output.ShouldContain(AnsiColors.Reset + AnsiColors.BrightCyan); // Cyan maps to bright cyan
        testTerminal.Output.ShouldContain("Foo");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== SupportsColor Gating Tests ==========

    public static async Task Should_write_plain_text_when_color_not_supported()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { SupportsColor = false };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Write("no color", ConsoleColor.Red);
        Terminal.WriteLine("still none", ConsoleColor.Green, ConsoleColor.Black);
        Terminal.WriteErrorLine("error plain", ConsoleColor.Yellow);

        // Assert - no escape sequences anywhere
        testTerminal.AllOutput.ShouldNotContain("\u001b");
        testTerminal.Output.ShouldContain("no color");
        testTerminal.Output.ShouldContain("still none");
        testTerminal.ErrorOutput.ShouldContain("error plain");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_widgets_plain_when_color_not_supported()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40, SupportsColor = false };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WritePanel("panel content", header: null, foregroundColor: ConsoleColor.White, backgroundColor: ConsoleColor.DarkBlue);
        Terminal.WriteTable(t => t.AddColumn("H").AddRow("V"), ConsoleColor.Cyan);
        testTerminal.WriteTable(t => t.AddColumn("X").AddRow("Y"), ConsoleColor.Red);

        // Assert
        testTerminal.Output.ShouldNotContain("\u001b");
        testTerminal.Output.ShouldContain("panel content");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TerminalStaticColor
