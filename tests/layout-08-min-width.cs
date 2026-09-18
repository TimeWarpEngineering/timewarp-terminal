#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal-layout/timewarp-terminal-layout.csproj

// Narrow terminal: bordered panels must not collapse to width 0

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.FlexLayout.MinWidth
{

  [TestTag("Layout")]
  public class LayoutMinWidthTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<LayoutMinWidthTests>();

    public static async Task Should_keep_bordered_panels_above_zero_width()
    {
      Layout layout = new LayoutBuilder()
        .Direction(FlexDirection.Row)
        .Gap(1)
        .Item(i => i.Grow(1), panel => panel.Header("A").Content("one"))
        .Item(i => i.Grow(1), panel => panel.Header("B").Content("two"))
        .Build();

      IReadOnlyList<LayoutBox> boxes = layout.CalculateBoxes(10);
      boxes.Count.ShouldBe(2);
      boxes[0].Width.ShouldBeGreaterThan(0);
      boxes[1].Width.ShouldBeGreaterThan(0);
      boxes[0].Width.ShouldBeGreaterThanOrEqualTo(4);
      boxes[1].Width.ShouldBeGreaterThanOrEqualTo(4);

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.FlexLayout.MinWidth
