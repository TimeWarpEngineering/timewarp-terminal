#!/usr/bin/env -S dotnet --
// layout-dashboard - Demonstrates flexbox-composed terminal layouts
#:project $(SourceDirectory)timewarp-terminal-layout/timewarp-terminal-layout.csproj

using TimeWarp.Flexbox;
using TimeWarp.Terminal;
using TimeWarp.Terminal.Layout;

TimeWarpTerminal terminal = TimeWarpTerminal.Default;

terminal
  .WriteLine()
  .WriteLine("Layout Dashboard Demo".Cyan().Bold())
  .WriteLine("Column of a status-bar row and a grow table")
  .WriteLine();

terminal.WriteLayout(layout => layout
  .Direction(FlexDirection.Column)
  .Gap(1)
  .Row(row => row
    .Gap(2)
    .Item(i => i.Grow(1), panel => panel
      .Header("Status".Green())
      .Content("Build: OK\nTests: 12 passed")
      .BorderColor(AnsiColors.Green))
    .Item(i => i.Grow(1), panel => panel
      .Header("Version".Cyan())
      .Content("TimeWarp.Terminal.Layout\n1.0.1")
      .BorderColor(AnsiColors.Cyan)))
  .Row(row => row
    .Item(i => i.Grow(1), table => table
      .AddColumns("Suite", "Result", "Duration")
      .AddRow("unit", "pass".Green(), "0.4s")
      .AddRow("integration", "pass".Green(), "1.2s")
      .AddRow("samples", "pass".Green(), "0.8s"))));

terminal.WriteLine();

return 0;
