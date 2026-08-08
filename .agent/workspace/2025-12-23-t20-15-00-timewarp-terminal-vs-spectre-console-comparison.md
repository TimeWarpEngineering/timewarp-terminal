# TimeWarp (Nuru + Terminal) vs Spectre.Console Feature Comparison

**Analysis Date:** 2025-12-23  
**TimeWarp.Nuru Version:** 3.0.0-beta.22  
**TimeWarp.Terminal Version:** 1.0.0-beta.2  
**Spectre.Console Version:** 1.0.0 (current as of documentation)

## Executive Summary

**TimeWarp** is a modular CLI ecosystem consisting of:
- **TimeWarp.Nuru** - Route-based CLI framework with REPL, tab completion, telemetry, and command parsing
- **TimeWarp.Terminal** - Terminal abstractions and widgets (Panel, Table, Rule, ANSI colors)

Together, they provide a **full replacement for Spectre.Console** with:
- Full **AOT/NativeAOT** and **trimming** compatibility
- Superior **testability** with separated interfaces and comprehensive test doubles
- Modern **route-based CLI** design inspired by ASP.NET Core Minimal APIs
- Rich **REPL support** with syntax highlighting and key bindings (Emacs, Vi, VS Code)
- **OpenTelemetry integration** for observability
- **Source generators** for compile-time route validation

---

## Feature Comparison Matrix

### Legend
- ✅ = Fully Supported
- ⚠️ = Partially Supported  
- ❌ = Not Supported
- 🔄 = Planned/In Development

---

## 1. Command Line Interface Framework

| Feature | TimeWarp.Nuru | Spectre.Console.Cli | Notes |
|---------|---------------|---------------------|-------|
| **Route-Based Commands** | ✅ `Map("deploy {env}")` | ❌ Class-based | Nuru uses ASP.NET Core style |
| **Command Parser** | ✅ Built-in | ✅ `CommandApp` | |
| **Strongly Typed Parameters** | ✅ `{id:int}`, `{date:DateTime}` | ✅ Settings classes | Nuru has inline type constraints |
| **Optional Parameters** | ✅ `{tag?}` | ✅ | |
| **Catch-All Parameters** | ✅ `{*args}` | ⚠️ Limited | |
| **Options with Values** | ✅ `--config {mode}` | ✅ | |
| **Boolean Flags** | ✅ `--verbose`, `-v` | ✅ | |
| **Option Aliases** | ✅ `--verbose,-v` | ✅ | |
| **Repeated Options** | ✅ `--tag {t}*` | ⚠️ | Arrays via repeated flags |
| **Parameter Descriptions** | ✅ `{env\|Target environment}` | ✅ Attributes | Inline in pattern |
| **Subcommands** | ✅ `git commit -m {msg}` | ✅ Nested commands | |
| **Command Composition** | ✅ Fluent builder | ✅ | |
| **Help Generation** | ✅ Auto-generated ANSI | ✅ | |
| **Custom Type Converters** | ✅ `IRouteTypeConverter` | ✅ | |
| **Enum Support** | ✅ `{env:Environment}` | ✅ | |
| **Source Generator Validation** | ✅ Compile-time errors | ❌ Runtime only | |
| **Attributed Routes** | ✅ `[NuruRoute("...")]` | ✅ Attributes | |

### Built-in Type Converters (Nuru)

| Type | Example |
|------|---------|
| `int`, `long`, `double`, `decimal` | `{count:int}` |
| `bool` | `{flag:bool}` |
| `DateTime`, `DateOnly`, `TimeOnly` | `{date:DateTime}` |
| `Guid`, `TimeSpan` | `{id:Guid}` |
| `Uri`, `FileInfo`, `DirectoryInfo` | `{url:Uri}` |
| `IPAddress` | `{addr:IPAddress}` |
| Custom enums | `{env:Environment}` |

---

## 2. REPL (Read-Eval-Print Loop)

