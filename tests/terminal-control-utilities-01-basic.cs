#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test control/utility APIs on ITerminal (Beep, TreatControlCAsInput, Title, KeyAvailable)

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalControlUtilities
{

  [TestTag("Terminal")]
  public class TerminalControlUtilityTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalControlUtilityTests>();

    public static async Task Should_beep_increment_count()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.Beep();

      // Assert
      terminal.BeepCount.ShouldBe(1);

      await Task.CompletedTask;
    }

    public static async Task Should_beep_multiple_times_increment_count()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.Beep();
      terminal.Beep();
      terminal.Beep();

      // Assert
      terminal.BeepCount.ShouldBe(3);

      await Task.CompletedTask;
    }

    public static async Task Should_beep_with_parameters_capture_frequency_and_duration()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.Beep(800, 200);

      // Assert
      terminal.LastBeepFrequency.ShouldBe(800);
      terminal.LastBeepDuration.ShouldBe(200);
      terminal.BeepCount.ShouldBe(1);

      await Task.CompletedTask;
    }

    public static async Task Should_beep_with_parameters_overwrite_previous()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.Beep(500, 100);
      terminal.Beep(1000, 300);

      // Assert
      terminal.LastBeepFrequency.ShouldBe(1000);
      terminal.LastBeepDuration.ShouldBe(300);
      terminal.BeepCount.ShouldBe(2);

      await Task.CompletedTask;
    }

    public static async Task Should_treat_control_c_as_input_default_false()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.TreatControlCAsInput.ShouldBeFalse();

      await Task.CompletedTask;
    }

    public static async Task Should_treat_control_c_as_input_get_and_set()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.TreatControlCAsInput = true;

      // Assert
      terminal.TreatControlCAsInput.ShouldBeTrue();

      // Act
      terminal.TreatControlCAsInput = false;

      // Assert
      terminal.TreatControlCAsInput.ShouldBeFalse();

      await Task.CompletedTask;
    }

    public static async Task Should_title_default_empty_string()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.Title.ShouldBe(string.Empty);

      await Task.CompletedTask;
    }

    public static async Task Should_title_get_and_set()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.Title = "My App";

      // Assert
      terminal.Title.ShouldBe("My App");

      // Act
      terminal.Title = "Different Title";

      // Assert
      terminal.Title.ShouldBe("Different Title");

      await Task.CompletedTask;
    }

    public static async Task Should_key_available_false_when_no_keys_queued()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Assert
      terminal.KeyAvailable.ShouldBeFalse();

      await Task.CompletedTask;
    }

    public static async Task Should_key_available_true_when_keys_queued()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKey(ConsoleKey.A);

      // Assert
      terminal.KeyAvailable.ShouldBeTrue();

      await Task.CompletedTask;
    }

    public static async Task Should_key_available_false_after_keys_dequeued()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKey(ConsoleKey.A);

      // Act
      terminal.ReadKey();

      // Assert
      terminal.KeyAvailable.ShouldBeFalse();

      await Task.CompletedTask;
    }

    public static async Task Should_key_available_true_with_multiple_keys()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKeys("abc");

      // Assert
      terminal.KeyAvailable.ShouldBeTrue();

      // Act - read one key
      terminal.ReadKey();

      // Assert - still true
      terminal.KeyAvailable.ShouldBeTrue();

      // Act - read remaining keys
      terminal.ReadKey();
      terminal.ReadKey();

      // Assert - now false
      terminal.KeyAvailable.ShouldBeFalse();

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TerminalControlUtilities
