// ═══════════════════════════════════════════════════════════════════════════════
// TEST COMMAND
// ═══════════════════════════════════════════════════════════════════════════════
// Run CI test suite

namespace DevCli;

/// <summary>
/// Run CI test suite
/// </summary>
[NuruRoute("test", Description = "Run CI test suite")]
internal sealed class TestCommand : ICommand<Unit>
{
  internal sealed class Handler : ICommandHandler<TestCommand, Unit>
  {
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(TestCommand command, CancellationToken ct)
    {
      string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
      if (!File.Exists(Path.Combine(repoRoot, "timewarp-terminal.slnx")))
      {
        repoRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
      }

      Terminal.WriteLine("Running tests...");

      int exitCode = await Shell.Builder("dotnet")
        .WithArguments("test", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "--no-build", "-v", "n")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();

      if (exitCode != 0)
      {
        throw new InvalidOperationException("Tests failed!");
      }

      Terminal.WriteLine("\n✓ All tests passed");
      return Unit.Value;
    }
  }
}
