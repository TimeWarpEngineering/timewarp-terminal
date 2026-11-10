#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

#region Purpose
// Terminal cursor and window-geometry facade tests.
#endregion
#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalStaticGeometry
{

  [TestTag("Terminal")]
  public class TerminalStaticGeometryTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalStaticGeometryTests>();

    // ========== Cursor Properties Tests (ITerminal - task 020-004) ==========

    public static async Task Should_get_cursor_left()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { CursorLeft = 10 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        int left = Terminal.CursorLeft;

        // Assert
        left.ShouldBe(10);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_cursor_left()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.CursorLeft = 20;

        // Assert
        testTerminal.CursorLeft.ShouldBe(20);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_cursor_top()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { CursorTop = 5 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        int top = Terminal.CursorTop;

        // Assert
        top.ShouldBe(5);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_cursor_top()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.CursorTop = 15;

        // Assert
        testTerminal.CursorTop.ShouldBe(15);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_cursor_visible()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { CursorVisible = false };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        bool visible = Terminal.CursorVisible;

        // Assert
        visible.ShouldBeFalse();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_cursor_visible()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.CursorVisible = false;

        // Assert
        testTerminal.CursorVisible.ShouldBeFalse();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Window/Buffer Geometry Tests (ITerminal - task 020-005) ==========

    public static async Task Should_get_window_height()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowHeight = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        int height = Terminal.WindowHeight;

        // Assert
        height.ShouldBe(40);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_window_height_after_test_terminal_change()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        testTerminal.WindowHeight = 50;

        // Assert
        Terminal.WindowHeight.ShouldBe(50);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_window_width()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 150 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        int width = Terminal.WindowWidth;

        // Assert
        width.ShouldBe(150);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_buffer_width()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { BufferWidth = 200 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        int width = Terminal.BufferWidth;

        // Assert
        width.ShouldBe(200);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_buffer_width_after_test_terminal_change()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        testTerminal.BufferWidth = 250;

        // Assert
        Terminal.BufferWidth.ShouldBe(250);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_buffer_height()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { BufferHeight = 500 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        int height = Terminal.BufferHeight;

        // Assert
        height.ShouldBe(500);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_buffer_height_after_test_terminal_change()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        testTerminal.BufferHeight = 600;

        // Assert
        Terminal.BufferHeight.ShouldBe(600);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

  }

}
