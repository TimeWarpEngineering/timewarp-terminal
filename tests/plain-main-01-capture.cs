#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

#region Purpose
// Compiles the README plain-Main sample: IConsole and ITerminal writes asserted with TestTerminal.
#endregion

#region Design
// The method bodies match readme.md "Testing a plain Main".
// AppStatus takes IConsole. Greeting takes ITerminal. One TestTerminal is passed to both.
#endregion

// CA1849: The README sample calls sync WriteLine.
#pragma warning disable CA1849

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.PlainMain
{

  /// <summary>
  /// README plain <c>Main</c> sample: write through <see cref="IConsole"/> and <see cref="ITerminal"/>,
  /// then assert with <see cref="TestTerminal"/>.
  /// </summary>
  [TestTag("PlainMain")]
  public class PlainMainCaptureTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<PlainMainCaptureTests>();

    public static async Task Should_capture_iconsole_and_iterminal_output()
    {
      using TestTerminal terminal = new();

      AppStatus.Write(terminal, "ready");
      Greeting.Write(terminal, "World");

      terminal.OutputContains("ready").ShouldBeTrue();
      terminal.OutputContains("Hello, World").ShouldBeTrue();

      await Task.CompletedTask;
    }
  }

  static class AppStatus
  {
    public static void Write(IConsole console, string status)
    {
      console.WriteLine(status);
    }
  }

  static class Greeting
  {
    public static void Write(ITerminal terminal, string name)
    {
      terminal.WriteLine($"Hello, {name}");
    }
  }
}
