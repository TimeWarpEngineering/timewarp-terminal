#!/usr/bin/env -S dotnet --
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj

// Tests for TestTerminalContext integration with Terminal.Instance
#pragma warning disable CA1849

#if !JARIBU_MULTI
return await RunAllTests();
#endif

namespace TimeWarp.Terminal.Tests.Core.TestTerminalContextIntegration
{

  [TestTag("TestTerminalContext")]
  public class TestTerminalContextIntegrationTests
  {
    [ModuleInitializer]
    internal static void Register() => RegisterTests<TestTerminalContextIntegrationTests>();

    public static async Task Should_set_terminal_instance_when_context_set()
    {
      // Arrange
      ITerminal original = TimeWarp.Terminal.Terminal.Instance;
      using TestTerminal testTerminal = new();

      // Act
      TestTerminalContext.SetCurrent(testTerminal);

      // Assert
      TimeWarp.Terminal.Terminal.Instance.ShouldBe(testTerminal);

      // Cleanup
      TestTerminalContext.ClearCurrent();
      TimeWarp.Terminal.Terminal.Instance = original;

      await Task.CompletedTask;
    }

    public static async Task Should_restore_terminal_instance_when_context_cleared()
    {
      // Arrange
      ITerminal original = TimeWarp.Terminal.Terminal.Instance;
      using TestTerminal testTerminal = new();
      TestTerminalContext.SetCurrent(testTerminal);

      // Act
      TestTerminalContext.ClearCurrent();

      // Assert
      TimeWarp.Terminal.Terminal.Instance.ShouldBe(original);

      await Task.CompletedTask;
    }

    public static async Task Should_restore_terminal_instance_on_dispose()
    {
      // Arrange
      ITerminal original = TimeWarp.Terminal.Terminal.Instance;

      // Act
      using (TestTerminal testTerminal = new())
      {
        TestTerminalContext.SetCurrent(testTerminal);
        TimeWarp.Terminal.Terminal.Instance.ShouldBe(testTerminal);
      }

      // Assert - Terminal.Instance should be restored after dispose
      TimeWarp.Terminal.Terminal.Instance.ShouldBe(original);

      await Task.CompletedTask;
    }

    public static async Task Should_route_static_terminal_calls_to_test_terminal()
    {
      // Arrange
      ITerminal original = TimeWarp.Terminal.Terminal.Instance;
      using TestTerminal testTerminal = new();
      TestTerminalContext.SetCurrent(testTerminal);

      // Act
      TimeWarp.Terminal.Terminal.WriteLine("Hello from static API");

      // Assert
      testTerminal.OutputContains("Hello from static API").ShouldBeTrue();

      // Cleanup
      TestTerminalContext.ClearCurrent();
      TimeWarp.Terminal.Terminal.Instance = original;

      await Task.CompletedTask;
    }

    public static async Task Should_provide_terminal_property()
    {
      // Arrange
      using TestTerminal testTerminal = new();
      TestTerminalContext.SetCurrent(testTerminal);

      // Act
      TestTerminal retrieved = TestTerminalContext.Terminal;

      // Assert
      retrieved.ShouldBe(testTerminal);

      // Cleanup
      TestTerminalContext.ClearCurrent();

      await Task.CompletedTask;
    }

    public static async Task Should_throw_when_terminal_accessed_without_context()
    {
      // Arrange - ensure no context
      TestTerminalContext.ClearCurrent();

      // Act & Assert
      Should.Throw<InvalidOperationException>(() => _ = TestTerminalContext.Terminal);

      await Task.CompletedTask;
    }

    public static async Task Should_restore_on_use_scope_dispose()
    {
      // Arrange
      ITerminal original = TimeWarp.Terminal.Terminal.Instance;
      using TestTerminal testTerminal = new();

      // Act
      using (IDisposable scope = TestTerminalContext.Use(testTerminal))
      {
        TimeWarp.Terminal.Terminal.Instance.ShouldBe(testTerminal);
        TimeWarp.Terminal.Terminal.WriteLine("Scoped output");
      }

      // Assert
      testTerminal.OutputContains("Scoped output").ShouldBeTrue();
      TimeWarp.Terminal.Terminal.Instance.ShouldBe(original);

      await Task.CompletedTask;
    }
  }

}
