# Round 1 — general
**Date:** 2026-11-09
**Scope reviewed:** same as framework

## Summary

The change adds `.UseTelemetry(options => options.ServiceName = "timewarp-terminal-dev")` to the `NuruApp.CreateBuilder()` chain in `tools/dev-cli/dev.cs`, wraps `RunAsync` with `using IDisposable? incomingTrace = IncomingTraceContext.Restore();`, adds `tools/dev-cli/endpoints/incoming-trace-context.cs`, and adds `tools/dev-cli/telemetry.md`. Risk is low.

Verified against the repo:
- `incoming-trace-context.cs` is byte-identical to timewarp-nuru `tools/dev-cli/endpoints/incoming-trace-context.cs` (empty `diff`). It sits in `namespace DevCli`, which `tools/dev-cli/Directory.Build.props` already adds as a global using.
- The `UseTelemetry` call and the `using` + `RunAsync` pattern match timewarp-nuru `tools/dev-cli/dev.cs` lines 62 and 73–74. The service name is a string literal, as the task requires.
- No `new ActivitySource(` or `ActivitySource(` appears in any `*.cs` file in the repo, so registering no source is correct.
- `dotnet build tools/dev-cli/dev.cs -c Release` succeeded.
- AOT `./bin/dev --capabilities` exits 0 when `TRACEPARENT` is unset, set to a valid value, or set to an invalid one, so the `UnsafeAccessor` path works under AOT and a bad header is ignored.
- `telemetry.md` names `OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_SERVICE_NAME`, `TRACEPARENT`, `TRACESTATE`, and `OTEL_EXPORTER_OTLP_PROTOCOL`, and leaves out the timewarp-nuru stage table because this repo has no stage spans.

The implementer restatement matches the task requirements, so there is no misread.

## Issues

None.
