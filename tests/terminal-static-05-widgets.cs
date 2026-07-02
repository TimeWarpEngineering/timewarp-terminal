#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test Terminal static facade widget methods
// CA1849: We deliberately test sync methods in async test methods
#pragma warning disable CA1849

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TerminalStaticWidget
{

  [TestTag("Terminal")]
  public class TerminalStaticWidgetTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TerminalStaticWidgetTests>();

    // ========== Table Tests ==========

    public static async Task Should_write_table_with_builder()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteTable(table => table
            .AddColumn("Name")
            .AddColumn("Value")
            .AddRow("Foo", "123"));

        // Assert
        testTerminal.Output.ShouldContain("┌");
        testTerminal.Output.ShouldContain("Foo");
        testTerminal.Output.ShouldContain("123");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_preconfigured_table()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        Table table = new TableBuilder()
            .AddColumn("Name")
            .AddColumn("Value")
            .AddRow("Foo", "123")
            .Build();

        // Act
        Terminal.WriteTable(table);

        // Assert
        testTerminal.Output.ShouldContain("┌");
        testTerminal.Output.ShouldContain("Foo");
        testTerminal.Output.ShouldContain("123");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Panel Tests ==========

    public static async Task Should_write_panel_with_builder()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WritePanel(panel => panel
            .Header("Configuration")
            .Content("Setting: value"));

        // Assert
        testTerminal.Output.ShouldContain("Configuration");
        testTerminal.Output.ShouldContain("Setting: value");
        testTerminal.Output.ShouldContain("╭");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_panel_with_content_and_header()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WritePanel("Hello World", header: "Greeting");

        // Assert
        testTerminal.Output.ShouldContain("Greeting");
        testTerminal.Output.ShouldContain("Hello World");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_panel_with_content_only()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WritePanel("Simple content", header: null);

        // Assert
        testTerminal.Output.ShouldContain("Simple content");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Rule Tests ==========

    public static async Task Should_write_rule_with_title()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteRule("Section Title");

        // Assert
        testTerminal.Output.ShouldContain("Section Title");
        testTerminal.Output.ShouldContain("─");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_rule_without_title()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteRule();

        // Assert
        testTerminal.Output.ShouldContain("─");
        // Output should be approximately WindowWidth (40) + newline
        testTerminal.Output.Length.ShouldBeGreaterThanOrEqualTo(40);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_rule_with_builder()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteRule(rule => rule
            .Title("Configuration")
            .Style(LineStyle.Doubled)
            .Color(AnsiColors.Cyan));

        // Assert
        testTerminal.Output.ShouldContain("Configuration");
        testTerminal.Output.ShouldContain("═");
        testTerminal.Output.ShouldContain(AnsiColors.Cyan);
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Link Tests ==========

    public static async Task Should_write_hyperlink()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act
        Terminal.WriteLink("https://github.com", "GitHub");

        // Assert - Should contain OSC 8 escape sequences
        testTerminal.Output.ShouldContain("\x1b]8;;");
        testTerminal.Output.ShouldContain("https://github.com");
        testTerminal.Output.ShouldContain("GitHub");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    // ========== Null Argument Tests ==========

    public static async Task Should_throw_write_table_with_null_configure()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => Terminal.WriteTable((Action<TableBuilder>)null!));
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_throw_write_table_with_null_table()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => Terminal.WriteTable((Table)null!));
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_throw_write_panel_with_null_configure()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => Terminal.WritePanel(null!));
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_throw_write_rule_with_null_configure()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => Terminal.WriteRule((Action<RuleBuilder>)null!));
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_throw_write_link_with_null_url()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => Terminal.WriteLink(null!, "text"));
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }

    public static async Task Should_throw_write_link_with_null_text()
    {
      // Arrange
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        // Act & Assert
        Should.Throw<ArgumentNullException>(() => Terminal.WriteLink("https://example.com", null!));
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TerminalStaticWidget
