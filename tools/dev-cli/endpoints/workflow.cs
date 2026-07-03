#region Purpose
// Development CLI for TimeWarp.Terminal repository operations
#endregion

// ═══════════════════════════════════════════════════════════════════════════════
// WORKFLOW COMMAND
// ═══════════════════════════════════════════════════════════════════════════════
// Orchestrates the full CI/CD pipeline.
// For PR: clean -> build -> verify-samples -> test
// For release: clean -> build -> verify-samples -> test -> check-version -> pack -> push -> notify timewarp-software

namespace DevCli;

/// <summary>
/// Run full CI/CD pipeline
/// </summary>
[NuruRoute("workflow", Description = "Run full CI/CD pipeline")]
internal sealed class WorkflowCommand : ICommand<Unit>
{
  [Option("api-key", Description = "NuGet API key for publishing (from OIDC Trusted Publishing)")]
  public string? ApiKey { get; set; }

  internal sealed class Handler : ICommandHandler<WorkflowCommand, Unit>
  {
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(WorkflowCommand command, CancellationToken ct)
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
      if (exitCode != 0)
      {
        throw new InvalidOperationException("Clean failed!");
      }

      // Step 2: Build
      Terminal.WriteLine("\nStep 2/4: Build");
      exitCode = await Shell.Builder("dotnet")
        .WithArguments("build", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "-c", "Release")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();
      if (exitCode != 0)
      {
        throw new InvalidOperationException("Build failed!");
      }

      // Step 3: Verify Samples
      Terminal.WriteLine("\nStep 3/4: Verify Samples");
      await VerifySamplesAsync(repoRoot);

      // Step 4: Test
      Terminal.WriteLine("\nStep 4/4: Test");
      await RunTestSuiteAsync(repoRoot);

      Terminal.WriteLine("\n✓ CI Pipeline completed successfully");
    }

