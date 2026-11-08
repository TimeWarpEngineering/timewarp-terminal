#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Terminal color-state and control-utility facade tests.
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

    // ========== Color State Tests (ITerminal - task 020-006) ==========

    public static async Task Should_get_foreground_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { ForegroundColor = ConsoleColor.Cyan };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        ConsoleColor color = Terminal.ForegroundColor;

        // Assert
        color.ShouldBe(ConsoleColor.Cyan);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_foreground_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.ForegroundColor = ConsoleColor.Magenta;

        // Assert
        testTerminal.ForegroundColor.ShouldBe(ConsoleColor.Magenta);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_background_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { BackgroundColor = ConsoleColor.DarkBlue };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        ConsoleColor color = Terminal.BackgroundColor;

        // Assert
        color.ShouldBe(ConsoleColor.DarkBlue);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_background_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.BackgroundColor = ConsoleColor.DarkGreen;

        // Assert
        testTerminal.BackgroundColor.ShouldBe(ConsoleColor.DarkGreen);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_reset_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;
      testTerminal.ForegroundColor = ConsoleColor.Red;
      testTerminal.BackgroundColor = ConsoleColor.Blue;

      try
      {
        // Act
        Terminal.ResetColor();

        // Assert
        testTerminal.ForegroundColor.ShouldBe(ConsoleColor.Gray);
        testTerminal.BackgroundColor.ShouldBe(ConsoleColor.Black);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Control/Utility Tests (ITerminal - task 020-007) ==========

    public static async Task Should_beep()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Beep();

        // Assert
        testTerminal.BeepCount.ShouldBe(1);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_beep_with_frequency_and_duration()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Beep(800, 200);

        // Assert
        testTerminal.BeepCount.ShouldBe(1);
        testTerminal.LastBeepFrequency.ShouldBe(800);
        testTerminal.LastBeepDuration.ShouldBe(200);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_treat_control_c_as_input()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { TreatControlCAsInput = true };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        bool treatAsInput = Terminal.TreatControlCAsInput;

        // Assert
        treatAsInput.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_treat_control_c_as_input()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.TreatControlCAsInput = true;

        // Assert
        testTerminal.TreatControlCAsInput.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_title()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { Title = "Test Title" };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        string title = Terminal.Title;

        // Assert
        title.ShouldBe("Test Title");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_title()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Title = "New Title";

        // Assert
        testTerminal.Title.ShouldBe("New Title");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_key_available()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;
      testTerminal.QueueKey(ConsoleKey.A);

      try
      {
        // Act
        bool available = Terminal.KeyAvailable;

        // Assert
        available.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_key_available_false_when_empty()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        bool available = Terminal.KeyAvailable;

        // Assert
        available.ShouldBeFalse();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

  }

}
