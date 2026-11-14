# Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-terminal

## Description

Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-terminal.
Filter: (all)
Outdated:
  TimeWarp.Jaribu 1.0.0-beta.15 -> 1.0.0-beta.16 (patch)
  TimeWarp.Nuru 4.0.0-beta.12 -> 4.0.0-beta.13 (patch)
  TimeWarp.Nuru.DevCli 4.0.0-beta.12 -> 4.0.0-beta.13 (patch)
  TimeWarp.SourceGenerators 1.0.0-beta.14 -> 1.0.0-beta.16 (patch)
  Microsoft.CodeAnalysis.CSharp.CodeStyle 5.9.0 -> 5.12.0 (minor)
Edit Directory.Packages.props in this repo only.

Version: set <Version> in source/Directory.Build.props from 2.0.0-beta.1 to 2.0.0-beta.2 in this pull request. The prerelease suffix is -beta.N, never +beta.N. The wave releases it after merge.

## Session

- Minted by ganda task wave run 20261114T054552Z-9a0f39
