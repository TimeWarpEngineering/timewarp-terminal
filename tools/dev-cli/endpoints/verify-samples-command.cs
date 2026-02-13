// ═══════════════════════════════════════════════════════════════════════════════
// VERIFY SAMPLES COMMAND
// ═══════════════════════════════════════════════════════════════════════════════
// Verify sample compilation

namespace DevCli;

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

      // Find all .cs runfiles in samples directory
      string[] sampleFiles = Directory.GetFiles(samplesDir, "*.cs", SearchOption.TopDirectoryOnly);

      if (sampleFiles.Length == 0)
      {
        Terminal.WriteLine("No sample files found");
        return Unit.Value;
      }

      Terminal.WriteLine($"Found {sampleFiles.Length} sample file(s)");

      foreach (string sampleFile in sampleFiles)
      {
        string fileName = Path.GetFileName(sampleFile);
        Terminal.WriteLine($"\n  Verifying {fileName}...");

        // Run from samples directory so relative paths work
        int exitCode = await Shell.Builder("dotnet")
          .WithArguments("run", sampleFile, "--", "--help")
          .WithWorkingDirectory(samplesDir)
          .RunAsync();

        if (exitCode != 0)
        {
          Terminal.WriteLine($"    ⚠ {fileName} failed verification (exit code: {exitCode})");
          Environment.Exit(1);
        }
        else
        {
          Terminal.WriteLine($"    ✓ {fileName} verified");
        }
      }

      Terminal.WriteLine($"\n✓ All {sampleFiles.Length} sample(s) verified successfully");
      return Unit.Value;
    }
  }
}
