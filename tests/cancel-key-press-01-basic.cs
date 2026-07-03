#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Tests for CancelKeyPress event on ITerminal
#pragma warning disable CA1508 // Avoid dead conditional code

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

// Test static Terminal facade forwards CancelKeyPress add/remove to the current Instance
ITerminal originalInstance = Terminal.Instance;
using TestTerminal facadeTerminal = new();
bool staticEventRaised = false;
ConsoleSpecialKey staticSpecialKey = ConsoleSpecialKey.ControlBreak;

try
{
  Terminal.Instance = facadeTerminal;

  // Subscribe via the static facade; the handler lands on the instance
  Terminal.CancelKeyPress += StaticHandler;
  facadeTerminal.SimulateCancelKeyPress(ConsoleSpecialKey.ControlC);

  if (!staticEventRaised || staticSpecialKey != ConsoleSpecialKey.ControlC)
  {
    Console.WriteLine("❌ FAILED: Static facade add did not forward to Instance");
    return;
  }

  Console.WriteLine("✓ Static Terminal.CancelKeyPress add forwards to Instance");

  // Unsubscribe via the static facade; the handler is removed from the instance
  Terminal.CancelKeyPress -= StaticHandler;
  staticEventRaised = false;
  facadeTerminal.SimulateCancelKeyPress();

  if (staticEventRaised)
  {
    Console.WriteLine("❌ FAILED: Static facade remove did not forward to Instance");
    return;
  }

  Console.WriteLine("✓ Static Terminal.CancelKeyPress remove forwards to Instance");
}
finally
{
  Terminal.Instance = originalInstance;
}

Console.WriteLine("\n🧪 All CancelKeyPress tests passed!");

void Handler(object? sender, ConsoleCancelEventArgs args)
{
  eventRaised = true;
  receivedSpecialKey = args.SpecialKey;
  receivedCancel = args.Cancel;
}

void StaticHandler(object? sender, ConsoleCancelEventArgs args)
{
  staticEventRaised = true;
  staticSpecialKey = args.SpecialKey;
}
