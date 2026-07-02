#!/usr/bin/env -S dotnet --
// rule-widget-demo - Demonstrates the Rule widget for horizontal divider lines
// GitHub Issue: https://github.com/TimeWarpEngineering/timewarp-terminal/issues/89
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

using TimeWarp.Terminal;

// Get a terminal instance
TimeWarpTerminal terminal = TimeWarpTerminal.Default;

terminal
  .WriteLine()
  .WriteLine("Rule Widget Demo".Cyan().Bold())
  .WriteLine("Demonstrates horizontal divider lines with optional centered text")
  .WriteLine();

// Simple horizontal line
terminal
  .WriteLine("1. Simple Rule (no title):")
  .WriteRule()
  .WriteLine();

// Rule with centered title
terminal
  .WriteLine("2. Rule with centered title:")
  .WriteRule("Section Title")
  .WriteLine();

// Rule with styled title
terminal
  .WriteLine("3. Rule with styled title:")
  .WriteRule("Results".Cyan().Bold())
  .WriteLine();

// Different line styles
terminal
  .WriteLine("4. Different line styles:")
  .WriteLine()
  .WriteLine("   Thin (default):")
  .WriteRule("Thin Style", LineStyle.Thin)
  .WriteLine()
  .WriteLine("   Doubled:")
  .WriteRule("Doubled Style", LineStyle.Doubled)
  .WriteLine()
  .WriteLine("   Heavy:")
  .WriteRule("Heavy Style", LineStyle.Heavy)
  .WriteLine();

// Colored rules using styled text
terminal
  .WriteLine("5. Colored rules (using styled text):")
  .WriteRule("Success".Green())
  .WriteRule("Warning".Yellow())
  .WriteRule("Error".Red())
  .WriteLine();

// Practical example — fluent chaining
terminal
  .WriteLine("6. Practical example - fluent chaining:")
  .WriteLine()
  .WriteRule("Build Output")
  .WriteLine("  Compiling project...")
  .WriteLine("  Build succeeded.")
  .WriteLine()
  .WriteRule("Test Results", LineStyle.Doubled)
  .WriteLine("  ✓ 42 tests passed")
  .WriteLine("  ✗ 0 tests failed")
  .WriteLine();

return 0;
