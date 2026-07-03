# Address 1.0 release readiness code review findings

## Description

Complete code review of the TimeWarp.Terminal library (all 25 source files, ~7,200 lines)
in preparation for moving from 1.0.0-beta.13 to a stable 1.0.0 release. Six review passes
covered: core abstractions (IConsole/ITerminal/TimeWarpConsole/TimeWarpTerminal), the static
facade + hyperlinks, the shipped test doubles, the table subsystem, text/ANSI/unicode
utilities + panel/rule widgets, and packaging/API-surface/semver readiness.

Blockers must be fixed before 1.0. Majors should be fixed or explicitly accepted (many are
behavioral contracts that become breaking changes to fix after 1.0). Minors are judgment
calls — triage each.

## Checklist — Blockers (must fix before 1.0)

- [x] `terminal-static.cs:447,495` — `WritePanel(string, string?)` and
      `WritePanel(string, string?, ConsoleColor?, ConsoleColor?)` make
      `Terminal.WritePanel("content")` a CS0121 ambiguous-call compile error; that exact
      call is the documented example at line 443. Collapse the overload set.
      FIXED: removed the redundant `(string, string?)` overload (source-compatible — the
      4-param overload covers every previously-compilable call) and added a positional
      regression test in tests/terminal-static-05-widgets.cs; the ambiguity had zero test
      coverage because existing tests only used named args or the ITerminal extension.
- [x] `widgets/rule-widget.cs:102-110` — colored rule with a title longer than the width
      recomputes negative left/right line lengths and `new string(char, -n)` throws
      ArgumentOutOfRangeException, bypassing the min-width fallback computed at line 74.
      FIXED: Render now computes the layout once so the title-only fallback covers the
      colored path too; width is also clamped to >= 0 (fixes the related negative-width
      minor). Regression test added in tests/rule-widget-01-basic.cs.
