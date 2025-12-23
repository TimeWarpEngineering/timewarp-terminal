// ===============================================================================
// DEV CLI - TIMEWARP.TERMINAL DEVELOPMENT TOOL
// ===============================================================================
//
// This is the unified development CLI for TimeWarp.Terminal that provides:
// - CI/CD orchestration commands for GitHub Actions
// - Development workflow automation
// - AOT-compiled binary for fast execution
// - Attributed routes for flexible command organization
//
// Architecture:
//   - Uses TimeWarp.Nuru with attributed routes for command registration
//   - Mediator pattern for clean separation of commands and handlers
//   - TimeWarp.Amuru for cross-platform shell and .NET operations
//   - AOT-compatible design for maximum performance
//
// Commands:
//   dev ci                 - Run full CI/CD pipeline (auto-detects mode)
//   dev ci --mode pr       - PR workflow: build -> verify-samples -> test
//   dev ci --mode release  - Release workflow: build -> check-version -> pack -> push
//   dev build              - Build all TimeWarp.Terminal projects
//   dev clean              - Clean solution and artifacts
//   dev test               - Run test suite
//   dev verify-samples     - Verify sample compilation
//   dev check-version      - Check if version already published
// ===============================================================================

NuruCoreApp app = NuruApp.CreateBuilder(args)
  .ConfigureServices(services => services.AddMediator())
  .AddAutoHelp()
  .WithMetadata("dev", "Development CLI for TimeWarp.Terminal")
  .Build();

return await app.RunAsync(args);
