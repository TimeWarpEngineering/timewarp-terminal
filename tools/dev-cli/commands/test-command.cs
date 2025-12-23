// ===============================================================================
// TEST COMMAND
// ===============================================================================
// Runs the test suite.
// TODO: Implement when tests are added to the repository.

namespace DevCli.Commands;

/// <summary>
/// Run the test suite.
/// </summary>
[NuruRoute("test", Description = "Run the test suite")]
internal sealed class TestCommand : ICommand<Unit>
{
  internal sealed class Handler : ICommandHandler<TestCommand, Unit>
  {
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public ValueTask<Unit> Handle(TestCommand command, CancellationToken ct)
    {
      // Get repo root
      string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

      // Verify we're in the right place
      if (!File.Exists(Path.Combine(repoRoot, "timewarp-terminal.slnx")))
      {
        repoRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
        if (!File.Exists(Path.Combine(repoRoot, "timewarp-terminal.slnx")))
        {
          throw new InvalidOperationException("Could not find repository root (timewarp-terminal.slnx not found)");
        }
      }

      Terminal.WriteLine("Running test suite...");
      Terminal.WriteLine($"Working from: {repoRoot}");

      // TODO: Implement test runner when tests are added
      // Expected location: tests/ci-tests/run-ci-tests.cs or tests/ directory
      Terminal.WriteLine("");
      Terminal.WriteLine("WARNING: No tests implemented yet.");
      Terminal.WriteLine("TODO: Add tests to the repository and update this command.");
      Terminal.WriteLine("");
      Terminal.WriteLine("Test command completed (no tests to run).");

      return ValueTask.FromResult(Unit.Value);
    }
  }
}
