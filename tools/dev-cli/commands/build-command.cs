// ═══════════════════════════════════════════════════════════════════════════════
// BUILD COMMAND
// ═══════════════════════════════════════════════════════════════════════════════
// Build all TimeWarp.Terminal projects

namespace DevCli;

/// <summary>
/// Build all TimeWarp.Terminal projects
/// </summary>
[NuruRoute("build", Description = "Build all TimeWarp.Terminal projects")]
internal sealed class BuildCommand : ICommand<Unit>
{
  internal sealed class Handler : ICommandHandler<BuildCommand, Unit>
  {
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(BuildCommand command, CancellationToken ct)
    {
      string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
      if (!File.Exists(Path.Combine(repoRoot, "timewarp-terminal.slnx")))
      {
        repoRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
      }

      Terminal.WriteLine("Building solution...");

      int exitCode = await Shell.Builder("dotnet")
        .WithArguments("build", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "-c", "Release")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();

      if (exitCode != 0)
      {
        throw new InvalidOperationException("Build failed!");
      }

      Terminal.WriteLine("\n✓ Build completed successfully");
      return Unit.Value;
    }
  }
}
