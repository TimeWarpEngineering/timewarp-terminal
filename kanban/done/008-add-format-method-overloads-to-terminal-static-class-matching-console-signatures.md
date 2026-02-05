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

## Results

### What Was Implemented
Added 12 format method overloads to the `Terminal` static class that mirror `System.Console`'s signatures:
- 4 `Write` overloads: `(string format, object? arg0)`, `(string format, object? arg0, object? arg1)`, `(string format, object? arg0, object? arg1, object? arg2)`, `(string format, params object?[] args)`
- 4 `WriteLine` overloads: Same signature pattern as Write
- 4 `WriteErrorLine` overloads: Same signature pattern as Write

### Files Changed
1. `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/Cramer-2025-12-22-dev/source/timewarp-terminal/terminal-static.cs`
   - Added `using System.Globalization;` statement
   - Added 12 format method implementations
   - All methods include XML documentation

2. `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-terminal/Cramer-2025-12-22-dev/tests/terminal-static-04-format.cs` (new file)
   - 16 comprehensive unit tests covering all overloads

### Key Decisions
- Used `CultureInfo.InvariantCulture` for consistent formatting across locales (as specified)
- Placed new methods in a "Format Overloads" section between WriteErrorLineAsync and Input Methods
- All methods use expression-bodied syntax for conciseness
- Params array version uses `object?[]` to match Console signatures

### Test Outcomes
All 16 tests pass successfully:
- 4 Write format tests (single, two, three args, params)
- 4 WriteLine format tests (single, two, three args, params)
- 4 WriteErrorLine format tests (single, two, three args, params)
- 2 Numeric formatting tests (D4 and F2 formats)
- 2 Null argument tests

### Verification
- Build: Succeeded with 0 warnings, 0 errors
- Tests: All 16 tests pass
