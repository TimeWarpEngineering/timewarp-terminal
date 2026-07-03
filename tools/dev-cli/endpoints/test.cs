#region Purpose
// Development CLI for TimeWarp.Terminal repository operations
#endregion

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
      string? repoRoot = Git.FindRoot()
        ?? throw new InvalidOperationException("Could not find git repository root");

      // The test suite is .NET 10 file-based apps (runfiles) under tests/,
      // not VSTest projects, so `dotnet test` on the solution finds nothing
      string testsDir = Path.Combine(repoRoot, "tests");
      string[] testFiles = Directory.GetFiles(testsDir, "*.cs", SearchOption.TopDirectoryOnly);
      Array.Sort(testFiles, StringComparer.Ordinal);

      Terminal.WriteLine($"Running {testFiles.Length} test file(s)...");

      List<string> failed = [];
      foreach (string testFile in testFiles)
      {
        string fileName = Path.GetFileName(testFile);
        Terminal.WriteLine($"  Running {fileName}...");
        int exitCode = await Shell.Builder("dotnet")
          .WithArguments(testFile)
          .WithWorkingDirectory(repoRoot)
          .RunAsync();
        if (exitCode != 0)
        {
          failed.Add(fileName);
        }
      }

      if (failed.Count > 0)
      {
        Terminal.WriteLine($"\n✗ {failed.Count} of {testFiles.Length} test file(s) failed:");
        foreach (string fileName in failed)
        {
          Terminal.WriteLine($"    {fileName}");
        }

        throw new InvalidOperationException("Tests failed!");
      }

      Terminal.WriteLine($"\n✓ All {testFiles.Length} test file(s) passed");
      return Unit.Value;
    }
  }
}
