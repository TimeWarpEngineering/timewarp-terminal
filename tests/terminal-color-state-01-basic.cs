#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test color state properties on ITerminal

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalColorState
{

  [TestTag("Terminal")]
  public class TerminalColorStateTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalColorStateTests>();

    public static async Task Should_get_and_set_foreground_color()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.ForegroundColor = ConsoleColor.Red;

      // Assert
      terminal.ForegroundColor.ShouldBe(ConsoleColor.Red);

      await Task.CompletedTask;
    }

    public static async Task Should_get_and_set_background_color()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.BackgroundColor = ConsoleColor.Blue;

      // Assert
      terminal.BackgroundColor.ShouldBe(ConsoleColor.Blue);

      await Task.CompletedTask;
    }

    public static async Task Should_default_foreground_color_to_gray()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.ForegroundColor.ShouldBe(ConsoleColor.Gray);

      await Task.CompletedTask;
    }

    public static async Task Should_default_background_color_to_black()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.BackgroundColor.ShouldBe(ConsoleColor.Black);

      await Task.CompletedTask;
    }

    public static async Task Should_reset_color_reset_foreground_to_gray()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.ForegroundColor = ConsoleColor.Green;

      // Act
      terminal.ResetColor();

      // Assert
      terminal.ForegroundColor.ShouldBe(ConsoleColor.Gray);

      await Task.CompletedTask;
    }

    public static async Task Should_reset_color_reset_background_to_black()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.BackgroundColor = ConsoleColor.Yellow;

      // Act
      terminal.ResetColor();

      // Assert
      terminal.BackgroundColor.ShouldBe(ConsoleColor.Black);

      await Task.CompletedTask;
    }

    public static async Task Should_reset_color_reset_both_colors()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.ForegroundColor = ConsoleColor.Cyan;
      terminal.BackgroundColor = ConsoleColor.Magenta;

      // Act
      terminal.ResetColor();

      // Assert
      terminal.ForegroundColor.ShouldBe(ConsoleColor.Gray);
      terminal.BackgroundColor.ShouldBe(ConsoleColor.Black);

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TerminalColorState
