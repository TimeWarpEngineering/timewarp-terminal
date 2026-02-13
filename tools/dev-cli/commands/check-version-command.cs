// ═══════════════════════════════════════════════════════════════════════════════
// CHECK VERSION COMMAND
// ═══════════════════════════════════════════════════════════════════════════════
// Check if version already published to NuGet

namespace DevCli.Commands;

/// <summary>
/// Check if version already published
/// </summary>
[NuruRoute("check-version", Description = "Check if version already published")]
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
      string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
      if (!File.Exists(Path.Combine(repoRoot, "timewarp-terminal.slnx")))
      {
        repoRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
      }

      // Read version from Directory.Build.props
      string propsPath = Path.Combine(repoRoot, "source", "timewarp-terminal", "timewarp-terminal.csproj");
      if (!File.Exists(propsPath))
      {
        // Try alternative locations
        propsPath = Path.Combine(repoRoot, "source", "Directory.Build.props");
      }

      string? version = null;
      if (File.Exists(propsPath))
      {
        string content = await File.ReadAllTextAsync(propsPath, ct);
        // Simple regex-like parsing for Version tag
        int versionStart = content.IndexOf("<Version>");
        if (versionStart > 0)
        {
          versionStart += "<Version>".Length;
          int versionEnd = content.IndexOf("</Version>", versionStart);
          if (versionEnd > versionStart)
          {
            version = content[versionStart..versionEnd];
          }
        }
      }

      if (string.IsNullOrEmpty(version))
      {
        throw new InvalidOperationException("Could not determine version from project files");
      }

      Terminal.WriteLine($"Current version: {version}");
      Terminal.WriteLine("Checking NuGet.org...");

      // Check if package exists on NuGet
      using HttpClient client = new();
      string packageId = "TimeWarp.Terminal";
      string url = $"https://api.nuget.org/v3-flatcontainer/{packageId.ToLowerInvariant()}/{version}/{packageId.ToLowerInvariant()}.nuspec";

      try
      {
        Uri uri = new(url);
        HttpResponseMessage response = await client.GetAsync(uri, ct);
        if (response.IsSuccessStatusCode)
        {
          Terminal.WriteLine($"\n✗ Version {version} already exists on NuGet.org");
          Terminal.WriteLine("  Cannot publish - version must be incremented");
          throw new InvalidOperationException($"Version {version} already published");
        }
        else
        {
          Terminal.WriteLine($"\n✓ Version {version} is available for publishing");
        }
      }
      catch (HttpRequestException ex)
      {
        Terminal.WriteLine($"\n⚠ Could not check NuGet: {ex.Message}");
        Terminal.WriteLine("  Assuming version is available");
      }

      return Unit.Value;
    }
  }
}
