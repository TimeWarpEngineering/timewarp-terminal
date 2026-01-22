# Add color support overloads and ANSI integration to Terminal static class

## Description

Add color parameter overloads and ensure seamless integration with the existing ANSI extension methods (C# 14 extension blocks). This enables users to write colored output using both explicit parameters and the fluent extension method style.

## Checklist

- [ ] Add `WriteLine(string message, ConsoleColor foregroundColor)` overload
- [ ] Add `WriteLine(string message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)` overload
- [ ] Add `Write(string message, ConsoleColor foregroundColor)` overload
- [ ] Add `WriteErrorLine(string message, ConsoleColor foregroundColor)` overload
- [ ] Add `WriteTable` color parameter support
- [ ] Add `WritePanel` color parameter support
- [ ] Ensure ANSI extension methods work with `Terminal.Instance.Write()`
- [ ] Add XML documentation for color overloads
- [ ] Write unit tests for color support
- [ ] Update README with color usage examples

## Notes

Integration with ANSI extension blocks (C# 14):
```csharp
using static TimeWarp.Terminal.Terminal;

// Fluent ANSI style (existing extension methods)
WriteLine("Error!".Red().Bold());
WriteLine("Success!".Green());

// Explicit color parameters
WriteLine("Error!", ConsoleColor.Red);
WriteLine("Warning!", ConsoleColor.Yellow, ConsoleColor.Black);
```
