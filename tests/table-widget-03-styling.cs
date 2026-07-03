#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test Table widget with styled content (ANSI colors)

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TableWidgetStyling
{

  [TestTag("Widgets")]
  public class TableWidgetStylingTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TableWidgetStylingTests>();

    public static async Task Should_render_styled_cell_content()
    {
      // Arrange
      string styledValue = $"{AnsiColors.Red}Error{AnsiColors.Reset}";
      Table table = new TableBuilder()
        .AddColumn("Status")
        .AddRow(styledValue)
        .Build();

      // Act
      string[] lines = table.Render(40);

      // Assert
      // The styled content should be preserved in the output
      lines[3].ShouldContain(AnsiColors.Red);
      lines[3].ShouldContain("Error");
      lines[3].ShouldContain(AnsiColors.Reset);

      await Task.CompletedTask;
    }

    public static async Task Should_calculate_column_width_correctly_with_ansi_codes()
    {
      // Arrange
      // "Error" is 5 visible characters, but with ANSI codes the string is longer
      string styledValue = $"{AnsiColors.Red}Error{AnsiColors.Reset}";
      Table table = new TableBuilder()
        .AddColumn("Status") // 6 chars
        .AddRow(styledValue) // 5 visible chars
        .Build();

      // Act
      string[] lines = table.Render(40);

      // Assert
      // The column should be sized for "Status" (6 chars) not the full ANSI string length
      // Header row should have proper alignment
      lines[1].ShouldContain("Status");

      // The visible width of the content row should match the header width
      // (both padded to 6 characters)
      string contentLine = TimeWarp.Terminal.AnsiStringUtils.StripAnsiCodes(lines[3]);
      contentLine.ShouldContain("Error ");

      await Task.CompletedTask;
    }

    public static async Task Should_render_header_with_column_header_color()
    {
      // Arrange
      TableColumn column = new("Important")
      {
        HeaderColor = AnsiColors.Yellow
      };
      Table table = new TableBuilder()
        .AddColumn(column)
        .AddRow("Value")
        .Build();

      // Act
      string[] lines = table.Render(40);

      // Assert
      lines[1].ShouldContain(AnsiColors.Yellow);
      lines[1].ShouldContain("Important");
      lines[1].ShouldContain(AnsiColors.Reset);

      await Task.CompletedTask;
    }

    public static async Task Should_handle_multiple_styled_cells_in_same_row()
    {
      // Arrange
      string green = $"{AnsiColors.Green}OK{AnsiColors.Reset}";
      string red = $"{AnsiColors.Red}FAIL{AnsiColors.Reset}";
      Table table = new TableBuilder()
        .AddColumn("Test 1")
        .AddColumn("Test 2")
        .AddRow(green, red)
        .Build();

      // Act
      string[] lines = table.Render(40);

      // Assert
      string dataRow = lines[3];
      dataRow.ShouldContain(AnsiColors.Green);
      dataRow.ShouldContain("OK");
      dataRow.ShouldContain(AnsiColors.Red);
      dataRow.ShouldContain("FAIL");

      await Task.CompletedTask;
    }

    public static async Task Should_truncate_long_content_with_ellipsis()
    {
      // Arrange
      TableColumn column = new("Description")
      {
        MaxWidth = 10
      };
      Table table = new TableBuilder()
        .AddColumn(column)
        .AddRow("This is a very long description that should be truncated")
        .Build();

      // Act
      string[] lines = table.Render(40);

      // Assert
      // Content should be truncated to 10 chars with "..." at end
      string dataRow = lines[3];
      dataRow.ShouldContain("...");
      // Should not contain the full text
      dataRow.ShouldNotContain("truncated");

      await Task.CompletedTask;
    }

    public static async Task Should_preserve_color_when_truncating_with_End_mode()
    {
      // Arrange
      TableColumn column = new("Description")
      {
        MaxWidth = 10
      };
      string styled = $"{AnsiColors.Red}This is a long red value{AnsiColors.Reset}";
      Table table = new TableBuilder()
        .AddColumn(column)
        .AddRow(styled)
        .Build();

      // Act
      string[] lines = table.Render(40);
      string dataRow = lines[3];

      // Assert - kept text keeps its color; a reset closes it before the plain ellipsis,
      // so the ellipsis, padding, and border after it are unstyled
      dataRow.ShouldContain(AnsiColors.Red + "This is");
      dataRow.ShouldContain(AnsiColors.Reset + "...");
      TimeWarp.Terminal.AnsiStringUtils.StripAnsiCodes(dataRow).ShouldContain("This is...");

      await Task.CompletedTask;
    }

    public static async Task Should_replay_color_opened_before_cut_when_truncating_with_Start_mode()
    {
      // Arrange - the red color opens in the discarded prefix but must still style the kept tail
      TableColumn column = new("Path")
      {
        MaxWidth = 10,
        TruncateMode = TruncateMode.Start
      };
      string styled = $"prefix text {AnsiColors.Red}red tail{AnsiColors.Reset}";
      Table table = new TableBuilder()
        .AddColumn(column)
        .AddRow(styled)
        .Build();

      // Act
      string[] lines = table.Render(40);
      string dataRow = lines[3];

      // Assert - plain ellipsis, then the replayed color styles the kept tail
      dataRow.ShouldContain("..." + AnsiColors.Red);
      dataRow.ShouldContain("ed tail" + AnsiColors.Reset);
      TimeWarp.Terminal.AnsiStringUtils.StripAnsiCodes(dataRow).ShouldContain("...ed tail");

      await Task.CompletedTask;
    }

    public static async Task Should_keep_foreground_after_border_reset_when_writing_table_with_color()
    {
      // Regression: BorderColor emits a Reset inside each rendered line, which used to cancel
      // the foreground requested via the WriteTable color overload for the rest of the line
      using TestTerminal terminal = new() { WindowWidth = 40 };

      // Act
      terminal.WriteTable(
        table => table
          .AddColumn("Name")
          .AddRow("Foo")
          .BorderColor(AnsiColors.Yellow),
        ConsoleColor.Cyan);

      // Assert - the foreground code re-appears after each embedded Reset
      terminal.Output.ShouldContain(AnsiColors.Yellow);
      terminal.Output.ShouldContain(AnsiColors.Reset + AnsiColors.BrightCyan); // ConsoleColor.Cyan maps to bright cyan
      terminal.Output.ShouldContain("Foo");

      await Task.CompletedTask;
    }

    public static async Task Should_handle_empty_cells()
    {
      // Arrange
      Table table = new TableBuilder()
        .AddColumn("A")
        .AddColumn("B")
        .AddColumn("C")
        .AddRow("1", "", "3")
        .Build();

      // Act
      string[] lines = table.Render(40);

      // Assert
      // Should render without errors, empty cell should just be spaces
      lines[3].ShouldContain("1");
      lines[3].ShouldContain("3");

      await Task.CompletedTask;
    }

    public static async Task Should_handle_fewer_cells_than_columns()
    {
      // Arrange
      Table table = new TableBuilder()
        .AddColumn("A")
        .AddColumn("B")
        .AddColumn("C")
        .AddRow("1") // Only one cell for three columns
        .Build();

      // Act
      string[] lines = table.Render(40);

      // Assert
      // Should render without errors, missing cells should be empty
      lines[3].ShouldContain("1");

      await Task.CompletedTask;
    }

    public static async Task Should_handle_null_cell_values()
    {
      // Arrange
      Table table = new TableBuilder()
        .AddColumn("A")
        .AddColumn("B")
        .AddRow("1", null!)
        .Build();

      // Act
      string[] lines = table.Render(40);

      // Assert
      // Should render without errors
      lines[3].ShouldContain("1");

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TableWidgetStyling
