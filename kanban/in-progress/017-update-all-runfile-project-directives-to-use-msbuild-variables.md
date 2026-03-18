# Update all runfile #:project directives to use MSBuild variables

## Description

Update all `#:project` directives in runfiles to use MSBuild variables (e.g., `$(SourceDirectory)`) instead of hardcoded relative paths. This leverages .NET 10's support for MSBuild property expansion in `#:project` directives (PR #51108).

## Checklist

### samples/ (4 remaining)
- [x] samples/table-widget.cs (already updated)
- [ ] samples/hyperlink-widget.cs
- [ ] samples/emoji-table-widget.cs
- [ ] samples/rule-widget.cs
- [ ] samples/panel-widget.cs

### tests/ (23 files)
- [ ] tests/table-widget-01-basic.cs
- [ ] tests/table-widget-02-borders.cs
- [ ] tests/table-widget-03-styling.cs
- [ ] tests/table-widget-04-expand.cs
- [ ] tests/table-widget-05-shrink.cs
- [ ] tests/table-widget-06-emoji.cs
- [ ] tests/table-widget-07-grow.cs
- [ ] tests/panel-widget-01-basic.cs
- [ ] tests/panel-widget-02-terminal-extensions.cs
- [ ] tests/panel-widget-03-word-wrap.cs
- [ ] tests/rule-widget-01-basic.cs
- [ ] tests/rule-widget-02-terminal-extensions.cs
- [ ] tests/hyperlink-01-basic.cs
- [ ] tests/terminal-static-01-basic.cs
- [ ] tests/terminal-static-02-properties.cs
- [ ] tests/terminal-static-03-operations.cs
- [ ] tests/terminal-static-04-format.cs
- [ ] tests/terminal-static-05-widgets.cs
- [ ] tests/terminal-static-06-color.cs
- [ ] tests/ansi-string-utils-01-basic.cs
- [ ] tests/ansi-string-utils-02-wrap-text.cs
- [ ] tests/ansi-string-utils-03-emoji-width.cs
- [ ] tests/unicode-width-01-basic.cs

### Verification
- [ ] Run all updated runfiles to verify they work
- [ ] Commit changes

## Notes

### MSBuild Variable Support

.NET 10.0.201 supports MSBuild variable expansion in `#:project` directives (merged in PR #51108). The `Directory.Build.props` imports `msbuild/repository.props` which defines:

- `$(SourceDirectory)` = `$(RepositoryRoot)source/`
- `$(TestsDirectory)` = `$(RepositoryRoot)tests/`
- `$(SamplesDirectory)` = `$(RepositoryRoot)samples/`

### Change Pattern

Replace:
```csharp
#:project ../source/timewarp-terminal/timewarp-terminal.csproj
```

With:
```csharp
#:project $(SourceDirectory)timewarp-terminal/timewarp-terminal.csproj
```

### Benefits

1. **Consistency** - All runfiles use the same path resolution mechanism
2. **Maintainability** - If project structure changes, only `repository.props` needs updating
3. **Clarity** - Intent is clearer (`SourceDirectory` vs `../source`)
