#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test Terminal static facade class - Properties

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalStaticProperties
{

  [TestTag("Terminal")]
  public class TerminalStaticPropertyTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalStaticPropertyTests>();

    public static async Task Should_get_window_width_from_instance()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 120 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        int width = Terminal.WindowWidth;

        // Assert
        width.ShouldBe(120);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_is_interactive_from_instance()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { IsInteractive = true };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        bool interactive = Terminal.IsInteractive;

        // Assert
        interactive.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_supports_color_from_instance()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { SupportsColor = true };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        bool supportsColor = Terminal.SupportsColor;

        // Assert
        supportsColor.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_supports_hyperlinks_from_instance()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { SupportsHyperlinks = true };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        bool supportsHyperlinks = Terminal.SupportsHyperlinks;

        // Assert
        supportsHyperlinks.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_default_supports_color_to_true_in_test_terminal()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act & Assert - TestTerminal defaults to SupportsColor = true
        Terminal.SupportsColor.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_default_supports_hyperlinks_to_false_in_test_terminal()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act & Assert
        Terminal.SupportsHyperlinks.ShouldBeFalse();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_require_both_stdin_and_stdout_for_is_interactive()
    {
      // Arrange
      TimeWarpTerminal terminal = TimeWarpTerminal.Default;

      // Act
      bool interactive = terminal.IsInteractive;

      // Assert - interactive requires BOTH stdin and stdout attached to a terminal
      // (e.g., `app | tee` redirects stdout only and must not report interactive)
      bool expected = !Console.IsInputRedirected && !Console.IsOutputRedirected;
      interactive.ShouldBe(expected);

      await Task.CompletedTask;
    }

    public static async Task Should_disable_color_when_no_color_is_non_empty()
    {
      // Arrange
      string? original = Environment.GetEnvironmentVariable("NO_COLOR");

      try
      {
        Environment.SetEnvironmentVariable("NO_COLOR", "1");

        // Act & Assert - a non-empty NO_COLOR disables color regardless of terminal state
        TimeWarpTerminal.Default.SupportsColor.ShouldBeFalse();
      }
      finally
      {
        Environment.SetEnvironmentVariable("NO_COLOR", original);
      }

      await Task.CompletedTask;
    }

    public static async Task Should_disable_color_when_term_is_dumb()
    {
      // Arrange
      string? original = Environment.GetEnvironmentVariable("TERM");

      try
      {
        Environment.SetEnvironmentVariable("TERM", "dumb");

        // Act & Assert
        TimeWarpTerminal.Default.SupportsColor.ShouldBeFalse();
      }
      finally
      {
        Environment.SetEnvironmentVariable("TERM", original);
      }

      await Task.CompletedTask;
    }

    public static async Task Should_not_throw_for_treat_control_c_as_input_without_console()
    {
      // Arrange
      TimeWarpTerminal terminal = TimeWarpTerminal.Default;

      // Act & Assert - getter and setter must follow the swallow-and-default policy
      bool value = Should.NotThrow(() => terminal.TreatControlCAsInput);
      Should.NotThrow(() => terminal.TreatControlCAsInput = value);

      if (Console.IsInputRedirected)
      {
        // With no console attached the getter defaults to false
        value.ShouldBeFalse();
      }

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TerminalStaticProperties
