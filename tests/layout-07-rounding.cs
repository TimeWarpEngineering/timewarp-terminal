#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal-layout/timewarp-terminal-layout.csproj

// Pin CalculateBoxes tiling contract: no gaps/overlaps, total width = container

using TimeWarp.Flexbox;

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.FlexLayout.Rounding
{

  [TestTag("Layout")]
  public class LayoutRoundingTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<LayoutRoundingTests>();

    public static async Task Should_tile_grow_row_with_gap()
    {
      AssertTiles(
        width: 80,
        gap: 2,
        grows: [1f, 2f]);

      await Task.CompletedTask;
    }

    public static async Task Should_tile_equal_grow_on_odd_width()
    {
      AssertTiles(
        width: 81,
        gap: 2,
        grows: [1f, 1f]);

      await Task.CompletedTask;
    }

    public static async Task Should_tile_three_equal_grow_items_without_gap()
    {
      AssertTiles(
        width: 80,
        gap: 0,
        grows: [1f, 1f, 1f]);

      await Task.CompletedTask;
    }

    private static void AssertTiles(int width, int gap, float[] grows)
    {
      LayoutBuilder builder = new LayoutBuilder()
        .Direction(FlexDirection.Row)
        .Gap(gap);

      for (int i = 0; i < grows.Length; i++)
      {
        float grow = grows[i];
        string label = $"I{i}";
        builder.Item(flex => flex.Grow(grow), label);
      }

      Layout layout = builder.Build();
      IReadOnlyList<LayoutBox> boxes = layout.CalculateBoxes(width);
      boxes.Count.ShouldBe(grows.Length);

      for (int i = 0; i < boxes.Count; i++)
      {
        boxes[i].Width.ShouldBeGreaterThan(0);
        boxes[i].X.ShouldBeGreaterThanOrEqualTo(0);
      }

      for (int i = 0; i < boxes.Count - 1; i++)
      {
        int expectedNext = boxes[i].X + boxes[i].Width + gap;
        boxes[i + 1].X.ShouldBe(expectedNext);
      }

      LayoutBox last = boxes[^1];
      (last.X + last.Width).ShouldBe(width);
    }
  }

} // namespace TimeWarp.Terminal.Tests.FlexLayout.Rounding
