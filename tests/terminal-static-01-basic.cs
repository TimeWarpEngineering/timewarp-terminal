#!/usr/bin/dotnet --
#:project ../source/timewarp-terminal/timewarp-terminal.csproj

// Test Terminal static facade class
// CA1849: We deliberately test sync methods in async test methods
#pragma warning disable CA1849

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalStatic
{

[TestTag("Terminal")]
public class TerminalStaticBasicTests
{
  [ModuleInitializer]
  internal static void Register() => RegisterTests<TerminalStaticBasicTests>();

  public static async Task Should_default_instance_to_timewarp_terminal()
  {
    // Arrange & Act
    ITerminal instance = Terminal.Instance;

    // Assert
    instance.ShouldNotBeNull();
    instance.ShouldBeOfType<TimeWarpTerminal>();

    await Task.CompletedTask;
  }

  public static async Task Should_allow_setting_custom_instance()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();

    try
    {
      // Act
      Terminal.Instance = testTerminal;

      // Assert
      Terminal.Instance.ShouldBe(testTerminal);
    }
    finally
    {
      // Restore
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_throw_when_setting_null_instance()
  {
    // Arrange
    ITerminal original = Terminal.Instance;

    try
    {
      // Act & Assert
      Should.Throw<ArgumentNullException>(() => Terminal.Instance = null!);
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_write_to_instance()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      Terminal.Write("Hello");

      // Assert
      testTerminal.Output.ShouldBe("Hello");
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_write_empty_string_for_null()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      Terminal.Write(null);

      // Assert
      testTerminal.Output.ShouldBe(string.Empty);
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_writeline_to_instance()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      Terminal.WriteLine("Hello");

      // Assert
      testTerminal.Output.ShouldBe("Hello" + Environment.NewLine);
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_writeline_with_no_args()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      Terminal.WriteLine();

      // Assert
      testTerminal.Output.ShouldBe(Environment.NewLine);
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_writeline_async()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      await Terminal.WriteLineAsync("Async");

      // Assert
      testTerminal.Output.ShouldBe("Async" + Environment.NewLine);
    }
    finally
    {
      Terminal.Instance = original;
    }
  }

  public static async Task Should_write_error_line()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      Terminal.WriteErrorLine("Error");

      // Assert
      testTerminal.ErrorOutput.ShouldBe("Error" + Environment.NewLine);
    }
    finally
    {
      Terminal.Instance = original;
    }

    await Task.CompletedTask;
  }

  public static async Task Should_write_error_line_async()
  {
    // Arrange
    ITerminal original = Terminal.Instance;
    using TestTerminal testTerminal = new();
    Terminal.Instance = testTerminal;

    try
    {
      // Act
      await Terminal.WriteErrorLineAsync("AsyncError");

      // Assert
      testTerminal.ErrorOutput.ShouldBe("AsyncError" + Environment.NewLine);
    }
    finally
    {
      Terminal.Instance = original;
    }
  }
}

} // namespace TimeWarp.Terminal.Tests.Core.TerminalStatic
