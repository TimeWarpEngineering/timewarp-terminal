#!/usr/bin/dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test IConsole encoding and redirection properties
using System.Text;

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.ConsoleEncoding
{

[TestTag("IConsole")]
[TestTag("Encoding")]
public class ConsoleEncodingBasicTests
{
  [ModuleInitializer]
  internal static void Register() => RegisterTests<ConsoleEncodingBasicTests>();

  public static async Task Should_default_input_encoding_to_utf8_in_test_console()
  {
    // Arrange & Act
    using TestConsole console = new();

    // Assert
    console.InputEncoding.ShouldBe(Encoding.UTF8);

    await Task.CompletedTask;
  }

  public static async Task Should_default_output_encoding_to_utf8_in_test_console()
  {
    // Arrange & Act
    using TestConsole console = new();

    // Assert
    console.OutputEncoding.ShouldBe(Encoding.UTF8);

    await Task.CompletedTask;
  }

  public static async Task Should_set_input_encoding_in_test_console()
  {
    // Arrange
    using TestConsole console = new();
    Encoding unicode = Encoding.Unicode;

    // Act
    console.InputEncoding = unicode;

    // Assert
    console.InputEncoding.ShouldBe(unicode);

    await Task.CompletedTask;
  }

  public static async Task Should_set_output_encoding_in_test_console()
  {
    // Arrange
    using TestConsole console = new();
    Encoding ascii = Encoding.ASCII;

    // Act
    console.OutputEncoding = ascii;

    // Assert
    console.OutputEncoding.ShouldBe(ascii);

    await Task.CompletedTask;
  }

  public static async Task Should_default_is_input_redirected_to_false_in_test_console()
  {
    // Arrange & Act
    using TestConsole console = new();

    // Assert
    console.IsInputRedirected.ShouldBeFalse();

    await Task.CompletedTask;
  }

  public static async Task Should_default_is_output_redirected_to_false_in_test_console()
  {
    // Arrange & Act
    using TestConsole console = new();

    // Assert
    console.IsOutputRedirected.ShouldBeFalse();

    await Task.CompletedTask;
  }

  public static async Task Should_default_is_error_redirected_to_false_in_test_console()
  {
    // Arrange & Act
    using TestConsole console = new();

    // Assert
    console.IsErrorRedirected.ShouldBeFalse();

    await Task.CompletedTask;
  }

  public static async Task Should_set_is_input_redirected_in_test_console()
  {
    // Arrange
    using TestConsole console = new();

    // Act
    console.IsInputRedirected = true;

    // Assert
    console.IsInputRedirected.ShouldBeTrue();

    await Task.CompletedTask;
  }

  public static async Task Should_set_is_output_redirected_in_test_console()
  {
    // Arrange
    using TestConsole console = new();

    // Act
    console.IsOutputRedirected = true;

    // Assert
    console.IsOutputRedirected.ShouldBeTrue();

    await Task.CompletedTask;
  }

  public static async Task Should_set_is_error_redirected_in_test_console()
  {
    // Arrange
    using TestConsole console = new();

    // Act
    console.IsErrorRedirected = true;

    // Assert
    console.IsErrorRedirected.ShouldBeTrue();

    await Task.CompletedTask;
  }
}

[TestTag("ITerminal")]
[TestTag("Encoding")]
public class TerminalEncodingBasicTests
{
  [ModuleInitializer]
  internal static void Register() => RegisterTests<TerminalEncodingBasicTests>();

  public static async Task Should_default_input_encoding_to_utf8_in_test_terminal()
  {
    // Arrange & Act
    using TestTerminal terminal = new();

    // Assert
    terminal.InputEncoding.ShouldBe(Encoding.UTF8);

    await Task.CompletedTask;
  }

  public static async Task Should_default_output_encoding_to_utf8_in_test_terminal()
  {
    // Arrange & Act
    using TestTerminal terminal = new();

    // Assert
    terminal.OutputEncoding.ShouldBe(Encoding.UTF8);

    await Task.CompletedTask;
  }

  public static async Task Should_set_input_encoding_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();
    Encoding unicode = Encoding.Unicode;

    // Act
    terminal.InputEncoding = unicode;

    // Assert
    terminal.InputEncoding.ShouldBe(unicode);

    await Task.CompletedTask;
  }

  public static async Task Should_set_output_encoding_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();
    Encoding ascii = Encoding.ASCII;

    // Act
    terminal.OutputEncoding = ascii;

    // Assert
    terminal.OutputEncoding.ShouldBe(ascii);

    await Task.CompletedTask;
  }

  public static async Task Should_default_is_input_redirected_to_false_in_test_terminal()
  {
    // Arrange & Act
    using TestTerminal terminal = new();

    // Assert
    terminal.IsInputRedirected.ShouldBeFalse();

    await Task.CompletedTask;
  }

  public static async Task Should_default_is_output_redirected_to_false_in_test_terminal()
  {
    // Arrange & Act
    using TestTerminal terminal = new();

    // Assert
    terminal.IsOutputRedirected.ShouldBeFalse();

    await Task.CompletedTask;
  }

  public static async Task Should_default_is_error_redirected_to_false_in_test_terminal()
  {
    // Arrange & Act
    using TestTerminal terminal = new();

    // Assert
    terminal.IsErrorRedirected.ShouldBeFalse();

    await Task.CompletedTask;
  }

  public static async Task Should_set_is_input_redirected_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();

    // Act
    terminal.IsInputRedirected = true;

    // Assert
    terminal.IsInputRedirected.ShouldBeTrue();

    await Task.CompletedTask;
  }

  public static async Task Should_set_is_output_redirected_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();

    // Act
    terminal.IsOutputRedirected = true;

    // Assert
    terminal.IsOutputRedirected.ShouldBeTrue();

    await Task.CompletedTask;
  }

  public static async Task Should_set_is_error_redirected_in_test_terminal()
  {
    // Arrange
    using TestTerminal terminal = new();

    // Act
    terminal.IsErrorRedirected = true;

    // Assert
    terminal.IsErrorRedirected.ShouldBeTrue();

    await Task.CompletedTask;
  }

  public static async Task Should_access_encoding_via_iconsole_interface()
  {
    // Arrange
    using TestTerminal terminal = new();
#pragma warning disable CA1859 // Intentionally testing IConsole interface access
    IConsole console = terminal;
#pragma warning restore CA1859
    Encoding utf32 = Encoding.UTF32;

    // Act
    console.InputEncoding = utf32;
    console.OutputEncoding = utf32;

    // Assert
    console.InputEncoding.ShouldBe(utf32);
    console.OutputEncoding.ShouldBe(utf32);

    await Task.CompletedTask;
  }

  public static async Task Should_access_redirection_via_iconsole_interface()
  {
    // Arrange
    using TestTerminal terminal = new();
#pragma warning disable CA1859 // Intentionally testing IConsole interface access
    IConsole console = terminal;
#pragma warning restore CA1859

    // Act
    console.IsInputRedirected.ShouldBeFalse();
    console.IsOutputRedirected.ShouldBeFalse();
    console.IsErrorRedirected.ShouldBeFalse();

    // Assert - properties are accessible via IConsole interface
    console.ShouldBeAssignableTo<IConsole>();

    await Task.CompletedTask;
  }
}

} // namespace TimeWarp.Terminal.Tests.Core.ConsoleEncoding
