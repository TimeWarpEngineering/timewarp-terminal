#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test IConsole rich input APIs (Read, ReadKey)
#pragma warning disable CA1859 // Intentionally testing interface access

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.RichInput
{

  [TestTag("IConsole")]
  [TestTag("Input")]
  public class ConsoleReadBasicTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<ConsoleReadBasicTests>();

    public static async Task Should_return_single_character_from_queue()
    {
      // Arrange
      using TestConsole console = new();
      console.QueueCharacters("A");

      // Act
      int result = console.Read();

      // Assert
      result.ShouldBe('A');

      await Task.CompletedTask;
    }

    public static async Task Should_return_negative_one_when_queue_empty()
    {
      // Arrange
      using TestConsole console = new();

      // Act
      int result = console.Read();

      // Assert
      result.ShouldBe(-1);

      await Task.CompletedTask;
    }

    public static async Task Should_return_characters_in_fifo_order()
    {
      // Arrange
      using TestConsole console = new();
      console.QueueCharacters("ABC");

      // Act & Assert
      console.Read().ShouldBe('A');
      console.Read().ShouldBe('B');
      console.Read().ShouldBe('C');
      console.Read().ShouldBe(-1);

      await Task.CompletedTask;
    }

    public static async Task Should_throw_notsupported_for_readkey_in_testconsole()
    {
      // Arrange
      using TestConsole console = new();

      // Act & Assert
      NotSupportedException exception = Should.Throw<NotSupportedException>(() => console.ReadKey());
      exception.Message.ShouldContain("TestConsole does not support key-by-key input");

      await Task.CompletedTask;
    }

    public static async Task Should_access_read_via_iconsole_interface()
    {
      // Arrange
      using TestConsole console = new();
      IConsole iconsole = console;
      console.QueueCharacters("X");

      // Act
      int result = iconsole.Read();

      // Assert
      result.ShouldBe('X');

      await Task.CompletedTask;
    }
  }

  [TestTag("ITerminal")]
  [TestTag("Input")]
  public class TerminalReadBasicTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalReadBasicTests>();

    public static async Task Should_return_character_from_key_queue()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKeys("H");

      // Act
      int result = terminal.Read();

      // Assert
      result.ShouldBe('H');

      await Task.CompletedTask;
    }

    public static async Task Should_return_negative_one_when_key_queue_empty()
    {
      // Arrange
      using TestTerminal terminal = new();

      // Act
      int result = terminal.Read();

      // Assert
      result.ShouldBe(-1);

      await Task.CompletedTask;
    }

    public static async Task Should_return_characters_from_queued_keys_in_order()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKeys("abc");

      // Act & Assert
      terminal.Read().ShouldBe('a');
      terminal.Read().ShouldBe('b');
      terminal.Read().ShouldBe('c');
      terminal.Read().ShouldBe(-1);

      await Task.CompletedTask;
    }

    public static async Task Should_read_key_without_intercept_parameter()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKey(ConsoleKey.Enter);

      // Act
      ConsoleKeyInfo keyInfo = terminal.ReadKey();

      // Assert
      keyInfo.Key.ShouldBe(ConsoleKey.Enter);

      await Task.CompletedTask;
    }

    public static async Task Should_read_key_with_intercept_true()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKey(ConsoleKey.Tab);

      // Act
      ConsoleKeyInfo keyInfo = terminal.ReadKey(true);

      // Assert
      keyInfo.Key.ShouldBe(ConsoleKey.Tab);

      await Task.CompletedTask;
    }

    public static async Task Should_read_key_with_intercept_false()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKey(ConsoleKey.Escape);

      // Act
      ConsoleKeyInfo keyInfo = terminal.ReadKey(false);

      // Assert
      keyInfo.Key.ShouldBe(ConsoleKey.Escape);

      await Task.CompletedTask;
    }

    public static async Task Should_access_read_via_iconsole_interface()
    {
      // Arrange
      using TestTerminal terminal = new();
      IConsole iconsole = terminal;
      terminal.QueueKeys("Z");

      // Act
      int result = iconsole.Read();

      // Assert
      result.ShouldBe('Z');

      await Task.CompletedTask;
    }

    public static async Task Should_access_readkey_via_iconsole_interface()
    {
      // Arrange
      using TestTerminal terminal = new();
      IConsole iconsole = terminal;
      terminal.QueueKey(ConsoleKey.Spacebar);

      // Act
      ConsoleKeyInfo keyInfo = iconsole.ReadKey();

      // Assert
      keyInfo.Key.ShouldBe(ConsoleKey.Spacebar);

      await Task.CompletedTask;
    }

    public static async Task Should_dequeue_key_on_read()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKey(ConsoleKey.A);
      terminal.QueueKey(ConsoleKey.B);

      // Act
      ConsoleKeyInfo first = terminal.ReadKey();
      ConsoleKeyInfo second = terminal.ReadKey();

      // Assert
      first.Key.ShouldBe(ConsoleKey.A);
      second.Key.ShouldBe(ConsoleKey.B);

      await Task.CompletedTask;
    }

    public static async Task Should_return_key_char_from_read()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKeys("X");

      // Act
      int result = terminal.Read();

      // Assert
      result.ShouldBe('X');

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.RichInput
