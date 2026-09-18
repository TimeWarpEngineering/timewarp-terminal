#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal-layout/timewarp-terminal-layout.csproj

// Test column of rows (dashboard sketch)

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.FlexLayout.Nesting
{

  [TestTag("Layout")]
  public class LayoutNestingTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<LayoutNestingTests>();

    public static async Task Should_compose_column_of_rows()
    {
      Panel status = new PanelBuilder().Header("Status").Content("OK").Build();
      Panel version = new PanelBuilder().Header("Ver").Content("1.0").Build();
      Table logs = new TableBuilder()
        .AddColumns("Msg")
        .AddRow("boot")
        .Build();

      Layout layout = new LayoutBuilder()
        .Direction(FlexDirection.Column)
        .Gap(1)
        .Row(r => r.Gap(2).Item(status).Item(version))
        .Row(r => r.Item(i => i.Grow(1), logs))
        .Build();

      IReadOnlyList<LayoutBox> boxes = layout.CalculateBoxes(60);
      boxes.Count.ShouldBe(3);

      // First row: two panels side by side
      boxes[0].Y.ShouldBe(boxes[1].Y);
      boxes[0].X.ShouldBeLessThan(boxes[1].X);

      // Second row below
      boxes[2].Y.ShouldBeGreaterThan(boxes[0].Y);

      string[] lines = layout.Render(60);
      string joined = string.Join('\n', lines);
      joined.ShouldContain("Status");
      joined.ShouldContain("Ver");
      joined.ShouldContain("boot");

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.FlexLayout.Nesting
