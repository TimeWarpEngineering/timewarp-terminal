#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test Terminal format method overloads
// CA1849: We deliberately test sync methods in async test methods
#pragma warning disable CA1849

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalStatic
{

  [TestTag("Terminal")]
  public class TerminalStaticFormatTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalStaticFormatTests>();

    // ========== Write Format Tests ==========

    public static async Task Should_write_format_single_arg()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Write("Hello {0}", "World");

        // Assert
        testTerminal.Output.ShouldBe("Hello World");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_format_two_args()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Write("{0} {1}", "Hello", "World");

        // Assert
        testTerminal.Output.ShouldBe("Hello World");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_format_three_args()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Write("{0} {1} {2}", "a", "b", "c");

        // Assert
        testTerminal.Output.ShouldBe("a b c");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_format_params()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Write("{0}{1}{2}", "x", "y", "z");

        // Assert
        testTerminal.Output.ShouldBe("xyz");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== WriteLine Format Tests ==========

    public static async Task Should_writeline_format_single_arg()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteLine("Hello {0}", "World");

        // Assert
        testTerminal.Output.ShouldBe("Hello World" + Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_writeline_format_two_args()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteLine("{0} {1}", "Hello", "World");

        // Assert
        testTerminal.Output.ShouldBe("Hello World" + Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_writeline_format_three_args()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteLine("{0} {1} {2}", "a", "b", "c");

        // Assert
        testTerminal.Output.ShouldBe("a b c" + Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_writeline_format_params()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteLine("{0}{1}{2}", "x", "y", "z");

        // Assert
        testTerminal.Output.ShouldBe("xyz" + Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== WriteErrorLine Format Tests ==========

    public static async Task Should_write_error_format_single_arg()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteErrorLine("Error: {0}", "404");

        // Assert
        testTerminal.ErrorOutput.ShouldBe("Error: 404" + Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_error_format_two_args()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteErrorLine("{0}: {1}", "Error", "fail");

        // Assert
        testTerminal.ErrorOutput.ShouldBe("Error: fail" + Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_error_format_three_args()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteErrorLine("{0} {1} {2}", "a", "b", "c");

        // Assert
        testTerminal.ErrorOutput.ShouldBe("a b c" + Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_error_format_params()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteErrorLine("{0}{1}{2}", "x", "y", "z");

        // Assert
        testTerminal.ErrorOutput.ShouldBe("xyz" + Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Numeric Formatting Tests ==========

    public static async Task Should_write_format_with_numeric()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Write("Number: {0:D4}", 42);

        // Assert
        testTerminal.Output.ShouldBe("Number: 0042");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_writeline_format_with_numeric()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act - Use F2 format (fixed-point) instead of C (currency) since Terminal uses InvariantCulture
        Terminal.WriteLine("Value: {0:F2}", 99.99);

        // Assert
        testTerminal.Output.ShouldBe("Value: 99.99" + Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Null Argument Tests ==========

    public static async Task Should_write_format_null_arg()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.Write("Value: {0}", (object?)null);

        // Assert
        testTerminal.Output.ShouldBe("Value: ");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_error_format_null_arg()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteErrorLine("Error: {0}", (object?)null);

        // Assert
        testTerminal.ErrorOutput.ShouldBe("Error: " + Environment.NewLine);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== FormatProvider Tests ==========

    public static async Task Should_format_with_custom_format_provider()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act - a provider with a comma decimal separator, independent of OS cultures
        System.Globalization.NumberFormatInfo commaProvider = new() { NumberDecimalSeparator = "," };
        Terminal.FormatProvider = commaProvider;
        Terminal.Write("Value: {0:0.0}", 1.5);

        // Assert
        testTerminal.Output.ShouldBe("Value: 1,5");
      }
      finally
      {
        Terminal.FormatProvider = null;
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_format_with_current_culture_by_default()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new();
      Terminal.Instance = testTerminal;

      try
      {
        // Act - FormatProvider defaults to null = CurrentCulture resolved per call
        Terminal.FormatProvider.ShouldBeNull();
        Terminal.Write("Value: {0:0.0}", 1.5);

        // Assert
        string expected = string.Format(System.Globalization.CultureInfo.CurrentCulture, "Value: {0:0.0}", 1.5);
        testTerminal.Output.ShouldBe(expected);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TerminalStatic
