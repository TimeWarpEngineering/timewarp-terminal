## Interface Inheritance Pattern

- `ITerminal : IConsole` uses `new` on sync Write methods for covariant return types (IConsole → ITerminal). Async methods return `Task` and are inherited as-is — no `new` needed.
- Implementations must provide explicit interface implementations (`IConsole IConsole.Write(...)`) to bridge both return types. Without these, the compiler won't satisfy both interface contracts.

## Files That Must Change Together

- `iconsole.cs` ↔ `iterminal.cs` — adding a sync Write method to IConsole requires a `new` override in ITerminal for fluent chaining, plus explicit interface implementations in `timewarp-terminal.cs`, `test-terminal.cs`, and `test-console.cs`.
- Adding an async method to IConsole requires implementations in all four: `timewarp-terminal.cs`, `timewarp-console.cs`, `test-terminal.cs`, `test-console.cs`.
