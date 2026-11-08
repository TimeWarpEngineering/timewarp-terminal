# Enable dev CLI telemetry with UseTelemetry

## Goal (implementer restatement)

Every `dev` command should emit one OpenTelemetry command span and a duration metric so a ganda walk or a CI job can see which commands are slow. TimeWarp.Nuru only emits that code when this repo's `tools/dev-cli/dev.cs` calls `UseTelemetry` with the literal service name `timewarp-terminal-dev`. Copy `IncomingTraceContext` and restore the caller's `TRACEPARENT` around `RunAsync` so the command span nests under ganda or CI. This repo has no `ActivitySource` of its own, so none is registered. Export stays off unless `OTEL_EXPORTER_OTLP_ENDPOINT` is set. `dev --capabilities`, `dev build`, and the AOT publish of the dev CLI must still succeed. `tools/dev-cli/telemetry.md` names the environment variables.

## Description

Every `dev` command in this repo should emit OpenTelemetry spans and metrics so we can see
which dev commands are slow across the fleet. Telemetry in TimeWarp.Nuru is a compile-time
opt-in: the `TimeWarp.Nuru.DevCli` package ships no telemetry code, so each repo's
`tools/dev-cli/dev.cs` must call `UseTelemetry` on the builder. Without it the source
generator emits nothing.

Reference implementation: timewarp-nuru `tools/dev-cli/dev.cs` (kanban 556) and
`tools/dev-cli/telemetry.md`. Nuru docs: `documentation/user/features/telemetry.md`.

Export is gated at runtime on `OTEL_EXPORTER_OTLP_ENDPOINT` (set in the developer shell to
`https://otlp.timewarp.work` with `OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf`). No endpoint
means no export, so the change is safe in CI even without a collector.

## Requirements

- `tools/dev-cli/dev.cs` calls `.UseTelemetry(options => options.ServiceName = "timewarp-terminal-dev")`
  on the `NuruApp.CreateBuilder()` chain. The service name must be a string literal.
- Every route, including the DevCli-provided ones (clean, check-version, release, self-install),
  gets the automatic `TimeWarp.Nuru.Behavior` command span and duration metric.
- Copy `IncomingTraceContext` from timewarp-nuru `tools/dev-cli/endpoints/incoming-trace-context.cs`
  and wrap `app.RunAsync(args)` in `IncomingTraceContext.Restore()` so command spans nest under
  the caller's `TRACEPARENT` (ganda, CI).
- If the repo has its own `ActivitySource`, register it with `options.ActivitySources.Add("<literal>")`.
- `dev --capabilities` and `dev build` still work; AOT publish of the dev CLI still succeeds.

## Checklist

- [x] Add `UseTelemetry` call to `tools/dev-cli/dev.cs`
- [x] Add `IncomingTraceContext` and wrap `RunAsync`
- [x] Register repo-specific ActivitySources if any (none in this repo)
- [x] Build dev CLI (`bin/dev build`) and run one command with `OTEL_EXPORTER_OTLP_ENDPOINT` set; confirm a span arrives
- [x] Add a short `tools/dev-cli/telemetry.md` naming the env vars (copy from timewarp-nuru)

## Results

`tools/dev-cli/dev.cs` calls `UseTelemetry` with the literal service name `timewarp-terminal-dev`. `IncomingTraceContext.Restore()` runs before `RunAsync`. A search of `*.cs` found no `ActivitySource` in this repo, so none is registered. `tools/dev-cli/telemetry.md` names `OTEL_EXPORTER_OTLP_ENDPOINT`, `OTEL_SERVICE_NAME`, `TRACEPARENT`, `TRACESTATE`, and `OTEL_EXPORTER_OTLP_PROTOCOL`. This repo's `workflow` command does not emit stage spans, so the doc does not copy the timewarp-nuru stage table.

`dotnet build tools/dev-cli/dev.cs -c Release` succeeded. `dotnet run --file tools/dev-cli/dev.cs -- self-install` AOT-published `./bin/dev`. `./bin/dev --capabilities` lists `build`, `test`, `verify-samples`, `workflow`, `check-version`, `clean`, `release`, and `self-install`. `./bin/dev build` ended with `Build completed successfully` (0 warnings, 0 errors).

`./bin/dev check-version` with `OTEL_EXPORTER_OTLP_ENDPOINT=http://127.0.0.1:14318`, `OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf`, and `TRACEPARENT=00-a036a036a036a036a036a036a036a036-b036b036b036b036-01` posted a `check-version` span to that collector. The span's service name was `timewarp-terminal-dev`, its activity source was `TimeWarp.Nuru.Behavior`, its trace id was `a036a036a036a036a036a036a036a036`, and its parent span id was `b036b036b036b036`. The metrics payload included `nuru.behavior.commands.duration` for `check-version`. The command exited 0: source version `2.0.0-beta.1` is newer than NuGet `1.0.2`. The same binary with those variables unset sent nothing to the collector.

`ganda repo audit` had one advisory (`vscode-window-icon`). `ganda repo audit --fix` wrote a gitignored `.vscode/settings.json` window title. The follow-up audit passed 35 checks, failed 0, skipped 2.

### How to validate

Smoke: from this worktree, run `./bin/dev --capabilities`, then `./bin/dev build`. With a collector listening, run `OTEL_EXPORTER_OTLP_ENDPOINT=http://127.0.0.1:14318 OTEL_EXPORTER_OTLP_PROTOCOL=http/protobuf TRACEPARENT=00-a036a036a036a036a036a036a036a036-b036b036b036b036-01 ./bin/dev check-version`.

Expect: capabilities JSON includes `build`, `clean`, `check-version`, `release`, and `self-install`. The build ends with `Build completed successfully` and 0 warnings. The collector receives one `check-version` span for service `timewarp-terminal-dev` on source `TimeWarp.Nuru.Behavior`, trace id `a036a036a036a036a036a036a036a036`, parented to span `b036b036b036b036`, plus a `nuru.behavior.commands.duration` metric. `check-version` exits 0 while source version `2.0.0-beta.1` is ahead of published `1.0.2`. Unset `OTEL_EXPORTER_OTLP_ENDPOINT` exports nothing.

## Session

- Created: b3be0249-1852-425a-9e90-5a7e56049503 (2026-11-08)
- Implementation: 01a1bc61-6e53-72e3-bef1-dd5da359f792 (2026-11-09). Audit --fix wrote the gitignored VS Code window title. No repo ActivitySource to register.
- Implement oracle: implemented by implementer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a1bc61-6e53-72e3-bef1-dd5da359f792, max-turns 500, Done — 2026-11-08T18:24:49Z

## Notes

Fleet rollout: Steve asked on 2026-11-07 for telemetry on all dev CLI commands in every live
reposet repo. The same task exists in each live repo that has a `tools/dev-cli`. Only
timewarp-nuru had it before this rollout.
