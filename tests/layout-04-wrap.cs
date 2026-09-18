#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal-layout/timewarp-terminal-layout.csproj

// Test FlexWrap onto a second row

using TimeWarp.Flexbox;

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.FlexLayout.Wrap
{

  [TestTag("Layout")]
  public class LayoutWrapTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<LayoutWrapTests>();

    public static async Task Should_wrap_items_that_do_not_fit_one_row()
    {
      Layout layout = new LayoutBuilder()
        .Direction(FlexDirection.Row)
        .Wrap(TimeWarp.Flexbox.Wrap.Wrap)
        .Gap(1)
        .Item(i => i.Width(8), "One")
        .Item(i => i.Width(8), "Two")
        .Item(i => i.Width(8), "Three")
        .Build();

      IReadOnlyList<LayoutBox> boxes = layout.CalculateBoxes(20);
      boxes.Count.ShouldBe(3);
      boxes[0].Y.ShouldBe(boxes[1].Y);
      boxes[2].Y.ShouldBeGreaterThan(boxes[0].Y);

      string[] lines = layout.Render(20);
      string joined = string.Join('\n', lines);
      joined.ShouldContain("One");
      joined.ShouldContain("Two");
      joined.ShouldContain("Three");

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.FlexLayout.Wrap
