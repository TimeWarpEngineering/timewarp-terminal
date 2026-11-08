#region Purpose
// Release mode of the workflow command: promote the CI Packages artifact and push it.
#endregion

#region Design
// Same Handler as workflow.cs. This part is the six release steps plus the
// isolated nupkg/snupkg push and the best-effort site rebuild dispatch.
#endregion

namespace DevCli;

public sealed partial class WorkflowCommand
{
  internal sealed partial class Handler
  {
    private async Task RunReleaseWorkflowAsync(string repoRoot, string? apiKey, CancellationToken cancellationToken)
    {
      Terminal.WriteLine("Pipeline: tag-gate -> check-version -> locate-run -> download-artifact -> verify -> push");
      Terminal.WriteLine("");

      Environment.ExitCode = 0;

      WriteStepBanner("Step 1/6: Release Gate — Tag Assertions");

      string? eventName = Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME");
      string? propsVersion = PropsVersionReader.Read(repoRoot);

      if (eventName == "release")
      {
        string? refName = Environment.GetEnvironmentVariable("GITHUB_REF_NAME");
        TagAssertionResult tagResult = TagAssertion.Validate(refName, propsVersion);

        if (!tagResult.IsValid)
        {
          Terminal.WriteErrorLine($"Release gate failed: {tagResult.Error}");
          AbortPipeline("release tag does not match source version");
          return;
        }

        Terminal.WriteLine($"Tag assertion passed: {tagResult.ExpectedTag}");
      }
      else
      {
        Terminal.WriteLine("Tag assertion skipped: GITHUB_EVENT_NAME is not 'release' (break-glass/local release has no triggering ref tag to assert against; the tag-pin check below still applies).");
      }

      if (string.IsNullOrWhiteSpace(propsVersion))
      {
        Terminal.WriteErrorLine("Release gate failed: could not read <Version> from source/Directory.Build.props.");
        AbortPipeline("props version unreadable");
        return;
      }

      TagPinOutcome tagPinOutcome = await CheckTagPinAsync(propsVersion);
      string pinTag = $"v{propsVersion}";

      switch (tagPinOutcome.Status)
      {
        case TagPinStatus.NoTag:
          Terminal.WriteLine($"Tag pin: {pinTag} not yet tagged.");
          break;

        case TagPinStatus.Match:
          Terminal.WriteLine($"Tag pin passed: HEAD is at {pinTag}.");
          break;

        case TagPinStatus.Mismatch:
          Terminal.WriteErrorLine($"Release gate failed: tag {pinTag} already exists at commit {ShortSha(tagPinOutcome.TagCommit)}; this run is at {ShortSha(tagPinOutcome.HeadCommit)}. A partial-publish resume must run from the tag's commit (or bump the version if source changed).");
          AbortPipeline("tag pin mismatch");
          return;

        case TagPinStatus.GitError:
          Terminal.WriteErrorLine($"Release gate failed: tag pin check could not run — {tagPinOutcome.Detail}");
          AbortPipeline("tag pin check could not run");
          return;

        default:
          Terminal.WriteErrorLine($"Release gate failed: unhandled tag pin status '{tagPinOutcome.Status}'.");
          AbortPipeline("unhandled tag pin status");
          return;
      }

      AncestorCheckOutcome ancestorOutcome = await CheckHeadAncestorOfMasterAsync();

      switch (ancestorOutcome.Status)
      {
        case AncestorCheckStatus.NotAncestor:
          Terminal.WriteErrorLine("Release gate failed: current commit is not an ancestor of master. Releases must be cut from commits on master.");
          AbortPipeline("commit not on master");
          return;

        case AncestorCheckStatus.MasterUnresolvable:
          Terminal.WriteErrorLine("Release gate failed: cannot resolve origin/master or master — ensure the checkout has full history (fetch-depth: 0) and a master ref exists.");
          AbortPipeline("master ref unresolvable");
          return;

        case AncestorCheckStatus.GitError:
          Terminal.WriteErrorLine($"Release gate failed: ancestor check could not run — {ancestorOutcome.Detail}");
          AbortPipeline("ancestor check could not run");
          return;

        case AncestorCheckStatus.Ancestor:
          Terminal.WriteLine("Ancestor check passed: HEAD is on master.");
          break;

        default:
          Terminal.WriteErrorLine($"Release gate failed: unhandled ancestor check status '{ancestorOutcome.Status}'.");
          AbortPipeline("unhandled ancestor check status");
          return;
      }

      WriteStepBanner("Step 2/6: Check Version");
      CheckVersionCommand.Handler checkVersionHandler = new(Terminal, NuGetVersionService, RepoConfigService, PackableProjectService);
      await checkVersionHandler.Handle(new CheckVersionCommand(), cancellationToken);

      if (Environment.ExitCode != 0)
      {
        AbortPipeline(CheckVersionAbort.FormatAbortReason(checkVersionHandler.Outcome, checkVersionHandler.GitHubPackagesFailure));
        return;
      }

      IReadOnlyList<PackableProject> packableProjects = await PackableProjectService
        .GetPackableProjectsAsync(repoRoot, cancellationToken)
        .ConfigureAwait(false);

      if (packableProjects.Count == 0)
      {
        Terminal.WriteErrorLine("Release gate failed: no packable projects found under source/.");
        AbortPipeline("no packable projects found");
        return;
      }

      List<ReleasePackage> releasePackages = [];
      foreach (PackableProject project in packableProjects)
      {
        string? projectVersion = await ResolveProjectVersionAsync(repoRoot, project.ProjectPath, cancellationToken);

        if (string.IsNullOrWhiteSpace(projectVersion))
        {
          Terminal.WriteErrorLine($"Release gate failed: could not evaluate <Version> for {project.PackageId} ({Path.GetRelativePath(repoRoot, project.ProjectPath)}).");
          AbortPipeline("package version unresolvable");
          return;
        }

        releasePackages.Add(new ReleasePackage(project.PackageId, projectVersion));
      }

      List<ReleasePackage> driftedPackages = [.. releasePackages.Where(package => !string.Equals(package.Version, propsVersion, StringComparison.Ordinal))];

      if (driftedPackages.Count > 0)
      {
        Terminal.WriteErrorLine($"Release gate failed: package version(s) differ from source/Directory.Build.props <Version> {propsVersion}: {string.Join(", ", driftedPackages.Select(package => $"{package.PackageId} {package.Version}"))}. All packages ship at one version — remove any <Version> from the csproj (or set IsPackable=false with a stated reason).");
        AbortPipeline("package version differs from props version");
        return;
      }

      Terminal.WriteLine($"Packable set ({releasePackages.Count}) at {propsVersion}: {string.Join(", ", releasePackages.Select(package => package.PackageId))}");

      WriteStepBanner("Step 3/6: Locate CI Run");

      LocateRunOutcome locateOutcome = await LocateCiRunAsync();

      switch (locateOutcome.Status)
      {
        case LocateRunStatus.GhUnavailable:
          Terminal.WriteErrorLine("Release gate failed: release mode promotes CI-built artifacts and requires the gh CLI. On runners GH_TOKEN is provided by workflow.yml; locally install gh and run 'gh auth login'.");
          AbortPipeline("gh CLI unavailable");
          return;

        case LocateRunStatus.GhFailed:
          Terminal.WriteErrorLine($"Release gate failed: gh run list failed — {locateOutcome.Detail}. If this is transient (network/rate limit), retry; for auth issues run 'gh auth login'.");
          AbortPipeline("gh run list failed");
          return;

        case LocateRunStatus.NoMatchingRun:
          Terminal.WriteErrorLine($"Release gate failed: no successful CI run of workflow.yml exists for commit {locateOutcome.HeadSha}. Only tested CI artifacts are published — this commit must pass CI first. If a run failed, fix and re-run it (gh run rerun <run-id>).");
          AbortPipeline("no successful CI run found");
          return;

        case LocateRunStatus.Found:
          break;

        default:
          Terminal.WriteErrorLine($"Release gate failed: unhandled locate-run status '{locateOutcome.Status}'.");
          AbortPipeline("unhandled locate-run status");
          return;
      }

      Terminal.WriteLine($"Found {locateOutcome.CandidateRuns.Count} candidate CI run(s) for commit {ShortSha(locateOutcome.HeadSha)}.");

      WriteStepBanner("Step 4/6: Download Artifact");

      DownloadArtifactOutcome downloadOutcome = await DownloadPackagesArtifactAsync(repoRoot, locateOutcome.CandidateRuns);

      if (downloadOutcome.Status == DownloadArtifactStatus.Exhausted)
      {
        if (downloadOutcome.ExpiredEncounters.Count > 0)
        {
          string expiredDetail = string.Join("; ", downloadOutcome.ExpiredEncounters.Select(encounter => $"run {encounter.RunId} ({encounter.Event}): {string.Join(", ", encounter.ArtifactNames)}"));
          Terminal.WriteErrorLine($"Release gate failed: every candidate CI run's Packages-* artifact has expired — {expiredDetail}. Re-run CI to produce a fresh tested artifact (gh run rerun {downloadOutcome.ExpiredEncounters[0].RunId}).");
        }
        else
        {
          Terminal.WriteErrorLine($"Release gate failed: no candidate CI run for commit {locateOutcome.HeadSha} uploaded a Packages-* artifact. Re-run CI to produce one (gh run rerun {locateOutcome.CandidateRuns[0].DatabaseId}).");
        }

        AbortPipeline("no usable CI artifact found");
        return;
      }

      Terminal.WriteLine($"Downloaded '{downloadOutcome.ArtifactName}' from run {downloadOutcome.Run!.DatabaseId} ({downloadOutcome.Run.Event}).");

      WriteStepBanner("Step 5/6: Verify Package Set");

      string artifactsDir = Path.Combine(repoRoot, ArtifactsSubPath);
      string[] actualNupkgPaths = Directory.Exists(artifactsDir) ? Directory.GetFiles(artifactsDir, "*.nupkg") : [];
      List<string> actualFileNames = [.. actualNupkgPaths.Select(path => Path.GetFileName(path)!)];

      PackageSetVerification verification = CiRunPromotion.VerifyPackageSet(actualFileNames, packableProjects, propsVersion);

      if (!verification.IsMatch)
      {
        if (verification.Missing.Count > 0)
        {
          Terminal.WriteErrorLine($"Release gate failed: downloaded artifact is missing package(s): {string.Join(", ", verification.Missing)}.");
        }

        if (verification.Unexpected.Count > 0)
        {
          Terminal.WriteErrorLine($"Release gate failed: downloaded artifact has unexpected package(s): {string.Join(", ", verification.Unexpected)}.");
        }

        Terminal.WriteErrorLine($"CI run likely predates the version bump — re-run CI on commit {locateOutcome.HeadSha} and retry.");
        AbortPipeline("downloaded package set does not match derived packable set");
        return;
      }

      List<string> missingSymbols = [];
      foreach (ReleasePackage package in releasePackages)
      {
        if (!File.Exists(Path.Combine(artifactsDir, package.SymbolFileName)))
        {
          missingSymbols.Add(package.SymbolFileName);
        }
      }

      if (missingSymbols.Count > 0)
      {
        Terminal.WriteErrorLine($"Release gate failed: downloaded artifact is missing symbol package(s): {string.Join(", ", missingSymbols)}.");
        AbortPipeline("downloaded artifact is missing symbol packages");
        return;
      }

      Terminal.WriteLine($"Package set verified: {string.Join(", ", releasePackages.Select(package => package.FileName))}");

      WriteStepBanner("Step 6/6: Push to NuGet");

      if (string.IsNullOrEmpty(apiKey))
      {
        Terminal.WriteErrorLine("Release gate failed: no NuGet API key. The release job passes the OIDC key from nuget/login.");
        AbortPipeline("nuget api key missing");
        return;
      }

      await PushPackagesAsync(repoRoot, releasePackages, apiKey, cancellationToken);

      if (Environment.ExitCode != 0)
      {
        return;
      }

      foreach (ReleasePackage package in releasePackages)
      {
        await NotifySoftwareSiteAsync(repoRoot, package.PackageId, package.Version);
      }

      Terminal.WriteLine("");
      Terminal.WriteLine("===============================================================================");
      Terminal.WriteLine("  Pipeline SUCCEEDED - Packages published to NuGet.org");
      Terminal.WriteLine("===============================================================================");
    }