    private async Task RunReleaseWorkflowAsync(string repoRoot, string? apiKey, CancellationToken ct)
    {
      Terminal.WriteLine("Release Pipeline: clean -> build -> verify-samples -> test -> check-version -> pack");
      Terminal.WriteLine("");

      // Step 1: Clean
      Terminal.WriteLine("Step 1/6: Clean");
      int exitCode = await Shell.Builder("dotnet")
        .WithArguments("clean", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "-v", "q")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();
      if (exitCode != 0)
      {
        throw new InvalidOperationException("Clean failed!");
      }

      // Step 2: Build
      Terminal.WriteLine("\nStep 2/6: Build");
      exitCode = await Shell.Builder("dotnet")
        .WithArguments("build", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "-c", "Release")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();
      if (exitCode != 0)
      {
        throw new InvalidOperationException("Build failed!");
      }

      // Step 3: Verify Samples — a release must never ship from a commit
      // whose samples or tests were not exercised on the release event itself
      Terminal.WriteLine("\nStep 3/6: Verify Samples");
      await VerifySamplesAsync(repoRoot);

      // Step 4: Test
      Terminal.WriteLine("\nStep 4/6: Test");
      await RunTestSuiteAsync(repoRoot);

      // Step 5: Check Version
      Terminal.WriteLine("\nStep 5/6: Check Version");
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

      // A release event must publish the version named by its tag — otherwise publishing
      // a "v1.0.0" GitHub release with props still at an older version silently pushes that stale version
      string? tagName = Environment.GetEnvironmentVariable("GITHUB_REF_NAME");
      if (string.IsNullOrEmpty(tagName))
      {
        string? gitHubRef = Environment.GetEnvironmentVariable("GITHUB_REF");
        if (!string.IsNullOrEmpty(gitHubRef) && gitHubRef.StartsWith("refs/tags/", StringComparison.Ordinal))
        {
          tagName = gitHubRef["refs/tags/".Length..];
        }
      }

      string? releaseEventName = Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME");
      if (releaseEventName == "release" && !string.IsNullOrEmpty(tagName))
      {
        string tagVersion = tagName.StartsWith('v') ? tagName[1..] : tagName;
        if (!string.Equals(tagVersion, version, StringComparison.Ordinal))
        {
          Terminal.WriteLine("\n✗ Release tag does not match project version");
          Terminal.WriteLine($"  Tag:     {tagName} (version {tagVersion})");
          Terminal.WriteLine($"  Project: {version}");
          throw new InvalidOperationException($"Release tag '{tagName}' does not match project version '{version}'");
        }

        Terminal.WriteLine($"✓ Release tag {tagName} matches project version");
      }
      else
      {
        Terminal.WriteLine("No release tag available (local run) - skipping tag/version match check");
      }

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

      // Step 6: Pack
      Terminal.WriteLine("\nStep 6/6: Pack");
      string artifactsDir = Path.Combine(repoRoot, "artifacts", "packages");
      Directory.CreateDirectory(artifactsDir);

      exitCode = await Shell.Builder("dotnet")
        .WithArguments("pack", Path.Combine(repoRoot, "source", "timewarp-terminal", "timewarp-terminal.csproj"), "-c", "Release", "-o", artifactsDir, "-p:ContinuousIntegrationBuild=true")
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
        string[] packages = Directory.GetFiles(artifactsDir, "*.nupkg");
        foreach (string package in packages)
        {
          string packageName = Path.GetFileName(package);
          Terminal.WriteLine($"  Pushing {packageName}...");

          exitCode = await DotNet.NuGet()
            .Push(package)
            .WithSource("https://api.nuget.org/v3/index.json")
            .WithApiKey(apiKey)
            .RunAsync(ct);

          if (exitCode != 0)
          {
            throw new InvalidOperationException($"NuGet push failed: {packageName}");
          }
        }

        Terminal.WriteLine("✓ Packages pushed to NuGet.org");

        await NotifySoftwareSiteAsync(repoRoot, version);
      }
    }

    private async Task NotifySoftwareSiteAsync(string repoRoot, string version)
    {
      // Signal timewarp-software to rebuild the site so the new release shows up
      // immediately instead of waiting for its nightly cron backstop. Best effort:
      // a failure here must never fail a release that already pushed to NuGet.
      // Cross-repo repository_dispatch needs a credential with write access to
      // timewarp-software — locally gh's stored auth suffices; in GitHub Actions
      // the default GITHUB_TOKEN cannot reach other repos, so workflow.yml mints a
      // short-lived installation token from the org's Rebuild Dispatcher GitHub App
      // and passes it as GH_TOKEN.
      Terminal.WriteLine("\nNotifying timewarp-software to rebuild the site...");
      int exitCode = await Shell.Builder("gh")
        .WithArguments
        (
          "api",
          "repos/TimeWarpEngineering/timewarp-software/dispatches",
          "-f", "event_type=rebuild",
          "-f", "client_payload[package]=TimeWarp.Terminal",
          "-f", $"client_payload[version]={version}"
        )
        .WithWorkingDirectory(repoRoot)
        .RunAsync();

      if (exitCode == 0)
      {
        Terminal.WriteLine("✓ timewarp-software rebuild dispatched");
      }
      else
      {
        Terminal.WriteLine("⚠ Could not dispatch timewarp-software rebuild (non-fatal; the site rebuilds nightly)");
      }
    }

    private async Task VerifySamplesAsync(string repoRoot)
    {
      string samplesDir = Path.Combine(repoRoot, "samples");
      if (!Directory.Exists(samplesDir))
      {
        return;
      }

      string[] sampleFiles = Directory.GetFiles(samplesDir, "*.cs", SearchOption.TopDirectoryOnly);
      Array.Sort(sampleFiles, StringComparer.Ordinal);
      foreach (string sampleFile in sampleFiles)
      {
        string fileName = Path.GetFileName(sampleFile);
        Terminal.WriteLine($"  Verifying {fileName}...");
        int exitCode = await Shell.Builder("dotnet")
          .WithArguments("run", sampleFile, "--", "--help")
          .WithWorkingDirectory(samplesDir)
          .RunAsync();
        if (exitCode != 0)
        {
          throw new InvalidOperationException($"Sample verification failed: {fileName}");
        }
      }
    }

    private async Task RunTestSuiteAsync(string repoRoot)
    {
      // The test suite is .NET 10 file-based apps (runfiles) under tests/,
      // not VSTest projects, so `dotnet test` on the solution finds nothing
      string testsDir = Path.Combine(repoRoot, "tests");
      string[] testFiles = Directory.GetFiles(testsDir, "*.cs", SearchOption.TopDirectoryOnly);
      Array.Sort(testFiles, StringComparer.Ordinal);

      List<string> failed = [];
      foreach (string testFile in testFiles)
      {
        string fileName = Path.GetFileName(testFile);
        Terminal.WriteLine($"  Running {fileName}...");
        int exitCode = await Shell.Builder("dotnet")
          .WithArguments(testFile)
          .WithWorkingDirectory(repoRoot)
          .RunAsync();
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

      Terminal.WriteLine($"  ✓ {testFiles.Length} test file(s) passed");
    }
  }
}
