#!/usr/bin/env -S dotnet --

#region Purpose
// Development CLI for TimeWarp.Terminal repository operations
#endregion
#region Design
// UseTelemetry is on so a CI job or a ganda walk that shells out to dev
// exports one command span per route, including the DevCli routes. The OTLP
// endpoint stays unset here: a non-literal OtlpEndpoint is ignored, and an
// unset OTEL_EXPORTER_OTLP_ENDPOINT exports nothing. ServiceName is the
// literal timewarp-terminal-dev. OTEL_SERVICE_NAME overrides it. This repo has
// no ActivitySource of its own, so none is registered. IncomingTraceContext
// restores TRACEPARENT before RunAsync because TelemetryBehavior parents its
// span to Activity.Current. See tools/dev-cli/telemetry.md.
#endregion
// ═══════════════════════════════════════════════════════════════════════════════
// DEV CLI - TIMEWARP.TERMINAL DEVELOPMENT TOOL
// ═══════════════════════════════════════════════════════════════════════════════
//
// This is the unified development CLI for TimeWarp.Terminal that provides:
// - CI/CD orchestration commands for GitHub Actions
// - Development workflow automation
// - AOT-compiled binary for fast execution
// - Endpoints for flexible command organization
//
// Usage:
//   As runfile:  dotnet tools/dev-cli/dev.cs <command>
//   As AOT:      ./dev <command>
//
// Architecture:
//   - Uses TimeWarp.Nuru with endpoints for command registration
//   - TimeWarp.Amuru for cross-platform shell and .NET operations
//   - AOT-compatible design for maximum performance
//
// Commands (Phase 1 - CI/CD Orchestration):
//   dev workflow            - Run full CI/CD pipeline (auto-detects mode)
//   dev workflow --mode pr  - PR workflow: clean -> build -> verify-samples -> test
//   dev workflow --mode release  - Release workflow: tag-gate -> check-version -> locate-run -> download-artifact -> verify -> push
//   dev build              - Build all TimeWarp.Terminal projects
//   dev clean              - Clean solution and artifacts
//   dev test               - Run CI test suite
//   dev verify-samples     - Verify sample compilation
//   dev check-version      - Check if version already published
//   dev self-install       - AOT compile and install dev CLI to ./bin
//
// Telemetry (tools/dev-cli/telemetry.md):
//   UseTelemetry service name is the literal "timewarp-terminal-dev".
//   Export follows OTEL_EXPORTER_OTLP_ENDPOINT. Unset exports nothing.
//   TRACEPARENT / TRACESTATE join a parent process's trace.
//
// To bootstrap:
//   dotnet run tools/dev-cli/dev.cs -- self-install
//   direnv allow
//   dev --help
// ═══════════════════════════════════════════════════════════════════════════════

NuruApp app = NuruApp.CreateBuilder()
  .WithDescription("Development CLI for timewarp-terminal")
  .UseTelemetry(options => options.ServiceName = "timewarp-terminal-dev")
  .ConfigureServices(services =>
  {
    services.AddSingleton<IRepoCleanService, RepoCleanService>();
    services.AddSingleton<NuGetVersionService>();
    services.AddSingleton<IRepoConfigService, RepoConfigService>();
    services.AddSingleton<IPackableProjectService, PackableProjectService>();
  })
  .DiscoverEndpoints()
  .Build();

using IDisposable? incomingTrace = IncomingTraceContext.Restore();
return await app.RunAsync(args);