- [x] `timewarp-terminal.cs:219` — CursorVisible setter is gated behind
      `OperatingSystem.IsWindows()` and silently no-ops on Linux/macOS, but
      `Console.CursorVisible`'s *setter* is supported on Unix (only the getter throws).
      Hide/show cursor is core REPL functionality broken on the primary platforms.
      FIXED: Windows gate removed from the setter (getter keeps its gate — that one IS
      Windows-only); verified under a pseudo-TTY on Linux that ESC[?25l/ESC[?25h are now
      emitted.
- [x] `Directory.Packages.props:9` — stable 1.0.0 would depend on prerelease
      TimeWarp.Builder 1.0.0-beta.3 (non-private; IBuilder<T> is implemented by public
      builder types) and trip NU5104. Needs a stable TimeWarp.Builder or the dependency
      removed/internalized.
      FIXED: bumped to the stable TimeWarp.Builder 1.0.0 published on NuGet; builds clean.

## Checklist — Major (fix or explicitly accept; behavioral contracts freeze at 1.0)

### Platform gating & core correctness
- [x] `timewarp-terminal.cs:825` — Title setter Windows-gated but Console.Title's setter
      works on Unix; silently no-ops on Linux/macOS.
      FIXED: gate removed from the setter (getter keeps it — the BCL getter is Windows-only).
- [x] `timewarp-terminal.cs:789` — parameterless Beep() Windows-gated but Console.Beep()
      is cross-platform (BEL on Unix); only Beep(freq, duration) is Windows-only.
      FIXED: gate removed from Beep(); the (freq, duration) overload keeps its gate.
- [x] `timewarp-terminal.cs:835` — KeyAvailable catches only IOException but Console
      throws InvalidOperationException when stdin is redirected — crashes in exactly the
      redirected scenario the fallback exists for.
      FIXED: InvalidOperationException now also caught, returning false.
- [x] `iterminal.cs:262` (and members) — interface XML docs promise unconditional behavior
      while TimeWarpTerminal silently no-ops on non-Windows and swallows exceptions;
      document the actual contract before it freezes at 1.0.

      FIXED: <remarks> added to every ITerminal member whose implementation platform-gates
      or swallows exceptions (18 members documented; IConsole members verified as pure
      pass-throughs needing no change).
### Static facade
- [x] `terminal-static.cs:60-64` + `test-terminal-context.cs:86` — Terminal.Instance is a
      process-global mutable static; TestTerminalContext stores Current in AsyncLocal but
      still swaps the global, so the documented "parallel test isolation" guarantee is
      false — parallel tests last-writer-win and can restore stale instances.
      DECIDED + FIXED (2026-07-03): "AsyncLocal wins" — Terminal.Instance's getter returns
      TestTerminalContext.Current ?? the process-global field; SetCurrent/Use/ClearCurrent
      now only touch the AsyncLocal (the global is never mutated by the context), so the
      documented parallel isolation is actually true, and direct Terminal.Instance = x
      still works for serial tests. This also moots the earlier disposed-terminal leak fix
      (the context never installs anything into the global). FormatProvider snapshot/restore
      retained. Parallel-isolation regression test added (two concurrent Use scopes).
- [x] `terminal-static.cs:92-135,163` — colored Write/WriteLine/WriteErrorLine emit ANSI
      unconditionally, ignoring SupportsColor / NO_COLOR / redirection; raw escapes land
      in piped output.
      DECIDED + FIXED (2026-07-03): all ConsoleColor-parameter paths now consult
      SupportsColor and degrade to plain text when unsupported — the four facade
      Write/WriteLine/WriteErrorLine overloads, static WritePanel/WriteTable, and the
      ITerminal WritePanel/WriteTable extension helpers. Caller-embedded ANSI strings
      (e.g. "text".Cyan(), BorderColor) remain the caller's responsibility. Regression
      tests added in terminal-static-06-color.cs.
- [x] `terminal-static.cs:184-290` — format overloads use InvariantCulture where
      System.Console uses current culture; silent formatting differences for migrated
      code, undocumented.
      DECIDED + FIXED (2026-07-03): added `Terminal.FormatProvider` (IFormatProvider?,
      default null = CultureInfo.CurrentCulture resolved per call — TextWriter.FormatProvider
      semantics, avoids freezing a static-init culture snapshot). All 12 format overloads
      now use it; set to InvariantCulture for deterministic output. Typed IFormatProvider
      (not CultureInfo) per BCL convention; lives on Terminal (no separate TerminalFormatting
      class) for discoverability. TestTerminalContext snapshots/restores it alongside
      Instance. NOTE for release notes: default behavior changes from invariant to
      current culture (Console parity).
- [x] `terminal-static.cs:591-599` — static Terminal.WriteLink always emits raw OSC 8,
      while the same-named ITerminal.WriteLink extension checks SupportsHyperlinks and
      falls back to plain text; two same-named APIs with different behavior.
      DECIDED + FIXED (2026-07-03): static WriteLink now checks SupportsHyperlinks and
      writes the plain display text when unsupported, identical to the extension.
      Regression test added in hyperlink-01-basic.cs.
- [x] `terminal-static.cs` — facade exposes TreatControlCAsInput but omits the
      CancelKeyPress event both ITerminal and System.Console provide; Ctrl+C handling
      requires reaching through Terminal.Instance.
      FIXED: static Terminal.CancelKeyPress event added, forwarding add/remove to Instance;
      forwarding covered by new tests in cancel-key-press-01-basic.cs.
- [x] `ansi-hyperlink-extensions.cs:30` — CreateLink does no URI validation/escaping; a
      URL containing ESC/BEL/ST terminates the OSC 8 sequence early = terminal
      escape-injection vector via attacker-influenced URLs.

      FIXED: URLs are sanitized before embedding — C0 controls and DEL percent-encoded —
      and all OSC 8 builders route through the single sanitizer; regression tests added.
### Test doubles (shipped public API)
- [x] `test-terminal-context.cs:94-99` — ClearCurrent with an empty snapshot stack nulls
      Context.Value but never restores Terminal.Instance; AsyncLocal writes don't flow
      back from async helpers, so Instance can be left pointing at a disposed
      TestTerminal for the rest of the run (ObjectDisposedException in later tests).
      FIXED: ClearCurrent with no snapshot resets Terminal.Instance to a fresh
      TimeWarpTerminal instead of leaving a disposed TestTerminal installed; test added.
- [x] `test-terminal.cs:237-246` — Read() only drains KeyQueue and returns -1, never
      consuming constructor input: `new TestTerminal("abc").Read()` returns -1 while
      ReadLine() returns "abc"; contradicts Console where both consume the same stream.
      FIXED: Read() falls back to InputReader when the KeyQueue is empty, matching Console
      shared-stream semantics; interleaving tests added.
- [x] `test-terminal.cs:456` — KeyAvailable reflects only KeyQueue even though ReadKey()
      synthesizes keys from constructor input, so `while (KeyAvailable) ReadKey()` loops
      behave opposite to a real console.
      FIXED: KeyAvailable now also reflects unread constructor input, mirroring ReadKey's
      sources; test added.
- [x] `test-terminal.cs:469-514,536` — QueueKey ignores shift/ctrl when computing KeyChar
      (shift:true yields 'a' not 'A'; ctrl:true yields 'a' not ''); QueueKeys("A")
      produces shift:false.
      FIXED: shift produces uppercase KeyChar, ctrl produces the control character for A-Z,
      QueueKeys sets shift for uppercase letters; tests added.
- [x] `test-console.cs:200-201` — TestConsole.ReadKey() throws NotSupportedException
      while inheriting IConsole docs that promise a value; first-party implementation
      violates its own contract (suggests ReadKey belongs on ITerminal — breaking to move
      later).

      DECIDED + FIXED (2026-07-03): ReadKey() moved from IConsole to ITerminal — key-by-key
      input is interactive-terminal functionality with no meaning for a stream-oriented
      console. TestConsole's throwing member and TimeWarpConsole's implementation removed;
      contract test added asserting the member lives on ITerminal only. BREAKING vs beta
      (intentional, pre-1.0).
### Widgets / text
- [x] `widgets/unicode-width.cs:260-279` — GetTextWidth treats every multi-rune grapheme
      as width 2, so NFD combining sequences ("e" + U+0301) measure 2 instead of 1;
      misaligns borders for decomposed Latin text.
      FIXED: multi-rune clusters now measure by content — ZWJ/VS16/skin-tone/flag pairs are 2,
      anything else sums rune widths so NFD combining sequences measure correctly.
- [x] `widgets/unicode-width.cs:163-180` — blanket 0x1F000-0x1FAFF wide range makes
      narrow EAW=N blocks wide (playing cards, ornamental dingbats, alchemical symbols);
      regional-indicator branch unreachable inside it; genuinely wide ranges missing
      (Hangul Jamo Ext-A, Tangut, Kana Extended, CJK Compat tail).
      FIXED: blanket range replaced with Unicode 16 EAW/emoji-presentation ranges (narrow
      blocks fall through to width 1, only their emoji exceptions are wide); regional
      indicators reachable again; Tangut/Kana/Nushu/Hangul Jamo Ext-A/CJK Compat tail added.
- [x] `widgets/ansi-string-utils.cs:14-15` — strip/measure regex only matches SGR ('m')
      CSI + OSC 8; cursor movement, erase, private-mode sequences count as visible
      characters, corrupting all width math for real ANSI streams.
      FIXED: source-generated regex now matches all CSI finals, all OSC (BEL or ST terminated),
      and two-byte ESC sequences; wrap carry-state only re-emits SGR and OSC 8.
- [x] `widgets/ansi-string-utils.cs:186-289` — WrapText treats an ANSI code mid-word as a
      word boundary, so a single styled word can wrap mid-word and get TrimStart'ed.
      FIXED: words are tokenized on visible whitespace only, so mid-word ANSI stays attached
      and the word wraps as one unit; over-wide words grapheme-break with codes in place.
- [x] `widgets/panel-widget.cs:226-229` — with WordWrap(false), long lines are never
      truncated (comment says "Pad or truncate" but PadRightVisible never truncates) and
      push past the right border, breaking the box.
      FIXED: over-long lines are truncated ANSI/grapheme-aware to the content area (plain cut,
      no ellipsis — panel has no ellipsis convention) with reset before the border.
- [x] `widgets/table-widget.cs:569` — TruncateWithEllipsis strips all ANSI codes in every
      branch despite the line-493 comment claiming codes are preserved; truncated cells
      silently lose colors.
      FIXED: truncation preserves embedded ANSI in the kept span for End/Start/Middle; style
      active at a cut is reset before the plain ellipsis, and style opened before a Start/
      Middle cut is replayed onto the kept tail via the shared TruncateVisible helpers.
- [x] `widgets/table-widget.cs:276-282,223-234` — Grow columns hard-set to width 0 when
      fixed columns fill the terminal (content silently vanishes) and MinWidth is never
      consulted, contradicting table-column.cs docs.
      FIXED: Grow columns are floored at max(4, MinWidth) and overflow is absorbed by the
      existing proportional shrink; tight-terminal regression test added.
- [x] `widgets/table-builder.cs:150` — Build() returns the live Table instance (no
      snapshot); building twice yields the same object and post-Build builder calls
      mutate the "built" table.

      FIXED: Build() returns an independent snapshot (copied settings and lists); snapshot
      independence test added.
### Packaging / release pipeline
- [x] `tools/dev-cli/endpoints/workflow.cs:121` — release pipeline is
      clean→build→check-version→pack→push with no test or verify-samples step; 1.0.0
      could publish from a commit whose tests never ran on the release event.
      FIXED: release path is now clean→build→verify-samples→test→check-version→pack.
      Bonus: `dev test` (and both pipeline paths) previously ran `dotnet test` on a
      solution with zero VSTest projects — a false green. The test step now runs the
      tests/*.cs runfiles and fails on any failure.
- [x] `timewarp-terminal.csproj:23` — README.md is packed as a file but PackageReadmeFile
      is never set, so nuget.org shows no readme.
      FIXED: PackageReadmeFile set; verified <readme> lands in the nuspec.
- [x] `Directory.Build.props:66` + `csproj:13` — IsAotCompatible=true is claimed while
      all trim/AOT diagnostics (IL2026/IL2067/IL2070/IL2075/IL3050/IL2104/IL3053) are
      globally NoWarn'd "not yet implemented"; the package advertises AOT compat that is
      unverified.
      FIXED: the blanket IL2026/IL2067/IL2070/IL2075/IL3050/IL2104/IL3053 NoWarn is removed
      from the root props — the library now builds clean under full trim/AOT analysis
      (EnableTrimAnalyzer/EnableAotAnalyzer), and the dev-cli AOT publish (which consumes
      the library) succeeds end-to-end. IL2026/IL3050/IL2104/IL3053 remain suppressed only
      in tools/dev-cli for TimeWarp.Nuru.DevCli package-content files using reflection
      JsonSerializer, documented in that props file.
- [x] `README.md:193` — Table quickstart calls `.Shrink()` which does not exist on
      TableBuilder; front-page sample does not compile.
      FIXED: sample now ends at .Expand() with a note that shrink-to-fit is automatic.
- [x] `tools/dev-cli/endpoints/workflow.cs:204` — no symbol package (snupkg) and no
      ContinuousIntegrationBuild; no debugger symbols published for 1.0.
      FIXED: IncludeSymbols + snupkg set in the csproj, pack passes
      ContinuousIntegrationBuild=true; verified .snupkg is produced and the nuspec
      repository element carries the commit.

## Checklist — Minor (triage)

- [x] `timewarp-terminal.cs:647` — NO_COLOR honored even when set to empty string (spec
      says non-empty disables); no TERM=dumb / FORCE_COLOR handling.
      FIXED: NO_COLOR disables color only when non-empty (per no-color.org) and TERM=dumb is
      honored; FORCE_COLOR deliberately out of scope. Rule documented on ITerminal.SupportsColor.
- [x] `timewarp-terminal.cs:293` — GetCursorPosition reads CursorLeft then CursorTop
      non-atomically instead of Console.GetCursorPosition(); pair can tear.
      FIXED: uses atomic Console.GetCursorPosition() with the same (0,0) fallback.
- [x] `timewarp-terminal.cs:807` — TreatControlCAsInput lacks the try/catch every other
      member has; throws when no console attached.
      FIXED: getter returns false / setter no-ops on IOException and InvalidOperationException,
      matching the class-wide swallow policy; documented on the interface member.
- [x] `iterminal.cs:175` — MoveBufferArea/CursorSize/Window-Buffer setters bake
      Windows-legacy-console APIs into the cross-platform interface; unremovable after 1.0.
      REVISED + FIXED (2026-07-03): initially accepted, then overturned on review — these are
      legacy conhost features that don't work in Windows Terminal either, and freezing them at
      1.0 would be permanent stub debt for every implementer. REMOVED: MoveBufferArea,
      CursorSize, SetWindowSize, SetWindowPosition, SetBufferSize, WindowLeft, WindowTop,
      LargestWindowWidth, LargestWindowHeight. DEMOTED to get-only: WindowWidth, WindowHeight,
      BufferWidth, BufferHeight (TestTerminal keeps public setters for test configuration).
      BREAKING vs beta (intentional, pre-1.0).
- [x] `timewarp-terminal.cs:642` — IsInteractive checks only input redirection; stdout
      piped still reports interactive, and docs don't say which stream.
      FIXED: IsInteractive now requires both stdin and stdout unredirected; docs state exactly
      which streams are consulted.
- [x] `ansi-hyperlink-extensions.cs:30` vs `terminal-static.cs:591` —
      CreateLink(displayText, url) and WriteLink(url, text) take the same two strings in
      opposite order; invites silently transposed args.
      FIXED: CreateLink re-ordered to (url, displayText = null), aligned with WriteLink and the
      extensions; all callers and tests updated. BREAKING vs beta (intentional, pre-1.0).
- [x] `terminal-static.cs:128,163,591` — overload asymmetry: WriteLine has (fg,bg) but
      Write/WriteErrorLine have fg-only or none; WriteLink has no WriteLinkLine.
      FIXED: added Write(msg, fg, bg), WriteErrorLine(msg, fg, bg), and WriteLinkLine(url, text)
      — all SupportsColor/SupportsHyperlinks-gated, non-optional params (no new ambiguity).
- [x] `terminal-static.cs:102,110-114` — colored-overload docs claim null message writes
      "only the line terminator" but color prefix + reset escapes are still emitted.
      FIXED: behavior now matches the docs — a null message writes plain with no color codes
      across all colored overloads.
- [x] `test-terminal.cs:261-265` — ReadKey at EOF fabricates Ctrl+D forever (real console
      throws when redirected); undocumented sentinel becomes contract.
      FIXED (documented): the Ctrl+D EOF sentinel is now stated in the ReadKey docs — loops
      should treat '\u0004' as end-of-input. Behavior intentionally kept for REPL testing.
- [x] `test-terminal.cs:47,456` — KeyQueue and StringWriters unsynchronized while
      System.Console members are documented thread-safe; production-safe code can fail
      only under the double.
      FIXED (documented): class-level remarks state the double is single-threaded by design,
      unlike System.Console; no locking added.
- [x] `test-terminal.cs:399-415` — SimulateCancelKeyPress reflects into
      ConsoleCancelEventArgs' non-public ctor and silently no-ops if lookup fails
      (trimming/AOT/future BCL); false-passing consumer tests.
      FIXED: SimulateCancelKeyPress now throws InvalidOperationException if the BCL internal
      constructor cannot be found or invoked — no more silent false-passing tests.
- [x] `test-terminal.cs:419-420` — Clear() appends a "[CLEAR]" marker line to captured
      output; behavior only documented in internal Design region, not public docs.
      FIXED (documented): the verbatim "[CLEAR]" marker and its rationale are now in the
      public Clear() docs.
- [x] `test-terminal.cs:628-630` — Dispose disposes consumer-assigned Standard*Stream
      property values; duplicated summary doc block at 606-611; no cursor/window arg
      validation vs Console's ArgumentOutOfRangeException; TestTerminalContext class doc
      mentions DI resolution Resolve() doesn't implement (test-terminal-context.cs:21).
      PARTIAL: duplicated Dispose summary removed and the class-doc DI-resolution claim
      rewritten (2026-07-03); Dispose-of-consumer-streams and arg validation still open.
      FIXED: Dispose now disposes only the constructor-created streams, never consumer-assigned
      replacements (regression test added). Lax cursor/window arg validation accepted as
      test-double leniency.
- [x] `widgets/ansi-string-utils.cs:387-418` — wrap state machine never clears OSC 8
      hyperlink state on the end sequence and SGR reset wrongly wipes hyperlink state;
      wrapped lines re-open closed hyperlinks.
      FIXED: SGR and hyperlink are independent carry channels; empty-URL OSC 8 clears the link
      and SGR reset no longer wipes hyperlink state.
- [x] `ansi-colors.cs:210-253` — GetForeground maps dark and bright ConsoleColors to the
      same SGR code (Red/DarkRed both 31; bright 91-97 unused; background likewise).
      FIXED: standard SGR mapping — Dark* = 30-37/40-47, normal = 90-97/100-107, DarkGray =
      bright black, Gray = dim white. RELEASE NOTE: visible output changes for ConsoleColor
      overload users.
- [x] `widgets/ansi-string-utils.cs:74-137` — Pad*/Center with negative width throw
      ArgumentOutOfRangeException from `new string(c, negative)`; clamp or document.
      FIXED: negative widths clamp to 0 in Pad*/Center; documented.
- [x] `widgets/table-widget.cs:139` — AddRow stores null params array without check; NRE
      at Render far from call site (AddColumns throws eagerly by contrast).
      FIXED: AddRow now throws ArgumentNullException eagerly; test added.
- [x] `widgets/terminal-table-extensions.cs:93-96` — WriteTable fg/bg prefix is cancelled
      by the first embedded AnsiColors.Reset (border/cell colors), so the color overload
      malfunctions when combined with BorderColor or colored cells.
      FIXED: the requested fg/bg prefix is re-applied after every embedded Reset in all four
      colored-widget write loops, so BorderColor/styled cells no longer cancel it.
- [x] `widgets/table-widget.cs:292` — Expand silently ignored when Border is None;
      undocumented.
      FIXED (documented): Expand's no-effect-with-BorderStyle.None behavior stated on both the
      property and the builder method.
- [x] `widgets/table-widget.cs:298-305` — Expand distributes width to MaxWidth-capped
      columns, violating the MaxWidth contract.
      FIXED: Expand skips capped columns and redistributes to uncapped ones; tests added.
- [x] `widgets/table-widget.cs:198-199` — 3-char overhead reserved for Grow columns that
      later collapse to width 0, over-shrinking fixed columns.
      RESOLVED BY EARLIER FIX (2026-07-03): Grow columns are now floored at max(4, MinWidth)
      and never collapse to width 0 (the zeroing block and its render-skip were removed in
      the Grow/MinWidth major fix), so overhead is only ever reserved for columns that
      actually render.
- [x] `widgets/table-builder.cs:153-157` — ToTable() doc references an implicit operator
      that doesn't exist.
      FIXED: doc rewritten as an explicit alternative to Build().
- [x] Overload pairs differing only by trailing optional ConsoleColor params
      (`terminal-table-extensions.cs:36/57,78/112`, `terminal-panel-extensions.cs:31-56,
      109-141,149-173`, facade equivalents) — optional-param overloads are unreachable
      without explicit args and the redundant pairs freeze into the 1.0 surface.
      ACCEPTED (2026-07-03): the genuinely ambiguous pair (WritePanel string overloads) was
      collapsed; the remaining Action/Table/Panel pairs resolve deterministically (the
      no-optional-parameter candidate wins), and collapsing them would make null-literal
      calls like WritePanel(null!) ambiguous — verified when the test suite caught exactly
      that. Frozen knowingly.
- [x] `tools/dev-cli/endpoints/workflow.cs:147` — check-version never compares the props
      version to the GitHub release tag that triggered the run; a v1.0.0 release event
      with props still at beta would silently push another beta.
      FIXED: release runs compare the GitHub release tag (leading v stripped) to the props
      version and fail on mismatch; local runs note the skip.
- [x] `Directory.Build.props:17` — net10.0-only target excludes LTS (net8.0) consumers at
      1.0 launch; single-TFM is a deliberate choice worth confirming.
      ACCEPTED (2026-07-03): net10.0-only is the deliberate org-wide baseline across TimeWarp
      repos (runfile tests/samples require .NET 10 regardless). Multi-targeting can be
      added later without breaking existing consumers.
- [x] `source/Directory.Build.props:6` — missing PackageProjectUrl, RepositoryType, and a
      PackageReleaseNotes strategy.
      FIXED: PackageProjectUrl, RepositoryType=git, and PackageReleaseNotes (releases URL) added.
- [x] `terminal-static.cs:48` — type `TimeWarp.Terminal.Terminal` collides with its
      namespace (CA1724 suppressed); permanent qualification awkwardness — confirm as
      accepted design.
      ACCEPTED (2026-07-03): deliberate Console-like ergonomics — `Terminal.WriteLine(...)`
      reads exactly like `Console.WriteLine(...)` for the primary audience; CA1724 stays
      suppressed with this rationale.
- [x] `README.md:59-67` — README teaches raw `Terminal.Instance =` swapping and never
      mentions TestTerminalContext, the safer shipped API.
      FIXED: testing section now leads with the scoped TestTerminalContext.Use pattern and its
      parallel-isolation guarantees; direct Instance swap demoted to a serial-tests note.

## Notes

- Review executed by six parallel subsystem reviewers; all blocker findings were
  independently re-verified against the source before filing.
- Clean areas worth noting: TimeWarpConsole delegation, CancelKeyPress event forwarding,
  proportional shrink arithmetic in table-widget, grapheme-based cell truncation, ragged
  row handling, box-chars/border-style/line-style, license/icon packaging metadata, OIDC
  trusted publishing setup, all-public-types-sealed + internal widget constructors, and
  version single-sourcing via source/Directory.Build.props.
- `./bin/dev check-version` already reports 1.0.0-beta.13 as published — the version bump
  to 1.0.0 should happen only after blockers (at minimum) are resolved.

## Session

- Created + review: 096d9aa9-8cec-4987-a576-91698523d859 (2026-07-03)
