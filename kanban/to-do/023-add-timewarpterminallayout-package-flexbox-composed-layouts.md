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

- [ ] TimeWarp.Flexbox published as a STABLE release on public nuget.org
      (currently 1.0.0-beta.3 on private GitHub Packages; a public package cannot
      depend on it — same NU5104/private-feed gate as the TimeWarp.Builder story)
- [ ] TimeWarp.Flexbox passes trim/AOT analysis (Terminal earns its IsAotCompatible
      claim; the layout package must not regress that)

## Checklist

- [ ] Create `source/timewarp-terminal-layout/` project (IsPackable, same strict
      analyzer set, PackageReadmeFile/snupkg like the main package)
- [ ] Solve float → character-cell rounding: Yoga computes float positions/sizes;
      terminal cells are integers. Decide the rounding contract (Yoga has pixel-grid
      rounding — evaluate whether PointScaleFactor=1 gives stable integer cells) and
      pin it with tests: adjacent items must tile exactly (no gaps/overlaps), total
      width must equal the container width
- [ ] Design the item model: existing widgets (Panel, Table, Rule) plus raw text as
      flex items; widgets need measure functions (content min/max width) wired to
      flexbox measure callbacks, using UnicodeWidth/AnsiStringUtils for visible width
- [ ] LayoutBuilder API consistent with existing builders (see sketch below);
      `terminal.WriteLayout(...)` extension + static facade mirror
- [ ] Respect SupportsColor gating and WindowWidth like existing widget extensions
- [ ] Wrapping/overflow semantics: FlexWrap for rows of cards; min-width collapse
      behavior when the terminal is too narrow (reuse the Grow-floor philosophy from
      table: items never collapse to zero silently)
- [ ] Runfile tests under tests/ (layout-01-basic, row/column, grow, wrap, nesting,
      emoji/ANSI content inside items) + samples/layout-dashboard.cs
- [ ] Release pipeline: pack/push both packages (workflow.cs pack step currently
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
