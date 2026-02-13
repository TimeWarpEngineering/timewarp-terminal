// ═══════════════════════════════════════════════════════════════════════════════
// SELF INSTALL COMMAND
// ═══════════════════════════════════════════════════════════════════════════════
// AOT compiles and installs the dev CLI to ./bin

namespace DevCli.Commands;

/// <summary>
/// AOT compile and install dev CLI to ./bin
/// </summary>
[NuruRoute("self-install", Description = "AOT compile and install dev CLI to ./bin")]
internal sealed class SelfInstallCommand : ICommand<Unit>
{
  internal sealed class Handler : ICommandHandler<SelfInstallCommand, Unit>
  {
    private readonly ITerminal Terminal;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(SelfInstallCommand command, CancellationToken ct)
    {
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

      string binDir = Path.Combine(repoRoot, "bin");
      Directory.CreateDirectory(binDir);

      string devCliPath = Path.Combine(repoRoot, "tools", "dev-cli", "dev.cs");
      string outputPath = Path.Combine(binDir, "dev");

      Terminal.WriteLine("Building dev CLI with AOT compilation...");
      Terminal.WriteLine($"Source: {devCliPath}");
      Terminal.WriteLine($"Output: {outputPath}");

      // Publish AOT
      int exitCode = await Shell.Builder("dotnet")
        .WithArguments("publish", devCliPath, "-c", "Release", "-o", binDir, "-p:PublishAot=true", "--self-contained")
        .WithWorkingDirectory(repoRoot)
        .RunAsync();

      if (exitCode != 0)
      {
        throw new InvalidOperationException("AOT compilation failed!");
      }

      // Make executable on Unix
      if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
      {
        await Shell.Builder("chmod")
          .WithArguments("+x", outputPath)
          .RunAsync();
      }

      Terminal.WriteLine($"\n✓ dev CLI installed to: {outputPath}");
      Terminal.WriteLine("\nTo use it:");
      Terminal.WriteLine("  ./bin/dev --help          # Run directly");
      Terminal.WriteLine("  direnv allow              # Or add to PATH via .envrc");

      return Unit.Value;
    }
  }
}
