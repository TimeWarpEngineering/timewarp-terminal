# Mirror new APIs on Terminal static class

## Description

Mirror all new `IConsole` and `ITerminal` members on the `Terminal` static class. This ensures the static facade provides complete Console replacement.

Parent: #020

Depends on: #020-001 through #020-007 (all other child tasks must be complete)

## Checklist

### Implementation
- [ ] Mirror stream access APIs from IConsole on `Terminal`
- [ ] Mirror encoding APIs from IConsole on `Terminal`
- [ ] Mirror redirection state APIs from IConsole on `Terminal`
- [ ] Mirror rich input APIs from IConsole on `Terminal`
- [ ] Mirror cursor properties from ITerminal on `Terminal`
- [ ] Mirror window/buffer geometry from ITerminal on `Terminal`
- [ ] Mirror color state APIs from ITerminal on `Terminal`
- [ ] Mirror control/utility APIs from ITerminal on `Terminal`

### Testing
- [ ] Write unit tests for static stream access methods
- [ ] Write unit tests for static encoding properties
- [ ] Write unit tests for static redirection properties
- [ ] Write unit tests for static input methods
- [ ] Write unit tests for static cursor properties
- [ ] Write unit tests for static window/buffer methods
- [ ] Write unit tests for static color state methods
- [ ] Write unit tests for static control/utility methods

## Session

- Created: ses_2f2ab32c3ffeoD0gwPTVU0agTi (2026-03-22)

## Notes

### Files to modify
- `terminal-static.cs` - add all static members

### Design considerations
- All static members route to `Instance` (the configured `ITerminal`)
- Properties should be simple pass-through: `public static int WindowHeight => Instance.WindowHeight;`
- Methods should be simple pass-through: `public static void Beep() => Instance.Beep();`
- This task should be done LAST after all interface changes are complete

### Reference
- See existing `Terminal` static class for pattern

### Coding Standards
Follow the `/csharp` skill for all implementation work.
