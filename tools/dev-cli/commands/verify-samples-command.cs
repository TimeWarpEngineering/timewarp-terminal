// ===============================================================================
// VERIFY SAMPLES COMMAND
// ===============================================================================
// Builds all samples to verify they compile correctly.
// TODO: Implement when samples are added to the repository.

namespace DevCli.Commands;

/// <summary>
/// Build all samples to verify they compile.
/// </summary>
[NuruRoute("verify-samples", Description = "Verify all samples compile")]
internal sealed class VerifySamplesCommand : ICommand<Unit>
{
  internal sealed class Handler : ICommandHandler<VerifySamplesCommand, Unit>
  {
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public ValueTask<Unit> Handle(VerifySamplesCommand command, CancellationToken ct)
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

      string samplesDir = Path.Combine(repoRoot, "samples");

      Terminal.WriteLine("=== Verifying Samples ===");
      Terminal.WriteLine($"Samples directory: {samplesDir}");
      Terminal.WriteLine("");

      // TODO: Implement sample verification when samples are added
      // Expected location: samples/ directory with *.cs runfiles or *.csproj projects
      if (!Directory.Exists(samplesDir))
      {
        Terminal.WriteLine("WARNING: No samples directory found.");
        Terminal.WriteLine("TODO: Add samples to the repository and update this command.");
        Terminal.WriteLine("");
        Terminal.WriteLine("Verify samples command completed (no samples to verify).");
        return ValueTask.FromResult(Unit.Value);
      }

      // Check for any sample files
      bool hasRunfiles = Directory.EnumerateFiles(samplesDir, "*.cs", SearchOption.AllDirectories).Any();
      bool hasProjects = Directory.EnumerateFiles(samplesDir, "*.csproj", SearchOption.AllDirectories).Any();

      if (!hasRunfiles && !hasProjects)
      {
        Terminal.WriteLine("WARNING: No samples found in samples directory.");
        Terminal.WriteLine("TODO: Add samples to the repository.");
        Terminal.WriteLine("");
        Terminal.WriteLine("Verify samples command completed (no samples to verify).");
        return ValueTask.FromResult(Unit.Value);
      }

      Terminal.WriteLine("TODO: Sample verification not yet implemented.");
      Terminal.WriteLine("Verify samples command completed.");
      return ValueTask.FromResult(Unit.Value);
    }
  }
}
