#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Test Table widget expand functionality

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TableWidgetExpand
{

  [TestTag("Widgets")]
  public class TableWidgetExpandTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TableWidgetExpandTests>();

    public static async Task Should_expand_table_to_terminal_width()
    {
      // Arrange
      Table table = new TableBuilder()
        .AddColumn("Name")
        .AddColumn("Value")
        .AddRow("A", "1")
        .Expand()
        .Build();

      // Act
      string[] lines = table.Render(80);

      // Assert
      // The table should expand to fill 80 characters
      int topLineWidth = TimeWarp.Terminal.AnsiStringUtils.GetVisibleLength(lines[0]);
      topLineWidth.ShouldBe(80);

      await Task.CompletedTask;
    }

    public static async Task Should_not_expand_when_expand_is_not_set()
    {
      // Arrange
      Table table = new TableBuilder()
        .AddColumn("Name")
        .AddColumn("Value")
        .AddRow("A", "1")
        .Build();

      // Act
      string[] lines = table.Render(80);

      // Assert
      // The table should be sized to content, not terminal width
      int topLineWidth = TimeWarp.Terminal.AnsiStringUtils.GetVisibleLength(lines[0]);
      topLineWidth.ShouldBeLessThan(80);

      await Task.CompletedTask;
    }

    public static async Task Should_distribute_extra_width_evenly_across_columns()
    {
      // Arrange
      Table table = new TableBuilder()
        .AddColumn("A") // natural width: 1
        .AddColumn("B") // natural width: 1
        .AddRow("1", "2")
        .Expand()
        .Build();

      // Act - render to 50 chars
      // Natural width: 2 borders + 1 separator + 4 padding spaces + 2 content = 9
      // Extra: 50 - 9 = 41 chars to distribute across 2 columns
      string[] lines = table.Render(50);

      // Assert
      int topLineWidth = TimeWarp.Terminal.AnsiStringUtils.GetVisibleLength(lines[0]);
      topLineWidth.ShouldBe(50);

      await Task.CompletedTask;
    }

    public static async Task Should_not_expand_columns_beyond_max_width()
    {
      // Regression: Expand distributed extra width to every column, pushing
      // MaxWidth-capped columns past their cap.
      // Arrange
      TableColumn cappedColumn = new("A") { MaxWidth = 5 };
      Table table = new TableBuilder()
        .AddColumn(cappedColumn)
        .AddColumn("B")
        .AddRow("abc", "1")
        .Expand()
        .Build();

      // Act
      string[] lines = table.Render(50);

      // Assert - table still fills the terminal; the uncapped column absorbs the extra
      int topLineWidth = TimeWarp.Terminal.AnsiStringUtils.GetVisibleLength(lines[0]);
      topLineWidth.ShouldBe(50);

      // Capped column stays at MaxWidth 5: first border segment is 5 content + 2 padding,
      // so the first T-junction sits at index 8 (after "┌" + 7 horizontals)
      lines[0].IndexOf('┬').ShouldBe(8);

      await Task.CompletedTask;
    }

    public static async Task Should_stop_expanding_when_all_columns_are_capped()
    {
      // Arrange - every column has a MaxWidth, so expansion must stop at the caps
      TableColumn col1 = new("A") { MaxWidth = 5 };
      TableColumn col2 = new("B") { MaxWidth = 5 };
      Table table = new TableBuilder()
        .AddColumn(col1)
        .AddColumn(col2)
        .AddRow("abc", "1")
        .Expand()
        .Build();

      // Act
      string[] lines = table.Render(80);

      // Assert - 2 borders + 1 separator + 4 padding + 2*5 content = 17, not 80
      int topLineWidth = TimeWarp.Terminal.AnsiStringUtils.GetVisibleLength(lines[0]);
      topLineWidth.ShouldBe(17);

      await Task.CompletedTask;
    }

    public static async Task Should_not_expand_borderless_table()
    {
      // Arrange
      Table table = new TableBuilder()
        .AddColumn("Name")
        .AddColumn("Value")
        .AddRow("A", "1")
        .Expand()
        .Border(BorderStyle.None)
        .Build();

      // Act
      string[] lines = table.Render(80);

      // Assert
      // Borderless tables don't expand because there's no visual border to fill
      int lineWidth = TimeWarp.Terminal.AnsiStringUtils.GetVisibleLength(lines[0]);
      lineWidth.ShouldBeLessThan(80);

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.Core.TableWidgetExpand
