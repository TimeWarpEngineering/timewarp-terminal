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

    // ========== WindowWidth Tests ==========

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

    // ========== ITerminal Getter Tests ==========

    public static async Task Should_expose_geometry_via_iterminal_getters()
    {
      // Arrange
      using TestTerminal terminal = new()
      {
        WindowWidth = 100,
        WindowHeight = 30,
        BufferWidth = 120,
        BufferHeight = 400
      };
#pragma warning disable CA1859 // Intentionally testing ITerminal interface access
      ITerminal iterminal = terminal;
#pragma warning restore CA1859

      // Act & Assert
      iterminal.WindowWidth.ShouldBe(100);
      iterminal.WindowHeight.ShouldBe(30);
      iterminal.BufferWidth.ShouldBe(120);
      iterminal.BufferHeight.ShouldBe(400);

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TerminalWindowBufferGeometry
