// ═══════════════════════════════════════════════════════════════════════════════
// VERIFY SAMPLES COMMAND
// ═══════════════════════════════════════════════════════════════════════════════
// Verify sample compilation

namespace DevCli.Commands;

/// <summary>
/// Verify sample compilation
/// </summary>
[NuruRoute("verify-samples", Description = "Verify sample compilation")]
internal sealed class VerifySamplesCommand : ICommand<Unit>
{
  internal sealed class Handler : ICommandHandler<VerifySamplesCommand, Unit>
  {
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(VerifySamplesCommand command, CancellationToken ct)
    {
      string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
      if (!File.Exists(Path.Combine(repoRoot, "timewarp-terminal.slnx")))
      {
        repoRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
      }

      string samplesDir = Path.Combine(repoRoot, "samples");
      if (!Directory.Exists(samplesDir))
      {
        Terminal.WriteLine("No samples directory found - skipping");
        return Unit.Value;
      }

      Terminal.WriteLine("Verifying sample projects...");

      // Find all .csproj files in samples directory
      string[] sampleProjects = Directory.GetFiles(samplesDir, "*.csproj", SearchOption.AllDirectories);

      if (sampleProjects.Length == 0)
      {
        Terminal.WriteLine("No sample projects found");
        return Unit.Value;
      }

      Terminal.WriteLine($"Found {sampleProjects.Length} sample project(s)");

      foreach (string project in sampleProjects)
      {
        string projectName = Path.GetFileName(project);
        Terminal.WriteLine($"\n  Building {projectName}...");

        int exitCode = await Shell.Builder("dotnet")
          .WithArguments("build", project, "-c", "Release", "--no-restore")
          .WithWorkingDirectory(repoRoot)
          .RunAsync();

        if (exitCode != 0)
        {
          throw new InvalidOperationException($"Sample project failed to build: {projectName}");
        }
      }

      Terminal.WriteLine($"\n✓ All {sampleProjects.Length} sample(s) verified successfully");
      return Unit.Value;
    }
  }
}
