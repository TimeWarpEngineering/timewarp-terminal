#!/usr/bin/dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Tests for CancelKeyPress event on ITerminal
#pragma warning disable CA1508 // Avoid dead conditional code

using TimeWarp.Terminal;

// Create test terminal
using TestTerminal terminal = new();

bool eventRaised = false;
ConsoleSpecialKey receivedSpecialKey = ConsoleSpecialKey.ControlC;
bool receivedCancel = false;

// Subscribe to CancelKeyPress
terminal.CancelKeyPress += Handler;

// Simulate Ctrl+C
terminal.SimulateCancelKeyPress(ConsoleSpecialKey.ControlC);

// Verify event was raised
if (!eventRaised)
{
  Console.WriteLine("❌ FAILED: Event was not raised");
  return;
}

if (receivedSpecialKey != ConsoleSpecialKey.ControlC)
{
  Console.WriteLine($"❌ FAILED: Expected ControlC, got {receivedSpecialKey}");
  return;
}

Console.WriteLine("✓ Event raised correctly");
Console.WriteLine($"✓ SpecialKey: {receivedSpecialKey}");

// Test Ctrl+Break
eventRaised = false;
terminal.SimulateCancelKeyPress(ConsoleSpecialKey.ControlBreak);

if (!eventRaised || receivedSpecialKey != ConsoleSpecialKey.ControlBreak)
{
  Console.WriteLine("❌ FAILED: Ctrl+Break test failed");
  return;
}

Console.WriteLine("✓ Ctrl+Break works correctly");

// Test that Cancel property can be read
eventRaised = false;
terminal.SimulateCancelKeyPress();

Console.WriteLine($"✓ Cancel property accessible (initial: {receivedCancel})");

// Unsubscribe and verify no event raised
terminal.CancelKeyPress -= Handler;
eventRaised = false;
terminal.SimulateCancelKeyPress();

if (eventRaised)
{
  Console.WriteLine("❌ FAILED: Event raised after unsubscribe");
  return;
}

Console.WriteLine("✓ Unsubscribe works correctly");

// Test with no handler (should not throw)
using TestTerminal terminal2 = new();
terminal2.SimulateCancelKeyPress(); // No handler attached
Console.WriteLine("✓ No exception when no handler attached");

Console.WriteLine("\n🧪 All CancelKeyPress tests passed!");

void Handler(object? sender, ConsoleCancelEventArgs args)
{
  eventRaised = true;
  receivedSpecialKey = args.SpecialKey;
  receivedCancel = args.Cancel;
}
