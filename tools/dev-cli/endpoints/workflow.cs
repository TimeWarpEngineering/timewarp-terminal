#region Purpose
// CI/CD pipeline for TimeWarp.Terminal: PR/merge builds and tests; release promotes the CI artifact.
#endregion

#region Design
// Release mode never rebuilds. It promotes the Packages-* artifact the green
// master push already built and tested for this commit (tag-gate, check-version,
// locate-run, download-artifact, verify, push). The pure locate/verify rules
// live in TimeWarp.Nuru.DevCli (CiRunPromotion, TagAssertion, CiModeDetector).
// Each nupkg and its snupkg is copied into its own temp directory before
// `dotnet nuget push`. A nupkg push also uploads a sibling snupkg in the same
// directory, which is what logged Created and then Conflict on symbolpackage
// for v2.0.0-beta.1. Isolated pushes send each file once.
// Packing is Release-only. Samples and tests still build the source projects
// in Debug, so the merge pipeline hashes every nupkg and snupkg after the
// Release build and fails if a later step changes those bytes.
// workflow_dispatch maps to merge unless --mode release is passed. The workflow
// file only passes that behind confirm=release.
#endregion

// ═══════════════════════════════════════════════════════════════════════════════
// WORKFLOW COMMAND
// ═══════════════════════════════════════════════════════════════════════════════
// Orchestrates the CI/CD pipeline.
// PR/merge: clean -> build -> verify-samples -> test
// Release:  tag-gate -> check-version -> locate-run -> download-artifact -> verify -> push

namespace DevCli;

/// <summary>
/// Run full CI/CD pipeline
/// </summary>
[NuruRoute("workflow", Description = "Run full CI/CD pipeline")]
public sealed partial class WorkflowCommand : ICommand<Unit>
{
  [Option("mode", Description = "CI mode: pr, merge, or release (auto-detected from GITHUB_EVENT_NAME if not specified)")]
  public string? Mode { get; set; }

  [Option("api-key", Description = "NuGet API key for publishing (from OIDC Trusted Publishing)")]
  public string? ApiKey { get; set; }

  internal sealed partial class Handler : IRequestHandler<WorkflowCommand, Unit>
  {
    private const string ArtifactsSubPath = "artifacts/packages";
    private const string NuGetSource = "https://api.nuget.org/v3/index.json";

    private readonly ITerminal Terminal;
    private readonly NuGetVersionService NuGetVersionService;
    private readonly IRepoConfigService RepoConfigService;
    private readonly IPackableProjectService PackableProjectService;

    public Handler
    (
      ITerminal terminal,
      NuGetVersionService nuGetVersionService,
      IRepoConfigService repoConfigService,
      IPackableProjectService packableProjectService
    )
    {
      Terminal = terminal;
      NuGetVersionService = nuGetVersionService;
      RepoConfigService = repoConfigService;
      PackableProjectService = packableProjectService;
    }

    public async ValueTask<Unit> Handle(WorkflowCommand command, CancellationToken cancellationToken)
    {
      ArgumentNullException.ThrowIfNull(command);

      string? eventName = Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME");
      CiMode mode = CiModeDetector.DetermineMode(command.Mode, eventName);

      if (string.IsNullOrEmpty(command.Mode))
      {
        string displayEventName = eventName ?? "(not set)";
        Terminal.WriteLine($"Detected GITHUB_EVENT_NAME: {displayEventName} -> Mode: {mode}");
      }

      Terminal.WriteLine("===============================================================================");
      Terminal.WriteLine($"  CI/CD Pipeline - Mode: {mode}");
      Terminal.WriteLine("===============================================================================");
      Terminal.WriteLine("");

      string repoRoot = ResolveRepoRoot();

      if (mode == CiMode.Release)
      {
        await RunReleaseWorkflowAsync(repoRoot, command.ApiKey, cancellationToken);
      }
      else
      {
        await RunPrWorkflowAsync(repoRoot, cancellationToken);
      }

      return Unit.Value;
    }

    private async Task RunPrWorkflowAsync(string repoRoot, CancellationToken cancellationToken)
    {
      cancellationToken.ThrowIfCancellationRequested();
      Terminal.WriteLine("CI Pipeline: clean -> build -> verify-samples -> test");
      Terminal.WriteLine("");

      Terminal.WriteLine("Step 1/4: Clean");
      int exitCode = await Shell.Builder("dotnet")
        .WithArguments("clean", Path.Combine(repoRoot, "timewarp-terminal.slnx"), "-v", "q")
        .WithWorkingDirectory(repoRoot)
        .RunAsync(cancellationToken);
      if (exitCode != 0)
      {
        throw new InvalidOperationException("Clean failed!");
      }

      // A previous local pack must not be uploaded beside this build's packages.
      ResetPackagesDirectory(repoRoot);

      Terminal.WriteLine("\nStep 2/4: Build");
      exitCode = await Shell.Builder("dotnet")
        .WithArguments
        (
          "build",
          Path.Combine(repoRoot, "timewarp-terminal.slnx"),
          "-c", "Release",
          "-p:ContinuousIntegrationBuild=true"
        )
        .WithWorkingDirectory(repoRoot)
        .RunAsync(cancellationToken);
      if (exitCode != 0)
      {
        throw new InvalidOperationException("Build failed!");
      }

      string packagesDir = Path.Combine(repoRoot, ArtifactsSubPath);
      Dictionary<string, string> packageHashes = HashPackages(packagesDir);
      RequireReleasePackages(packageHashes);

      Terminal.WriteLine("\nStep 3/4: Verify Samples");
      await VerifySamplesAsync(repoRoot, cancellationToken);

      Terminal.WriteLine("\nStep 4/4: Test");
      TestCommand.Handler testHandler = new(Terminal);
      await testHandler.Handle(new TestCommand { Coverage = true }, cancellationToken);

      AssertPackagesUnchanged(packagesDir, packageHashes);

      Terminal.WriteLine("\n✓ CI Pipeline completed successfully");
    }