| Feature | TimeWarp.Nuru | Spectre.Console | Notes |
|---------|---------------|-----------------|-------|
| **Interactive REPL Mode** | ✅ `RunReplAsync()` | ❌ | Full REPL support |
| **Custom Prompts** | ✅ `options.Prompt = "myapp> "` | ❌ | |
| **Welcome/Goodbye Messages** | ✅ | ❌ | |
| **Command History** | ✅ Persistent | ❌ | |
| **History Navigation** | ✅ Up/Down arrows | ❌ | |
| **History Search** | ✅ Ctrl+R reverse search | ❌ | |
| **Clear History** | ✅ `clear-history` command | ❌ | |
| **Tab Completion** | ✅ Route-aware | ❌ | |
| **Dynamic Completion** | ✅ Custom `ICompletionSource` | ❌ | Query at tab-press |
| **Static Shell Completion** | ✅ Bash, Zsh, PowerShell, Fish | ❌ | |
| **Syntax Highlighting** | ✅ PSReadLine-style | ❌ | |
| **Key Binding Profiles** | ✅ Emacs, Vi, VS Code, Windows | ❌ | |
| **Custom Key Bindings** | ✅ `KeyBindingBuilder` | ❌ | |
| **Kill Ring (Clipboard)** | ✅ Emacs-style | ❌ | |
| **Word Operations** | ✅ Alt+F, Alt+B, etc. | ❌ | |
| **Undo/Redo** | ✅ Ctrl+Z, Ctrl+Y | ❌ | |
| **Multi-line Input** | ✅ | ❌ | |
| **Interactive Mode Flag** | ✅ `--interactive`, `-i` | ❌ | |

---

## 3. Tab Completion

| Feature | TimeWarp.Nuru | Spectre.Console | Notes |
|---------|---------------|-----------------|-------|
| **Route-Based Completion** | ✅ Automatic from routes | ❌ | |
| **Dynamic Completion Sources** | ✅ `ICompletionSource` | ❌ | |
| **Parameter Completion** | ✅ Per-parameter sources | ❌ | |
| **Type-Based Completion** | ✅ Enum values auto-complete | ❌ | |
| **Shell Script Generation** | ✅ `--generate-completion bash` | ❌ | |
| **Bash Completion** | ✅ | ❌ | |
| **Zsh Completion** | ✅ | ❌ | |
| **PowerShell Completion** | ✅ | ❌ | |
| **Fish Completion** | ✅ | ❌ | |
| **Completion Descriptions** | ✅ With tooltips | ❌ | |

---

## 4. Dependency Injection & Services

| Feature | TimeWarp.Nuru | Spectre.Console.Cli | Notes |
|---------|---------------|---------------------|-------|
| **DI Container** | ✅ Microsoft.Extensions.DI | ⚠️ Limited | |
| **ConfigureServices** | ✅ `builder.ConfigureServices()` | ⚠️ | |
| **Handler DI Injection** | ✅ Inject services into handlers | ⚠️ | |
| **IConfiguration Support** | ✅ Full .NET configuration | ❌ | |
| **Settings Files** | ✅ `.settings.json` | ❌ | |
| **Command Line Overrides** | ✅ `--Section:Key=Value` | ❌ | |
| **Configuration Validation** | ✅ `ValidateOnStart()` | ❌ | |
| **FluentValidation Integration** | ✅ | ❌ | |

---

## 5. Mediator Pattern & Pipeline

| Feature | TimeWarp.Nuru | Spectre.Console | Notes |
|---------|---------------|-----------------|-------|
| **Mediator Integration** | ✅ Mediator.SourceGenerator | ❌ | |
| **Mixed Delegate/Mediator** | ✅ Per-route choice | ❌ | |
| **Pipeline Behaviors** | ✅ `IPipelineBehavior<,>` | ❌ | |
| **Logging Behavior** | ✅ | ❌ | |
| **Performance Monitoring** | ✅ | ❌ | |
| **Authorization Behavior** | ✅ `IRequireAuthorization` | ❌ | |
| **Retry Behavior** | ✅ Exponential backoff | ❌ | |
| **Exception Handling Behavior** | ✅ | ❌ | |
| **Telemetry Behavior** | ✅ OpenTelemetry spans | ❌ | |

---

## 6. Telemetry & Observability

| Feature | TimeWarp.Nuru | Spectre.Console | Notes |
|---------|---------------|-----------------|-------|
| **OpenTelemetry Integration** | ✅ Full OTEL support | ❌ | |
| **Distributed Tracing** | ✅ Activity spans | ❌ | |
| **Metrics** | ✅ Commands invoked/errored | ❌ | |
| **Command Duration Metrics** | ✅ | ❌ | |
| **Aspire Dashboard Integration** | ✅ | ❌ | |
| **Custom Activity Source** | ✅ `NuruActivitySource` | ❌ | |
| **Auto-Flush on Completion** | ✅ | ❌ | |

