#region Purpose
// Development CLI for TimeWarp.Terminal repository operations
#endregion

#region Design
// The test suite is .NET 11 file-based apps (Jaribu runfiles) under tests/,
// not VSTest or MTP projects, so `dotnet test` on the solution finds nothing.
// --coverage restores the local dotnet-coverage tool (.config/dotnet-tools.json)
// and wraps a single inner `dev test` run so one profiler session collects
// Cobertura across all 47 runfile child processes into
// artifacts/coverage/coverage.cobertura.xml (filtered by tests/coverage.runsettings).
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
public sealed class TestCommand : ICommand<Unit>
{
  [Option("coverage", "c", Description = "Collect Cobertura code coverage into artifacts/coverage/coverage.cobertura.xml")]
  public bool Coverage { get; set; }

  public sealed class Handler : IRequestHandler<TestCommand, Unit>
  {
    private const string CoverageDirectoryRelative = "artifacts/coverage";
    private const string CoverageOutputRelative = "artifacts/coverage/coverage.cobertura.xml";
    private const string CoverageSettingsRelative = "tests/coverage.runsettings";

    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(TestCommand command, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(command);

      string repoRoot = Git.FindRoot()
        ?? throw new InvalidOperationException("Could not find git repository root");

      if (command.Coverage)
      {
        await RunWithCoverageAsync(repoRoot, cancellationToken);
      }
      else
      {
        await RunRunfilesAsync(repoRoot, cancellationToken);
      }

      return Unit.Value;
    }

    private async Task RunWithCoverageAsync(string repoRoot, CancellationToken cancellationToken)
    {
      string coverageDirectory = Path.Combine(repoRoot, CoverageDirectoryRelative);
      if (Directory.Exists(coverageDirectory))
      {
        Directory.Delete(coverageDirectory, recursive: true);
      }

      Directory.CreateDirectory(coverageDirectory);

      int restoreExitCode = await Shell.Builder("dotnet")
        .WithArguments("tool", "restore")
        .WithWorkingDirectory(repoRoot)
        .WithNoValidation()
        .RunAsync(cancellationToken);
      if (restoreExitCode != 0)
      {
        throw new InvalidOperationException($"dotnet tool restore failed (exit {restoreExitCode}).");
      }

      string[] innerCommand = BuildInnerTestInvocation(repoRoot);
      string[] arguments =
      [
        "tool",
        "run",
        "dotnet-coverage",
        "--",
        "collect",
        "--nologo",
        "--disable-console-output",
        "--settings",
        CoverageSettingsRelative,
        "--output-format",
        "cobertura",
        "--output",
        CoverageOutputRelative,
        "--",
        .. innerCommand
      ];

      int exitCode = await Shell.Builder("dotnet")
        .WithArguments(arguments)
        .WithWorkingDirectory(repoRoot)
        .WithNoValidation()
        .RunAsync(cancellationToken);
      if (exitCode != 0)
      {
        throw new InvalidOperationException("Tests failed!");
      }

      string coverageFile = Path.Combine(repoRoot, CoverageOutputRelative);
      if (!File.Exists(coverageFile))
      {
        throw new InvalidOperationException(
          $"Tests passed, but expected Cobertura output was not written: {CoverageOutputRelative}");
      }

      Terminal.WriteLine($"Coverage written to {CoverageOutputRelative}");
    }

    private static string[] BuildInnerTestInvocation(string repoRoot)
    {
      string? processPath = Environment.ProcessPath;
      if (!string.IsNullOrEmpty(processPath))
      {
        string fileName = Path.GetFileNameWithoutExtension(processPath);
        if (!string.Equals(fileName, "dotnet", StringComparison.OrdinalIgnoreCase))
        {
          return [processPath, "test"];
        }
      }

      return ["dotnet", "run", "--file", Path.Combine(repoRoot, "tools", "dev-cli", "dev.cs"), "--", "test"];
    }

    private async Task RunRunfilesAsync(string repoRoot, CancellationToken cancellationToken)
    {
      // The test suite is .NET 11 file-based apps (runfiles) under tests/,
      // not VSTest projects, so `dotnet test` on the solution finds nothing
      string testsDir = Path.Combine(repoRoot, "tests");
      string[] testFiles = Directory.GetFiles(testsDir, "*.cs", SearchOption.TopDirectoryOnly);
      Array.Sort(testFiles, StringComparer.Ordinal);

      Terminal.WriteLine($"Running {testFiles.Length} test file(s)...");

      List<string> failed = [];
      foreach (string testFile in testFiles)
      {
        cancellationToken.ThrowIfCancellationRequested();
        string fileName = Path.GetFileName(testFile);
        Terminal.WriteLine($"  Running {fileName}...");
        int exitCode = await Shell.Builder("dotnet")
          .WithArguments(testFile)
          .WithWorkingDirectory(repoRoot)
          .WithNoValidation()
          .RunAsync(cancellationToken);
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
    }
  }
}
