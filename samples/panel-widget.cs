#!/usr/bin/dotnet --
// panel-widget-demo - Demonstrates the Panel widget for bordered boxes
// GitHub Issue: https://github.com/TimeWarpEngineering/timewarp-terminal/issues/90
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

using TimeWarp.Terminal;

// Get a terminal instance
TimeWarpTerminal terminal = TimeWarpTerminal.Default;

terminal
  .WriteLine()
  .WriteLine("Panel Widget Demo".Cyan().Bold())
  .WriteLine("Demonstrates bordered boxes with optional headers and styled content")
  .WriteLine();

// Simple panel with content
terminal
  .WriteLine("1. Simple Panel:")
  .WritePanel("This is important information")
  .WriteLine();

// Panel with header
terminal
  .WriteLine("2. Panel with header:")
  .WritePanel("Content goes here", "Notice")
  .WriteLine();

// Different border styles
terminal
  .WriteLine("3. Different border styles:")
  .WriteLine()
  .WriteLine("   Rounded (default):")
  .WritePanel(panel => panel
    .Header("Rounded")
    .Content("Soft corners ╭╮╰╯")
    .Border(BorderStyle.Rounded))
  .WriteLine()
  .WriteLine("   Square:")
  .WritePanel(panel => panel
    .Header("Square")
    .Content("Sharp corners ┌┐└┘")
    .Border(BorderStyle.Square))
  .WriteLine()
  .WriteLine("   Double:")
  .WritePanel(panel => panel
    .Header("Double")
    .Content("Double lines ╔╗╚╝")
    .Border(BorderStyle.Doubled))
  .WriteLine()
  .WriteLine("   Heavy:")
  .WritePanel(panel => panel
    .Header("Heavy")
    .Content("Thick lines ┏┓┗┛")
    .Border(BorderStyle.Heavy))
  .WriteLine();

// Multi-line content
terminal
  .WriteLine("4. Multi-line content:")
  .WritePanel(panel => panel
    .Header("Team Members")
    .Content("Alice - Developer\nBob - Designer\nCharlie - Manager")
    .Border(BorderStyle.Rounded))
  .WriteLine();

// Padding options
terminal
  .WriteLine("5. Padding options:")
  .WriteLine()
  .WriteLine("   Default padding (horizontal=1, vertical=0):")
  .WritePanel("Compact")
  .WriteLine()
  .WriteLine("   More padding (horizontal=3, vertical=1):")
  .WritePanel(panel => panel
    .Content("Spacious content")
    .Padding(3, 1))
  .WriteLine();

// Colored borders
terminal
  .WriteLine("6. Colored borders:")
  .WritePanel(panel => panel
    .Header("Success".Green())
    .Content("Operation completed successfully")
    .BorderColor(AnsiColors.Green))
  .WritePanel(panel => panel
    .Header("Warning".Yellow())
    .Content("Proceed with caution")
    .BorderColor(AnsiColors.Yellow))
  .WritePanel(panel => panel
    .Header("Error".Red())
    .Content("Something went wrong")
    .BorderColor(AnsiColors.Red))
  .WriteLine();

// Fixed width panel
terminal
  .WriteLine("7. Fixed width panel (30 characters):")
  .WritePanel(panel => panel
    .Header("Fixed")
    .Content("30 chars wide")
    .Width(30))
  .WriteLine();

// Styled header and content
terminal
  .WriteLine("8. Styled header and content:")
  .WritePanel(panel => panel
    .Header("💠 Ardalis".Cyan().Bold())
    .Content("Steve 'Ardalis' Smith\n" + "Software Architect".Gray())
    .Border(BorderStyle.Rounded)
    .BorderColor(AnsiColors.Cyan)
    .Padding(2, 1))
  .WriteLine();

// Pre-configured panel via builder
terminal
  .WriteLine("9. Pre-configured Panel via builder:")
  .WritePanel(panel => panel
    .Header("Configuration")
    .Content("Environment: Production\nDebug: false\nVersion: 1.0.0")
    .Border(BorderStyle.Doubled)
    .BorderColor(AnsiColors.Magenta)
    .PaddingHorizontal(2)
    .PaddingVertical(1))
  .WriteLine();

// Practical example — fluent chaining
terminal
  .WriteLine("10. Practical example - fluent chaining:")
  .WriteLine()
  .WriteRule("Build Summary")
  .WritePanel(panel => panel
    .Header("Build Status".Bold())
    .Content($"{"Project:".Gray()}  TimeWarp.Terminal\n" +
             $"{"Status:".Gray()}   {"✓ Success".Green()}\n" +
             $"{"Duration:".Gray()} 2.34s")
    .Border(BorderStyle.Rounded)
    .BorderColor(AnsiColors.BrightGreen)
    .Padding(2, 1))
  .WriteLine();

return 0;
