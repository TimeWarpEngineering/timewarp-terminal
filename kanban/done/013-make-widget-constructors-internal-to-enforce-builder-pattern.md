# Make widget constructors internal to enforce builder pattern

## Description

Make Table, Panel, and Rule constructors internal so users must go through TableBuilder, PanelBuilder, and RuleBuilder. This eliminates the broken-chain DX where users start fluent chaining with `new Table().AddColumn().AddRow()` then have to break out to property setters for Border, ShowHeaders, BorderColor, etc. The builders already have the full fluent API — this just removes the bad path.

Follows the same pattern as Nuru's `EndpointBuilder` where `.Done()` returns to the parent and the chain never breaks.

## Checklist

- [x] Make Table constructor internal
- [x] Make Panel constructor internal
- [x] Make Rule constructor internal
- [x] Update all samples to use builder pattern (`Action<XxxBuilder>` or `new XxxBuilder()`)
- [x] Update tests to use builder pattern
- [x] Update README.md examples
- [x] Update XML doc comments in source files
- [x] Delete NestedXxxBuilder types (dead code — zero references)
- [x] Make Table fluent methods (AddColumn, AddColumns, AddRow) internal to prevent post-build mutation
