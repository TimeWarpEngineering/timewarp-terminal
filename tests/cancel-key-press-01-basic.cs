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

  // Subscribe via the static facade; the facade forwarder is attached to the instance
  Terminal.CancelKeyPress += StaticHandler;
  facadeTerminal.SimulateCancelKeyPress(ConsoleSpecialKey.ControlC);

  if (!staticEventRaised || staticSpecialKey != ConsoleSpecialKey.ControlC)
  {
    Console.WriteLine("❌ FAILED: Static facade add did not forward to Instance");
    return;
  }

  Console.WriteLine("✓ Static Terminal.CancelKeyPress add forwards to Instance");

  // Unsubscribe via the static facade; the facade forwarder is detached from the instance
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
  Terminal.CancelKeyPress -= StaticHandler;
  Terminal.Instance = originalInstance;
}

// Process-global Instance assignment moves the facade forwarder
using TestTerminal firstTerminal = new();
using TestTerminal secondTerminal = new();
bool reboundRaised = false;

try
{
  Terminal.Instance = firstTerminal;
  Terminal.CancelKeyPress += ReboundHandler;
  Terminal.Instance = secondTerminal;

  secondTerminal.SimulateCancelKeyPress();
  if (!reboundRaised)
  {
    Console.WriteLine("❌ FAILED: Forwarder did not follow process-global Instance assignment");
    return;
  }

  reboundRaised = false;
  firstTerminal.SimulateCancelKeyPress();
  if (reboundRaised)
  {
    Console.WriteLine("❌ FAILED: Forwarder remained on the previous Instance after reassignment");
    return;
  }

  Console.WriteLine("✓ Static Terminal.CancelKeyPress forwarder follows Instance assignment");
}
finally
{
  Terminal.CancelKeyPress -= ReboundHandler;
  Terminal.Instance = originalInstance;
}

// Subscribe inside Use, unsubscribe after the scope ends — must not leak on the test terminal
using TestTerminal scopedTerminal = new();
bool scopedRaised = false;

try
{
  using (TestTerminalContext.Use(scopedTerminal))
  {
    Terminal.CancelKeyPress += ScopedHandler;
    scopedTerminal.SimulateCancelKeyPress();
    if (!scopedRaised)
    {
      Console.WriteLine("❌ FAILED: Subscribe inside Use did not attach to the scoped terminal");
      return;
    }
  }

  Terminal.CancelKeyPress -= ScopedHandler;
  scopedRaised = false;
  scopedTerminal.SimulateCancelKeyPress();
  if (scopedRaised)
  {
    Console.WriteLine("❌ FAILED: Unsubscribe after Use leaked the handler on the test terminal");
    return;
  }

  Console.WriteLine("✓ Static Terminal.CancelKeyPress unsubscribe after Use does not leak");
}
finally
{
  Terminal.CancelKeyPress -= ScopedHandler;
}

// Process-global Instance assignment inside Use rebinds to the assigned field, not Current
using TestTerminal useScopeTerminal = new();
using TestTerminal assignedTerminal = new();
bool assignedRaised = false;

try
{
  using (TestTerminalContext.Use(useScopeTerminal))
  {
    Terminal.CancelKeyPress += AssignedHandler;
    Terminal.Instance = assignedTerminal;

    assignedTerminal.SimulateCancelKeyPress();
    if (!assignedRaised)
    {
      Console.WriteLine("❌ FAILED: Forwarder did not follow process-global Instance assignment inside Use");
      return;
    }

    assignedRaised = false;
    useScopeTerminal.SimulateCancelKeyPress();
    if (assignedRaised)
    {
      Console.WriteLine("❌ FAILED: Forwarder stayed on the Use-scoped terminal after Instance assignment");
      return;
    }
  }

  assignedRaised = false;
  assignedTerminal.SimulateCancelKeyPress();
  if (!assignedRaised)
  {
    Console.WriteLine("❌ FAILED: Forwarder left the process-global Instance after Use ended");
    return;
  }

  Console.WriteLine("✓ Static Terminal.CancelKeyPress assignment inside Use follows process-global Instance");
}
finally
{
  Terminal.CancelKeyPress -= AssignedHandler;
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

void ReboundHandler(object? sender, ConsoleCancelEventArgs args)
{
  reboundRaised = true;
}

void ScopedHandler(object? sender, ConsoleCancelEventArgs args)
{
  scopedRaised = true;
}

void AssignedHandler(object? sender, ConsoleCancelEventArgs args)
{
  assignedRaised = true;
}
