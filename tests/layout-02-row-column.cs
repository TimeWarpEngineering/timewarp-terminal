#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal-layout/timewarp-terminal-layout.csproj

// Test Row vs Column direction stacking

using TimeWarp.Flexbox;

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.FlexLayout.RowColumn
{

  [TestTag("Layout")]
  public class LayoutRowColumnTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<LayoutRowColumnTests>();

    public static async Task Should_place_row_items_horizontally()
    {
      Layout layout = new LayoutBuilder()
        .Direction(FlexDirection.Row)
        .Item("A")
        .Item("B")
        .Build();

      IReadOnlyList<LayoutBox> boxes = layout.CalculateBoxes(40);
      boxes.Count.ShouldBe(2);
      boxes[0].Y.ShouldBe(boxes[1].Y);
      boxes[0].X.ShouldBeLessThan(boxes[1].X);

      await Task.CompletedTask;
    }

    public static async Task Should_stack_column_items_vertically()
    {
      Layout layout = new LayoutBuilder()
        .Direction(FlexDirection.Column)
        .Item("Top")
        .Item("Bottom")
        .Build();

      IReadOnlyList<LayoutBox> boxes = layout.CalculateBoxes(40);
      boxes.Count.ShouldBe(2);
      boxes[0].X.ShouldBe(boxes[1].X);
      boxes[0].Y.ShouldBeLessThan(boxes[1].Y);

      string[] lines = layout.Render(40);
      string joined = string.Join('\n', lines);
      joined.ShouldContain("Top");
      joined.ShouldContain("Bottom");

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.FlexLayout.RowColumn
