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

        // Assert
        testTerminal.Output.ShouldContain(AnsiColors.Red);
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

        // Assert
        testTerminal.Output.ShouldContain(AnsiColors.Green);
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

        // Assert
        testTerminal.Output.ShouldContain(AnsiColors.White);
        testTerminal.Output.ShouldContain(AnsiColors.BgBlue);
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

        // Assert
        testTerminal.ErrorOutput.ShouldContain(AnsiColors.Red);
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

        // Assert
        testTerminal.Output.ShouldContain(AnsiColors.White);
        testTerminal.Output.ShouldContain(AnsiColors.BgBlue); // DarkBlue maps to BgBlue
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

        // Assert
        testTerminal.Output.ShouldContain(AnsiColors.White);
        testTerminal.Output.ShouldContain(AnsiColors.BgBlue); // DarkBlue maps to BgBlue
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
        // Act
        Terminal.WriteLine(null, ConsoleColor.Red);

        // Assert
        testTerminal.Output.ShouldContain(AnsiColors.Red);
        testTerminal.Output.ShouldContain(AnsiColors.Reset);
        testTerminal.Output.ShouldNotContain("null");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_map_all_console_colors_correctly()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act & Assert - verify each ConsoleColor produces an ANSI escape sequence
        foreach (ConsoleColor color in Enum.GetValues<ConsoleColor>())
        {
          testTerminal.ClearOutput();
          Terminal.Write($"Test {color}", color);

          // All outputs should contain escape sequence start and reset
          testTerminal.Output.ShouldContain("\x1b[");
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
