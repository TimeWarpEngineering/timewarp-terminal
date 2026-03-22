#!/usr/bin/dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test IConsole stream access APIs
using System.IO;

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
}

[TestTag("ITerminal")]
[TestTag("Stream")]
public class TerminalStreamAccessTests
{
  [ModuleInitializer]
  internal static void Register() => RegisterTests<TerminalStreamAccessTests>();

  public static async Task Should_return_stream_from_open_standard_input_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();

    // Act
    Stream stream = terminal.OpenStandardInput();

    // Assert
    stream.ShouldNotBeNull();
    stream.ShouldBeSameAs(terminal.StandardInputStream);

    await Task.CompletedTask;
  }

  public static async Task Should_return_stream_from_open_standard_output_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();

    // Act
    Stream stream = terminal.OpenStandardOutput();

    // Assert
    stream.ShouldNotBeNull();
    stream.ShouldBeSameAs(terminal.StandardOutputStream);

    await Task.CompletedTask;
  }

  public static async Task Should_return_stream_from_open_standard_error_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();

    // Act
    Stream stream = terminal.OpenStandardError();

    // Assert
    stream.ShouldNotBeNull();
    stream.ShouldBeSameAs(terminal.StandardErrorStream);

    await Task.CompletedTask;
  }

  public static async Task Should_return_text_reader_from_in_property_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();

    // Act
    TextReader reader = terminal.In;

    // Assert
    reader.ShouldNotBeNull();

    await Task.CompletedTask;
  }

  public static async Task Should_return_text_writer_from_out_property_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();

    // Act
    TextWriter writer = terminal.Out;

    // Assert
    writer.ShouldNotBeNull();

    await Task.CompletedTask;
  }

  public static async Task Should_return_text_writer_from_error_property_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();

    // Act
    TextWriter writer = terminal.Error;

    // Assert
    writer.ShouldNotBeNull();

    await Task.CompletedTask;
  }

  public static async Task Should_set_in_reader_via_set_in_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();
    StringReader newReader = new("test input");

    // Act
    terminal.SetIn(newReader);

    // Assert
    terminal.In.ShouldBeSameAs(newReader);

    await Task.CompletedTask;
  }

  public static async Task Should_set_out_writer_via_set_out_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();
    StringWriter newWriter = new();

    // Act
    terminal.SetOut(newWriter);

    // Assert
    terminal.Out.ShouldBeSameAs(newWriter);

    await Task.CompletedTask;
  }

  public static async Task Should_set_error_writer_via_set_error_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();
    StringWriter newWriter = new();

    // Act
    terminal.SetError(newWriter);

    // Assert
    terminal.Error.ShouldBeSameAs(newWriter);

    await Task.CompletedTask;
  }

  public static async Task Should_allow_custom_standard_input_stream_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();
    MemoryStream customStream = new();

    // Act
    terminal.StandardInputStream = customStream;
    Stream stream = terminal.OpenStandardInput();

    // Assert
    stream.ShouldBeSameAs(customStream);

    await Task.CompletedTask;
  }

  public static async Task Should_allow_custom_standard_output_stream_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();
    MemoryStream customStream = new();

    // Act
    terminal.StandardOutputStream = customStream;
    Stream stream = terminal.OpenStandardOutput();

    // Assert
    stream.ShouldBeSameAs(customStream);

    await Task.CompletedTask;
  }

  public static async Task Should_allow_custom_standard_error_stream_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();
    MemoryStream customStream = new();

    // Act
    terminal.StandardErrorStream = customStream;
    Stream stream = terminal.OpenStandardError();

    // Assert
    stream.ShouldBeSameAs(customStream);

    await Task.CompletedTask;
  }

  public static async Task Should_access_stream_apis_via_iconsole_interface()
  {
    // Arrange
    using TestTerminal terminal = new();
#pragma warning disable CA1859 // Intentionally testing IConsole interface access
    IConsole console = terminal;
#pragma warning restore CA1859

    // Act
    Stream inputStream = console.OpenStandardInput();
    Stream outputStream = console.OpenStandardOutput();
    Stream errorStream = console.OpenStandardError();
    TextReader reader = console.In;
    TextWriter outWriter = console.Out;
    TextWriter errorWriter = console.Error;

    // Assert
    inputStream.ShouldNotBeNull();
    outputStream.ShouldNotBeNull();
    errorStream.ShouldNotBeNull();
    reader.ShouldNotBeNull();
    outWriter.ShouldNotBeNull();
    errorWriter.ShouldNotBeNull();

    await Task.CompletedTask;
  }

  public static async Task Should_set_readers_writers_via_iconsole_interface()
  {
    // Arrange
    using TestTerminal terminal = new();
#pragma warning disable CA1859 // Intentionally testing IConsole interface access
    IConsole console = terminal;
#pragma warning restore CA1859
    StringReader newReader = new("test");
    StringWriter newOutWriter = new();
    StringWriter newErrorWriter = new();

    // Act
    console.SetIn(newReader);
    console.SetOut(newOutWriter);
    console.SetError(newErrorWriter);

    // Assert
    console.In.ShouldBeSameAs(newReader);
    console.Out.ShouldBeSameAs(newOutWriter);
    console.Error.ShouldBeSameAs(newErrorWriter);

    await Task.CompletedTask;
  }
}

} // namespace TimeWarp.Terminal.Tests.Core.StreamAccess
