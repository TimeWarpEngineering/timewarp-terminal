#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal-layout/timewarp-terminal-layout.csproj

// Test ITerminal WriteLayout + static TerminalLayout facade
// CA1849: We deliberately test sync methods in async test methods
#pragma warning disable CA1849

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.FlexLayout.TerminalExtensions
{

  [TestTag("Layout")]
  public class LayoutTerminalExtensionTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<LayoutTerminalExtensionTests>();

    public static async Task Should_write_layout_honoring_window_width()
    {
      using TestTerminal testTerminal = new() { WindowWidth = 50 };

      testTerminal.WriteLayout(layout => layout
        .Direction(FlexDirection.Row)
        .Gap(2)
        .Item(i => i.Grow(1), "Left")
        .Item(i => i.Grow(1), "Right"));

      testTerminal.Output.ShouldContain("Left");
      testTerminal.Output.ShouldContain("Right");

      string[] outputLines = testTerminal.Output.Split('\n', StringSplitOptions.RemoveEmptyEntries);
      foreach (string line in outputLines)
      {
        AnsiStringUtils.GetVisibleLength(line).ShouldBeLessThanOrEqualTo(50);
      }

      await Task.CompletedTask;
    }

    public static async Task Should_write_via_static_facade_using_instance_swap()
    {
      ITerminal original = Terminal.Instance;
      using TestTerminal testTerminal = new() { WindowWidth = 40 };
      Terminal.Instance = testTerminal;

      try
      {
        TerminalLayout.WriteLayout(layout => layout
          .Direction(FlexDirection.Column)
          .Item("Alpha")
          .Item("Beta"));

        testTerminal.Output.ShouldContain("Alpha");
        testTerminal.Output.ShouldContain("Beta");
      }
      finally
      {
        Terminal.Instance = original;
      }

      await Task.CompletedTask;
    }
  }

} // namespace TimeWarp.Terminal.Tests.FlexLayout.TerminalExtensions
