#!/usr/bin/dotnet --
#:project ../../source/timewarp-terminal/timewarp-terminal.csproj

// Demonstrates the Table widget for rendering columnar data
using TimeWarp.Terminal;

// Create a terminal for colored output
ITerminal terminal = new TimeWarpTerminal();

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
  .WriteLine("Use .Shrink(false) to disable shrinking.")
  .WriteLine();

// Example 11: Disable shrinking (allow overflow)
terminal
  .WriteLine("11. Shrink Disabled (allows horizontal overflow)")
  .WriteLine("------------------------------------------------")
  .WriteTable(t => t
    .AddColumn("Path")
    .AddRow("/home/user/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/very-long-feature-branch-name")
    .Border(BorderStyle.Rounded)
    .Shrink(false))
  .WriteLine();

// Example 12: TruncateMode options
string longText = "This-is-a-very-long-text-that-will-be-truncated-differently";

terminal
  .WriteLine("12. TruncateMode Options")
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

// Example 13: Fluent chaining across Write methods
terminal
  .WriteLine("13. Fluent Chaining")
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
  .WriteLine()
  .WriteLine("Demo complete!");