    private async Task PushPackagesAsync(string repoRoot, IReadOnlyList<ReleasePackage> packages, string apiKey, CancellationToken cancellationToken)
    {
      string artifactsDir = Path.Combine(repoRoot, ArtifactsSubPath);

      foreach (ReleasePackage package in packages)
      {
        string nupkgPath = Path.Combine(artifactsDir, package.FileName);
        string snupkgPath = Path.Combine(artifactsDir, package.SymbolFileName);

        if (!File.Exists(nupkgPath))
        {
          throw new FileNotFoundException($"Package not found: {nupkgPath}");
        }

        if (!File.Exists(snupkgPath))
        {
          throw new FileNotFoundException($"Symbol package not found: {snupkgPath}");
        }

        await PushFileIsolatedAsync(nupkgPath, apiKey, cancellationToken);
        if (Environment.ExitCode != 0)
        {
          return;
        }

        await PushFileIsolatedAsync(snupkgPath, apiKey, cancellationToken);
        if (Environment.ExitCode != 0)
        {
          return;
        }
      }

      Terminal.WriteLine("\n✓ Packages pushed to NuGet.org");
    }

    // Copy one package into an empty directory and push that copy. dotnet nuget
    // push of a nupkg also pushes a sibling snupkg when one sits beside it.
    private async Task PushFileIsolatedAsync(string sourcePath, string apiKey, CancellationToken cancellationToken)
    {
      string fileName = Path.GetFileName(sourcePath);
      string isolatedDir = Directory.CreateTempSubdirectory("timewarp-terminal-push-").FullName;

      try
      {
        string isolatedPath = Path.Combine(isolatedDir, fileName);
        File.Copy(sourcePath, isolatedPath);

        Terminal.WriteLine($"  Pushing {fileName}...");

        int exitCode = await Shell.Builder("dotnet")
          .WithArguments("nuget", "push", isolatedPath, "--source", NuGetSource, "--api-key", apiKey, "--skip-duplicate")
          .WithWorkingDirectory(isolatedDir)
          .WithNoValidation()
          .RunAsync(cancellationToken);

        if (exitCode != 0)
        {
          Terminal.WriteErrorLine($"NuGet push failed: {fileName}");
          AbortPipeline("nuget push failed");
        }
      }
      finally
      {
        Directory.Delete(isolatedDir, recursive: true);
      }
    }

