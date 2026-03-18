#!/usr/bin/dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test Terminal static facade class - Operations
// CA1849: We deliberately test sync methods in async test methods
// IDE0008: Allow var for brevity in tests
#pragma warning disable CA1849, IDE0008

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalStaticOperations
{

[TestTag("Terminal")]
public class TerminalStaticOperationTests
{
  [ModuleInitializer]
  internal static void Register() => RegisterTests<TerminalStaticOperationTests>();

  public static async Task Should_clear_terminal()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    Terminal.Instance = testTerminal;
    Terminal.WriteLine("Some content");

    try
    {
      // Act
      Terminal.Clear();

      // Assert - TestTerminal writes [CLEAR] marker
      testTerminal.Output.ShouldContain("[CLEAR]");
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_set_cursor_position()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      Terminal.SetCursorPosition(10, 5);

      // Assert
      var position = testTerminal.GetCursorPosition();
      position.Left.ShouldBe(10);
      position.Top.ShouldBe(5);
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_get_cursor_position()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    Terminal.Instance = testTerminal;
    testTerminal.SetCursorPosition(15, 20);

    try
    {
      // Act
      var position = Terminal.GetCursorPosition();

      // Assert
      position.Left.ShouldBe(15);
      position.Top.ShouldBe(20);
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_read_line_from_instance()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new("user input");
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      string? result = Terminal.ReadLine();

      // Assert
      result.ShouldBe("user input");
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_read_key_from_instance()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    testTerminal.QueueKeyInfo(new ConsoleKeyInfo('A', ConsoleKey.A, false, false, false));
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      ConsoleKeyInfo key = Terminal.ReadKey();

      // Assert
      key.Key.ShouldBe(ConsoleKey.A);
      key.KeyChar.ShouldBe('A');
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_read_key_with_intercept()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    testTerminal.QueueKeyInfo(new ConsoleKeyInfo('B', ConsoleKey.B, false, false, false));
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      ConsoleKeyInfo key = Terminal.ReadKey(intercept: true);

      // Assert
      key.Key.ShouldBe(ConsoleKey.B);
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_return_null_when_readline_has_no_input()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new(); // Empty input
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      string? result = Terminal.ReadLine();

      // Assert
      result.ShouldBeNull();
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }
}

} // namespace TimeWarp.Terminal.Tests.Core.TerminalStaticOperations
