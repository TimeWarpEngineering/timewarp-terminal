// ===============================================================================
// CHECK VERSION COMMAND
// ===============================================================================
// Verifies that NuGet packages are not already published before release.

namespace DevCli.Commands;

/// <summary>
/// Check if NuGet packages are already published.
/// </summary>
[NuruRoute("check-version", Description = "Check if packages are already published on NuGet")]
internal sealed class CheckVersionCommand : ICommand<Unit>
{
  internal sealed class Handler : ICommandHandler<CheckVersionCommand, Unit>
  {
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(CheckVersionCommand command, CancellationToken ct)
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

      // Read version from source/timewarp-terminal/timewarp-terminal.csproj
      string csprojPath = Path.Combine(repoRoot, "source", "timewarp-terminal", "timewarp-terminal.csproj");

      if (!File.Exists(csprojPath))
      {
        throw new FileNotFoundException($"Could not find {csprojPath}");
      }

      XDocument doc = XDocument.Load(csprojPath);
      string? version = doc.Descendants("Version").FirstOrDefault()?.Value;

      if (string.IsNullOrEmpty(version))
      {
        throw new InvalidOperationException("Could not find version in source/timewarp-terminal/timewarp-terminal.csproj");
      }

      Terminal.WriteLine($"Checking if packages with version {version} are already published on NuGet.org...");

      // Packages to check
      string[] packages =
      [
        "TimeWarp.Terminal"
      ];

      List<string> alreadyPublished = [];

      foreach (string package in packages)
      {
        Terminal.WriteLine($"\nChecking {package}...");

        CommandOutput result = await DotNet.PackageSearch(package)
          .WithExactMatch()
          .WithPrerelease()
          .WithSource("https://api.nuget.org/v3/index.json")
          .Build()
          .CaptureAsync();

        // Check if the version appears in the output
        if (result.Stdout.Contains($"| {version} |", StringComparison.Ordinal))
        {
          Terminal.WriteLine($"  WARNING: {package} {version} is already published to NuGet.org");
          alreadyPublished.Add(package);
        }
        else
        {
          Terminal.WriteLine($"  OK: {package} {version} is not yet published");
        }
      }

      if (alreadyPublished.Count > 0)
      {
        throw new InvalidOperationException(
          $"Package(s) already published: {string.Join(", ", alreadyPublished)}. " +
          "Please increment the version in source/timewarp-terminal/timewarp-terminal.csproj");
      }

      Terminal.WriteLine("\nAll packages are ready to publish!");
      return Unit.Value;
    }
  }
}
