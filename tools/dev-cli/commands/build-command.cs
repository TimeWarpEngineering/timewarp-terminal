// ===============================================================================
// BUILD COMMAND
// ===============================================================================
// Builds all TimeWarp.Terminal projects using Release configuration.

namespace DevCli.Commands;

/// <summary>
/// Build all TimeWarp.Terminal projects.
/// </summary>
[NuruRoute("build", Description = "Build all TimeWarp.Terminal projects")]
internal sealed class BuildCommand : ICommand<Unit>
{
  [Option("clean", "c", Description = "Clean before building")]
  public bool Clean { get; set; }

  [Option("verbose", "v", Description = "Verbose output")]
  public bool Verbose { get; set; }

  internal sealed class Handler : ICommandHandler<BuildCommand, Unit>
  {
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(BuildCommand command, CancellationToken ct)
    {
      // Get repo root (dev-cli is in tools/dev-cli/)
      string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));

      // Verify we're in the right place
      if (!File.Exists(Path.Combine(repoRoot, "timewarp-terminal.slnx")))
      {
        // Try alternative path resolution for when running via dotnet run
        repoRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
        if (!File.Exists(Path.Combine(repoRoot, "timewarp-terminal.slnx")))
        {
          throw new InvalidOperationException("Could not find repository root (timewarp-terminal.slnx not found)");
        }
      }

      Terminal.WriteLine("Building TimeWarp.Terminal library...");
      Terminal.WriteLine($"Working from: {repoRoot}");

      // Clean first if requested
      if (command.Clean)
      {
        Terminal.WriteLine("\nCleaning before build...");
        string verbosity = command.Verbose ? "normal" : "minimal";

        CommandResult cleanResult = DotNet.Clean()
          .WithProject(Path.Combine(repoRoot, "timewarp-terminal.slnx"))
          .WithVerbosity(verbosity)
          .Build();

        if (await cleanResult.RunAsync() != 0)
        {
          throw new InvalidOperationException("Clean failed!");
        }
      }

      // Build the library project
      string[] projectsToBuild =
      [
        "source/timewarp-terminal/timewarp-terminal.csproj"
      ];

      string verbosityLevel = command.Verbose ? "normal" : "minimal";

      foreach (string projectPath in projectsToBuild)
      {
        string fullPath = Path.Combine(repoRoot, projectPath);
        Terminal.WriteLine($"\nBuilding {projectPath}...");

        CommandResult buildCommandResult = DotNet.Build()
          .WithProject(fullPath)
          .WithConfiguration("Release")
          .WithVerbosity(verbosityLevel)
          .Build();

        if (command.Verbose)
        {
          Terminal.WriteLine(buildCommandResult.ToCommandString());
        }

        int exitCode = await buildCommandResult.RunAsync();

        if (exitCode != 0)
        {
          throw new InvalidOperationException($"Failed to build {projectPath}!");
        }
      }

      Terminal.WriteLine("\nBuild completed successfully!");
      return Unit.Value;
    }
  }
}
