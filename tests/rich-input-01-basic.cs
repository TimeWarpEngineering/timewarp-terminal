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

    public static async Task Should_declare_readkey_on_iterminal_not_iconsole()
    {
      // ReadKey is interactive-terminal functionality; it moved from IConsole to
      // ITerminal for 1.0 so stream-oriented consoles are not forced to throw.
      typeof(IConsole).GetMethod("ReadKey", Type.EmptyTypes).ShouldBeNull();
      typeof(ITerminal).GetMethod("ReadKey", Type.EmptyTypes).ShouldNotBeNull();

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

    public static async Task Should_access_readkey_via_iterminal_interface()
    {
      // Arrange
      using TestTerminal terminal = new();
      ITerminal iterminal = terminal;
      terminal.QueueKey(ConsoleKey.Spacebar);

      // Act
      ConsoleKeyInfo keyInfo = iterminal.ReadKey();

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

    public static async Task Should_read_constructor_input_when_key_queue_empty()
    {
      // Arrange
      using TestTerminal terminal = new("abc");

      // Act & Assert
      terminal.Read().ShouldBe('a');
      terminal.Read().ShouldBe('b');
      terminal.Read().ShouldBe('c');
      terminal.Read().ShouldBe(-1);

      await Task.CompletedTask;
    }

    public static async Task Should_interleave_read_with_readline_on_constructor_input()
    {
      // Arrange
      using TestTerminal terminal = new("abc\ndef");

      // Act & Assert
      terminal.Read().ShouldBe('a');
      terminal.ReadLine().ShouldBe("bc");
      terminal.ReadLine().ShouldBe("def");
      terminal.Read().ShouldBe(-1);

      await Task.CompletedTask;
    }

    public static async Task Should_prefer_key_queue_over_constructor_input_on_read()
    {
      // Arrange
      using TestTerminal terminal = new("y");
      terminal.QueueKeys("x");

      // Act & Assert
      terminal.Read().ShouldBe('x');
      terminal.Read().ShouldBe('y');
      terminal.Read().ShouldBe(-1);

      await Task.CompletedTask;
    }

    public static async Task Should_produce_uppercase_key_char_when_queue_key_shift()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKey(ConsoleKey.A, shift: true);

      // Act
      ConsoleKeyInfo keyInfo = terminal.ReadKey();

      // Assert
      keyInfo.Key.ShouldBe(ConsoleKey.A);
      keyInfo.KeyChar.ShouldBe('A');
      keyInfo.Modifiers.ShouldBe(ConsoleModifiers.Shift);

      await Task.CompletedTask;
    }

    public static async Task Should_produce_control_char_when_queue_key_ctrl()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKey(ConsoleKey.C, ctrl: true);

      // Act
      ConsoleKeyInfo keyInfo = terminal.ReadKey();

      // Assert
      keyInfo.Key.ShouldBe(ConsoleKey.C);
      keyInfo.KeyChar.ShouldBe('\u0003');
      keyInfo.Modifiers.ShouldBe(ConsoleModifiers.Control);

      await Task.CompletedTask;
    }

    public static async Task Should_set_shift_flag_when_queue_keys_uppercase()
    {
      // Arrange
      using TestTerminal terminal = new();
      terminal.QueueKeys("Ab");

      // Act
      ConsoleKeyInfo upper = terminal.ReadKey();
      ConsoleKeyInfo lower = terminal.ReadKey();

      // Assert
      upper.Key.ShouldBe(ConsoleKey.A);
      upper.KeyChar.ShouldBe('A');
      upper.Modifiers.ShouldBe(ConsoleModifiers.Shift);
      lower.Key.ShouldBe(ConsoleKey.B);
      lower.KeyChar.ShouldBe('b');
      lower.Modifiers.ShouldBe((ConsoleModifiers)0);

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.RichInput
