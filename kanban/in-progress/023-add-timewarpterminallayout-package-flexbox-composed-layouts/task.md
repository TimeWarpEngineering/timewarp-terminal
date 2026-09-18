# Add TimeWarp.Terminal.Layout package - flexbox-composed layouts

## Description

New companion package `TimeWarp.Terminal.Layout` (in this repo as
`source/timewarp-terminal-layout/`) that composes existing widgets — panels, tables,
rules, text — into flexbox layouts using TimeWarp.Flexbox (the C# Yoga port, verified
against Yoga's 530-test conformance suite). Fills the biggest functional gap vs
Spectre.Console: today every widget renders solo at full terminal width; there is no
way to put two panels side by side, build a status-bar row, or compose a dashboard.

One-shot render for ordinary scrolling CLI output (the `dev`/`ganda` style), NOT an
interactive full-screen framework — that is timewarp-tui's job. This package is also
the low-stakes proving ground for the float-layout → character-cell integration that
timewarp-tui will inherit.

Companion package (not a TimeWarp.Terminal dependency) keeps Terminal's dependency
surface at exactly one stable package for users who just want colored output.

## Gates (must clear before work starts)

- [x] TimeWarp.Flexbox published as a STABLE release on public nuget.org
      (1.0.0 on nuget.org; Layout depends on that public package, not GitHub Packages)
- [x] TimeWarp.Flexbox passes trim/AOT analysis (package is IsAotCompatible;
      Layout is also IsAotCompatible / trim+AOT analyzers and does not add Flexbox
      to TimeWarp.Terminal)

## Checklist

- [x] Create `source/timewarp-terminal-layout/` project (IsPackable, same strict
      analyzer set, PackageReadmeFile/snupkg like the main package)
- [x] Solve float → character-cell rounding: Yoga computes float positions/sizes;
      terminal cells are integers. Decide the rounding contract (Yoga has pixel-grid
      rounding — evaluate whether PointScaleFactor=1 gives stable integer cells) and
      pin it with tests: adjacent items must tile exactly (no gaps/overlaps), total
      width must equal the container width
- [x] Design the item model: existing widgets (Panel, Table, Rule) plus raw text as
      flex items; widgets need measure functions (content min/max width) wired to
      flexbox measure callbacks, using UnicodeWidth/AnsiStringUtils for visible width
- [x] LayoutBuilder API consistent with existing builders (see sketch below);
      `terminal.WriteLayout(...)` extension + static facade mirror
- [x] Respect SupportsColor gating and WindowWidth like existing widget extensions
- [x] Wrapping/overflow semantics: FlexWrap for rows of cards; min-width collapse
      behavior when the terminal is too narrow (reuse the Grow-floor philosophy from
      table: items never collapse to zero silently)
- [x] Runfile tests under tests/ (layout-01-basic, row/column, grow, wrap, nesting,
      emoji/ANSI content inside items) + samples/layout-dashboard.cs
- [x] Release pipeline: pack/push both packages (workflow.cs pack step currently
      packs only timewarp-terminal.csproj); check-version must gate on both ids

## API Sketch

```csharp
terminal.WriteLayout(layout => layout
  .Direction(FlexDirection.Row)
  .Gap(2)
  .Item(i => i.Grow(1), panel => panel.Header("Build").Content(buildSummary))
  .Item(i => i.Grow(2), table => table.AddColumns("Test", "Result").AddRow(...))
);

// Column of rows (dashboard)
terminal.WriteLayout(layout => layout
  .Direction(FlexDirection.Column)
  .Row(r => r.Item(statusPanel).Item(versionPanel))
  .Row(r => r.Item(i => i.Grow(1), logTable))
);
```

## Notes

- Part of the layered stack: flexbox (pure layout math, leaf) → terminal (I/O
  foundation, shipped 1.0.0 2026-07-03) → Terminal.Layout (static composition) →
  timewarp-tui (interactive OpenTUI clone). See timewarp-tui card 262 for the
  shared-primitives decision that should land before/alongside this.
- Do NOT retrofit the table widget's internal column math onto flexbox: that code is
  correct, regression-tested, and shipped in 1.0; swapping it would churn observable
  output (rounding) for no user-visible gain.

## Session

- Created: 096d9aa9-8cec-4987-a576-91698523d859 (2026-07-03)
- Implementer: grok-4.6 (2026-09-18)

## Results

Companion package `TimeWarp.Terminal.Layout` ships from `source/timewarp-terminal-layout/`. It composes Panel, Table, Rule, and raw text into Yoga flexbox layouts (TimeWarp.Flexbox 1.0.0 on nuget.org) and writes one-shot scrolling CLI output. TimeWarp.Terminal still has no Flexbox dependency.

**Public API** (types live in `TimeWarp.Terminal`, same as Panel/Table/Rule — the package id is `TimeWarp.Terminal.Layout`):

- `ITerminal.WriteLayout(Action<LayoutBuilder>)` plus a `SupportsColor`-gated ConsoleColor overload; width is `WindowWidth`
- Static facade `TerminalLayout.WriteLayout(...)` via `Terminal.Instance`
- `LayoutBuilder` matches the task sketch (`Direction`, `Gap`, `Wrap`, `Item` overloads, nested `Row`/`Column`)
- `Layout.Render(width)` and `Layout.CalculateBoxes(width)` for output and integer-cell tests

**Rounding contract:** Config `PointScaleFactor=1`. Yoga measure funcs on unequal `Grow` items break pixel-grid tiling, so layout is two-pass: pass 1 allocates widths (grow items use flex-basis 0, no measure funcs); pass 2 pins integer width/height with grow/shrink frozen. `ToCell` requires values within 0.001 of an integer. Adjacent items tile; last.X+Width equals container width (pinned in `tests/layout-07-rounding.cs`).

**Min-width floor:** bordered Panel/Table ≥ 4 cells, text ≥ 1 (max grapheme), titled Rule ≥ title+4. Default `FlexShrink=1` so items can shrink to the floor, never to 0. Table column math is unchanged — tables still `Render(assignedWidth)`.

**Release:** `workflow.cs` discovers packable projects via `IPackableProjectService`, checks every package id at the shared `source/Directory.Build.props` version (fail only if *all* are already published; partial is resume), and packs each project. `dev check-version` already derived the packable set.

Version left at **1.0.1** (first Layout nupkg at repo version; Terminal 1.0.1 is already on nuget.org, so a release of this commit is a partial publish of Layout only).

### Files changed

- `source/timewarp-terminal-layout/` — new packable project
- `Directory.Packages.props` — TimeWarp.Flexbox 1.0.0
- `timewarp-terminal.slnx` — layout project
- `tools/dev-cli/endpoints/workflow.cs` — multi-package check-version + pack
- `tests/layout-01-basic.cs` … `layout-10-widgets.cs`
- `tests/Directory.Build.props` — layout project reference
- `samples/layout-dashboard.cs`
- `readme.md`, `skills/terminal/SKILL.md`, package README

### Test outcomes

- `dotnet build timewarp-terminal.slnx -c Release` — 0 warnings / 0 errors
- layout-01 through layout-10 — all passed
- `tests/panel-widget-01-basic.cs` and `tests/table-widget-07-grow.cs` — passed (no table-math retrofit)
- `dotnet pack` produces `TimeWarp.Terminal.Layout.1.0.1.nupkg` + snupkg
- `samples/layout-dashboard.cs` — exit 0; two panels side by side plus a table

### How to validate

**Smoke**

```bash
cd /path/to/timewarp-terminal
dotnet build timewarp-terminal.slnx -c Release
dotnet tests/layout-01-basic.cs
dotnet tests/layout-07-rounding.cs
dotnet tests/layout-08-min-width.cs
(cd samples && dotnet run layout-dashboard.cs -- --help)
dotnet pack source/timewarp-terminal-layout/timewarp-terminal-layout.csproj -c Release -o artifacts/packages
```

**Expect**

- Build: `0 Warning(s)`, `0 Error(s)`
- layout-01: `Passed: 1` (Left/Right present, same Y, increasing X)
- layout-07: `Passed: 3` (grow+gap, odd width 81, three equal grow items tile to container width)
- layout-08: `Passed: 1` (two bordered panels at width 10 each have Width ≥ 4)
- sample: exit 0; two headered panels on one row, then a test-results table
- pack: `artifacts/packages/TimeWarp.Terminal.Layout.1.0.1.nupkg` exists (and `.snupkg`)

**Automated gate**

```bash
# from repo root — Jaribu runfiles, not `dotnet test` on the .slnx
for f in tests/layout-*.cs; do dotnet "$f" || exit 1; done
./bin/dev test          # full tests/ suite including layout-*
./bin/dev verify-samples
```

**Depends on:** .NET 10 SDK; TimeWarp.Flexbox 1.0.0 from nuget.org (central pin in `Directory.Packages.props`).

**Not in scope:** publishing to nuget.org (host open-pr / release workflow); interactive full-screen TUI (timewarp-tui); retrofitting table column math onto flexbox.
