#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test Terminal static facade class - New APIs from tasks 020-001 through 020-007
using System.Text;

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalStaticNewApis
{

  [TestTag("Terminal")]
  public class TerminalStaticNewApisTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalStaticNewApisTests>();

    // ========== Stream Access Tests (IConsole - task 020-001) ==========

    public static async Task Should_open_standard_input()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Stream stream = Terminal.OpenStandardInput();

        // Assert
        stream.ShouldBe(testTerminal.StandardInputStream);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_open_standard_output()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Stream stream = Terminal.OpenStandardOutput();

        // Assert
        stream.ShouldBe(testTerminal.StandardOutputStream);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_open_standard_error()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Stream stream = Terminal.OpenStandardError();

        // Assert
        stream.ShouldBe(testTerminal.StandardErrorStream);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_in_reader()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        TextReader reader = Terminal.In;

        // Assert
        reader.ShouldBe(testTerminal.In);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_out_writer()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        TextWriter writer = Terminal.Out;

        // Assert
        writer.ShouldBe(testTerminal.Out);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_error_writer()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        TextWriter writer = Terminal.Error;

        // Assert
        writer.ShouldBe(testTerminal.Error);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_in_reader()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;
      StringReader newReader = new("test");

      try
      {
        // Act
        Terminal.SetIn(newReader);

        // Assert
        testTerminal.In.ShouldBe(newReader);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_out_writer()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;
      StringWriter newWriter = new();

      try
      {
        // Act
        Terminal.SetOut(newWriter);

        // Assert
        testTerminal.Out.ShouldBe(newWriter);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_error_writer()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;
      StringWriter newWriter = new();

      try
      {
        // Act
        Terminal.SetError(newWriter);

        // Assert
        testTerminal.Error.ShouldBe(newWriter);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Encoding Tests (IConsole - task 020-002) ==========

    public static async Task Should_get_input_encoding()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { InputEncoding = Encoding.ASCII };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Encoding encoding = Terminal.InputEncoding;

        // Assert
        encoding.ShouldBe(Encoding.ASCII);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_input_encoding()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.InputEncoding = Encoding.Unicode;

        // Assert
        testTerminal.InputEncoding.ShouldBe(Encoding.Unicode);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_output_encoding()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { OutputEncoding = Encoding.UTF32 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Encoding encoding = Terminal.OutputEncoding;

        // Assert
        encoding.ShouldBe(Encoding.UTF32);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_output_encoding()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.OutputEncoding = Encoding.Latin1;

        // Assert
        testTerminal.OutputEncoding.ShouldBe(Encoding.Latin1);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Redirection Tests (IConsole - task 020-002) ==========

    public static async Task Should_get_is_input_redirected()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { IsInputRedirected = true };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        bool redirected = Terminal.IsInputRedirected;

        // Assert
        redirected.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_is_output_redirected()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { IsOutputRedirected = true };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        bool redirected = Terminal.IsOutputRedirected;

        // Assert
        redirected.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_is_error_redirected()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { IsErrorRedirected = true };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        bool redirected = Terminal.IsErrorRedirected;

        // Assert
        redirected.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Rich Input Tests (IConsole - task 020-003) ==========

    public static async Task Should_read_character()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;
      testTerminal.QueueKeys("abc");

      try
      {
        // Act
        int ch = Terminal.Read();

        // Assert
        ch.ShouldBe('a');
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_read_key_without_intercept()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;
      testTerminal.QueueKey(ConsoleKey.Enter);

      try
      {
        // Act
        ConsoleKeyInfo keyInfo = Terminal.ReadKey();

        // Assert
        keyInfo.Key.ShouldBe(ConsoleKey.Enter);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

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

    // ========== Color State Tests (ITerminal - task 020-006) ==========

    public static async Task Should_get_foreground_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { ForegroundColor = ConsoleColor.Cyan };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        ConsoleColor color = Terminal.ForegroundColor;

        // Assert
        color.ShouldBe(ConsoleColor.Cyan);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_foreground_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.ForegroundColor = ConsoleColor.Magenta;

        // Assert
        testTerminal.ForegroundColor.ShouldBe(ConsoleColor.Magenta);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_background_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { BackgroundColor = ConsoleColor.DarkBlue };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        ConsoleColor color = Terminal.BackgroundColor;

        // Assert
        color.ShouldBe(ConsoleColor.DarkBlue);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_background_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.BackgroundColor = ConsoleColor.DarkGreen;

        // Assert
        testTerminal.BackgroundColor.ShouldBe(ConsoleColor.DarkGreen);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_reset_color()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;
      testTerminal.ForegroundColor = ConsoleColor.Red;
      testTerminal.BackgroundColor = ConsoleColor.Blue;

      try
      {
        // Act
        Terminal.ResetColor();

        // Assert
        testTerminal.ForegroundColor.ShouldBe(ConsoleColor.Gray);
        testTerminal.BackgroundColor.ShouldBe(ConsoleColor.Black);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Control/Utility Tests (ITerminal - task 020-007) ==========

    public static async Task Should_beep()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Beep();

        // Assert
        testTerminal.BeepCount.ShouldBe(1);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_beep_with_frequency_and_duration()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Beep(800, 200);

        // Assert
        testTerminal.BeepCount.ShouldBe(1);
        testTerminal.LastBeepFrequency.ShouldBe(800);
        testTerminal.LastBeepDuration.ShouldBe(200);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_treat_control_c_as_input()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { TreatControlCAsInput = true };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        bool treatAsInput = Terminal.TreatControlCAsInput;

        // Assert
        treatAsInput.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_treat_control_c_as_input()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.TreatControlCAsInput = true;

        // Assert
        testTerminal.TreatControlCAsInput.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_title()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { Title = "Test Title" };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        string title = Terminal.Title;

        // Assert
        title.ShouldBe("Test Title");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_set_title()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Title = "New Title";

        // Assert
        testTerminal.Title.ShouldBe("New Title");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_key_available()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;
      testTerminal.QueueKey(ConsoleKey.A);

      try
      {
        // Act
        bool available = Terminal.KeyAvailable;

        // Assert
        available.ShouldBeTrue();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_get_key_available_false_when_empty()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        bool available = Terminal.KeyAvailable;

        // Assert
        available.ShouldBeFalse();
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TerminalStaticNewApis
