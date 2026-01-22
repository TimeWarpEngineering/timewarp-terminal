#!/usr/bin/dotnet --
#:project ../source/timewarp-terminal/timewarp-terminal.csproj

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
}

} // namespace TimeWarp.Terminal.Tests.Core.TerminalStaticProperties