---

## 7. Testing Support

| Feature | TimeWarp (Nuru + Terminal) | Spectre.Console | Notes |
|---------|---------------------------|-----------------|-------|
| **Test Console** | ✅ `TestConsole` | ✅ `TestConsole` | |
| **Test Terminal** | ✅ `TestTerminal` | ⚠️ Combined | Separated concerns |
| **NuruTestContext** | ✅ Full app testing | ❌ | |
| **Captured Output** | ✅ `.Output`, `.ErrorOutput` | ✅ | |
| **Scripted Input** | ✅ Constructor input | ✅ | |
| **Key Queue** | ✅ `QueueKey()`, `QueueKeys()` | ⚠️ Limited | |
| **Arrow Key Testing** | ✅ `QueueArrow()` | ⚠️ | |
| **REPL Testing** | ✅ `QueueLine()` for sessions | ❌ | |
| **Exit Code Assertions** | ✅ | ✅ | |
| **Zero-Modification Testing** | ✅ Runfile test harness | ❌ | |
| **Output Assertions** | ✅ `OutputContains()` | ⚠️ Manual | |
| **Configurable Window Width** | ✅ | ✅ | |

---

## 8. Core Console Abstractions (Terminal)

| Feature | TimeWarp.Terminal | Spectre.Console | Notes |
|---------|-------------------|-----------------|-------|
| **Console Interface** | ✅ `IConsole` | ✅ `IAnsiConsole` | |
| **Terminal Interface** | ✅ `ITerminal` extends `IConsole` | ⚠️ Combined | Separated concerns |
| **Write/WriteLine** | ✅ | ✅ | |
| **WriteError/WriteErrorLine** | ✅ | ✅ | |
| **Async Write** | ✅ `WriteLineAsync` | ⚠️ Limited | |
| **ReadLine** | ✅ | ✅ | |
| **ReadKey** | ✅ `ReadKey(bool intercept)` | ✅ | |
| **Cursor Control** | ✅ `SetCursorPosition`, `GetCursorPosition` | ✅ | |
| **Window Width** | ✅ `WindowWidth` | ✅ | |
| **Clear Screen** | ✅ `Clear()` | ✅ | |
| **Interactive Detection** | ✅ `IsInteractive` | ✅ | |
| **Color Support Detection** | ✅ `SupportsColor` | ✅ | |
| **Hyperlink Detection** | ✅ `SupportsHyperlinks` | ✅ | |

---

## 9. ANSI Colors & Styling (Terminal)

| Feature | TimeWarp.Terminal | Spectre.Console | Notes |
|---------|-------------------|-----------------|-------|
| **Basic Colors (8)** | ✅ | ✅ | |
| **Bright Colors (8)** | ✅ | ✅ | |
| **256-Color (8-bit)** | ✅ CSS Named Colors | ✅ Full 256 | |
| **24-bit True Color** | 🔄 Planned | ✅ RGB/Hex | |
| **CSS Named Colors** | ✅ 100+ colors | ✅ | |
| **Background Colors** | ✅ `OnRed()`, etc. | ✅ | |
| **Bold/Dim/Italic** | ✅ | ✅ | |
| **Underline/Strikethrough** | ✅ | ✅ | |
| **Blink/Reverse/Hidden** | ✅ | ✅ | |
| **Fluent Extensions** | ✅ `"text".Red().Bold()` | ⚠️ Different | C# 14 extensions |
| **Custom Style** | ✅ `.WithStyle(ansiCode)` | ✅ | |
| **Markup Language** | ❌ | ✅ `[bold red]text[/]` | |
| **Syntax Highlighting Colors** | ✅ `SyntaxColors.*` | ⚠️ | PSReadLine-style |

---

## 10. Hyperlinks (Terminal)

| Feature | TimeWarp.Terminal | Spectre.Console | Notes |
|---------|-------------------|-----------------|-------|
| **OSC 8 Hyperlinks** | ✅ | ✅ | |
| **Create Link** | ✅ `AnsiHyperlinks.CreateLink()` | ✅ | |
| **Fluent Extension** | ✅ `"text".Link(url)` | ✅ `[link=url]` | |
| **Terminal Extension** | ✅ `WriteLink()`, `WriteLinkLine()` | ✅ | |
| **Auto-Detection** | ✅ WT_SESSION, TERM_PROGRAM, etc. | ✅ | |
| **Graceful Fallback** | ✅ | ✅ | |

