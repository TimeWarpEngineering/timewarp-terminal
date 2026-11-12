# Set ContinuousIntegrationBuild only in CI for dev workflow

## Description

`dev workflow` currently passes `-p:ContinuousIntegrationBuild=true` on the local Release build, which weakens incremental builds on a developer or agent machine.

Call site: `tools/dev-cli/endpoints/workflow.cs:124` (`dotnet build timewarp-terminal.slnx -c Release -p:ContinuousIntegrationBuild=true`).

Use the same gate as Mediator's `Directory.Build.props:8-9`: `<ContinuousIntegrationBuild Condition="'$(GITHUB_ACTIONS)' == 'true'">true</ContinuousIntegrationBuild>` (or an equivalent `CI` env gate). The property should be true in GitHub Actions / CI packaging, and unset or false for a local `dev workflow` / `dev build`.

## Requirements

- Local `dev workflow` / `dev build` must not force `ContinuousIntegrationBuild=true`.
- CI packs and official workflow runs still set it (GITHUB_ACTIONS or CI).
- Prefer setting the property in MSBuild props with a condition, rather than a hard-coded `-p:` on every local invocation, if that keeps CI behavior.

## Checklist

- [ ] Remove or gate the unconditional `-p:ContinuousIntegrationBuild=true` at `workflow.cs:124`
- [ ] Confirm CI / release packaging still sets `ContinuousIntegrationBuild`
- [ ] Confirm a local `dev workflow` build is incremental across no-change reruns

## Acceptance criteria

- A no-change local `dev workflow` (or the build step it runs) can incrementally build.
- CI-produced packages still get `ContinuousIntegrationBuild` (deterministic / SourceLink-style CI semantics).

## Notes

- The 2026-11-12 audit also saw **52 `dotnet build-server shutdown` spans in 7 days** on SigNoz service `dotnet-cli`. That call is **not** in this repo. The hunt lives on timewarp-jenzi 011. One known in-tree caller elsewhere is a Jaribu test helper (`timewarp-jaribu` `testing-platform-props.transitive-consumer.cs:203`), which is unlikely to explain the 52.
- Sibling: timewarp-flexbox 157 (same pack flag at `workflow-command.cs:155`).
- Filed by Jenzi from cross-repo build-telemetry audit 2026-11-12.
