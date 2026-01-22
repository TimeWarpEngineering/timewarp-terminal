# Add format method overloads to Terminal static class (matching Console signatures)

## Description

Add format string overloads to the `Terminal` static class that mirror `System.Console`'s signatures. This enables users to use format strings like `Terminal.WriteLine("User {0} logged in", userName)` with the same syntax they're accustomed to from Console.

## Checklist

- [ ] Implement `Write(string format, object? arg0)`
- [ ] Implement `Write(string format, object? arg0, object? arg1)`
- [ ] Implement `Write(string format, object? arg0, object? arg1, object? arg2)`
- [ ] Implement `Write(string format, params object?[] args)`
- [ ] Implement `WriteLine(string format, object? arg0)`
- [ ] Implement `WriteLine(string format, object? arg0, object? arg1)`
- [ ] Implement `WriteLine(string format, object? arg0, object? arg1, object? arg2)`
- [ ] Implement `WriteLine(string format, params object?[] args)`
- [ ] Implement `WriteErrorLine(string format, object? arg0)`
- [ ] Implement `WriteErrorLine(string format, object? arg0, object? arg1)`
- [ ] Implement `WriteErrorLine(string format, object? arg0, object? arg1, object? arg2)`
- [ ] Implement `WriteErrorLine(string format, params object?[] args)`
- [ ] Use `CultureInfo.InvariantCulture` for consistent formatting
- [ ] Add XML documentation for format overloads
- [ ] Write unit tests for format methods with various argument counts

## Notes

## Implementation Plan

### File Modifications

**File:** `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/Cramer-2025-12-22-dev/source/timewarp-terminal/terminal-static.cs`

**1. Add `using System.Globalization;` statement**
- Insert after line 1 (namespace declaration)

**2. Add format method overloads**
- Insert after line 89 (after `WriteErrorLineAsync`), before line 91 (Input Methods section)
- 12 new methods total (4 Write, 4 WriteLine, 4 WriteErrorLine)

### Implementation Pattern
```csharp
public static void WriteLine(string format, object? arg0)
  => Instance.WriteLine(string.Format(CultureInfo.InvariantCulture, format, arg0));
```

### New Test File
**File:** `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/Cramer-2025-12-22-dev/tests/terminal-static-04-format.cs`
- 16 test cases covering all overloads
- Uses TestTerminal for output capture
- Tests numeric formatting, null args, params arrays

### Verification Steps
1. Compile: `dotnet build source/timewarp-terminal/timewarp-terminal.csproj`
2. Run new tests: `dotnet run --project tests/terminal-static-04-format.cs`
3. Verify existing tests still pass

### Summary
- 12 new format methods added
- 16 new test cases
- Uses CultureInfo.InvariantCulture for consistent formatting
- Full XML documentation for all methods

---

Console.WriteLine has these format signatures:
- `WriteLine(string format, object? arg0)`
- `WriteLine(string format, object? arg0, object? arg1)`
- `WriteLine(string format, object? arg0, object? arg1, object? arg2)`
- `WriteLine(string format, params object?[] args)`
