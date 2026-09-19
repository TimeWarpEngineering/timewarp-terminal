#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal-layout/timewarp-terminal-layout.csproj

// Test basic side-by-side text layout

using TimeWarp.Flexbox;

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.FlexLayout.Basic
{

  [TestTag("Layout")]
  public class LayoutBasicTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<LayoutBasicTests>();

    public static async Task Should_render_two_text_items_side_by_side()
    {
      Layout layout = new LayoutBuilder()
        .Direction(FlexDirection.Row)
        .Gap(2)
        .Item("Left")
        .Item("Right")
        .Build();

      string[] lines = layout.Render(40);

      string joined = string.Join('\n', lines);
      joined.ShouldContain("Left");
      joined.ShouldContain("Right");

      IReadOnlyList<LayoutBox> boxes = layout.CalculateBoxes(40);
      boxes.Count.ShouldBe(2);
      boxes[0].Y.ShouldBe(boxes[1].Y);
      boxes[0].X.ShouldBeLessThan(boxes[1].X);

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.FlexLayout.Basic