---

## 11. Widgets - Panel (Terminal)

| Feature | TimeWarp.Terminal | Spectre.Console | Notes |
|---------|-------------------|-----------------|-------|
| **Basic Panel** | ✅ `Panel` | ✅ | |
| **Header/Title** | ✅ | ✅ | |
| **Border Styles** | ✅ None, Rounded, Square, Doubled, Heavy | ✅ ~15 styles | |
| **Border Color** | ✅ | ✅ | |
| **Padding** | ✅ Horizontal/Vertical | ✅ | |
| **Fixed Width** | ✅ | ✅ | |
| **Word Wrap** | ✅ | ✅ | |
| **Fluent Builder** | ✅ `PanelBuilder` | ⚠️ | |
| **Nested Builder** | ✅ `NestedPanelBuilder<T>` | ❌ | |
| **ANSI in Content** | ✅ | ✅ | |

---

## 12. Widgets - Table (Terminal)

| Feature | TimeWarp.Terminal | Spectre.Console | Notes |
|---------|-------------------|-----------------|-------|
| **Basic Table** | ✅ | ✅ | |
| **Column Alignment** | ✅ Left, Center, Right | ✅ | |
| **Column Min/Max Width** | ✅ | ✅ | |
| **Header Color** | ✅ | ✅ | |
| **Border Styles** | ✅ 5 styles | ✅ Many more | |
| **Border Color** | ✅ | ✅ | |
| **Show/Hide Headers** | ✅ | ✅ | |
| **Row Separators** | ✅ | ✅ | |
| **Expand to Width** | ✅ | ✅ | |
| **Shrink to Fit** | ✅ Proportional | ⚠️ | |
| **Truncate Mode** | ✅ Start, Middle, End | ⚠️ | |
| **Fluent Builder** | ✅ `TableBuilder` | ⚠️ | |
| **Nested Builder** | ✅ `NestedTableBuilder<T>` | ❌ | |
| **Caption/Title** | ❌ | ✅ | |
| **Nested Tables** | ❌ | ✅ | |

---

## 13. Widgets - Rule (Terminal)

| Feature | TimeWarp.Terminal | Spectre.Console | Notes |
|---------|-------------------|-----------------|-------|
| **Basic Rule** | ✅ | ✅ | |
| **Centered Title** | ✅ | ✅ | |
| **Line Styles** | ✅ Thin, Doubled, Heavy | ✅ | |
| **Color** | ✅ | ✅ | |
| **Fixed Width** | ✅ | ⚠️ | |
| **Fluent Builder** | ✅ `RuleBuilder` | ⚠️ | |
| **Nested Builder** | ✅ `NestedRuleBuilder<T>` | ❌ | |

---

## 14. Widgets - NOT Yet in TimeWarp

| Widget | Spectre.Console | Priority | Notes |
|--------|-----------------|----------|-------|
| **Tree** | ✅ | 🔄 Medium | Hierarchical display |
| **Progress Bars** | ✅ | 🔄 High | Live updating |
| **Spinners** | ✅ | 🔄 High | Async status |
| **Status Display** | ✅ | 🔄 High | Live status |
| **Bar Chart** | ✅ | Low | |
| **Breakdown Chart** | ✅ | Low | |
| **Calendar** | ✅ | Low | |
| **Grid** | ✅ | Low | |
| **Layout** | ✅ | Low | |
| **Figlet** | ✅ | Low | ASCII art |
| **Canvas** | ✅ | Low | Pixel drawing |
| **Canvas Image** | ✅ | Low | |
| **JSON** | ✅ | Medium | Pretty-print |
| **Text Path** | ✅ | Low | |

---

## 15. Prompts & User Input

| Feature | TimeWarp | Spectre.Console | Notes |
|---------|----------|-----------------|-------|
| **Text Prompt** | ⚠️ REPL ReadLine | ✅ `TextPrompt<T>` | |
| **Confirmation** | 🔄 Planned | ✅ `Confirm()` | |
| **Selection** | 🔄 Planned | ✅ `SelectionPrompt<T>` | |
| **Multi-Selection** | 🔄 Planned | ✅ `MultiSelectionPrompt<T>` | |
| **Validation** | ✅ In routes | ✅ | |
| **Secret Input** | 🔄 Planned | ✅ | |

