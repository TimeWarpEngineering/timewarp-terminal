# Disable XML documentation file for dev-cli so worktree self-install succeeds

## Description

The worktree on-create hook runs `dev self-install`, which AOT-publishes the dev CLI runfile. With
`GenerateDocumentationFile` inherited from the root `Directory.Build.props`, the publish fails:

```
Microsoft.NET.Publish.targets(396,5): error MSB3030: Could not copy the file
".../runfile/dev-<hash>/bin/release/dev.xml" because it was not found.
```

Seen on the task 034 and task 035 claims. The dev CLI is a runfile, not a product assembly, so it
does not need an XML doc file. timewarp-flow and timewarp-architecture (a98eddc34) already carry the
same fix in their dev-cli props.

## Checklist

- [x] Set `GenerateDocumentationFile` to `false` in `tools/dev-cli/Directory.Build.props` with a comment
- [x] `dotnet run tools/dev-cli/dev.cs -- self-install` succeeds in a fresh task worktree
- [x] `./bin/dev build` passes
- [x] `ganda repo audit` passes

## Session

- Created: 376564 (2026-10-17)
- Implemented: 376564 (2026-10-17), cockpit session, one-line props change

## Results

Observed 2026-10-17 in the task 035 worktree:

- Before the change, the claim's on-create hook failed with MSB3030 on `bin/release/dev.xml`.
- After the change, `dotnet run tools/dev-cli/dev.cs -- self-install` printed
  `Successfully installed dev CLI` and `bin/dev` exists.
- `./bin/dev build` printed `Build completed successfully`.
- `ganda repo audit` printed `Repository passes all audit checks.`

## Notes

Product source is untouched. Only the dev-cli build props change.
