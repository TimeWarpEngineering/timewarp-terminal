#region Purpose
// Development CLI for TimeWarp.Terminal repository operations
#endregion

// ═══════════════════════════════════════════════════════════════════════════════
// CLEAN COMMAND
// ═══════════════════════════════════════════════════════════════════════════════
// Clean solution and artifacts

namespace DevCli;

/// <summary>
/// Clean solution and artifacts
/// </summary>
[NuruRoute("clean", Description = "Clean solution and artifacts")]
internal sealed class CleanCommand : ICommand<Unit>
{
  internal sealed class Handler : ICommandHandler<CleanCommand, Unit>
  {
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(CleanCommand command, CancellationToken ct)
    {
      string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
      if (!File.Exists(Path.Combine(repoRoot, "timewarp-terminal.slnx")))
      {
        repoRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
      }

      Terminal.WriteLine("Cleaning solution...");

      int exitCode = await Shell.Builder("dotnet")
        .WithArguments("clean", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "-v", "q")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();

      if (exitCode != 0)
      {
        throw new InvalidOperationException("Clean failed!");
      }

      // Clean artifacts directory
      string artifactsDir = Path.Combine(repoRoot, "artifacts");
      if (Directory.Exists(artifactsDir))
      {
        Directory.Delete(artifactsDir, recursive: true);
        Terminal.WriteLine("Cleaned artifacts/");
      }

      Terminal.WriteLine("\n✓ Clean completed");
      return Unit.Value;
    }
  }
}
