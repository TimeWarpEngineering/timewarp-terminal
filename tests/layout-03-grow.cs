#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal-layout/timewarp-terminal-layout.csproj

// Test Grow proportions and tiling

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.FlexLayout.Grow
{

  [TestTag("Layout")]
  public class LayoutGrowTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<LayoutGrowTests>();

    public static async Task Should_allocate_grow_widths_proportionally()
    {
      const int width = 80;
      const int gap = 2;

      Layout layout = new LayoutBuilder()
        .Direction(FlexDirection.Row)
        .Gap(gap)
        .Item(i => i.Grow(1), "A")
        .Item(i => i.Grow(2), "B")
        .Build();

      IReadOnlyList<LayoutBox> boxes = layout.CalculateBoxes(width);
      boxes.Count.ShouldBe(2);

      // Available after gap: 78; ratio 1:2 => 26 and 52
      boxes[0].Width.ShouldBe(26);
      boxes[1].Width.ShouldBe(52);
      boxes[0].X.ShouldBe(0);
      boxes[1].X.ShouldBe(boxes[0].X + boxes[0].Width + gap);
      (boxes[1].X + boxes[1].Width).ShouldBe(width);

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.FlexLayout.Grow
