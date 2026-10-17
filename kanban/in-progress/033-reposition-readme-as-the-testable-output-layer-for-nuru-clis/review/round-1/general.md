# Round 1 — general
**Date:** 2026-10-17
**Scope reviewed:** branch `task/033-reposition-readme-as-the-testable-output-layer-for` vs `master` (f690ad2, 9336214): `readme.md`, `Directory.Packages.props`, task kitchen.

## Summary

The README now opens with "testable output layer for Nuru CLIs". It adds "What you get", "When to use something else", and "Testing a Nuru command" sections, and it adds two clarifying sentences in the Interfaces and Layout sections. The pin bump (Nuru 3.0.0, Amuru 2.0.0) is outside the task's literal scope, but `ganda repo audit` required it and Results records it. Release build is green (0 warnings, 0 errors, re-run by the reviewer).

These claims checked out against the code:
- `IsAotCompatible` is set on both csproj files.
- `TimeWarp.Builder` is the only non-private package reference of `TimeWarp.Terminal`.
- Layout is a separate project that references Terminal.
- `TestTerminal` and `TestConsole` are both `IDisposable` and both have `OutputContains`. `TestTerminal.Output` exists.
- `TestTerminalContext.Use` is backed by `AsyncLocal`.
- The `TestTerminal` doc comment says it is not thread-safe.
- The full interface members match the new "full contracts" sentence: streams, encodings, redirection flags, cursor and window geometry, and `CancelKeyPress`.
- `NuruAppBuilder.UseTerminal(ITerminal)` exists in Nuru, and `NuruApp` falls back to `TimeWarpTerminal.Default`.

One claim about Nuru handler injection is false.

## Issues

### Issue 1 — Severity: bug
- File: readme.md:7
- Description: "A Nuru handler can take `ITerminal` or `IConsole`." Nuru does not provide `IConsole` as a built-in handler service. `ServiceValidator.AddBuiltInServices` (timewarp-nuru `source/timewarp-nuru-analyzers/validation/service-validator.cs:154`) adds only `ITerminal`, `IConfiguration`, `IConfigurationRoot`, `NuruApp`, and `CancellationToken`. `ServiceResolverEmitter` special-cases only `ITerminal` to `__fw_ITerminal`. The shipped `TimeWarp.Nuru.Analyzers.dll` in the 3.0.0 package has no `TimeWarp.Terminal.IConsole` string, but it has five `ITerminal` hits. A handler that takes `IConsole` without registering it gets an unregistered-service diagnostic and is not wired to `UseTerminal`'s `TestTerminal`. The task requires "no claims that are not true of the current code", so this breaks the contract.
- Suggestion: Say a Nuru handler takes `ITerminal`, which also implements `IConsole`. Alternatively, drop `IConsole` from that sentence. Keep line 82's `TestConsole` guidance for code that only depends on `IConsole` outside Nuru injection.
- Status: open
