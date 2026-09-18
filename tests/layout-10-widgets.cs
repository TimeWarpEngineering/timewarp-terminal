#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal-layout/timewarp-terminal-layout.csproj

// Test Panel, Table, Rule as layout items

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.FlexLayout.Widgets
{

  [TestTag("Layout")]
  public class LayoutWidgetTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<LayoutWidgetTests>();

    public static async Task Should_render_panel_table_and_rule_content()
    {
      Layout layout = new LayoutBuilder()
        .Direction(FlexDirection.Column)
        .Gap(1)
        .Item(panel => panel.Header("Build").Content("summary"))
        .Item(rule => rule.Title("Tests"))
        .Item(table => table
          .AddColumns("Test", "Result")
          .AddRow("unit", "pass"))
        .Build();

      string[] lines = layout.Render(60);
      string joined = string.Join('\n', lines);

      joined.ShouldContain("Build");
      joined.ShouldContain("summary");
      joined.ShouldContain("Tests");
      joined.ShouldContain("unit");
      joined.ShouldContain("pass");
      joined.ShouldContain("Result");

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.FlexLayout.Widgets
