# Releasing TimeWarp.Terminal

How this repo cuts and publishes a release: the version source of truth, the
normal flow from version bump to NuGet, the gates the pipeline enforces, the
break-glass path, and the trusted-publishing probe.

This guide documents this repo's instantiation of the TimeWarp release
convention. The release run promotes the CI-built `Packages-*` artifact. It
does not rebuild.

## Overview

- **Version source of truth:** `<Version>` in `source/Directory.Build.props`.
  There is no MinVer and no per-project version override. The tag is `v{Version}`.
- **One version, lockstep.** `TimeWarp.Terminal` and `TimeWarp.Terminal.Layout`
  share that version and release together. A project that must not ship sets
  `IsPackable=false`. Do not add `<Version>` to a csproj.
- **Packable set is derived.** `IsPackable` and `PackageId` come from MSBuild
  evaluation of every `*.csproj` under `source/`. Adding or removing a packable
  project changes what ships. The release gate also evaluates each project's
  `<Version>` and refuses one that differs from the props version.
- **Humans type a version exactly once**, in the props-bump pull request. The
  tag, the GitHub Release, the gates, and the NuGet push derive from that value.
- **Prerelease versions use the same pipeline.** `dev release` passes
  `--prerelease` when the version contains `-`.
- **Symbol packages ship once.** CI uploads each `.nupkg` and its sibling
  `.snupkg`. The push copies each file into its own directory before
  `dotnet nuget push`, because a nupkg push also uploads a sibling snupkg in
  the same directory.

## Normal release flow

### 1. Bump the version and merge to master

Open a pull request that changes `<Version>` in `source/Directory.Build.props`.
After it merges, the `push` event runs `workflow.yml` in merge mode:

`clean → build → verify-samples → test`

`GeneratePackageOnBuild` is set only for Release, in `source/Directory.Build.props`.
The Release build (`ContinuousIntegrationBuild=true`) writes the nupkgs and
snupkgs. A later Debug build, including the sample and test runfiles in the
same job, does not pack again. The merge pipeline hashes every nupkg and
snupkg after the build and fails if a later step changes them. On success,
and only for a push to `master`, the job uploads `Packages-{run_number}` with
`artifacts/packages/*.nupkg` and `artifacts/packages/*.snupkg`
(`if-no-files-found: error`, `retention-days: 7`). Older `Packages-*` artifacts
are then deleted, keeping the two newest. Pull requests do not upload. Nothing
is published yet.

The `push` trigger is an allowlist (`source/**`, `tests/**`, `samples/**`,
`tools/**`, `msbuild/**`, the workflow file, `Directory.Build.props`,
`Directory.Packages.props`, `global.json`). A push that matches nothing gets
no run. That is required: a successful run with no packages would look tested.
`dev release` (DevCli 4.0.0-beta.8 and later) walks first-parent history back
across kanban-only commits to the newest green run.

### 2. Cut the release with `dev release`

From a clean `master` worktree that is in sync with `origin/master`:

```bash
dev release --dry-run
dev release
```

`dev release` reads `<Version>` and runs its guards before creating the
annotated tag `v{Version}` and the GitHub Release. A missing successful
`workflow.yml` run at the release commit refuses up front. When the tag already
exists, resume that release with break-glass. Do not cut a second tag.

### 3. `release: published` promotes the artifact

Publishing the GitHub Release runs the `release` job. That job does not build
or test the product. It runs:

`tag-gate → check-version → locate-run → download-artifact → verify → push`

1. **Tag gate.** On a `release` event, `GITHUB_REF_NAME` must equal
   `v{Version}`. If the tag already exists, `HEAD` must be that tag's commit
   (tag pin). `HEAD` must be an ancestor of `origin/master` (full history;
   `fetch-depth: 0`).
2. **Check version.** The shared `check-version` gate runs. None and Partial
   proceed. All aborts ("version already released"). Partial is safe only
   because the tag pin already bound this run to the same commit as the earlier
   push. The packable set must be non-empty, and every project's evaluated
   version must equal the props version.
3. **Locate CI run.** `gh run list` finds successful `workflow.yml` runs at
   `HEAD`. Pull-request runs are excluded. A push run is preferred.
4. **Download artifact.** The first candidate with a non-expired `Packages-*`
   artifact is downloaded into `artifacts/packages`. An expired or missing
   artifact is a refusal. `gh run rerun <run-id>` regenerates the same commit's
   artifact.
5. **Verify.** Downloaded `.nupkg` names must equal `{PackageId}.{version}.nupkg`
   for the derived set, and each sibling `.snupkg` must be present.
6. **Push.** Each nupkg and each snupkg is pushed once to nuget.org with
   `--skip-duplicate`, using the short-lived OIDC API key. `--skip-duplicate`
   makes a resumed push idempotent. It does not check byte identity. The tag
   pin does that.

Success ends with `Pipeline SUCCEEDED - Packages published to NuGet.org`. The
job then asks timewarp-software to rebuild, once per package id. That dispatch
is best-effort.

## Why promotion, not rebuild

The published bytes are the bytes the master CI run built, sample-checked,
tested, and uploaded. The release job does not compile the product, so a later
SDK resolution cannot change what ships.

## Break-glass

A `workflow_dispatch` with the default `mode=merge` builds and tests. It does
not publish.

Publishing from dispatch requires both `mode: release` and `confirm: release`
(typed exactly). Any other confirm value fails the run at the confirmation step
before NuGet login. When both match, the job runs
`dev workflow --mode release`. The tag-pin check still requires this checkout
to be the tagged commit when `v{Version}` already exists.

Use break-glass to resume a release whose `release: published` run did not
finish (expired artifact, transient `gh` failure, partial NuGet push). Re-run
it from the tagged commit. Do not cut a second tag.

## Trusted publishing

NuGet credentials are not a repository secret. The release job grants
`id-token: write` and runs `nuget/login@v1` with `user: TimeWarp.Enterprises`
on a release event, a confirmed break-glass dispatch, or a probe. The OIDC
exchange yields a short-lived API key. The trusted-publishing policy (this
repo, `workflow.yml`, `TimeWarp.Enterprises`) is configured on nuget.org.

### Probe

```bash
gh workflow run workflow.yml --repo TimeWarpEngineering/timewarp-terminal -f mode=probe
```

The probe logs in with OIDC and stops. It does not build or publish. Login
failure means the nuget.org policy is missing or does not match this repo and
`workflow.yml`.

## Operator notes

- **Tokens.** `GH_TOKEN` on the release job is the job token, so locate-run and
  download-artifact can read this repo's Actions artifacts (`actions: read`).
  The timewarp-software dispatch uses a separate GitHub App token in
  `REBUILD_DISPATCH_TOKEN` (org variable `REBUILD_APP_ID`, org secret
  `REBUILD_APP_PRIVATE_KEY`). When the app is not configured, the dispatch warns
  and the site rebuilds on its nightly schedule.
- **Feeds.** `check-version` reads the feeds in `.timewarp/dev.jsonc`
  (`checkVersionConfig.feeds`). When that key is absent it reads nuget.org.
  The release job also grants `packages: read` so a configuration that includes
  the org GitHub Packages feed can authenticate with the job token.
- **Do not pack at release time.** If the artifact for a SHA is gone,
  `gh run rerun <run-id>` rebuilds it from that commit. Then re-run the release
  workflow.
