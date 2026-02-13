// ═══════════════════════════════════════════════════════════════════════════════
// CI COMMAND
// ═══════════════════════════════════════════════════════════════════════════════
// Orchestrates the full CI/CD pipeline.
// For PR: clean -> build -> verify-samples -> test
// For release: clean -> build -> check-version -> pack

namespace DevCli;

/// <summary>
/// Run full CI/CD pipeline
/// </summary>
[NuruRoute("ci", Description = "Run full CI/CD pipeline")]
internal sealed class CiCommand : ICommand<Unit>
{
  [Option("api-key", Description = "NuGet API key for publishing (from OIDC Trusted Publishing)")]
  public string? ApiKey { get; set; }

  internal sealed class Handler : ICommandHandler<CiCommand, Unit>
  {
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(CiCommand command, CancellationToken ct)
    {
      // Auto-detect from GitHub Actions environment
      string? eventName = Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME");
      bool isRelease = eventName == "release" || !string.IsNullOrEmpty(command.ApiKey);

      string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
      if (!File.Exists(Path.Combine(repoRoot, "timewarp-terminal.slnx")))
      {
        repoRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
      }

      if (isRelease)
      {
        await RunReleaseWorkflowAsync(repoRoot, command.ApiKey, ct);
      }
      else
      {
        await RunPrWorkflowAsync(repoRoot, ct);
      }

      return Unit.Value;
    }

    private async Task RunPrWorkflowAsync(string repoRoot, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Terminal.WriteLine("CI Pipeline: clean -> build -> verify-samples -> test");
      Terminal.WriteLine("");

      // Step 1: Clean
      Terminal.WriteLine("Step 1/4: Clean");
      int exitCode = await Shell.Builder("dotnet")
        .WithArguments("clean", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "-v", "q")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();
      if (exitCode != 0) throw new InvalidOperationException("Clean failed!");

      // Step 2: Build
      Terminal.WriteLine("\nStep 2/4: Build");
      exitCode = await Shell.Builder("dotnet")
        .WithArguments("build", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "-c", "Release")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();
      if (exitCode != 0) throw new InvalidOperationException("Build failed!");

      // Step 3: Verify Samples
      Terminal.WriteLine("\nStep 3/4: Verify Samples");
      string samplesDir = Path.Combine(repoRoot, "samples");
      if (Directory.Exists(samplesDir))
      {
        string[] sampleFiles = Directory.GetFiles(samplesDir, "*.cs", SearchOption.TopDirectoryOnly);
        foreach (string sampleFile in sampleFiles)
        {
          string fileName = Path.GetFileName(sampleFile);
          Terminal.WriteLine($"  Verifying {fileName}...");
          exitCode = await Shell.Builder("dotnet")
            .WithArguments("run", sampleFile, "--", "--help")
            .WithWorkingDirectory(samplesDir)
            .RunAsync();
          if (exitCode != 0) throw new InvalidOperationException($"Sample verification failed: {fileName}");
        }
      }

      // Step 4: Test
      Terminal.WriteLine("\nStep 4/4: Test");
      exitCode = await Shell.Builder("dotnet")
        .WithArguments("test", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "--no-build", "-v", "n")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();
      if (exitCode != 0) throw new InvalidOperationException("Tests failed!");

      Terminal.WriteLine("\n✓ CI Pipeline completed successfully");
    }

    private async Task RunReleaseWorkflowAsync(string repoRoot, string? apiKey, CancellationToken ct)
    {
      Terminal.WriteLine("Release Pipeline: clean -> build -> check-version -> pack");
      Terminal.WriteLine("");

      // Step 1: Clean
      Terminal.WriteLine("Step 1/4: Clean");
      int exitCode = await Shell.Builder("dotnet")
        .WithArguments("clean", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "-v", "q")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();
      if (exitCode != 0) throw new InvalidOperationException("Clean failed!");

      // Step 2: Build
      Terminal.WriteLine("\nStep 2/4: Build");
      exitCode = await Shell.Builder("dotnet")
        .WithArguments("build", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "-c", "Release")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();
      if (exitCode != 0) throw new InvalidOperationException("Build failed!");

      // Step 3: Check Version
      Terminal.WriteLine("\nStep 3/4: Check Version");
      string propsPath = Path.Combine(repoRoot, "source", "Directory.Build.props");
      string? version = null;
      if (File.Exists(propsPath))
      {
        string content = await File.ReadAllTextAsync(propsPath, ct);
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
          Environment.Exit(1);
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

      // Step 4: Pack
      Terminal.WriteLine("\nStep 4/4: Pack");
      string artifactsDir = Path.Combine(repoRoot, "artifacts", "packages");
      Directory.CreateDirectory(artifactsDir);

      exitCode = await Shell.Builder("dotnet")
        .WithArguments("pack", Path.Combine(repoRoot, "source", "timewarp-terminal", "timewarp-terminal.csproj"), "-c", "Release", "-o", artifactsDir)
        .WithWorkingDirectory(repoRoot)
        .RunAsync();

      if (exitCode != 0)
      {
        throw new InvalidOperationException("Pack failed!");
      }

      Terminal.WriteLine("\n✓ Release Pipeline completed successfully");
      Terminal.WriteLine($"  Packages created in: {artifactsDir}");

      // Push if api-key provided
      if (!string.IsNullOrEmpty(apiKey))
      {
        Terminal.WriteLine("\nPushing packages to NuGet...");
        Terminal.WriteLine("  (Push not yet implemented - manual push required)");
      }
    }
  }
}
