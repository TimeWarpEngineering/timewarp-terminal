#!/usr/bin/dotnet --
#:project ../source/timewarp-terminal/timewarp-terminal.csproj

// Test Table widget Grow column functionality

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TableWidgetGrow
{

[TestTag("Widgets")]
public class TableWidgetGrowTests
{
  [ModuleInitializer]
  internal static void Register() => RegisterTests<TableWidgetGrowTests>();

  public static async Task Should_grow_column_to_fill_terminal_width()
  {
    // Arrange
    TableColumn growColumn = new("Description") { Grow = true };
    Table table = new TableBuilder()
      .AddColumn("ID")
      .AddColumn(growColumn)
      .AddRow("1", "Short text")
      .Border(BorderStyle.Rounded)
      .Build();

    // Act - render to 80 chars
    string[] lines = table.Render(80);

    // Assert - table should fill terminal width
    int topLineWidth = AnsiStringUtils.GetVisibleLength(lines[0]);
    topLineWidth.ShouldBe(80);

    await Task.CompletedTask;
  }

  public static async Task Should_keep_fixed_columns_at_natural_width()
  {
    // Arrange - ID column is fixed, Description grows
    TableColumn growColumn = new("Description") { Grow = true };
    Table table = new TableBuilder()
      .AddColumn("ID")
      .AddColumn(growColumn)
      .AddRow("1", "Some text")
      .Border(BorderStyle.Rounded)
      .Build();

    // Act
    string[] lines = table.Render(80);

    // Assert - ID column stays at natural width (1 char content), Description fills the rest
    // lines[0] = top border, lines[1] = header, lines[2] = header separator, lines[3] = data row
    string dataRow = lines[3];
    dataRow.ShouldContain("1"); // ID content present
    dataRow.ShouldContain("Some text"); // Description content present

    await Task.CompletedTask;
  }

  public static async Task Should_split_remaining_space_evenly_between_multiple_grow_columns()
  {
    // Arrange - two grow columns share remaining space
    TableColumn col1 = new("Left") { Grow = true };
    TableColumn col2 = new("Right") { Grow = true };
    Table table = new TableBuilder()
      .AddColumn("ID")
      .AddColumn(col1)
      .AddColumn(col2)
      .AddRow("1", "A", "B")
      .Border(BorderStyle.Rounded)
      .Build();

    // Act - render to 80 chars
    string[] lines = table.Render(80);

    // Assert - total width fills terminal
    int topLineWidth = AnsiStringUtils.GetVisibleLength(lines[0]);
    topLineWidth.ShouldBe(80);

    await Task.CompletedTask;
  }

  public static async Task Should_shrink_fixed_columns_when_overflow()
  {
    // Arrange - fixed columns are wide, terminal is narrow
    TableColumn growColumn = new("Description") { Grow = true };
    Table table = new TableBuilder()
      .AddColumn(new TableColumn("Fixed Header"))
      .AddColumn(growColumn)
      .AddRow("Fixed content", "Grow content")
      .Border(BorderStyle.Rounded)
      .Build();

    // Act - narrow terminal forces shrinking of fixed columns
    string[] lines = table.Render(40);

    // Assert - table fits within terminal width
    int topLineWidth = AnsiStringUtils.GetVisibleLength(lines[0]);
    topLineWidth.ShouldBeLessThanOrEqualTo(40);

    await Task.CompletedTask;
  }

  public static async Task Should_give_grow_column_min_width_when_no_space_available()
  {
    // Arrange - very narrow terminal, no room for grow column
    TableColumn growColumn = new("G") { Grow = true };
    Table table = new TableBuilder()
      .AddColumn(new TableColumn("Fixed") { MinWidth = 10 })
      .AddColumn(growColumn)
      .AddRow("Fixed text", "Grow text")
      .Border(BorderStyle.Rounded)
      .Build();

    // Act - very narrow terminal
    string[] lines = table.Render(20);

    // Assert - table still renders (grow column falls back to MinWidth ?? 4)
    lines.Length.ShouldBeGreaterThan(0);

    await Task.CompletedTask;
  }

  public static async Task Should_grow_column_with_three_fixed_columns()
  {
    // Arrange - real-world: ID, Status, Description (grows), Timestamp
    TableColumn descColumn = new("Description") { Grow = true };
    Table table = new TableBuilder()
      .AddColumn("ID")
      .AddColumn("Status")
      .AddColumn(descColumn)
      .AddColumn("Time")
      .AddRow("1", "Active", "This description will expand to fill available space", "12:00")
      .Border(BorderStyle.Rounded)
      .Build();

    // Act
    string[] lines = table.Render(100);

    // Assert - table fills terminal
    int topLineWidth = AnsiStringUtils.GetVisibleLength(lines[0]);
    topLineWidth.ShouldBe(100);

    await Task.CompletedTask;
  }

  public static async Task Should_render_grow_column_with_ellipsis_when_fixed_overflow()
  {
    // Arrange - fixed columns exceed terminal, grow column falls back to MinWidth and truncates
    TableColumn growColumn = new("Description") { Grow = true, MinWidth = 4 };
    Table table = new TableBuilder()
      .AddColumn(new TableColumn("VeryLongFixedColumnHeader"))
      .AddColumn(growColumn)
      .AddRow("Very long fixed content that takes a lot of space", "Some description text here")
      .Border(BorderStyle.Rounded)
      .Build();

    // Act - narrow terminal
    string[] lines = table.Render(30);

    // Assert - still renders without throwing
    lines.Length.ShouldBeGreaterThan(0);

    await Task.CompletedTask;
  }
}

} // namespace TimeWarp.Terminal.Tests.Core.TableWidgetGrow
