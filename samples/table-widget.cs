#!/usr/bin/dotnet --
#:project ../source/timewarp-terminal/timewarp-terminal.csproj

// Demonstrates the Table widget for rendering columnar data
using TimeWarp.Terminal;

// Create a terminal for colored output
TimeWarpTerminal terminal = new();

terminal
  .WriteLine("Table Widget Demo")
  .WriteLine("==================\n");

// Example 1: Basic table with two columns
terminal
  .WriteLine("1. Basic Table")
  .WriteLine("--------------")
  .WriteTable(t => t
    .AddColumn("Name")
    .AddColumn("Value")
    .AddRow("Host", "localhost")
    .AddRow("Port", "8080")
    .AddRow("Protocol", "HTTP/2"))
  .WriteLine();

// Example 2: Table with alignment
terminal
  .WriteLine("2. Table with Column Alignment")
  .WriteLine("-------------------------------")
  .WriteTable(t => t
    .AddColumn("Package")
    .AddColumn("Downloads", Alignment.Right)
    .AddColumn("Version", Alignment.Center)
    .AddRow("Ardalis.GuardClauses", "12,543,210", "5.0.0")
    .AddRow("Ardalis.Result", "8,234,567", "10.0.0")
    .AddRow("TimeWarp.Terminal", "42,000", "1.0.0"))
  .WriteLine();

// Example 3: Table with styled content
terminal
  .WriteLine("3. Table with Styled Content")
  .WriteLine("----------------------------")
  .WriteTable(t => t
    .AddColumn("Test")
    .AddColumn("Status")
    .AddRow("Unit Tests", "PASSED".Green())
    .AddRow("Integration Tests", "PASSED".Green())
    .AddRow("E2E Tests", "FAILED".Red()))
  .WriteLine();

// Example 4: Different border styles
terminal
  .WriteLine("4. Border Styles")
  .WriteLine("----------------");

string[] borderNames = ["Square", "Rounded", "Double", "Heavy", "None"];
BorderStyle[] borderStyles = [BorderStyle.Square, BorderStyle.Rounded, BorderStyle.Doubled, BorderStyle.Heavy, BorderStyle.None];

for (int i = 0; i < borderStyles.Length; i++)
{
  terminal
    .WriteLine($"\n{borderNames[i]} Border:")
    .WriteTable(t => t
      .AddColumn("A")
      .AddColumn("B")
      .AddRow("1", "2")
      .Border(borderStyles[i]));
}

terminal.WriteLine();

// Example 5: Table with colored border
terminal
  .WriteLine("5. Colored Border")
  .WriteLine("-----------------")
  .WriteTable(t => t
    .AddColumn("Project")
    .AddColumn("Status")
    .AddRow("Backend", "Running")
    .AddRow("Frontend", "Building")
    .BorderColor(AnsiColors.Cyan)
    .Border(BorderStyle.Rounded))
  .WriteLine();

// Example 6: Headerless table
terminal
  .WriteLine("6. Headerless Table")
  .WriteLine("-------------------")
  .WriteTable(t => t
    .AddColumn("Key")
    .AddColumn("Value")
    .AddRow("API_KEY", "sk-abc123...")
    .AddRow("DB_HOST", "database.example.com")
    .AddRow("CACHE_TTL", "3600")
    .HideHeaders())
  .WriteLine();

// Example 7: Table with row separators
terminal
  .WriteLine("7. Table with Row Separators")
  .WriteLine("----------------------------")
  .WriteTable(t => t
    .AddColumn("Time")
    .AddColumn("Event")
    .AddRow("09:00", "Meeting started")
    .AddRow("10:30", "Coffee break")
    .AddRow("11:00", "Presentation")
    .ShowRowSeparators())
  .WriteLine();

// Example 8: Expanded table
terminal
  .WriteLine("8. Expanded Table (fills terminal width)")
  .WriteLine("-----------------------------------------")
  .WriteTable(t => t
    .AddColumn("Name")
    .AddColumn("Description")
    .AddRow("table", "Renders columnar data")
    .AddRow("panel", "Renders bordered boxes")
    .AddRow("rule", "Renders horizontal dividers")
    .Expand()
    .Border(BorderStyle.Rounded))
  .WriteLine();

// Example 9: Fluent builder pattern
terminal
  .WriteLine("9. Fluent Builder Pattern")
  .WriteLine("-------------------------")
  .WriteTable(t => t
    .AddColumns("Method", "Endpoint", "Status")
    .AddRow("GET", "/api/users", "200")
    .AddRow("POST", "/api/orders", "201")
    .AddRow("DELETE", "/api/items/42", "404")
    .Border(BorderStyle.Rounded))
  .WriteLine();

