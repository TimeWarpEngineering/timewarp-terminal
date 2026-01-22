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

Console.WriteLine has these format signatures:
- `WriteLine(string format, object? arg0)`
- `WriteLine(string format, object? arg0, object? arg1)`
- `WriteLine(string format, object? arg0, object? arg1, object? arg2)`
- `WriteLine(string format, params object?[] args)`

Implementation pattern:
```csharp
public static void WriteLine(string format, object? arg0)
  => Instance.WriteLine(string.Format(CultureInfo.InvariantCulture, format, arg0));
```