    private static string ResolveRepoRoot()
    {
      string? gitRoot = Git.FindRoot();
      if (gitRoot is not null && File.Exists(Path.Combine(gitRoot, "timewarp-terminal.slnx")))
      {
        return gitRoot;
      }

      string repoRoot = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", ".."));
      if (!File.Exists(Path.Combine(repoRoot, "timewarp-terminal.slnx")))
      {
        repoRoot = Path.GetFullPath(Directory.GetCurrentDirectory());
      }

      return repoRoot;
    }

    private static void ResetPackagesDirectory(string repoRoot)
    {
      string artifactsDir = Path.Combine(repoRoot, ArtifactsSubPath);
      if (Directory.Exists(artifactsDir))
      {
        Directory.Delete(artifactsDir, recursive: true);
      }
    }

    // Release packing writes both a nupkg and an snupkg. An empty directory
    // would otherwise look like a successful build whose artifact is missing.
    private static void RequireReleasePackages(Dictionary<string, string> packageHashes)
    {
      bool hasNupkg = false;
      bool hasSnupkg = false;
      foreach (string name in packageHashes.Keys)
      {
        if (name.EndsWith(".nupkg", StringComparison.OrdinalIgnoreCase))
        {
          hasNupkg = true;
        }
        else if (name.EndsWith(".snupkg", StringComparison.OrdinalIgnoreCase))
        {
          hasSnupkg = true;
        }
      }

      if (!hasNupkg || !hasSnupkg)
      {
        throw new InvalidOperationException(
          "Release build did not write both a nupkg and an snupkg. GeneratePackageOnBuild must pack Release builds into artifacts/packages.");
      }
    }

    // Samples and tests build the source projects in Debug. Those builds must
    // leave the Release packages byte-identical, because the upload step ships
    // whatever is in artifacts/packages when the job ends.
    private void AssertPackagesUnchanged(string packagesDir, Dictionary<string, string> packageHashes)
    {
      Dictionary<string, string> finalHashes = HashPackages(packagesDir);
      List<string> changedPackages = [];
      foreach ((string packageName, string packageHash) in packageHashes)
      {
        if (!finalHashes.TryGetValue(packageName, out string? finalHash))
        {
          changedPackages.Add($"{packageName} (disappeared)");
        }
        else if (!string.Equals(packageHash, finalHash, StringComparison.Ordinal))
        {
          changedPackages.Add($"{packageName} (changed)");
        }
      }

      foreach (string packageName in finalHashes.Keys)
      {
        if (!packageHashes.ContainsKey(packageName))
        {
          changedPackages.Add($"{packageName} (appeared)");
        }
      }

      if (changedPackages.Count > 0)
      {
        string detail = string.Join(", ", changedPackages.Order(StringComparer.Ordinal));
        Terminal.WriteErrorLine(
          $"Package integrity gate failed: the samples or tests steps rebuilt or repacked a package after the Release build: {detail}. The artifact must not ship.");
        throw new InvalidOperationException("Package integrity gate failed!");
      }

      Terminal.WriteLine($"Package integrity verified: {packageHashes.Count} packages unchanged since build.");
    }

    private static Dictionary<string, string> HashPackages(string packagesDir)
    {
      Dictionary<string, string> hashes = [with(StringComparer.Ordinal)];
      if (!Directory.Exists(packagesDir))
      {
        return hashes;
      }

      string[] patterns = ["*.nupkg", "*.snupkg"];
      foreach (string pattern in patterns)
      {
        foreach (string file in Directory.EnumerateFiles(packagesDir, pattern))
        {
          using FileStream stream = File.OpenRead(file);
          hashes[Path.GetFileName(file)] = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(stream));
        }
      }

      return hashes;
    }

    private static string ShortSha(string? sha) =>
      string.IsNullOrEmpty(sha) ? "(unknown)" : sha.Length > 7 ? sha[..7] : sha;

    private void WriteStepBanner(string title)
    {
      Terminal.WriteLine("");
      Terminal.WriteLine("===============================================================================");
      Terminal.WriteLine($"  {title}");
      Terminal.WriteLine("===============================================================================");
    }

    private void AbortPipeline(string reason)
    {
      Terminal.WriteLine("");
      Terminal.WriteLine("===============================================================================");
      Terminal.WriteLine($"  Pipeline ABORTED — {reason}");
      Terminal.WriteLine("===============================================================================");
      Environment.ExitCode = 1;
    }

    private async Task VerifySamplesAsync(string repoRoot, CancellationToken cancellationToken)
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
        cancellationToken.ThrowIfCancellationRequested();
        string fileName = Path.GetFileName(sampleFile);
        Terminal.WriteLine($"  Verifying {fileName}...");
        int exitCode = await Shell.Builder("dotnet")
          .WithArguments("run", sampleFile, "--", "--help")
          .WithWorkingDirectory(samplesDir)
          .RunAsync(cancellationToken);
        if (exitCode != 0)
        {
          throw new InvalidOperationException($"Sample verification failed: {fileName}");
        }
      }
    }
  }
}
