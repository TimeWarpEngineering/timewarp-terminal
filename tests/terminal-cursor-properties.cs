#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test cursor properties on ITerminal

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalCursorProperties
{

  [TestTag("Terminal")]
  public class TerminalCursorPropertyTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalCursorPropertyTests>();

    public static async Task Should_get_and_set_cursor_left()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.CursorLeft = 5;

      // Assert
      terminal.CursorLeft.ShouldBe(5);

      await Task.CompletedTask;
    }

    public static async Task Should_get_and_set_cursor_top()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.CursorTop = 10;

      // Assert
      terminal.CursorTop.ShouldBe(10);

      await Task.CompletedTask;
    }

    public static async Task Should_get_and_set_cursor_visible()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.CursorVisible = false;

      // Assert
      terminal.CursorVisible.ShouldBeFalse();

      await Task.CompletedTask;
    }

    public static async Task Should_default_cursor_visible_to_true()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.CursorVisible.ShouldBeTrue();

      await Task.CompletedTask;
    }

    public static async Task Should_get_and_set_cursor_size()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.CursorSize = 50;

      // Assert
      terminal.CursorSize.ShouldBe(50);

      await Task.CompletedTask;
    }

    public static async Task Should_default_cursor_size_to_100()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.CursorSize.ShouldBe(100);

      await Task.CompletedTask;
    }

    public static async Task Should_reject_cursor_size_below_1()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act & Assert
      Should.Throw<ArgumentOutOfRangeException>(() => terminal.CursorSize = 0);

      await Task.CompletedTask;
    }

    public static async Task Should_reject_cursor_size_above_100()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act & Assert
      Should.Throw<ArgumentOutOfRangeException>(() => terminal.CursorSize = 101);

      await Task.CompletedTask;
    }

    public static async Task Should_accept_cursor_size_at_minimum_1()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.CursorSize = 1;

      // Assert
      terminal.CursorSize.ShouldBe(1);

      await Task.CompletedTask;
    }

    public static async Task Should_accept_cursor_size_at_maximum_100()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.CursorSize = 100;

      // Assert
      terminal.CursorSize.ShouldBe(100);

      await Task.CompletedTask;
    }

    public static async Task Should_set_cursor_position_still_works()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.SetCursorPosition(15, 25);

      // Assert
      terminal.CursorLeft.ShouldBe(15);
      terminal.CursorTop.ShouldBe(25);

      await Task.CompletedTask;
    }

    public static async Task Should_get_cursor_position_still_works()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.SetCursorPosition(20, 30);

      // Act
      (int left, int top) = terminal.GetCursorPosition();

      // Assert
      left.ShouldBe(20);
      top.ShouldBe(30);

      await Task.CompletedTask;
    }

    public static async Task Should_cursor_properties_sync_with_position_methods()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act - Set via property, get via method
      terminal.CursorLeft = 12;
      terminal.CursorTop = 8;
      (int left, int top) = terminal.GetCursorPosition();

      // Assert
      left.ShouldBe(12);
      top.ShouldBe(8);

      // Act - Set via method, get via property
      terminal.SetCursorPosition(5, 3);

      // Assert
      terminal.CursorLeft.ShouldBe(5);
      terminal.CursorTop.ShouldBe(3);

      await Task.CompletedTask;
    }

    public static async Task Should_get_cursor_position_atomically_from_real_terminal()
    {
      // Arrange - TimeWarpTerminal uses the atomic Console.GetCursorPosition()
      TimeWarpTerminal terminal = TimeWarpTerminal.Default;

      // Act - must not throw, even when the console is redirected (falls back to (0, 0))
      (int left, int top) = Should.NotThrow(terminal.GetCursorPosition);

      // Assert
      left.ShouldBeGreaterThanOrEqualTo(0);
      top.ShouldBeGreaterThanOrEqualTo(0);

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TerminalCursorProperties
