#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test window and buffer geometry properties on ITerminal

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalWindowBufferGeometry
{

  [TestTag("Terminal")]
  public class TerminalWindowBufferGeometryTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalWindowBufferGeometryTests>();

    // ========== WindowHeight Tests ==========

    public static async Task Should_get_and_set_window_height()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.WindowHeight = 40;

      // Assert
      terminal.WindowHeight.ShouldBe(40);

      await Task.CompletedTask;
    }

    public static async Task Should_default_window_height_to_24()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.WindowHeight.ShouldBe(24);

      await Task.CompletedTask;
    }

    // ========== WindowLeft Tests ==========

    public static async Task Should_get_and_set_window_left()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.WindowLeft = 10;

      // Assert
      terminal.WindowLeft.ShouldBe(10);

      await Task.CompletedTask;
    }

    public static async Task Should_default_window_left_to_0()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.WindowLeft.ShouldBe(0);

      await Task.CompletedTask;
    }

    // ========== WindowTop Tests ==========

    public static async Task Should_get_and_set_window_top()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.WindowTop = 15;

      // Assert
      terminal.WindowTop.ShouldBe(15);

      await Task.CompletedTask;
    }

    public static async Task Should_default_window_top_to_0()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.WindowTop.ShouldBe(0);

      await Task.CompletedTask;
    }

    // ========== BufferWidth Tests ==========

    public static async Task Should_get_and_set_buffer_width()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.BufferWidth = 120;

      // Assert
      terminal.BufferWidth.ShouldBe(120);

      await Task.CompletedTask;
    }

    public static async Task Should_default_buffer_width_to_80()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.BufferWidth.ShouldBe(80);

      await Task.CompletedTask;
    }

    // ========== BufferHeight Tests ==========

    public static async Task Should_get_and_set_buffer_height()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.BufferHeight = 500;

      // Assert
      terminal.BufferHeight.ShouldBe(500);

      await Task.CompletedTask;
    }

    public static async Task Should_default_buffer_height_to_300()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.BufferHeight.ShouldBe(300);

      await Task.CompletedTask;
    }

    // ========== SetWindowSize Tests ==========

    public static async Task Should_set_window_size()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.SetWindowSize(100, 30);

      // Assert
      terminal.WindowWidth.ShouldBe(100);
      terminal.WindowHeight.ShouldBe(30);

      await Task.CompletedTask;
    }

    // ========== SetWindowPosition Tests ==========

    public static async Task Should_set_window_position()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.SetWindowPosition(5, 10);

      // Assert
      terminal.WindowLeft.ShouldBe(5);
      terminal.WindowTop.ShouldBe(10);

      await Task.CompletedTask;
    }

    // ========== SetBufferSize Tests ==========

    public static async Task Should_set_buffer_size()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.SetBufferSize(120, 400);

      // Assert
      terminal.BufferWidth.ShouldBe(120);
      terminal.BufferHeight.ShouldBe(400);

      await Task.CompletedTask;
    }

    // ========== MoveBufferArea Tests ==========

    public static async Task Should_call_move_buffer_area()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.MoveBufferArea
      (
        sourceLeft: 0,
        sourceTop: 0,
        sourceWidth: 10,
        sourceHeight: 5,
        targetLeft: 20,
        targetTop: 10,
        sourceChar: ' ',
        sourceForeColor: ConsoleColor.Gray,
        sourceBackColor: ConsoleColor.Black
      );

      // Assert
      terminal.MoveBufferAreaCallCount.ShouldBe(1);

      await Task.CompletedTask;
    }

    public static async Task Should_track_multiple_move_buffer_area_calls()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.MoveBufferArea(0, 0, 10, 5, 20, 10, ' ', ConsoleColor.Gray, ConsoleColor.Black);
      terminal.MoveBufferArea(5, 5, 15, 10, 30, 20, ' ', ConsoleColor.Gray, ConsoleColor.Black);
      terminal.MoveBufferArea(10, 10, 20, 15, 40, 30, ' ', ConsoleColor.Gray, ConsoleColor.Black);

      // Assert
      terminal.MoveBufferAreaCallCount.ShouldBe(3);

      await Task.CompletedTask;
    }

    // ========== LargestWindowWidth Tests ==========

    public static async Task Should_get_largest_window_width()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.LargestWindowWidth = 200;

      // Act
      int width = terminal.LargestWindowWidth;

      // Assert
      width.ShouldBe(200);

      await Task.CompletedTask;
    }

    public static async Task Should_default_largest_window_width_to_120()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.LargestWindowWidth.ShouldBe(120);

      await Task.CompletedTask;
    }

    // ========== LargestWindowHeight Tests ==========

    public static async Task Should_get_largest_window_height()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.LargestWindowHeight = 60;

      // Act
      int height = terminal.LargestWindowHeight;

      // Assert
      height.ShouldBe(60);

      await Task.CompletedTask;
    }

    public static async Task Should_default_largest_window_height_to_40()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.LargestWindowHeight.ShouldBe(40);

      await Task.CompletedTask;
    }

    // ========== WindowWidth Tests (now has setter) ==========

    public static async Task Should_get_and_set_window_width()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      terminal.WindowWidth = 100;

      // Assert
      terminal.WindowWidth.ShouldBe(100);

      await Task.CompletedTask;
    }

    public static async Task Should_default_window_width_to_80()
    {
      // Arrange & Act
      using TestTerminal terminal = new();

      // Assert
      terminal.WindowWidth.ShouldBe(80);

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TerminalWindowBufferGeometry