    private async Task NotifySoftwareSiteAsync(string repoRoot, string packageId, string version)
    {
      // Best effort. A failure here must never fail a release that already pushed.
      // Locally gh's stored auth suffices. In Actions the job token cannot reach
      // another repo, so workflow.yml passes an App installation token as
      // REBUILD_DISPATCH_TOKEN and this call uses it as GH_TOKEN only.
      Terminal.WriteLine($"\nNotifying timewarp-software to rebuild the site for {packageId}...");

      string? rebuildToken = Environment.GetEnvironmentVariable("REBUILD_DISPATCH_TOKEN");

      ShellBuilder dispatch = Shell.Builder("gh")
        .WithArguments
        (
          "api",
          "repos/TimeWarpEngineering/timewarp-software/dispatches",
          "-f", "event_type=rebuild",
          "-f", $"client_payload[package]={packageId}",
          "-f", $"client_payload[version]={version}"
        )
        .WithWorkingDirectory(repoRoot)
        .WithNoValidation();

      if (!string.IsNullOrEmpty(rebuildToken))
      {
        dispatch = dispatch.WithEnvironmentVariable("GH_TOKEN", rebuildToken);
      }

      int exitCode;
      try
      {
        exitCode = await dispatch.RunAsync();
      }
      catch (System.ComponentModel.Win32Exception)
      {
        exitCode = -1;
      }

      if (exitCode == 0)
      {
        Terminal.WriteLine("✓ timewarp-software rebuild dispatched");
      }
      else
      {
        Terminal.WriteLine("⚠ Could not dispatch timewarp-software rebuild (non-fatal; the site rebuilds nightly)");
      }
    }

    private static async Task<string?> ResolveProjectVersionAsync(string repoRoot, string projectPath, CancellationToken cancellationToken)
    {
      CommandOutput result = await Shell.Builder("dotnet")
        .WithArguments("msbuild", projectPath, "-getProperty:Version", "-nologo")
        .WithWorkingDirectory(repoRoot)
        .WithNoValidation()
        .CaptureAsync(cancellationToken);

      if (result.ExitCode != 0)
      {
        return null;
      }

      string version = result.Stdout.Trim();
      return string.IsNullOrWhiteSpace(version) ? null : version;
    }
  }
}
