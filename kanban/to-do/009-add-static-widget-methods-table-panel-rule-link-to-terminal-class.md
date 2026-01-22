# Add static widget methods (Table, Panel, Rule, Link) to Terminal class

## Description

Add static methods to the `Terminal` class that provide direct access to widget functionality (Table, Panel, Rule, Link) without requiring an instance. This enables users to use widgets with the same simple syntax as basic output methods.

## Checklist

- [ ] Add `WriteTable(Action<TableBuilder> configure)` static method
- [ ] Add `WriteTable(Table table)` static method
- [ ] Add `WritePanel(Action<PanelBuilder> configure)` static method
- [ ] Add `WritePanel(string content, string? header = null)` static method
- [ ] Add `WriteRule(string? title = null)` static method
- [ ] Add `WriteRule(Action<RuleBuilder> configure)` static method
- [ ] Add `WriteLink(string url, string text)` static method
- [ ] Add XML documentation for all widget methods
- [ ] Write unit tests for widget static methods

## Notes

Reference existing extension method implementations in `source/timewarp-terminal/widgets/`.

Example usage:
```csharp
using static TimeWarp.Terminal.Terminal;

// Simple table with builder
WriteTable(t => t
    .AddColumns("Name", "Stars")
    .AddRow("CleanArchitecture", "16.5k")
    .AddRow("GuardClauses", "3.2k"));

// Panel with content
WritePanel("Configuration loaded successfully", "Settings");

// Rule separator
WriteRule("Section Title");

// Hyperlink
WriteLink("https://example.com", "Click here");
```
