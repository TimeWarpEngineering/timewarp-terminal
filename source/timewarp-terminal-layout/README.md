# TimeWarp.Terminal.Layout

Flexbox-composed terminal layouts for [TimeWarp.Terminal](https://www.nuget.org/packages/TimeWarp.Terminal) widgets.

This is a **companion package** — it is not pulled in by `TimeWarp.Terminal`. Add it when you need side-by-side panels, status-bar rows, or dashboard-style composition. Layout uses [TimeWarp.Flexbox](https://www.nuget.org/packages/TimeWarp.Flexbox) (Yoga) for AOT-safe flexbox math and renders one-shot scrolling CLI output (not an interactive TUI).

## Installation

```bash
dotnet add package TimeWarp.Terminal.Layout
```

## Quick Start

```csharp
using TimeWarp.Terminal;
using TimeWarp.Flexbox;

ITerminal terminal = TimeWarpTerminal.Default;

terminal.WriteLayout(layout => layout
  .Direction(FlexDirection.Row)
  .Gap(2)
  .Item(i => i.Grow(1), panel => panel.Header("Build").Content("OK"))
  .Item(i => i.Grow(2), table => table
    .AddColumns("Test", "Result")
    .AddRow("unit", "pass")));

// Column of rows (dashboard)
terminal.WriteLayout(layout => layout
  .Direction(FlexDirection.Column)
  .Row(r => r.Item(statusPanel).Item(versionPanel))
  .Row(r => r.Item(i => i.Grow(1), logTable)));
```

## Notes

- Sizing uses `ITerminal.WindowWidth`.
- Color overloads respect `ITerminal.SupportsColor`.
- Items never collapse to width 0; bordered widgets keep a min-width floor of 4 cells.