// Example 10: Shrink to fit terminal width (default behavior)
terminal
  .WriteLine("10. Shrink to Fit Terminal Width (default)")
  .WriteLine("-------------------------------------------")
  .WriteTable(t => t
    .AddColumn("Repository")
    .AddColumn(new TableColumn("Worktree Path") { TruncateMode = TruncateMode.Start })
    .AddColumn("Branch")
    .AddRow("timewarp-terminal", "/home/user/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/feature-branch-name", "feature-xyz")
    .AddRow("timewarp-state", "/home/user/worktrees/github.com/TimeWarpEngineering/timewarp-state/main", "main")
    .Border(BorderStyle.Rounded))
  .WriteLine()
  .WriteLine("Note: Path column uses TruncateMode.Start to show the end of paths.")
  .WriteLine();

// Example 11: TruncateMode options
string longText = "This-is-a-very-long-text-that-will-be-truncated-differently";

terminal
  .WriteLine("11. TruncateMode Options")
  .WriteLine("------------------------")
  .WriteTable(t => t
    .AddColumn(new TableColumn("Mode") { MaxWidth = 8 })
    .AddColumn(new TableColumn("End (default)") { MaxWidth = 25, TruncateMode = TruncateMode.End })
    .AddColumn(new TableColumn("Start") { MaxWidth = 25, TruncateMode = TruncateMode.Start })
    .AddColumn(new TableColumn("Middle") { MaxWidth = 25, TruncateMode = TruncateMode.Middle })
    .AddRow("Result", longText, longText, longText)
    .Border(BorderStyle.Rounded))
  .WriteLine()
  .WriteLine("TruncateMode.End:    'long text...'  - Shows beginning (default)")
  .WriteLine("TruncateMode.Start:  '...long text'  - Shows end (good for paths)")
  .WriteLine("TruncateMode.Middle: 'long...text'   - Shows both ends")
  .WriteLine();

// Example 12: Fluent chaining across Write methods
terminal
  .WriteLine("12. Fluent Chaining")
  .WriteLine("-------------------")
  .WriteRule("Build Output")
  .WriteTable(t => t
    .AddColumn("Test")
    .AddColumn("Status")
    .AddRow("Unit", "PASSED".Green())
    .AddRow("Integration", "PASSED".Green())
    .AddRow("E2E", "FAILED".Red()))
  .WriteRule()
  .WriteLine("Done!")
  .WriteLine();

// Example 13: Grow column (flexbox-style)
// Like CSS flex-grow: fixed columns size to their content first,
// then the Grow column fills whatever terminal width remains.
// If fixed columns leave no room, the Grow column gets zero width
// and fixed columns shrink proportionally instead.
terminal
  .WriteLine("13. Grow Column (flex-grow semantics)")
  .WriteLine("--------------------------------------");

TableColumn[] statusColumns =
[
  new("ID"),
  new("Status"),
  new TableColumn("Description") { Grow = true }
];

terminal
  .WriteLine("Fixed columns narrow — Description gets all remaining width:")
  .WriteTable(t => t
    .AddColumns(statusColumns)
    .AddRow("1", "OK".Green(), "Short")
    .AddRow("2", "OK".Green(), "A bit longer")
    .AddRow("3", "OK".Green(), "Even longer than that")
    .Border(BorderStyle.Rounded))
  .WriteLine()
  .WriteLine("Fixed columns wide — less space left, Description is squeezed:")
  .WriteTable(t => t
    .AddColumns(statusColumns)
    .AddRow("10001", "Waiting For Review".Yellow(), "Short desc")
    .AddRow("99999", "Deployment Failed".Red(), "Another short desc")
    .AddRow("42042", "In Progress".Cyan(), "And one more")
    .Border(BorderStyle.Rounded))
  .WriteLine()
  .WriteLine("Fixed columns exceed terminal — Grow column gets zero, fixed columns shrink:")
  .WriteTable(t => t
    .AddColumns(statusColumns)
    .AddRow("feature/Cramer-2025-12-22-very-long-branch-name", "Waiting For Peer Review And Approval From The Full Teams".Yellow(), "irrelevant")
    .AddRow("feature/another-extremely-long-branch-name-here", "Deployment Failed And Needs Immediate Attention Right Now".Red(), "irrelevant")
    .Border(BorderStyle.Rounded))
  .WriteLine()
  .WriteLine("Note: ID and Status are fixed (size to content). Description is Grow (fills the rest).")
  .WriteLine("Demo complete!");
