#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

#region Purpose
// Terminal stream, encoding, redirection, and rich-input facade tests.
#endregion
using System.Text;

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalStaticStreams
{

  [TestTag("Terminal")]
  public class TerminalStaticStreamTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalStaticStreamTests>();

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

  }

}
