#!/usr/bin/dotnet --
#:project ../source/timewarp-terminal/timewarp-terminal.csproj

// Test Table widget with emoji content

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TableWidgetEmoji
{

[TestTag("Widgets")]
public class TableWidgetEmojiTests
{
  [ModuleInitializer]
  internal static void Register() => RegisterTests<TableWidgetEmojiTests>();

  public static async Task Should_render_table_with_emoji_in_cells_aligned()
  {
    // Arrange - weather report table from kanban issue
    Table table = new TableBuilder()
      .AddColumn("Info")
      .AddColumn("Value")
      .AddRow("📍 Location", "Dallas, United States")
      .AddRow("🌡️ Temperature", "25.2°C (77.4°F)")
      .AddRow("☁️ Condition", "Overcast")
      .Build();

    // Act
    string[] lines = table.Render(80);

    // Assert - all border lines should have the same visible length
    // Top border (line 0), header separator (line 2), bottom border (last line)
    int topWidth = AnsiStringUtils.GetVisibleLength(lines[0]);
    int separatorWidth = AnsiStringUtils.GetVisibleLength(lines[2]);
    int bottomWidth = AnsiStringUtils.GetVisibleLength(lines[^1]);

    topWidth.ShouldBe(separatorWidth);
    topWidth.ShouldBe(bottomWidth);

    // All data rows should also match
    for (int i = 1; i < lines.Length - 1; i++)
    {
      AnsiStringUtils.GetVisibleLength(lines[i]).ShouldBe(topWidth);
    }

    await Task.CompletedTask;
  }

  public static async Task Should_render_table_with_emoji_headers_aligned()
  {
    // Arrange
    Table table = new TableBuilder()
      .AddColumn("📍 Location")
      .AddColumn("🌡️ Temp")
      .AddRow("Dallas", "25°C")
      .AddRow("London", "12°C")
      .Build();

    // Act
    string[] lines = table.Render(80);

    // Assert - borders align
    int topWidth = AnsiStringUtils.GetVisibleLength(lines[0]);
    int bottomWidth = AnsiStringUtils.GetVisibleLength(lines[^1]);

    topWidth.ShouldBe(bottomWidth);

    for (int i = 1; i < lines.Length - 1; i++)
    {
      AnsiStringUtils.GetVisibleLength(lines[i]).ShouldBe(topWidth);
    }

    await Task.CompletedTask;
  }

  public static async Task Should_render_table_without_emoji_unchanged()
  {
    // Regression test: ASCII-only table renders correctly
    Table table = new TableBuilder()
      .AddColumn("Name")
      .AddColumn("Value")
      .AddRow("Foo", "123")
      .AddRow("Bar", "456")
      .Build();

    // Act
    string[] lines = table.Render(40);

    // Assert
    int topWidth = AnsiStringUtils.GetVisibleLength(lines[0]);
    int bottomWidth = AnsiStringUtils.GetVisibleLength(lines[^1]);

    topWidth.ShouldBe(bottomWidth);

    for (int i = 1; i < lines.Length - 1; i++)
    {
      AnsiStringUtils.GetVisibleLength(lines[i]).ShouldBe(topWidth);
    }

    await Task.CompletedTask;
  }
}

} // namespace TimeWarp.Terminal.Tests.Core.TableWidgetEmoji
