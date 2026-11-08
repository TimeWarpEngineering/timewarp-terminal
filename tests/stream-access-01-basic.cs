#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test IConsole stream access APIs

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.StreamAccess
{

  [TestTag("IConsole")]
  [TestTag("Stream")]
  public class ConsoleStreamAccessTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<ConsoleStreamAccessTests>();

    public static async Task Should_return_stream_from_open_standard_input_in_test_console()
    {
      // Arrange
      using TestConsole console = new();

      // Act
      Stream stream = console.OpenStandardInput();

      // Assert
      stream.ShouldNotBeNull();
      stream.ShouldBeSameAs(console.StandardInputStream);

      await Task.CompletedTask;
    }

    public static async Task Should_return_stream_from_open_standard_output_in_test_console()
    {
      // Arrange
      using TestConsole console = new();

      // Act
      Stream stream = console.OpenStandardOutput();

      // Assert
      stream.ShouldNotBeNull();
      stream.ShouldBeSameAs(console.StandardOutputStream);

      await Task.CompletedTask;
    }

    public static async Task Should_return_stream_from_open_standard_error_in_test_console()
    {
      // Arrange
      using TestConsole console = new();

      // Act
      Stream stream = console.OpenStandardError();

      // Assert
      stream.ShouldNotBeNull();
      stream.ShouldBeSameAs(console.StandardErrorStream);

      await Task.CompletedTask;
    }

    public static async Task Should_return_text_reader_from_in_property_in_test_console()
    {
      // Arrange
      using TestConsole console = new();

      // Act
      TextReader reader = console.In;

      // Assert
      reader.ShouldNotBeNull();

      await Task.CompletedTask;
    }

    public static async Task Should_return_text_writer_from_out_property_in_test_console()
    {
      // Arrange
      using TestConsole console = new();

      // Act
      TextWriter writer = console.Out;

      // Assert
      writer.ShouldNotBeNull();

      await Task.CompletedTask;
    }

    public static async Task Should_return_text_writer_from_error_property_in_test_console()
    {
      // Arrange
      using TestConsole console = new();

      // Act
      TextWriter writer = console.Error;

      // Assert
      writer.ShouldNotBeNull();

      await Task.CompletedTask;
    }

    public static async Task Should_set_in_reader_via_set_in_in_test_console()
    {
      // Arrange
      using TestConsole console = new();
      StringReader newReader = new("test input");

      // Act
      console.SetIn(newReader);

      // Assert
      console.In.ShouldBeSameAs(newReader);

      await Task.CompletedTask;
    }

    public static async Task Should_set_out_writer_via_set_out_in_test_console()
    {
      // Arrange
      using TestConsole console = new();
      StringWriter newWriter = new();

      // Act
      console.SetOut(newWriter);

      // Assert
      console.Out.ShouldBeSameAs(newWriter);

      await Task.CompletedTask;
    }

    public static async Task Should_set_error_writer_via_set_error_in_test_console()
    {
      // Arrange
      using TestConsole console = new();
      StringWriter newWriter = new();

      // Act
      console.SetError(newWriter);

      // Assert
      console.Error.ShouldBeSameAs(newWriter);

      await Task.CompletedTask;
    }

    public static async Task Should_allow_custom_standard_input_stream_in_test_console()
    {
      // Arrange
      using TestConsole console = new();
      MemoryStream customStream = new();

      // Act
      console.StandardInputStream = customStream;
      Stream stream = console.OpenStandardInput();

      // Assert
      stream.ShouldBeSameAs(customStream);

      await Task.CompletedTask;
    }

    public static async Task Should_allow_custom_standard_output_stream_in_test_console()
    {
      // Arrange
      using TestConsole console = new();
      MemoryStream customStream = new();

      // Act
      console.StandardOutputStream = customStream;
      Stream stream = console.OpenStandardOutput();

      // Assert
      stream.ShouldBeSameAs(customStream);

      await Task.CompletedTask;
    }

    public static async Task Should_allow_custom_standard_error_stream_in_test_console()
    {
      // Arrange
      using TestConsole console = new();
      MemoryStream customStream = new();

      // Act
      console.StandardErrorStream = customStream;
      Stream stream = console.OpenStandardError();

      // Assert
      stream.ShouldBeSameAs(customStream);

      await Task.CompletedTask;
    }

    public static async Task Should_not_dispose_consumer_assigned_standard_output_stream_in_test_console()
    {
      // Arrange
      TestConsole console = new();
      using MemoryStream customStream = new();
      console.StandardOutputStream = customStream;

      // Act
      console.Dispose();

      // Assert - consumer-assigned stream must remain usable after TestConsole.Dispose
      customStream.WriteByte(0x41);
      customStream.Length.ShouldBe(1L);

      await Task.CompletedTask;
    }

    public static async Task Should_write_to_set_out_writer_in_test_console()
    {
      // Arrange
      using TestConsole console = new();
      using StringWriter redirected = new();
      console.SetOut(redirected);

      // Act
      await console.WriteLineAsync("hello-out");

      // Assert
      redirected.ToString().ShouldContain("hello-out");
      console.Output.ShouldContain("hello-out");

      await Task.CompletedTask;
    }

    public static async Task Should_write_error_to_set_error_writer_in_test_console()
    {
      // Arrange
      using TestConsole console = new();
      using StringWriter redirected = new();
      console.SetError(redirected);

      // Act
      await console.WriteErrorLineAsync("hello-err");

      // Assert
      redirected.ToString().ShouldContain("hello-err");
      console.ErrorOutput.ShouldContain("hello-err");

      await Task.CompletedTask;
    }

    public static async Task Should_read_line_from_set_in_reader_in_test_console()
    {
      // Arrange
      using TestConsole console = new("constructor");
      console.SetIn(new StringReader("redirected"));

      // Act
      string? line = console.ReadLine();

      // Assert
      line.ShouldBe("redirected");

      await Task.CompletedTask;
    }

    public static async Task Should_read_from_set_in_reader_in_test_console()
    {
      // Arrange
      using TestConsole console = new();
      console.SetIn(new StringReader("Z"));

      // Act
      int result = console.Read();

      // Assert
      result.ShouldBe('Z');

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.StreamAccess