---

## 16. Technical Capabilities

| Feature | TimeWarp (Nuru + Terminal) | Spectre.Console | Notes |
|---------|---------------------------|-----------------|-------|
| **AOT Compatible** | ✅ Full | ⚠️ Partial | |
| **Trimming Compatible** | ✅ Full | ⚠️ Partial | |
| **Source Generators** | ✅ Route validation, invokers | ❌ | |
| **Compile-Time Errors** | ✅ Invalid patterns | ❌ | |
| **Minimal Dependencies** | ✅ | ⚠️ | |
| **Package Size** | ✅ Small | ⚠️ Larger | |
| **Modular Design** | ✅ Separate packages | ⚠️ Monolithic | |

---

## 17. API Design Philosophy

| Aspect | TimeWarp | Spectre.Console |
|--------|----------|-----------------|
| **Route Pattern DSL** | ✅ `"deploy {env} --force"` | ❌ Class-based |
| **Interface Separation** | ✅ `IConsole`, `ITerminal` | Single interface |
| **Builder Pattern** | ✅ All widgets | ⚠️ Mixed |
| **Nested Builders** | ✅ `Done()` returns parent | ❌ |
| **Fluent Extensions** | ✅ C# 14 | Standard |
| **ASP.NET Core Style** | ✅ `CreateBuilder()`, `Map()` | ❌ |
| **Message Types** | ✅ Query, Command, IdempotentCommand | ❌ |

---

## Summary: TimeWarp vs Spectre.Console

### TimeWarp Advantages:
- **Full AOT/Trimming** - No reflection, source-generated
- **Route-Based CLI** - Modern ASP.NET Core-style `Map()` API
- **Rich REPL** - Syntax highlighting, key bindings, history, completion
- **Tab Completion** - Dynamic and static shell completion
- **Testability** - Separated interfaces, NuruTestContext, key simulation
- **Telemetry** - Built-in OpenTelemetry/Aspire integration
- **Pipeline Behaviors** - Logging, auth, retry, telemetry
- **Source Generators** - Compile-time route validation
- **Modular** - Use only what you need

### Spectre.Console Advantages:
- **More Widgets** - Tree, Charts, Calendar, Canvas, Figlet, etc.
- **Rich Prompts** - Selection, Multi-Selection, Confirmation
- **Live Displays** - Progress bars, spinners, status
- **Markup Language** - Inline `[bold red]text[/]`
- **True Color** - 24-bit RGB support
- **Mature Ecosystem** - Larger community, more examples

---

## Feature Count Summary

| Category | TimeWarp (Nuru + Terminal) | Spectre.Console |
|----------|---------------------------|-----------------|
| CLI Framework | ✅ Full route-based | ✅ Class-based |
| REPL Support | ✅ Full-featured | ❌ None |
| Tab Completion | ✅ Dynamic + 4 shells | ❌ None |
| Widgets | 3 (Panel, Table, Rule) | 18+ |
| Prompts | 🔄 Planned | 4+ |
| Live Displays | 🔄 Planned | 4 |
| Telemetry | ✅ OpenTelemetry | ❌ None |
| Testing | ✅ Comprehensive | ⚠️ Basic |
| AOT | ✅ Full | ⚠️ Partial |

---

## Migration Path: Spectre.Console → TimeWarp

1. **CLI Commands**: Replace `CommandApp` with `NuruApp.CreateBuilder()` and `Map()` routes
2. **Tables/Panels/Rules**: Direct replacement with TimeWarp.Terminal widgets
3. **Colors**: Replace `[red]text[/]` markup with `"text".Red()` extensions
4. **Testing**: Replace `TestConsole` with `TestTerminal` + `NuruTestContext`
5. **Prompts**: 🔄 Awaiting TimeWarp prompt widgets (use Console.ReadLine for now)
6. **Progress**: 🔄 Awaiting TimeWarp progress widgets

---

## References

- [TimeWarp.Nuru Source](https://github.com/TimeWarpEngineering/timewarp-nuru)
- [TimeWarp.Terminal Source](source/timewarp-terminal/)
- [Spectre.Console Documentation](https://spectreconsole.net/)
- [Spectre.Console GitHub](https://github.com/spectreconsole/spectre.console)
